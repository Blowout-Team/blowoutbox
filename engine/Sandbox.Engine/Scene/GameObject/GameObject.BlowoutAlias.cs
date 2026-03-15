using BlowoutTeamSoft.Engine.Interfaces;
using BlowoutTeamSoft.Engine.Interfaces.AI;
using BlowoutTeamSoft.Engine.Interfaces.Animator;
using BlowoutTeamSoft.Engine.Interfaces.Audio;
using BlowoutTeamSoft.Engine.Interfaces.Physics;
using BlowoutTeamSoft.Engine.Interfaces.Rendering;
using BlowoutTeamSoft.Engine.Interfaces.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandbox;

public partial class GameObject
{
	internal static IDictionary<Type, Func<GameObject, IBlowoutGameSystem>> BlowoutSystemsAlias => new Dictionary<Type, Func<GameObject, IBlowoutGameSystem>>()
	{
		{ typeof(IBlowoutCamera), (x) => new CameraComponent() },
		{ typeof(IBlowoutParticle), (x) => new ParticleEffect() },
		{ typeof(IBlowoutAiNavigationAgent), (x) => new NavMeshAgent() },
		{ typeof(IBlowoutUIGraphic), (x) => new ScreenPanel() },
		{ typeof(IAnimator), (x) => new SkinnedModelRenderer() },
		{ typeof(ISkeletonAnimator), (x) => new SkinnedModelRenderer() },
		{ typeof(IPhysicsAnimator), (x) => new SkinnedModelRenderer() },
		{ typeof(IBlowoutAudioSystem), (x) => new SoundBoxComponent() },
		{ typeof(IBlowoutAudioSource), (x) => new SoundBoxComponent() }
		//{ typeof(IBlowoutPhysicsBody), (x) => new PhysicsBody(x.Scene.PhysicsWorld) }
	};

	public override T AddGameSystem<T>() =>
		Components.CreateFromAlias<T>(BlowoutSystemsAlias, true);
}
