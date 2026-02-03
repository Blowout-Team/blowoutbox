using BlowoutTeamSoft.Engine.Interfaces.Physics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandbox;

public partial class Surface : IBlowoutPhysicsSurface
{
	public override string Name { get => ResourceName; set => ResourceName = value; }
	public float Bounciness { get => BounceThreshold; set => BounceThreshold = value; }

	public Surface(IBlowoutPhysicsSurface from) 
	{
		if(from is Surface sf)
		{
			Bounciness = sf.Bounciness;
			Friction = sf.Friction;
			Elasticity = sf.Elasticity;
			return;
		}
		Name = from.Name;
		SetIdFromResourcePath(from.Name);
		Create(false);
	}
}
