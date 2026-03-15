using BlowoutTeamSoft.Engine;
using BlowoutTeamSoft.Engine.Interfaces;
using BlowoutTeamSoft.Engine.Interfaces.Maps;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;

namespace Sandbox;

public sealed class Map : IBlowoutMap
{
	private PhysicsWorld PhysicsWorld;

	/// <summary>
	/// The world physics objects
	/// </summary>
	public PhysicsGroup PhysicsGroup { get; private set; }


	/// <summary>
	/// The world geometry;
	/// </summary>
	public SceneMap SceneMap { get; private set; }

	[JsonIgnore, IgnoreDataMember]
	public int Id
	{
		get
		{
			using ( SHA256 sha256 = SHA256.Create() )
			{
				var hash = sha256.ComputeHash( Encoding.UTF8.GetBytes( Name ) );
				return BitConverter.ToInt32( hash, 0 );
			}

		}
	}

	public bool IsReady => SceneMap.IsValid;

	public string Name => SceneMap.MapName;

	public IProgress<float> Progress { get; }

	[JsonIgnore, IgnoreDataMember]
	Guid IBlowoutMap.Id
	{
		get
		{
			using ( SHA256 sha256 = SHA256.Create() )
			{
				var hash = sha256.ComputeHash( Encoding.UTF8.GetBytes( Name ) );
				return new Guid(hash);
			}

		}
	}

	private MapLoader _loader;

	private Map()
	{
	}

	public Map( string mapName, MapLoader loader )
	{
		_loader = loader;
		Progress = loader;
		SceneMap = new SceneMap( loader.World, mapName, loader );
		PhysicsWorld = loader.PhysicsWorld;
		CreatePhysics( SceneMap.MapFolder, loader.WorldOrigin );
	}

	public static async Task<Map> CreateAsync( string mapName, MapLoader loader, CancellationToken cancelToken = default )
	{
		var sceneMap = await SceneMap.CreateAsync( loader.World, mapName, loader, cancelToken );
		if ( !sceneMap.IsValid() )
			return null;

		var map = new Map
		{
			SceneMap = sceneMap,
			PhysicsWorld = loader.PhysicsWorld
		};

		map.CreatePhysics( sceneMap.MapFolder, loader.WorldOrigin );

		return map;
	}

	private void CreatePhysics( string mapFolder, Vector3 origin )
	{
		if ( !PhysicsWorld.IsValid() )
			return;

		var physicsGroup = PhysicsWorld.native.CreateAggregateInstance( $"{mapFolder}/world_physics.vphys", new Transform( origin ), 0, PhysicsMotionType.Static );
		if ( !physicsGroup.IsValid() )
		{
			Log.Warning( $"Couldn't find map physics: '{mapFolder}/world_physics.vphys'" );
			return;
		}

		PhysicsGroup = physicsGroup;
	}

	public void Delete()
	{
		if ( SceneMap.IsValid() )
		{
			SceneMap.Delete();
			SceneMap = null;
		}

		if ( PhysicsGroup.IsValid() )
		{
			PhysicsWorld.native.DestroyAggregateInstance( PhysicsGroup );
			PhysicsGroup = null;
		}

		PhysicsWorld = null;
	}

	public void Load()
	{
		SceneMap.CreateAsync( _loader.World, Name, Game.ActiveScene.EnabledToken).Wait();
	}

	public async Task LoadAsync( CancellationToken token = default )
	{
		await SceneMap.CreateAsync( _loader.World, Name, token );
	}

	public BlowoutEngineObject FindObjectById( Guid id )
	{
		return SceneMap.World.SceneObjects.Where( x => x.GameObject != null ).FirstOrDefault( x => x.GameObject.Id == id )?.GameObject;
	}

	public IBlowoutGameSystem FindGameSystemById( Guid id )
	{
		return SceneMap.World.SceneObjects.Where(x=> x.Component != null).FirstOrDefault( x => x.Component.Id == id ).Component;
	}
}
