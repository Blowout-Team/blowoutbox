using BlowoutTeamSoft.Engine.Exceptions;
using BlowoutTeamSoft.Engine.Interfaces.Geometry;
using BlowoutTeamSoft.Engine.Interfaces.Mesh;
using BlowoutTeamSoft.Engine.Render;
using BlowoutTeamSoft.Engine.Validators;
using NoAlloq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandbox;

public partial class Model : IBlowoutMesh, IBlowoutModel
{
	public BlowoutMeshId MeshHandle => new BlowoutMeshId( ShortAssetId );

	public IEnumerable<System.Numerics.Vector3> Vertices
	{
		get
		{
			Span<Vertex> buffer = stackalloc Vertex[VertexCount];
			var count = GetVerticesSpan( buffer );

			return buffer[..count].Select( x => x.Position.ToSystemNumerics() ).ToArray();
		}
		set
		{
			throw new BlowoutEngineException( "It is not possible to set the group vertex for an already pre-built model. For such operations, use Mesh (IBlowoutMesh) or dynamic Mesh." );
		}
	}
	public IEnumerable<BlowoutColor> Colors
	{
		get
		{
			Span<Vertex> buffer = stackalloc Vertex[VertexCount];
			var count = GetVerticesSpan( buffer );
			return buffer[..count].Select( x => x.Color.ToColor().ToBlowoutColor() ).ToArray();
		}
		set
		{
			throw new BlowoutEngineException( "It is not possible to set the group vertex for an already pre-built model. For such operations, use Mesh (IBlowoutMesh) or dynamic Mesh." );
		}
	}

	IBlowoutBounds IBlowoutModel.Bounds => Bounds;

	public BlowoutValidatorResult Validate()
	{
		if ( !IsValid )
			return BlowoutValidatorResult.WithError("Native handle is nullptr");

		return BlowoutValidatorResult.Success;
	}
}
