using BlowoutTeamSoft.Engine.Interfaces.Assets;
using Sandbox.Navigation;

namespace Editor;

public partial class SceneEditorSession : ResourceLibrary.IEventListener, NavMesh.IEventListener
{
	void ResourceLibrary.IEventListener.OnRegister( IBlowoutEngineAsset resource )
	{
		if ( resource is not PrefabFile prefab ) return;

		EditorScene.UpdatePrefabInstancesInScene( Scene, prefab );
	}

	void ResourceLibrary.IEventListener.OnUnregister(IBlowoutEngineAsset resource )
	{
		if ( resource is not PrefabFile prefab ) return;

		EditorScene.UpdatePrefabInstancesInScene( Scene, prefab );
	}

	void ResourceLibrary.IEventListener.OnExternalChangesPostLoad(IBlowoutEngineAsset resource )
	{
		if ( resource is not PrefabFile prefab ) return;

		EditorScene.UpdatePrefabInstancesInScene( Scene, prefab );
	}

	void NavMesh.IEventListener.OnAreaDefinitionChanged()
	{
		Scene.NavMesh?.UpdateAreaIds();
	}
}
