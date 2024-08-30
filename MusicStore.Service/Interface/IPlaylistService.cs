using MusicStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Service.Interface
{
    public interface IPlaylistService
    {
        // Get a playlist by its ID
        Playlist GetPlaylistById(Guid playlistId);

        // Get all playlists (if needed)
        List<Playlist> GetAllPlaylists();

        // Create a new playlist
        void CreatePlaylist(Playlist playlist);

        // Update an existing playlist
        void UpdatePlaylist(Playlist playlist);

        // Delete a playlist by its ID
        void DeletePlaylist(Guid playlistId);

        // Add a track to the playlist
        void AddTrackToPlaylist(Guid playlistId, Track track);

        // Remove a track from the playlist
        void RemoveTrackFromPlaylist(Guid playlistId, Guid trackId);
        Playlist GetPlaylistByUserId(string userId);

        Playlist CreatePlaylistForUser(string userId);
    }
}

