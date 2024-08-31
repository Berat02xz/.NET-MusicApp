using Microsoft.EntityFrameworkCore;
using MusicStore.Domain.Models;
using MusicStore.Repository.Interface;
using System.Diagnostics;

namespace MusicStore.Repository.Implementation
{
    public class PlaylistRepository : IPlaylistRepository
    {

        private readonly ApplicationDbContext _context;

        public PlaylistRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void AddPlaylist(Playlist playlist)
        {
            if (playlist == null)
            {
                throw new ArgumentNullException(nameof(playlist));
            }

            _context.Playlists.Add(playlist);
        }

        public void DeletePlaylist(Playlist playlist)
        {
            _context.Playlists.Remove(playlist);
            _context.SaveChanges();
        }

        public List<Playlist> GetAllPlaylists()
        {
            return _context.Playlists.Include(p => p.PlaylistTracks)
                                      .ThenInclude(pt => pt.Artists)
                                      .ToList();
        }

        public Playlist GetPlaylistById(Guid playlistId)
        {
            return _context.Playlists.Include(p => p.PlaylistTracks)
                                      .ThenInclude(pt => pt.Album)
                                      .FirstOrDefault(p => p.Id == playlistId);
        }

        public Playlist GetPlaylistByUserId(string userId)
        {
            // Log the user ID being queried
            Debug.WriteLine($"Querying playlist for user ID: {userId}");

            // Retrieve the user and their playlists
            var user = _context.Users
                .Include(u => u.Playlists)
                .ThenInclude(p => p.PlaylistTracks)
                .ThenInclude(p => p.Album)
                .FirstOrDefault(u => u.Id == userId);

            // Check if the user exists
            if (user == null)
            {
                Debug.WriteLine("User not found.");
                return null; // or handle as appropriate
            }

            // Log the number of playlists found
            Debug.WriteLine($"Number of playlists found for user: {user.Playlists.Count}");

            // Return the first playlist or handle as needed
            var playlist = user.Playlists.FirstOrDefault();
            if (playlist == null)
            {
                Debug.WriteLine("No playlists found for the user.");
            }
            else
            {
                Debug.WriteLine($"Found playlist: {playlist.Name}");
            }

            return playlist;
        }



        public void UpdatePlaylist(Playlist playlist)
        {
            _context.Playlists.Update(playlist);
            _context.SaveChanges();
        }
    }
}
