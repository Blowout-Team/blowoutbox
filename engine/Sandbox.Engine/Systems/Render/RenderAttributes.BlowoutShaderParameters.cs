using BlowoutTeamSoft.Engine.Exceptions;
using BlowoutTeamSoft.Engine.Interfaces.Rendering;
using BlowoutTeamSoft.Engine.Interfaces.Shaders;
using BlowoutTeamSoft.Engine.Render;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Sandbox;

public partial class RenderAttributes : IBlowoutShaderParameters
{
	public bool GetBool(string parameterName) =>
		GetBool(parameterName, false);

	public float GetFloat(string parameterName) =>
		GetFloat(parameterName, 0f);

	public int GetInt(string parameterName) =>
		GetInt(parameterName, 0);

	public Matrix4x4 GetMatrix4x4(string parameterName) =>
		GetMatrix(parameterName, Matrix.Identity);

	public IBlowoutTexture GetTexture(string parameterName) =>
		GetTexture(parameterName, null);

	public void SetBool(string parameterName, bool value) =>
		Set(parameterName, value);

	public void SetColor(string parameterName, BlowoutColor color) =>
		Set(parameterName, new Color(color.R, color.G, color.B, color.A));

	public void SetFloat(string parameterName, float value) =>
		Set(parameterName, value);

	public void SetInt(string parameterName, int value) =>
		Set(parameterName, value);

	public void SetMatrix4x4(string parameterName, Matrix4x4 value) =>
		Set(parameterName, value);

	public void SetTexture(string parameterName, IBlowoutTexture texture)
	{
		if(texture is Texture sourceTexture)
		{
			Set(parameterName, sourceTexture);
		}

		throw new BlowoutEngineException("it is not possible to set non Source Texture");
	}

	public void SetTexture(string parameterName, BlowoutDynamicRenderTextureHandle texture)
	{
		if (texture.Handle == null)
			return;

		if (texture.Handle is Texture sourceTexture)
		{
			Set(parameterName, sourceTexture);
		}

		throw new BlowoutEngineException("it is not possible to set non Source Texture");
	}

	public void SetVector(string parameterName, System.Numerics.Vector2 value)
	{
		Set(parameterName, value);
	}

	public void SetVector(string parameterName, System.Numerics.Vector3 value)
	{
		Set(parameterName, value);
	}
}
