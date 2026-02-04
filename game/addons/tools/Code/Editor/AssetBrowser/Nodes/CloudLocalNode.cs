using BlowoutTeamSoft.Engine.Interfaces.Assets;

namespace Editor.AssetBrowsing.Nodes;

partial class CloudLocalNode : AssetFilterNode, ResourceLibrary.IEventListener
{
	public CloudLocalNode() : base( "attach_file", "Referenced", "@referenced" )
	{
		EditorEvent.Register( this );
		UpdateCount();
	}

	~CloudLocalNode()
	{
		EditorEvent.Unregister( this );
	}

	void ResourceLibrary.IEventListener.OnSave( IBlowoutEngineAsset resource ) => UpdateCount();
	void ResourceLibrary.IEventListener.OnExternalChanges( IBlowoutEngineAsset resource ) => UpdateCount();

	void UpdateCount()
	{
		var packages = CloudAsset.GetAssetReferences( true );
		Count = packages.Count;
		Dirty();
	}
}
