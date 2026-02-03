using BlowoutTeamSoft.Engine;
using BlowoutTeamSoft.Engine.Core;
using BlowoutTeamSoft.Engine.Enums;
using BlowoutTeamSoft.Engine.Enums.Math;
using BlowoutTeamSoft.Engine.Interfaces;
using BlowoutTeamSoft.Engine.Interfaces.Physics;
using BlowoutTeamSoft.Engine.Math;
using BlowoutTeamSoft.Engine.NativeHandles;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Sandbox;

public sealed partial class PhysicsBody : IBlowoutPhysicsBody
{
	public BlowoutPhysicsBodyNativeHandle Handle => new BlowoutPhysicsBodyNativeHandle();

	public bool IsPhysicsLocked { get => Sleeping; set => Sleeping = value; }
	public bool IsAutomaticMassCenter { get => !OverrideMassCenter; set => OverrideMassCenter = !value; }

	public System.Numerics.Vector3 WorldMassCenter => MassCenter;

	public bool IsExecuting { get => Component.IsExecuting; set => Component.IsExecuting = value; }

	public bool IsActive => _component.IsActive;

	public bool IsAliveSystem => _component.IsValid;

	public BlowoutSystemMode SystemMode { get => Component.SystemMode; set => Component.SystemMode = value; }

	public BlowoutEngineObject Native => _component.Native;

	IBlowoutTransform IBlowoutPhysicsBody.Transform => _component.Transform;

	System.Numerics.Vector3 IBlowoutPhysicsBody.MassCenter
	{
		get => LocalMassCenter; 
		set
		{
			OverrideMassCenter = true;
			LocalMassCenter = value;
		}
	}
	System.Numerics.Vector3 IBlowoutPhysicsBody.Velocity { get => Velocity; set => Velocity = value; }
	System.Numerics.Vector3 IBlowoutPhysicsBody.AngularVelocity { get => AngularVelocity; set => AngularVelocity = value; }

	BlowoutEngineGameObject IBlowoutGameSystem.GameObject => GameObject;

	public void ApplyForce(System.Numerics.Vector3 velocity, BlowoutMathForceType mode)
	{
		switch (mode)
		{
			case BlowoutMathForceType.Acceleration:
				ApplyForce(new Sandbox.Vector3(velocity.X, velocity.Y, velocity.Z));
				return;
			case BlowoutMathForceType.Velocity:
				ApplyTorque(velocity);
				return;
			case BlowoutMathForceType.Impulse:
				ApplyImpulse(velocity);
				return;
		}
	}

	public void ApplyForce(System.Numerics.Vector3 velocity) =>
		ApplyForce(new Sandbox.Vector3(velocity.X, velocity.Y, velocity.Z));

	public void ApplyForceAtPosition(System.Numerics.Vector3 velocity, System.Numerics.Vector3 position, BlowoutMathForceType mode)
	{
		switch (mode)
		{
			case BlowoutMathForceType.Acceleration:
				ApplyForceAt(position, velocity);
				return;
			case BlowoutMathForceType.Velocity:
				ApplyForceAt(position, velocity);
				return;
			case BlowoutMathForceType.Impulse:
				ApplyImpulseAt(position, velocity);
				return;
		}
	}

	public void ApplyForceAtPosition(System.Numerics.Vector3 velocity, System.Numerics.Vector3 position) =>
		ApplyForceAt(position, velocity);

	public void ApplyImpulse(float power, System.Numerics.Vector3 perpetrator, float radius)
	{
		var sourceVector = new Sandbox.Vector3(perpetrator.X, perpetrator.Y, perpetrator.Z);
		Vector3 direction = Position - sourceVector;
		float distance = direction.Length;

		if (distance <= BlowoutMath.EPSILON)
			return;

		if (distance > radius)
			return;

		direction /= distance;

		float attenuation = 1f - (distance / radius);
		float impulseStrength = power * attenuation;

		Vector3 impulse = direction * impulseStrength;

		native.ApplyLinearImpulse(impulse);
	}

	public System.Numerics.Vector3 GetVelocityAtPoint(System.Numerics.Vector3 point) =>
		GetVelocityAtPoint(new Sandbox.Vector3(point.X, point.Y, point.Z));

	public void PhysicsAwake()
	{
		Sleeping = false;
	}

	public void PhysicsSleep()
	{
		Sleeping = true;
	}

	public void SetPosition(System.Numerics.Vector3 position)
	{
		Move(Transform.WithPosition(position), Time.Delta);
	}

	public void SetRotation(Quaternion rotation)
	{
		Move(Transform.WithRotation(rotation), Time.Delta);
	}
}
