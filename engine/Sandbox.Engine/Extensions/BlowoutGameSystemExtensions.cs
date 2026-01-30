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
			switch ( flags ) 
			{
				case GameObjectFlags.Hidden:
					return BlowoutSystemMode.HideHierarchy;
				case GameObjectFlags.NotSaved:
					return BlowoutSystemMode.EditorReadonly;
			}

			return BlowoutSystemMode.None;
		}
	}

	extension(ComponentFlags flags)
	{
		public BlowoutSystemMode ToSystemMode() =>
			flags switch
			{
				ComponentFlags.Hidden => BlowoutSystemMode.HideHierarchy,
				ComponentFlags.NotEditable => BlowoutSystemMode.EditorReadonly,
				_ => BlowoutSystemMode.None,
			};
	}

	extension(BlowoutSystemMode mode)
	{
		public GameObjectFlags ToSourceFlags()
		{
			switch ( mode )
			{
				case BlowoutSystemMode.HideHierarchy:
					return GameObjectFlags.Hidden;
				case BlowoutSystemMode.EditorReadonly:
					return GameObjectFlags.NotSaved;
			}

			Log.Info("Unknown game system mode for Source 2: " + mode);
			return GameObjectFlags.None;
		}

		public ComponentFlags ToSourceComponentFlags()
		{
			switch (mode)
			{
				case BlowoutSystemMode.HideHierarchy:
					return ComponentFlags.Hidden;
				case BlowoutSystemMode.EditorReadonly:
					return ComponentFlags.NotEditable;
			}

			Log.Info("Unknown game system mode for Source 2: " + mode);
			return ComponentFlags.None;
		}
	}
}
