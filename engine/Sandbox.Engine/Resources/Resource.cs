using BlowoutTeamSoft.Engine;
using BlowoutTeamSoft.Engine.Contexts;
using BlowoutTeamSoft.Engine.Core;
using BlowoutTeamSoft.Engine.Extensions;
using BlowoutTeamSoft.Engine.Interfaces;
using BlowoutTeamSoft.Engine.Interfaces.Assets;
using BlowoutTeamSoft.Engine.Interfaces.ScriptEngines;
using BlowoutTeamSoft.Engine.Logger;
using System.Text.Json.Serialization;

namespace Sandbox;

/// <summary>
/// A resource loaded in the engine, such as a <see cref="Model"/> or <see cref="Material"/>.
/// </summary>
[Expose]
public abstract partial class Resource : BlowoutEngineObject, IValid, IJsonConvert, BytePack.ISerializer,
	IBlowoutEngineAsset,
	IBlowoutSdkSyncObject
{
	/// <summary>
	/// ID of this resource,
	/// </summary>
	[Hide, JsonIgnore]
	public int ResourceId { get; protected set; }

	/// <summary>
	/// Path to this resource.
	/// </summary>
	[Hide, JsonIgnore]
	public string ResourcePath { get; protected set; }

	/// <summary>
	/// File name of the resource without the extension.
	/// </summary>
	[Hide, JsonIgnore]
	public string ResourceName { get; protected set; }

	/// <summary>
	/// This is what loads the resource. While this is alive the resource will be loaded.
	/// </summary>
	internal AsyncResourceLoader Manifest { get; set; }


	[Hide, JsonIgnore] public abstract bool IsValid { get; }

	[Hide, JsonIgnore]
	public string AssetName { get => ResourceName; set => ResourceName = value; }

	[Hide, JsonIgnore]
	public override string ObjectName { get => AssetName; set => AssetName = value; }

	[Hide, JsonIgnore]
	public int AssetId { get => ResourceId; set => ResourceId = value; }

	[Hide, JsonIgnore]
	public override Guid Id { get => new Guid( ResourceId, 0, 0, new byte[8] ); protected set => ResourceId = value.ToByteArray()[0]; }

	[Hide, JsonIgnore]
	public string AssetPath => ResourcePath;

	public bool IsAlive => this.IsValid;

	/// <summary>
	/// True if this resource has been changed but the changes aren't written to disk
	/// </summary>
	[Hide, JsonIgnore] public virtual bool HasUnsavedChanges => false;

	internal void Destroy()
	{
		// Unregister on main thread
		MainThread.Queue( () => { Game.Resources.Unregister( this ); } );

		if ( Manifest != default ) MainThread.QueueDispose( Manifest );
		Manifest = default;

		GC.SuppressFinalize( this );
	}

	~Resource()
	{
		Destroy();
	}

	public override BlowoutEngineObject CreateClone()
	{
		var clone = (Resource)MemberwiseClone();
		Game.Resources.Register( clone );

		return clone;
	}

	public override bool TryAsGameObject( out BlowoutEngineGameObject gameObject )
	{
		gameObject = null;
		return false;
	}

	public override T CastTo<T>()
	{
		if ( this is T target )
			return target;

		return default;
	}

	public override BlowoutEngineGameObject CreateInstance()
	{
		if ( this is PrefabFile prefabFile )
			return GameObject.Clone( prefabFile ).CreateInstance();

		return null;
	}

	internal static string FixPath( string filename )
	{
		if ( filename == null )
			return "";

		filename = filename.NormalizeFilename( false );
		if ( filename.EndsWith( "_c" ) ) filename = filename[..^2];
		filename = filename.TrimStart( '/' );

		return filename;
	}

	/// <summary>
	/// Sets the ResourcePath, ResourceName and ResourceId from a resource path
	/// </summary>
	internal void SetIdFromResourcePath( string resourcePath )
	{
		ResourcePath = FixPath( resourcePath );
		ResourceName = System.IO.Path.GetFileNameWithoutExtension( ResourcePath );
		ResourceId = ResourcePath.FastHash();

		Game.Resources.Register( this );
	}

	/// <summary>
	/// Accessor for loading native resources, not great, doesn't need to handle GameResource
	/// </summary>
	internal static Resource Load( Type t, string filename )
	{
		if ( t == typeof( Material ) ) return Material.Load( filename );
		if ( t == typeof( Texture ) ) return Texture.Load( filename );
		if ( t == typeof( Model ) ) return Model.Load( filename );
		if ( t == typeof( SoundFile ) ) return SoundFile.Load( filename );
		if ( t == typeof( AnimationGraph ) ) return AnimationGraph.Load( filename );
		if ( t == typeof( Shader ) ) return Shader.Load( filename );

		return null;
	}

	/// <summary>
	/// Called by OnResourceReloaded when a resource has been reloaded
	/// </summary>
	internal virtual void OnReloaded()
	{
	}

	internal static void OnResourceReloaded( string resourceName, IntPtr nativePointer )
	{
		Log.Trace( $"Resource Reloaded: '{resourceName}'" );

		if ( NativeResourceCache.TryGetValue( nativePointer.ToInt64(), out Resource value ) )
		{
			Log.Trace( $" - '{value}'" );
			value?.OnReloaded();
		}
	}

	public override string ToString()
	{
		return $"{GetType().Name}:{ResourceName}";
	}

	/// <summary>
	/// Should be called after the resource has been edited by the inspector
	/// </summary>
	public virtual void StateHasChanged()
	{

	}

	static object BytePack.ISerializer.BytePackRead( ref ByteStream bs, Type targetType )
	{
		var id = bs.Read<int>();
		return ResourceLibrary.Get<Resource>( id );
	}

	static void BytePack.ISerializer.BytePackWrite( object value, ref ByteStream bs )
	{
		if ( value is not Resource resource )
		{
			bs.Write( 0 );
			return;
		}

		bs.Write( resource.ResourceId );
	}
	public bool Equals( IBlowoutEngineAsset other )
	{
		return other is Resource res ? res == this : other.ObjectName == ObjectName && other.AssetId == AssetId;
	}

	public BlowoutScriptValueContext ToContext( string name, IBlowoutScriptEngine scriptEngine ) =>
		scriptEngine.TransformToContext( this );

	public void Sync( BlowoutScriptValueContext context, IBlowoutScriptEngine scriptEngine )
	{
		foreach ( var property in scriptEngine.PropertiesDispatcher.GetProperties( this, GetType() ) )
		{
			if ( property.SetValue == null )
				continue;
			var anyValue = context.GetAnyScriptValue( scriptEngine.NameNicify.Nicify( property.Name ) );
			if ( anyValue.EntityType == BlowoutTeamSoft.Engine.Enums.Sdk.BlowoutSdkTypeEntity.Unsupported )
			{
				BlowoutDebug.Logger.Warning( "Unsupported sdk entity type in source 2 resource packable asset: '" + property.Name + "'" );
				continue;
			}

			var target = scriptEngine.TransformToTarget( property.PropertyTarget, anyValue );
			property.SetValue( target );
		}
	}

	public void InitializeAsset( string path, int hash, IDisposable manifest )
	{
		// nothing do in native :P
	}

	public IBlowoutBitmap GetAssetIcon( int width, int height ) =>
		CreateAssetTypeIcon( width, height );

	public IBlowoutBitmap RenderThumbnail( int width, int height ) =>
		RenderThumbnail( new ThumbnailOptions() { Width = width, Height = height } );

	void IBlowoutEngineAsset.Destroy()
	{
		Destroy();
	}
}
