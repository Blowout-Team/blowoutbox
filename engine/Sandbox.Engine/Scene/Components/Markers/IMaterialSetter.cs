using BlowoutTeamSoft.Engine.Interfaces;

namespace Sandbox;

public abstract partial class Component
{
	public interface IMaterialSetter : IBlowoutGameSystem
	{
		public void SetMaterial( Material material, int triangle = -1 );
		public Material GetMaterial( int triangle = -1 );
	}
}
