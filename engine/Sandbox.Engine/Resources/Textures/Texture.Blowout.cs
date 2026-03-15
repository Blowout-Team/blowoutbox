using BlowoutTeamSoft.Engine.Enums.Rendering;
using BlowoutTeamSoft.Engine.Interfaces.Rendering;
using BlowoutTeamSoft.Engine.Render;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Sandbox;

public partial class Texture : IBlowoutTexture,
	IBlowoutTexture2D,
	IBlowoutRenderTexture
{
	public BlowoutTextureHandle Handle => new BlowoutTextureHandle( AssetId );

	public BlowoutTextureDimension Dimension
	{
		get
		{
			if ( Desc.IsArray )
				return BlowoutTextureDimension.Texture2DArray;

			if ( Desc.IsCube )
				return BlowoutTextureDimension.Cube;

			if ( Desc.m_nFlags == NativeEngine.RuntimeTextureSpecificationFlags.TSPEC_VOLUME_TEXTURE )
				return BlowoutTextureDimension.Texture3D;

			return BlowoutTextureDimension.Texture2D;
		}
	}

	[IgnoreDataMember, JsonIgnore]
	public BlowoutGraphicsFormat Format
	{
		get => ImageFormat.ToBlowoutFormat();
	}

	[IgnoreDataMember, JsonIgnore]
	public BlowoutTextureAccess Access
	{
		get
		{
			if ( IsRenderTarget )
				return BlowoutTextureAccess.RenderTarget;

			return BlowoutTextureAccess.Sample;
		}
	}

	[IgnoreDataMember, JsonIgnore]
	public int MipCount => Mips;

	public bool IsReadable => IsValid;

	public BlowoutTexture2DHandle Handle2D => new BlowoutTexture2DHandle( AssetId );

	public System.Numerics.Vector3 TexelSize => new System.Numerics.Vector3( Size.x, Size.y, 0f );

	public BlowoutRenderTextureHandle RenderTextureHandle => new BlowoutRenderTextureHandle( AssetId );

	public bool IsRandomWrite { get; set; }

	float IBlowoutTexture.Width => Width;

	float IBlowoutTexture.Height => Height;

	public void Apply()
	{
		MarkUsed();
	}

	public void EnsureCreated()
	{
	}

	public void Flush()
	{
		Dispose();
	}

	public void SetPixels( int mipLevel, ReadOnlySpan<BlowoutColor> pixels )
	{
		Update( pixels );
	}
}
