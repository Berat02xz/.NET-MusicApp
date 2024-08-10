using Microsoft.EntityFrameworkCore;
using MusicStore.Domain.Identity;
using MusicStore.Domain.Models;
using MusicStore.Repository.Interface;

namespace MusicStore.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private DbSet<MusicStoreUser> _entities;
        private DbSet<Playlist> _playlists;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
            _entities = context.Set<MusicStoreUser>();
            _playlists = context.Set<Playlist>();
        }

        public IEnumerable<MusicStoreUser> GetAll()
        {
            return _entities.AsEnumerable();
        }

        public MusicStoreUser Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("ID cannot be null or empty", nameof(id));
            }

            return _entities
                .Include(u => u.Playlists)
                .FirstOrDefault(u => u.Id == id);
        }

        public void Insert(MusicStoreUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _entities.Add(entity);
            _context.SaveChanges();
        }

        public void Update(MusicStoreUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _entities.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(MusicStoreUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _entities.Remove(entity);
            _context.SaveChanges();
        }

        // New methods to manage playlists

        public IEnumerable<Playlist> GetUserPlaylists(string userId)
        {
            return _playlists
                .Where(p => p.UserId == userId)
                .ToList();
        }

        public Playlist GetPlaylistById(Guid playlistId)
        {
            return _playlists
                .FirstOrDefault(p => p.Id == playlistId);
        }

        public void AddPlaylist(Playlist playlist)
        {
            if (playlist == null)
            {
                throw new ArgumentNullException(nameof(playlist));
            }
            _playlists.Add(playlist);
            _context.SaveChanges();
        }

        public void UpdatePlaylist(Playlist playlist)
        {
            if (playlist == null)
            {
                throw new ArgumentNullException(nameof(playlist));
            }
            _playlists.Update(playlist);
            _context.SaveChanges();
        }

        public void DeletePlaylist(Guid playlistId)
        {
            var playlist = _playlists.Find(playlistId);
            if (playlist != null)
            {
                _playlists.Remove(playlist);
                _context.SaveChanges();
            }
        }
    }
}
