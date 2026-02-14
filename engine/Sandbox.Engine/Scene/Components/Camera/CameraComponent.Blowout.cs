using BlowoutTeamSoft.Engine.Interfaces.Rendering;
using BlowoutTeamSoft.Engine.Render;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Sandbox;

public partial class CameraComponent : IBlowoutCamera
{
	public BlowoutCameraHandle Handle => new BlowoutCameraHandle( Id.Variant );

	[IgnoreDataMember, Hide]
	public System.Numerics.Matrix4x4 WorldCameraMatrix =>
		sceneCamera.ViewMatrix;

	public Index TargetIndex
	{
		get => Priority;
		set => Priority = value.Value;
	}

	[IgnoreDataMember, Hide]
	public float Aspect
	{
		get
		{
			if ( CustomSize == null )
			{
				return Screen.Size.x / Screen.Size.y;
			}

			return CustomSize.Value.x / CustomSize.Value.y;
		}
		set => CustomSize = new Vector2( value, 1f );
	}

	Matrix4x4 IBlowoutCamera.ProjectionMatrix => ProjectionMatrix;

	public System.Numerics.Vector3 WorldToViewportPoint( System.Numerics.Vector3 point )
	{
		var sr = ScreenRect;
		var v = sceneCamera.ToScreen( point );
		var result = new Vector2( v.x, v.y ) * sr.Size;

		return new System.Numerics.Vector3( result.x, result.y, 0f );
	}

	public System.Numerics.Vector3 WorldToScreenPoint( System.Numerics.Vector3 point )
	{
		var size = new Vector3( ScreenRect.Size.x, ScreenRect.Size.y );
		var v = sceneCamera.ToScreenWithDirection( point );

		var result = v * size;

		return result;
	}
}
