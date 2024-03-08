
using Texel;
using UdonSharp;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class LoadPlaylistData : UdonSharpBehaviour
{
    public Playlist playlist;
    public PlaylistData playlistData;

    public void _Load()
    {
        playlist._LoadFromCatalogueData(playlistData);
    }
}
