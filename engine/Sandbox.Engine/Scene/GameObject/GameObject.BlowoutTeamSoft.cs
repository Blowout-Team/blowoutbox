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
	public override T AddGameSystem<T>() =>
		Components.Create<T>(true);

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

	public override T CastTo<T>()
	{
		if (this is T result)
			return result;

		throw new BlowoutEngineException(string.Format("Unable cast game object to type '{0}'", typeof(T).FullName));
	}

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

	public override BlowoutEngineGameObject CreateClone() =>
		Clone();

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
