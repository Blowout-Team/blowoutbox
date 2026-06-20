using BlowoutTeamSoft.Engine;
using BlowoutTeamSoft.Engine.Geometry.Mesh;
using BlowoutTeamSoft.Engine.Interfaces.Geometry;
using BlowoutTeamSoft.Engine.Interfaces.Mesh;
using BlowoutTeamSoft.Engine.Render;
using BlowoutTeamSoft.Engine.Validators;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NativeEngine;
using NoAlloq;
using System.Linq;
using System.Runtime.InteropServices;

namespace Sandbox
{
	/// <summary>
	/// Possible primitive types of a <see cref="Mesh"/>.
	/// </summary>
	public enum MeshPrimitiveType
	{
		Points,
		Lines,
		LineStrip,
		Triangles,
		TriangleStrip
	}

	/// <summary>
	/// A <a href="https://en.wikipedia.org/wiki/Polygon_mesh">mesh</a> is a basic version of a <see cref="Model"/>,
	/// containing a set of vertices and indices which make up faces that make up a shape.
	///
	/// <para>A set of meshes can be used to create a <see cref="Model"/> via the <see cref="ModelBuilder"/> class.</para>
	/// </summary>
	public partial class Mesh : IBlowoutMesh, IValid, IBlowoutDynamicMesh
	{
		internal IMesh native;
		internal long instanceId;
		private int renderPrimType = (int)RenderPrimitiveType.RENDER_PRIM_TRIANGLES;

		private Model _coreModel;

		public Mesh() : this( null, MeshPrimitiveType.Triangles )
		{

		}

		public Mesh( Material material, MeshPrimitiveType primType = MeshPrimitiveType.Triangles ) : this( "mesh", material, primType )
		{
		}

		public Mesh( string name, Material material, MeshPrimitiveType primType = MeshPrimitiveType.Triangles )
		{
			if ( string.IsNullOrWhiteSpace( name ) )
				name = "mesh";

			renderPrimType = (int)MeshPrimTypeToRenderPrimType( primType );
			native = MeshGlue.CreateRenderMesh( material != null ? material.native : IntPtr.Zero, renderPrimType, name );

			if ( native.IsNull ) throw new Exception( "RenderMesh pointer cannot be null!" );

			instanceId = native.GetBindingPtr().ToInt64();
		}

		private Mesh( IMesh native, long instanceId )
		{
			if ( native.IsNull ) throw new Exception( "RenderMesh pointer cannot be null!" );

			this.native = native;
			this.instanceId = instanceId;
		}

		~Mesh()
		{
			var n = native;
			native = default;

			MainThread.Queue( () => n.DestroyStrongHandle() );
		}

		/// <inheritdoc cref="IValid.IsValid"/>
		public bool IsValid => native.IsValid && native.IsStrongHandleValid();

		public BlowoutValidatorResult Validate()
		{
			if ( !native.IsValid )
				return BlowoutValidatorResult.WithError("Native type is nullptr");
			return BlowoutValidatorResult.Success;
		}

		/// <summary>
		/// Sets the primitive type for this mesh.
		/// </summary>
		public MeshPrimitiveType PrimitiveType
		{
			set
			{
				renderPrimType = (int)MeshPrimTypeToRenderPrimType( value );
				MeshGlue.SetMeshPrimType( native, renderPrimType );
			}
		}

		/// <summary>
		/// Sets material for this mesh.
		/// </summary>
		public Material Material
		{
			set => MeshGlue.SetMeshMaterial( native, value != null ? value.native : IntPtr.Zero );
		}

