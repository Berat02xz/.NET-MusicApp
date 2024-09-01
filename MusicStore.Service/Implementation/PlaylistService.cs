using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicStore.Domain.Identity;
using MusicStore.Domain.Models;
using MusicStore.Repository.Interface;
using MusicStore.Service.Interface;
using System.Diagnostics;

namespace MusicStore.Service.Implementation
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly UserManager<MusicStoreUser> _userManager;

        public PlaylistService(IPlaylistRepository playlistRepository, UserManager<MusicStoreUser> userManager)
        {
            _playlistRepository = playlistRepository;
            _userManager = userManager;
        }

        public Playlist GetPlaylistById(Guid playlistId)
        {
            return _playlistRepository.GetPlaylistById(playlistId);
        }

        public List<Playlist> GetAllPlaylists()
        {
            return _playlistRepository.GetAllPlaylists();
        }

        public void CreatePlaylist(Playlist playlist)
        {
            _playlistRepository.AddPlaylist(playlist);
        }

        public Playlist CreatePlaylistForUser(string userId)
        {
            var user = _userManager.Users
                .Include(u => u.Playlists)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                Debug.WriteLine("User not found.");
                return null;
            }

            // Create a new playlist with pre-filled properties
            var newPlaylist = new Playlist
            {
                Name = user.FirstName + " Liked Songs",
                UrlPicture = "https://cdn.dribbble.com/users/278624/screenshots/4413242/playlist_cover2.png",
                Description = "Your personal playlist",
                CreatedAt = DateTime.Now
            };

            // Add the new playlist to the user's playlists
            user.Playlists.Add(newPlaylist);

            // Save changes to the database
            _userManager.UpdateAsync(user).Wait(); // Ensure async methods are properly awaited
            _playlistRepository.AddPlaylist(newPlaylist);

            Debug.WriteLine($"Created new playlist: {newPlaylist.Name}");
            return newPlaylist;
        }

        public void UpdatePlaylist(Playlist playlist)
        {
            _playlistRepository.UpdatePlaylist(playlist);
        }

        public void DeletePlaylist(Guid playlistId)
        {
            var playlist = _playlistRepository.GetPlaylistById(playlistId);
            if (playlist != null)
            {
                _playlistRepository.DeletePlaylist(playlist);
            }
        }

        public void AddTrackToPlaylist(Guid playlistId, Track track)
        {
            var playlist = _playlistRepository.GetPlaylistById(playlistId);
            if (playlist != null)
            {
                playlist.PlaylistTracks.Add(track);
                _playlistRepository.UpdatePlaylist(playlist);
            }
        }

        public void RemoveTrackFromPlaylist(Guid playlistId, Guid trackId)
        {
            var playlist = _playlistRepository.GetPlaylistById(playlistId);
            if (playlist != null)
            {
                var track = playlist.PlaylistTracks.FirstOrDefault(t => t.Id == trackId);
                if (track != null)
                {
                    playlist.PlaylistTracks.Remove(track);
                    _playlistRepository.UpdatePlaylist(playlist);
                }
            }
        }

        public Playlist GetPlaylistByUserId(string userId)
        {
            return _playlistRepository.GetPlaylistByUserId(userId);
        }

    }
}
