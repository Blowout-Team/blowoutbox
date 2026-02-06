using BlowoutTeamSoft.Engine.Enums;
using BlowoutTeamSoft.Engine.Extensions;
using BlowoutTeamSoft.Engine.Logger;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandbox.Engine.Extensions;

public static class BlowoutGameSystemExtensions
{
	extension(GameObjectFlags flags)
	{
		public BlowoutSystemMode ToSystemMode()
		{
			BlowoutSystemMode result = BlowoutSystemMode.None;
			if ( flags.HasFlag( GameObjectFlags.Hidden ) )
				result |= BlowoutSystemMode.HideHierarchy;
			if ( flags.HasFlag( GameObjectFlags.NotSaved ) )
				result |= BlowoutSystemMode.NotSavedYet;
			if ( flags.HasFlag( GameObjectFlags.NotNetworked ) )
				result |= BlowoutSystemMode.ExcludeNetwork;
			if ( flags.HasFlag( GameObjectFlags.NotSaved ) )
				result |= BlowoutSystemMode.EditorReadonly;

			return result;
		}
	}

	extension(ComponentFlags flags)
	{
		public BlowoutSystemMode ToSystemMode()
		{
			BlowoutSystemMode result = BlowoutSystemMode.None;

			if ( flags.HasFlag( ComponentFlags.NotSaved ) )
				result |= BlowoutSystemMode.NotSavedYet;
			if ( flags.HasFlag( ComponentFlags.Hidden ) )
				result |= BlowoutSystemMode.HideHierarchy;
			if ( flags.HasFlag( ComponentFlags.NotCloned ) )
				result |= BlowoutSystemMode.NotClonable;
			if ( flags.HasFlag( ComponentFlags.NotNetworked ) )
				result |= BlowoutSystemMode.ExcludeNetwork;
			if ( flags.HasFlag( ComponentFlags.NotSaved ) )
				result |= BlowoutSystemMode.EditorReadonly;

			//Log.Info("Unknown game system mode for Source 2: " + mode);
			return result;
		}
	}

	extension(BlowoutSystemMode mode)
	{
		public GameObjectFlags ToSourceFlags()
		{
			GameObjectFlags result = GameObjectFlags.None;

			if ( mode.HasFlag( BlowoutSystemMode.HideHierarchy ) )
				result |= GameObjectFlags.Hidden;
			if ( mode.HasFlag( BlowoutSystemMode.ExcludeNetwork ) )
				result |= GameObjectFlags.NotNetworked;
			if ( mode.HasFlag( BlowoutSystemMode.EditorReadonly ) )
				result |= GameObjectFlags.NotSaved;

			return result;
		}

		public ComponentFlags ToSourceComponentFlags()
		{
			ComponentFlags result = ComponentFlags.None;

			if ( mode.HasFlag( BlowoutSystemMode.HideHierarchy ) )
				result |= ComponentFlags.Hidden;
			if ( mode.HasFlag( BlowoutSystemMode.NotSavedYet ) )
				result |= ComponentFlags.NotSaved;
			if ( mode.HasFlag( BlowoutSystemMode.NotClonable ) )
				result |= ComponentFlags.NotCloned;
			if ( mode.HasFlag( BlowoutSystemMode.ExcludeNetwork ) )
				result |= ComponentFlags.NotNetworked;
			if ( mode.HasFlag( BlowoutSystemMode.EditorReadonly ) )
				result |= ComponentFlags.NotEditable;

			//Log.Info("Unknown game system mode for Source 2: " + mode);
			return result;
		}
	}
}
