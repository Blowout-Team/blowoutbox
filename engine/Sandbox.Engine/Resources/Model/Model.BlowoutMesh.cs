using BlowoutTeamSoft.Engine.Exceptions;
using BlowoutTeamSoft.Engine.Interfaces.Geometry;
using BlowoutTeamSoft.Engine.Interfaces.Mesh;
using BlowoutTeamSoft.Engine.Render;
using NoAlloq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandbox;

public partial class Model : IBlowoutMesh, IBlowoutModel
{
	public BlowoutMeshId MeshHandle => new BlowoutMeshId( AssetId );

	public IEnumerable<System.Numerics.Vector3> Vertices
	{
		get
		{
			//dehs: unsafe?
			Span<Vertex> buffer = stackalloc Vertex[VertexCount];
			var count = GetVerticesSpan( buffer );
			foreach ( var position in buffer[..count].Select( x => x.Position ) )
			{
				yield return position;
			}
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
			foreach ( var color in buffer[..count].Select( x => x.Color ) )
			{
				yield return color.ToColor().ToBlowoutColor();
			}
		}
		set
		{
			throw new BlowoutEngineException( "It is not possible to set the group vertex for an already pre-built model. For such operations, use Mesh (IBlowoutMesh) or dynamic Mesh." );
		}
	}

	IBlowoutBounds IBlowoutModel.Bounds => Bounds;

	void IDisposable.Dispose()
	{
		Dispose();
	}
}
