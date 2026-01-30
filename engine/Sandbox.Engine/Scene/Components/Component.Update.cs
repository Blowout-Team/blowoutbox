using BlowoutTeamSoft.Engine.Core.Interfaces;
using BlowoutTeamSoft.Engine.Interfaces;

namespace Sandbox;

public abstract partial class Component
{
	private IBlowoutTickable _tickable;
	private IBlowoutLateTickable _lateTickable;

	/// <summary>
	/// Called once before the first Update - when enabled.
	/// </summary>
	protected virtual void OnStart() { }

	/// <summary>
	/// When enabled, called every frame
	/// </summary>
	protected virtual void OnUpdate() { }

	/// <summary>
	/// When enabled, called on a fixed interval that is determined by the Scene. This
	/// is also the fixed interval in which the physics are ticked. Time.Delta is that
	/// fixed interval.
	/// </summary>
	protected virtual void OnFixedUpdate() { }

	bool _startCalled;

	internal void InternalOnStart()
	{
		if ( !Enabled ) return;
		if ( !ShouldExecute ) return;

		if ( _startCalled ) return;

		// Disable any interpolation during OnStart. We might be created in a Fixed Update context.
		using ( GameTransform.DisableInterpolation() )
		{
			Scene.pendingStartComponents.Remove( this );
			_startCalled = true;
			ExceptionWrap( "Start", OnStart );
			if (this is IBlowoutGameSystemLifecycle lifecycle)
				ExceptionWrap("Loaded", lifecycle.Loaded);

			if ( Scene is not null && !Scene.IsEditor )
			{
				ExceptionWrap( "Start", OnComponentStart );
			}
		}
	}

	internal virtual void InternalUpdate()
	{
		if ( !Enabled ) return;
		if ( !ShouldExecute ) return;

		InternalOnStart();
		ExceptionWrap( "Update", OnUpdate );
		if (_tickable != null)
			ExceptionWrap("Tick", () => _tickable.OnTick(Time.Delta));

		if ( Scene is not null && !Scene.IsEditor )
		{
			ExceptionWrap( "Update", OnComponentUpdate );
		}
	}

	internal virtual void InternalFixedUpdate()
	{
		if ( !Enabled ) return;
		if ( !ShouldExecute ) return;

		InternalOnStart();

		if ( Scene is not null && !Scene.IsEditor )
		{
			ExceptionWrap( "FixedUpdate", OnComponentFixedUpdate );
		}
	}
}
