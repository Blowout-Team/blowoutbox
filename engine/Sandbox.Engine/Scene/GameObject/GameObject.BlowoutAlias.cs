using BlowoutTeamSoft.Engine.Interfaces;
using BlowoutTeamSoft.Engine.Interfaces.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandbox;

public partial class GameObject
{
	internal static IDictionary<Type, Func<IBlowoutGameSystem>> BlowoutSystemsAlias => new Dictionary<Type, Func<IBlowoutGameSystem>>()
	{
		{ typeof(IBlowoutCamera), () => new CameraComponent() }
	};

	public override T AddGameSystem<T>() =>
		Components.CreateFromAlias<T>(BlowoutSystemsAlias, true);
}
