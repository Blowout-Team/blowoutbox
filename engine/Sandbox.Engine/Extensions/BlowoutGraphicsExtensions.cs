using BlowoutTeamSoft.Engine.Enums.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandbox;

public static class BlowoutGraphicsExtensions
{
	extension( BlowoutTextureFormat format )
	{
		public ImageFormat ToSourceFormat()
		{
			switch ( format )
			{
				case BlowoutTextureFormat.Unknown:
					return ImageFormat.None;

				case BlowoutTextureFormat.R8:
					return ImageFormat.I8;

				case BlowoutTextureFormat.RG8:
					return ImageFormat.IA88;

				case BlowoutTextureFormat.RGBA8:
					return ImageFormat.RGB888;

				case BlowoutTextureFormat.RGB111110Float:
					return ImageFormat.RG11_EAC;

				default:
					return ImageFormat.None;
			}
		}
	}

	extension( BlowoutGraphicsFormat format )
	{
		public ImageFormat ToSourceFormat()
		{
			switch ( format )
			{
				case BlowoutGraphicsFormat.None:
					return ImageFormat.None;

				case BlowoutGraphicsFormat.R8_UNorm:
					return ImageFormat.I8;

				case BlowoutGraphicsFormat.R8G8_UNorm:
					return ImageFormat.IA88;

				case BlowoutGraphicsFormat.R8G8B8_UNorm:
					return ImageFormat.RGB888;

				case BlowoutGraphicsFormat.B8G8R8_UNorm:
					return ImageFormat.BGR888;

				case BlowoutGraphicsFormat.R8G8B8A8_UNorm:
					return ImageFormat.RGBA8888;

				case BlowoutGraphicsFormat.B8G8R8A8_UNorm:
					return ImageFormat.BGRA8888;

				case BlowoutGraphicsFormat.R5G6B5_UNormPack16:
					return ImageFormat.RGB565;

				case BlowoutGraphicsFormat.B5G6R5_UNormPack16:
					return ImageFormat.BGR565;

				case BlowoutGraphicsFormat.B4G4R4A4_UNormPack16:
					return ImageFormat.BGRA4444;

				case BlowoutGraphicsFormat.B5G5R5A1_UNormPack16:
					return ImageFormat.BGRA5551;

				case BlowoutGraphicsFormat.R16_SFloat:
					return ImageFormat.R16F;

				case BlowoutGraphicsFormat.R32_SFloat:
					return ImageFormat.R32F;

				case BlowoutGraphicsFormat.R16G16_SFloat:
					return ImageFormat.RG1616F;

				case BlowoutGraphicsFormat.R32G32_SFloat:
					return ImageFormat.RG3232F;

				case BlowoutGraphicsFormat.R32G32B32_SFloat:
					return ImageFormat.RGB323232F;

				case BlowoutGraphicsFormat.R16G16B16A16_SFloat:
					return ImageFormat.RGBA16161616F;

				case BlowoutGraphicsFormat.R32G32B32A32_SFloat:
					return ImageFormat.RGBA32323232F;

				case BlowoutGraphicsFormat.D16_UNorm:
					return ImageFormat.D16;

				case BlowoutGraphicsFormat.D24_UNorm_S8_UInt:
					return ImageFormat.D24S8;

				case BlowoutGraphicsFormat.D32_SFloat:
					return ImageFormat.D32;

				case BlowoutGraphicsFormat.D32_SFloat_S8_UInt:
					return ImageFormat.D32FS8;

				case BlowoutGraphicsFormat.RGBA_DXT1_UNorm:
					return ImageFormat.DXT1;

				case BlowoutGraphicsFormat.RGBA_DXT3_UNorm:
					return ImageFormat.DXT3;

				case BlowoutGraphicsFormat.RGBA_DXT5_UNorm:
					return ImageFormat.DXT5;

				case BlowoutGraphicsFormat.R_BC4_UNorm:
					return ImageFormat.ATI1N;

				case BlowoutGraphicsFormat.RG_BC5_UNorm:
					return ImageFormat.ATI2N;

				case BlowoutGraphicsFormat.RGB_BC6H_UFloat:
					return ImageFormat.BC6H;

				case BlowoutGraphicsFormat.RGBA_BC7_UNorm:
					return ImageFormat.BC7;

				case BlowoutGraphicsFormat.RGB_ETC2_UNorm:
					return ImageFormat.R8G8B8_ETC2;

				case BlowoutGraphicsFormat.RGBA_ETC2_UNorm:
					return ImageFormat.R8G8B8A8_ETC2_EAC;

				case BlowoutGraphicsFormat.R_EAC_UNorm:
					return ImageFormat.R11_EAC;

				case BlowoutGraphicsFormat.RG_EAC_UNorm:
					return ImageFormat.RG11_EAC;

				default:
					return ImageFormat.None;
			}
		}
	}

	extension(ImageFormat sourceFormat )
	{
		public BlowoutGraphicsFormat ToBlowoutFormat()
		{
			switch ( sourceFormat )
			{
				case ImageFormat.None:
				case ImageFormat.Default:
					return BlowoutGraphicsFormat.None;

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
					Log.Warning("Unknown image format: " + sourceFormat);
					return BlowoutGraphicsFormat.None;
			}
		}
	}
}
