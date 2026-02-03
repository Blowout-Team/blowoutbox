using BlowoutTeamSoft.Engine.Interfaces;
using BlowoutTeamSoft.Engine.Interfaces.Geometry;
using BlowoutTeamSoft.Engine.Interfaces.Physics;
using BlowoutTeamSoft.Engine.NativeHandles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandbox;

public abstract partial class Collider : IBlowoutPhysicsCollision
{
	public BlowoutPhysicsCollisionNativeHandle Handle => new BlowoutPhysicsCollisionNativeHandle();

	public System.Numerics.Vector3 Impulse => SurfaceVelocity;

	public bool IsTriggerMode { get => IsTrigger; set => IsTrigger = value; }

	public IBlowoutBounds Bounds => LocalBounds;

	public IBlowoutPhysicsBody Physics => PhysicsBody;

	IBlowoutTransform IBlowoutPhysicsCollision.Transform => Transform;

	IBlowoutPhysicsSurface IBlowoutPhysicsCollision.Surface { get => Surface; set => new Surface(value); }
}
