using BlowoutTeamSoft.Engine.Enums.AI;
using BlowoutTeamSoft.Engine.Interfaces.AI;
using DotRecast.Detour;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace Sandbox.Navigation;

public partial class NavMesh : IBlowoutAiNavigation
{
	public System.Numerics.Vector3[] CalculatePath( IBlowoutAiNavigationAgent agent, System.Numerics.Vector3 destination, out BlowoutNavigationPathStatus pathStatus )
	{
		var calculation = CalculatePath( new CalculatePathRequest()
		{
			Agent = agent,
			Start = agent.WorldPosition,
			Target = destination,

		} );

		pathStatus = BlowoutNavigationPathStatus.Success;
		if(calculation.Status != NavMeshPathStatus.Complete)
		{
			switch ( calculation.Status )
			{
				case NavMeshPathStatus.StartNotFound or NavMeshPathStatus.PathNotFound:
					pathStatus = BlowoutNavigationPathStatus.Invalid;
					break;
				case NavMeshPathStatus.Partial:
					pathStatus = BlowoutNavigationPathStatus.Partial;
					break;
				case NavMeshPathStatus.Complete:
					pathStatus = BlowoutNavigationPathStatus.Success;
					break;
				default:
					pathStatus = BlowoutNavigationPathStatus.Fail;
					break;
			}

			if ( pathStatus != BlowoutNavigationPathStatus.Partial || pathStatus != BlowoutNavigationPathStatus.Success )
				return Array.Empty<System.Numerics.Vector3>();
		}

		return calculation.Points.Select( x => x.Position.ToSystemNumerics() ).ToArray();
	}

	public int CalculatePathSpan( IBlowoutAiNavigationAgent agent, System.Numerics.Vector3 destination, Span<System.Numerics.Vector3> buffer, out BlowoutNavigationPathStatus pathStatus )
	{
		NavMeshPath result = new();

		var searchRadius = agent != null ? new Vector3( agent.Radius * 2.01f, agent.Height * 1.51f, agent.Radius * 2.01f ) : crowd._agentPlacementHalfExtents;
		var filter = agent != null && agent is NavMeshAgent nativeAgent ? nativeAgent.agentInternal.option.filter : crowd.GetDefaultFilter();

		var startFound = query.FindNearestPoly( ToNav( agent.WorldPosition ), searchRadius, filter, out var startPoly, out var startLocation, out _ );
		if ( !startFound.Succeeded() )
		{
			pathStatus = BlowoutNavigationPathStatus.Invalid;
			return 0;
		}

		var targetFound = query.FindNearestPoly( ToNav( destination ), searchRadius, filter, out var targetPoly, out var targetLocation, out _ );
		if ( !targetFound.Succeeded() )
		{
			pathStatus = BlowoutNavigationPathStatus.Invalid;
			return 0;
		}

		var dtStatus = query.InitSlicedFindPath( startPoly, targetPoly, startLocation, targetLocation, filter, 0 );
		if ( dtStatus.Failed() )
		{
			pathStatus = BlowoutNavigationPathStatus.Invalid;
			return 0;
		}
		do
		{
			dtStatus = query.UpdateSlicedFindPath( crowd.Config().maxFindPathIterations, out var _ );
			if ( dtStatus.Failed() )
			{
				pathStatus = BlowoutNavigationPathStatus.Invalid;
				return 0;
			}
		} while ( dtStatus.InProgress() );

		result.Polygons = new( 128 );
		dtStatus = query.FinalizeSlicedFindPath( ref result.Polygons );
		if ( dtStatus.Failed() || result.Polygons.Count == 0 )
		{
			pathStatus = BlowoutNavigationPathStatus.Invalid;
			return 0;
		}

		var straightPathCache = ArrayPool<DtStraightPath>.Shared.Rent( 4096 );
		dtStatus = query.FindStraightPath( startLocation, targetLocation, result.Polygons, result.Polygons.Count, straightPathCache, out var filledPointCount, straightPathCache.Length, 0 );
		if ( dtStatus.Failed() )
		{
			ArrayPool<DtStraightPath>.Shared.Return( straightPathCache );
			pathStatus = BlowoutNavigationPathStatus.Invalid;
			return 0;
		}

		filledPointCount = Math.Max( filledPointCount, buffer.Length );
		for ( int i = 0; i < filledPointCount; i++ )
		{
			buffer[i] = FromNav( straightPathCache[i].pos );
		}
		ArrayPool<DtStraightPath>.Shared.Return( straightPathCache );

		if ( result.Polygons[^1] != targetPoly )
		{
			pathStatus = BlowoutNavigationPathStatus.Partial;
		}
		else
		{
			pathStatus = BlowoutNavigationPathStatus.Success;
		}


		return filledPointCount;
	}
}