		/// <summary>
		/// Add a sub mesh, drawing a range of the index buffer with its own <paramref name="material"/>.
		/// Create the vertex and index buffers before adding sub meshes.
		/// </summary>
		/// <param name="material">Material to draw this range with.</param>
		/// <param name="startIndex">First index of the range to draw.</param>
		/// <param name="indexCount">Number of indices to draw.</param>
		/// <param name="startVertex">Base vertex offset. Defaults to the whole vertex buffer when zero.</param>
		/// <param name="vertexCount">Number of vertices referenced. Defaults to the whole vertex buffer when zero.</param>
		public Mesh AddSubMesh( Material material, int startIndex, int indexCount, int startVertex = 0, int vertexCount = 0 )
		{
			MeshGlue.AddMeshDrawCall( native, material != null ? material.native : IntPtr.Zero, renderPrimType, startIndex, indexCount, startVertex, vertexCount );
			return this;
		}

		/// <summary>
		/// Sets AABB bounds for this mesh.
		/// </summary>
		public BBox Bounds
		{
			set
			{
				MeshGlue.SetMeshBounds( native, value.Mins, value.Maxs );
			}

			// TODO - get
		}

		/// <summary>
		/// Used to calculate texture size for texture streaming.
		/// </summary>
		public float UvDensity
		{
			set
			{
				MeshGlue.SetMeshUvDensity( native, value );
			}
		}

		private BBox _calculated;
		IBlowoutBounds IBlowoutModel.Bounds
		{
			get
			{
				if ( !HasVertexBuffer )
					return new BBox();

				return _calculated;
			}
		}

		public bool IsProcedural => true;

		public BlowoutMeshId MeshHandle => new BlowoutMeshId( instanceId );

		public IEnumerable<System.Numerics.Vector3> Vertices 
		{ 
			get 
			{
				if ( !HasVertexBuffer )
					return Enumerable.Empty<System.Numerics.Vector3>();

				System.Numerics.Vector3[] vertices = Array.Empty<System.Numerics.Vector3>();
				LockVertexBuffer<Vertex>((x) => vertices = x.Select(x=> x.Position.ToSystemNumerics()).ToArray());

				return vertices;
			}
			set
			{
				var vertices = value.Select( x => new Vertex(x) ).ToList();
				CreateBuffers(new VertexBuffer(vertices));
			}
		}

		public IEnumerable<BlowoutColor> Colors
		{
			get
			{
				if ( !HasVertexBuffer )
					return Enumerable.Empty<BlowoutColor>();

				BlowoutColor[] colors = Array.Empty<BlowoutColor>();
				LockVertexBuffer<Vertex>( ( x ) => colors = x.Select( x => x.Color.ToColor().ToBlowoutColor() ).ToArray() );

				return colors;
			}
			set
			{
				List<Vertex> vertices = new List<Vertex>( value.Count() );
				var enumerator = value.GetEnumerator();
				LockVertexBuffer<Vertex>( ( x ) =>
				{
					
					for ( int i = 0; i < x.Length; i++ )
					{
						if ( !enumerator.MoveNext() )
							return;
						x[i].Color = enumerator.Current.ToColor32();
					}
				} );
			}
		}

		/// <summary>
		/// Set how many vertices this mesh draws (if there's no index buffer)
		/// </summary>
		public void SetVertexRange( int start, int count )
		{
			MeshGlue.SetMeshVertexRange( native, start, count );
		}

		/// <summary>
		/// Set how many indices this mesh draws
		/// </summary>
		public void SetIndexRange( int start, int count )
		{
			MeshGlue.SetMeshIndexRange( native, start, count );
		}

		/// <summary>
		/// Create vertex and index buffers.
		/// </summary>
		/// <param name="vb">Input vertex buffer. If it is indexed (<see cref="VertexBuffer.Indexed"/>), then index buffer will also be created.</param>
		/// <param name="calculateBounds">Whether to recalculate bounds from the vertex buffer.</param>
		public void CreateBuffers( VertexBuffer vb, bool calculateBounds = true )
		{
			var vertices_span = CollectionsMarshal.AsSpan( vb.Vertex );
			CreateVertexBuffer( vb.Vertex.Count, vertices_span );

			if ( vb.Indexed )
			{
				// This sucks but probably temp
				var indices = new int[vb.Index.Count];
				for ( int i = 0; i < indices.Length; ++i )
				{
					indices[i] = vb.Index[i];
				}

				CreateIndexBuffer( vb.Index.Count, indices );
			}

			if ( calculateBounds )
			{
				var bounds = new BBox();
				foreach ( var v in vb.Vertex )
				{
					bounds = bounds.AddPoint( v.Position );
				}

				_calculated = bounds;
				Bounds = bounds;
			}
		}

