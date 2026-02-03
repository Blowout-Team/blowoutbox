using BlowoutTeamSoft.Engine.Interfaces;
using BlowoutTeamSoft.Engine.Interfaces.Physics;
using BlowoutTeamSoft.Engine.Interfaces.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandbox;

public partial class GameObject
{
	internal static IDictionary<Type, Func<GameObject, IBlowoutGameSystem>> BlowoutSystemsAlias => new Dictionary<Type, Func<GameObject, IBlowoutGameSystem>>()
	{
		{ typeof(IBlowoutCamera), (x) => new CameraComponent() },
		{ typeof(IBlowoutPhysicsBody), (x) => new PhysicsBody(x.Scene.PhysicsWorld) }
	};

	public override T AddGameSystem<T>() =>
		Components.CreateFromAlias<T>(BlowoutSystemsAlias, true);
}
