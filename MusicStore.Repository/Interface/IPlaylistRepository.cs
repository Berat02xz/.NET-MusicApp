using MusicStore.Domain.Models;


namespace MusicStore.Repository.Interface
{
    public interface IPlaylistRepository
    {
        Playlist GetPlaylistById(Guid playlistId);
        List<Playlist> GetAllPlaylists();
        void AddPlaylist(Playlist playlist);
        void UpdatePlaylist(Playlist playlist);
        void DeletePlaylist(Playlist playlist);
        Playlist GetPlaylistByUserId(string userId);

    }
}
