using BlowoutTeamSoft.Engine.Enums.Rendering;
using BlowoutTeamSoft.Engine.Interfaces.Rendering;
using BlowoutTeamSoft.Engine.Render;
using System;
using System.Collections.Generic;
using System.Text;
using static Sandbox.VertexLayout;

namespace Sandbox;

public partial class Texture : IBlowoutTexture,
	IBlowoutTexture2D,
	IBlowoutRenderTexture
{
	public BlowoutTextureHandle Handle => new BlowoutTextureHandle(AssetId);

	public BlowoutTextureDimension Dimension
	{
		get
		{
			if (Desc.IsArray)
				return BlowoutTextureDimension.Texture2DArray;

			if (Desc.IsCube)
				return BlowoutTextureDimension.Cube;

			if (Desc.m_nFlags == NativeEngine.RuntimeTextureSpecificationFlags.TSPEC_VOLUME_TEXTURE)
				return BlowoutTextureDimension.Texture3D;

			return BlowoutTextureDimension.Texture2D;
		}
	}

	public BlowoutGraphicsFormat Format
	{
		get
		{
			switch (ImageFormat)
			{
				case ImageFormat.None:
				case ImageFormat.Default:
					return BlowoutGraphicsFormat.None;

				// --- 8 bit ---
				case ImageFormat.I8:
					return BlowoutGraphicsFormat.R8_UNorm;

				case ImageFormat.A8:
					return BlowoutGraphicsFormat.R8_UNorm;

				case ImageFormat.IA88:
					return BlowoutGraphicsFormat.R8G8_UNorm;

				case ImageFormat.RGB888:
					return BlowoutGraphicsFormat.R8G8B8_UNorm;

				case ImageFormat.BGR888:
					return BlowoutGraphicsFormat.B8G8R8_UNorm;

				case ImageFormat.RGBA8888:
					return BlowoutGraphicsFormat.R8G8B8A8_UNorm;

				case ImageFormat.ABGR8888:
				case ImageFormat.BGRA8888:
					return BlowoutGraphicsFormat.B8G8R8A8_UNorm;

				case ImageFormat.ARGB8888:
					return BlowoutGraphicsFormat.B8G8R8A8_UNorm;

				case ImageFormat.RGB565:
					return BlowoutGraphicsFormat.R5G6B5_UNormPack16;

				case ImageFormat.BGR565:
					return BlowoutGraphicsFormat.B5G6R5_UNormPack16;

				case ImageFormat.BGRA4444:
					return BlowoutGraphicsFormat.B4G4R4A4_UNormPack16;

				case ImageFormat.BGRA5551:
					return BlowoutGraphicsFormat.B5G5R5A1_UNormPack16;

				case ImageFormat.BGRX5551:
					return BlowoutGraphicsFormat.B5G5R5A1_UNormPack16;

				case ImageFormat.R16F:
					return BlowoutGraphicsFormat.R16_SFloat;

				case ImageFormat.R32F:
					return BlowoutGraphicsFormat.R32_SFloat;

				case ImageFormat.RG1616F:
					return BlowoutGraphicsFormat.R16G16_SFloat;

				case ImageFormat.RG3232F:
					return BlowoutGraphicsFormat.R32G32_SFloat;

				case ImageFormat.RGB323232F:
					return BlowoutGraphicsFormat.R32G32B32_SFloat;

				case ImageFormat.RGBA16161616F:
					return BlowoutGraphicsFormat.R16G16B16A16_SFloat;

				case ImageFormat.RGBA32323232F:
					return BlowoutGraphicsFormat.R32G32B32A32_SFloat;

				case ImageFormat.RGBA16161616:
					return BlowoutGraphicsFormat.R16G16B16A16_UNorm;

				case ImageFormat.RG1616:
					return BlowoutGraphicsFormat.R16G16_UNorm;

				case ImageFormat.R16:
					return BlowoutGraphicsFormat.R16_UNorm;

				case ImageFormat.R32_UINT:
					return BlowoutGraphicsFormat.R32_UInt;

				case ImageFormat.D16:
				case ImageFormat.D16_SHADOW:
					return BlowoutGraphicsFormat.D16_UNorm;

				case ImageFormat.D24X8:
					return BlowoutGraphicsFormat.D24_UNorm;

				case ImageFormat.D24S8:
				case ImageFormat.LINEAR_D24S8:
					return BlowoutGraphicsFormat.D24_UNorm_S8_UInt;

				case ImageFormat.D32:
					return BlowoutGraphicsFormat.D32_SFloat;

				case ImageFormat.D32FS8:
					return BlowoutGraphicsFormat.D32_SFloat_S8_UInt;

				case ImageFormat.DXT1:
				case ImageFormat.DXT1_ONEBITALPHA:
					return BlowoutGraphicsFormat.RGBA_DXT1_UNorm;

				case ImageFormat.DXT3:
					return BlowoutGraphicsFormat.RGBA_DXT3_UNorm;

				case ImageFormat.DXT5:
				case ImageFormat.DXT5_NM:
					return BlowoutGraphicsFormat.RGBA_DXT5_UNorm;

				case ImageFormat.ATI1N:
					return BlowoutGraphicsFormat.R_BC4_UNorm;

				case ImageFormat.ATI2N:
					return BlowoutGraphicsFormat.RG_BC5_UNorm;

				case ImageFormat.BC6H:
					return BlowoutGraphicsFormat.RGB_BC6H_UFloat;

				case ImageFormat.BC7:
					return BlowoutGraphicsFormat.RGBA_BC7_UNorm;

				case ImageFormat.R8G8B8_ETC2:
					return BlowoutGraphicsFormat.RGB_ETC2_UNorm;

				case ImageFormat.R8G8B8A8_ETC2_EAC:
					return BlowoutGraphicsFormat.RGBA_ETC2_UNorm;

				case ImageFormat.R11_EAC:
					return BlowoutGraphicsFormat.R_EAC_UNorm;

				case ImageFormat.RG11_EAC:
					return BlowoutGraphicsFormat.RG_EAC_UNorm;

				default:
					return BlowoutGraphicsFormat.None;
			}
		}
	}

	public BlowoutTextureAccess Access
	{
		get
		{
			if (IsRenderTarget)
				return BlowoutTextureAccess.RenderTarget;

			return BlowoutTextureAccess.Sample;
		}
	}

	public int MipCount => Mips;

	public bool IsReadable => throw new NotImplementedException();

	public BlowoutTexture2DHandle Handle2D => new BlowoutTexture2DHandle(AssetId);

	public System.Numerics.Vector3 TexelSize => new System.Numerics.Vector3(Size.x, Size.y, 0f);

	public BlowoutRenderTextureHandle RenderTextureHandle => throw new NotImplementedException();

	public bool IsRandomWrite { get; set; }

	float IBlowoutTexture.Width => Width;

	float IBlowoutTexture.Height => Height;

	public void Apply()
	{
	}

	public void EnsureCreated()
	{
	}

	public void Flush()
	{
		Dispose();
	}

	public void SetPixels(int mipLevel, ReadOnlySpan<BlowoutColor> pixels)
	{
		Update(pixels);
	}
}
