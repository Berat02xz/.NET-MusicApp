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

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
            _entities = context.Set<MusicStoreUser>();
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

            return _entities.FirstOrDefault(u => u.Id == id);
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

        // New methods to manage playlists will need to be added here

    }
}
