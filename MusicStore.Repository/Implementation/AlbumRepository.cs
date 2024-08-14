using Microsoft.EntityFrameworkCore;
using MusicStore.Domain.Models;
using MusicStore.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Repository.Implementation
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly ApplicationDbContext _context;

        public AlbumRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Album> GetAllAlbums()
        {
            return _context.Albums
                .Include(a => a.Artists)
                .Include(a => a.Tracks)
                .ToList();
        }

        public Album GetAlbumById(Guid id)
        {
            return _context.Albums
                .Include(a => a.Artists)
                .Include(a => a.Tracks)
                    .ThenInclude(t => t.Artists)
                .FirstOrDefault(a => a.Id == id);
        }

        public void AddAlbum(Album album)
        {
            _context.Albums.Add(album);
            _context.SaveChanges();
        }

        public void UpdateAlbum(Album album)
        {
            _context.Albums.Update(album);
            _context.SaveChanges();
        }

        public void DeleteAlbum(Guid id)
        {
            var album = _context.Albums.Find(id);
            if (album != null)
            {
                _context.Albums.Remove(album);
                _context.SaveChanges();
            }
        }

        public void SaveChanges() // Implement this method
        {
            _context.SaveChanges();
        }
    }


}
