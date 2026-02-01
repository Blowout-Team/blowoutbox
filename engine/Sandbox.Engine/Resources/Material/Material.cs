using BlowoutTeamSoft.Engine.Interfaces.Rendering;
using BlowoutTeamSoft.Engine.Interfaces.Shaders;
using BlowoutTeamSoft.Engine.Render;
using NativeEngine;

namespace Sandbox;

/// <summary>
/// A material. Uses several <see cref="Texture"/>s and a <see cref="Shader"/> with specific settings for more interesting visual effects.
/// </summary>
public sealed partial class Material : Resource, IBlowoutMaterial
{
	internal IMaterial native;

	public override bool IsValid => native.IsValid;

	/// <summary>
	/// Name (or path) of the material.
	/// </summary>
	public override string Name { get; set; }

	/// <summary>
	/// Access to all of the attributes of this material.
	/// </summary>
	public RenderAttributes Attributes { get; internal set; }

	public IBlowoutShaderParameters Parameters => Attributes;

	public BlowoutMaterialHandle Handle => new BlowoutMaterialHandle(0);

	public BlowoutColor Color
	{
		get
		{
			var color = GetColor("color");
			return new(color.r, color.g, color.b, color.a);
		}
		set => Set("color", new Color(value.R, value.G, value.B, value.A));
	}

	/// <summary>
	/// Private constructor, use <see cref="FromNative(IMaterial, string)"/>
	/// </summary>
	/// <param name="native"></param>
	/// <param name="name"></param>
	/// <exception cref="System.Exception"></exception>
	private Material(IMaterial native, string name)
	{
		if (native.IsNull) throw new System.Exception("Material pointer cannot be null!");

		this.native = native;
		this.Name = name;

		SetIdFromResourcePath(name);

		CRenderAttributes attributes = this.native.GetRenderAttributes();
		Attributes = new RenderAttributes(attributes);
	}

	~Material()
	{
		Dispose();
	}

	internal void Dispose()
	{
		// kill the native pointer - it does with the native material
		// we want to reduce the risk that someone is holding on to it.
		Attributes?.Set(default);
		Attributes = null;

		if (!native.IsNull)
		{
			var n = native;
			native = default;

			MainThread.Queue(() => n.DestroyStrongHandle());
		}
	}

	/// <summary>
	/// Create a copy of this material
	/// </summary>
	public Material CreateCopy()
	{
		return FromNative(MaterialSystem2.CreateProceduralMaterialCopy(native, 0, true));
	}

	public void SetPass(int pass)
	{
		throw new NotImplementedException();
	}
}
