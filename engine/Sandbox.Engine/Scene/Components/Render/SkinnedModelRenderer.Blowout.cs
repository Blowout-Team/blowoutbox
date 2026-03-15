using BlowoutTeamSoft.Engine.Animator;
using BlowoutTeamSoft.Engine.Core;
using BlowoutTeamSoft.Engine.Enums;
using BlowoutTeamSoft.Engine.Interfaces;
using BlowoutTeamSoft.Engine.Interfaces.Animator;
using BlowoutTeamSoft.Engine.Interfaces.Animator.Layers;
using BlowoutTeamSoft.Engine.Interfaces.Physics;
using BlowoutTeamSoft.Engine.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Text;
using System.Threading;

namespace Sandbox;

public partial class SkinnedModelRenderer : IAnimator, ISkeletonAnimator, IPhysicsAnimator
{
	public bool IsPhysicsMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public bool IsHumanoid { get; }

	public int MainLayerId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

	public bool IsEnabled => Enabled;

	public System.Numerics.Vector3 RootMotionPosition => RootMotion.Position;

	public Quaternion RootMotionRotation => RootMotion.Rotation;

	public IAnimatorLayerSystem LayerSystem => throw new NotImplementedException();

	public IAnimatorAnimationInvoker Invoker => throw new NotImplementedException();

	public IAnimationSyncMaster SyncMaster => throw new NotImplementedException();

	public System.Numerics.Vector3 PivotPosition => throw new NotImplementedException();

	public void ClearIkEffector( string name ) =>
		ClearIk( name );

	public void Dispose()
	{
	}

	public IAnimation FindAnimation( string name )
	{
		throw new NotImplementedException();
	}

	public System.Numerics.Vector3 GetAngularBoneVelocity( int boneIndex ) =>
		GetBoneVelocity( boneIndex ).Angular;

	public BlowoutEngineGameObject GetBone( string name ) =>
		GetBoneObject( name );

	public BlowoutEngineGameObject GetBone( BlowoutBoneType type, BlowoutSide side ) =>
		default;
		//GetBoneObject(,)

	public BlowoutEngineGameObject GetBone( BlowoutBoneType type )
	{
		throw new NotImplementedException();
	}

	public BlowoutEngineGameObject GetBone( int index ) =>
		GetBoneObject( index );

	public BlowoutAnimationInfo GetCurrentAnimation( int layerIndex )
	{
		throw new NotImplementedException();
	}

	public float GetLinearBoneVelocity( int boneIndex )
	{
		throw new NotImplementedException();
	}

	public T GetValue<T>( Enum value ) where T : struct
	{
		throw new NotImplementedException();
	}

	public T GetValue<T>( string value ) where T : struct
	{
		throw new NotImplementedException();
	}

	public bool HasValue( Enum value )
	{
		throw new NotImplementedException();
	}

	public bool HasValue( string valueName )
	{
		if ( UseAnimGraph )
		{
			var param = AnimationGraph.GetParameterFromList( valueName );
			if ( !param.IsValid || !param.IsNull )
				return false;

			return true;
		}

		return SceneModel.CurrentSequence.SequenceNames.Contains( valueName );
	}

	public void Play( string stateName )
	{
		SceneModel.DirectPlayback.Play( stateName );
	}

	public void PlayState( string stateName, TimeSpan crossFade )
	{
		SceneModel.DirectPlayback.Play( stateName, Vector3.Zero, 0f, crossFade.Microseconds );
	}

	public void PlayState( string stateName, int layer, TimeSpan crossFade )
	{
		//if ( UseAnimGraph )
		//{
		//	SceneModel.SetAnimParameter(stateName, true);
		//	return;
		//}

		SceneModel.DirectPlayback.Play( stateName, Vector3.Zero, 1f, crossFade.Microseconds );
	}

	public IEnumerable<IAnimation> QueryAnimations( string name )
	{
		
		throw new NotImplementedException();
	}

	public void ReplaceAnimation( IAnimation animation )
	{
		throw new NotImplementedException();
	}

	public void ReplaceAnimation( string name, IAnimation animation )
	{
		throw new NotImplementedException();
	}

	public void SetIkEffector( string name, IBlowoutTransform target ) =>
		SetIk( name, new Transform( target.WorldPosition, target.WorldRotation, target.WorldScale ) );

	public void SetLookDirection( string name, System.Numerics.Vector3 eyeDirectionWorld ) =>
		SetLookDirection( name, eyeDirectionWorld, 1f );

	public void SetLookDirection( string name, BlowoutWeight<System.Numerics.Vector3> eyeDirectionWorld ) =>
		SetLookDirection( name, eyeDirectionWorld.Value, eyeDirectionWorld.Weight );

	public void SetValue<T>( string valueName, T value )
	{
		throw new NotImplementedException();
	}

	public void Stop()
	{
		SceneModel.DirectPlayback.Cancel();
	}

	public ValueTask WaitForAnimationAsync( string name, CancellationToken token = default )
	{
		throw new NotImplementedException();
	}

	public ValueTask WaitForAnimationAsync( string name, int layerId, CancellationToken token = default )
	{
		throw new NotImplementedException();
	}

	public IEnumerable WaitForAnimationCoroutine( string name, CancellationToken token = default )
	{
		throw new NotImplementedException();
	}

	public IEnumerable WaitForAnimationCoroutine( string name, int layerId, CancellationToken token = default )
	{
		throw new NotImplementedException();
	}

	public ValueTask WaitForEndAnimationAsync( string name, CancellationToken token = default )
	{
		throw new NotImplementedException();
	}

	public ValueTask WaitForEndAnimationAsync( string name, int layerId, CancellationToken token = default )
	{
		throw new NotImplementedException();
	}

	public IEnumerable WaitForEndAnimationCoroutine( string name, CancellationToken token = default )
	{
		throw new NotImplementedException();
	}

	public IEnumerable WaitForEndAnimationCoroutine( string name, int layerId, CancellationToken token = default )
	{
		throw new NotImplementedException();
	}

	ImmutableArray<IBlowoutTransform> IPhysicsAnimator.GetBoneTransforms( bool isWorld )
	{
		throw new NotImplementedException();
	}
}