		private static RenderPrimitiveType MeshPrimTypeToRenderPrimType( MeshPrimitiveType primType )
		{
			RenderPrimitiveType renderPrimType;
			switch ( primType )
			{
				case MeshPrimitiveType.Points:
					renderPrimType = RenderPrimitiveType.RENDER_PRIM_POINTS;
					break;
				case MeshPrimitiveType.Lines:
					renderPrimType = RenderPrimitiveType.RENDER_PRIM_LINES;
					break;
				case MeshPrimitiveType.LineStrip:
					renderPrimType = RenderPrimitiveType.RENDER_PRIM_LINE_STRIP;
					break;
				case MeshPrimitiveType.Triangles:
					renderPrimType = RenderPrimitiveType.RENDER_PRIM_TRIANGLES;
					break;
				case MeshPrimitiveType.TriangleStrip:
					renderPrimType = RenderPrimitiveType.RENDER_PRIM_TRIANGLE_STRIP;
					break;
				default:
					renderPrimType = RenderPrimitiveType.RENDER_PRIM_TRIANGLES;
					break;
			}

			return renderPrimType;
		}

		/// <summary>
		/// Triangulate a polygon made up of points, returns triangle indices into the list of vertices.
		/// </summary>
		public static unsafe Span<int> TriangulatePolygon( Span<Vector3> vertices )
		{
			if ( vertices.Length < 3 )
				return default;

			var vertexCount = vertices.Length;
			var indexCount = (vertexCount - 2) * 3;
			var indices = new int[indexCount];

			fixed ( int* pIndices = indices )
			fixed ( Vector3* pVertices = vertices )
			{
				indexCount = MeshGlue.TriangulatePolygon( (IntPtr)pVertices, vertices.Length, (IntPtr)pIndices, indexCount );
				return indices.AsSpan( 0, indexCount );
			}
		}

		internal static void ClipPolygon( Span<Vector3> vertices, Vector3 a, Vector3 b, out Vector3[] outVertices )
		{
			unsafe
			{
				fixed ( Vector3* pVertices = vertices )
				{
					var arrVectors = CUtlVectorVector.Create( 0, 0 );
					MeshGlue.ClipPolygonLineSegment( (IntPtr)pVertices, vertices.Length, a, b, arrVectors );

					outVertices = new Vector3[arrVectors.Count()];
					for ( var i = 0; i < outVertices.Length; ++i )
						outVertices[i] = arrVectors.Element( i );
					arrVectors.DeleteThis();
				}
			}
		}

		public Model ToModel()
		{
			_coreModel ??= new ModelBuilder().AddMesh( this ).Create();

			return _coreModel;
		}

		public void Dispose()
		{
			var n = native;
			native = default;

			MainThread.Queue( () => n.DestroyStrongHandle() );
			GC.SuppressFinalize( this );
		}

		public void Perform( BlowoutDynamicMeshBuilder builder )
		{
			var vertices = builder.Vertices.Zip( builder.Colors, builder.Normals.Zip( builder.Tangents, builder.UV ) )
				.Select( x => new Vertex( x.First) { Normal = x.Third.First, TexCoord0 = new Vector4(x.Third.Third.X, x.Third.Third.Y), Color = x.Second.ToColor32(), Tangent = x.Third.Second } ).ToList();
			
			CreateBuffers( new VertexBuffer( vertices ) );
		}

		public void SetBounds( IBlowoutBounds bounds ) =>
			Bounds = new BBox(bounds.Min, bounds.Max);
	}
}
