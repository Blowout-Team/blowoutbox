using BlowoutTeamSoft.Engine;
using BlowoutTeamSoft.Engine.Core;
using BlowoutTeamSoft.Engine.Exceptions;
using BlowoutTeamSoft.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandbox;
public partial class GameObject
{
	//public override T AddGameSystem<T>() =>
	//	Components.CreateFromAlias<T>(true);

	public override IBlowoutGameSystem AddGameSystem( Type type ) =>
		Components.Create( type );

	public override IEnumerable<IBlowoutGameSystem> GetChildGameSystems( Type type, bool isStrict )
	{
		var f = FindMode.InChildren;
		if ( !isStrict ) f |= FindMode.Enabled;

		return Components.GetAll( type, f );
	}

	public override IBlowoutGameSystem GetGameSystem( Type type )
	{
		return Components.Get( type, FindMode.InSelf | FindMode.EverythingInSelf );
	}

	public override IEnumerable<IBlowoutGameSystem> GetParentGameSystems( Type type, bool isStrict )
	{
		var f = FindMode.InParent;
		if ( !isStrict ) f |= FindMode.Enabled;

		return Components.GetAll( type, f );
	}

	public override IEnumerable<IBlowoutGameSystem> GetGameSystems( Type type )
	{
		return Components.GetAll( type, FindMode.InSelf | FindMode.EverythingInSelf );
	}

	public override T GetGameSystem<T>() =>
		Components.Get<T>(true);

	public override IEnumerable<T> GetGameSystems<T>()
		=>
		Components.GetAll<T>(FindMode.InSelf);

	public override T GetGameSystemInChildren<T>() =>
		Components.Get<T>(FindMode.InChildren);

	public override T GetGameSystemInParent<T>() =>
		Components.Get<T>(FindMode.InParent);

	public override IEnumerable<T> GetStrictGameSystemsInChildren<T>() =>
		Components.GetAll<T>(FindMode.EverythingInChildren);

	public override IEnumerable<T> GetGameSystemsInChildren<T>() =>
		Components.GetAll<T>(FindMode.InChildren);

	public override IEnumerable<T> GetGameSystemsInParent<T>() =>
		Components.GetAll<T>(FindMode.InParent);

	public override BlowoutEngineObject ToEngineObject() =>
		this;

	public override bool TryAsGameObject(out BlowoutEngineGameObject gameObject)
	{
		//dehs: maybe only when created??
		gameObject = this;
		return true;
	}

	public override bool TryGetGameSystem<T>(out T gameSystem)
	{
		if(Components.TryGet(out gameSystem, FindMode.InSelf))
		{
			return true;
		}

		return false;
	}

	public override bool RemoveGameSystem(IBlowoutGameSystem gameSystem)
	{
		if (gameSystem is Sandbox.Component component)
		{
			component.Destroy();
			return true;
		}

		return Components.RemoveGameSystem(gameSystem);
	}
}
