using MusicStore.Domain.Models;
using MusicStore.Repository;
using MusicStore.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MusicStore.Service.Implementation
{
    public class AlbumService : IAlbumService
    {
        private readonly ApplicationDbContext _context;

        public AlbumService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Album> GetAllAlbums()
        {
            return _context.Albums
                .Include(a => a.Tracks)
                .Include(a => a.Artists)
                .ToList();
        }

        public Album GetAlbumById(Guid id)
        {
            return _context.Albums
                .Include(a => a.Tracks)
                .Include(a => a.Artists)
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

        public List<Album> GetAlbumsByArtistId(Guid artistId)
        {
            return _context.Albums
                .Where(a => a.Artists.Any(artist => artist.Id == artistId))
                .Include(a => a.Tracks)
                .Include(a => a.Artists)
                .ToList();
        }

        public List<Album> GetAlbumsByTag(string tag)
        {
            return _context.Albums
                .Where(a => a.Tags.Contains(tag))
                .Include(a => a.Tracks)
                .Include(a => a.Artists)
                .ToList();
        }

        public List<Album> GetAlbumsByType(AlbumType type)
        {
            return _context.Albums
                .Where(a => a.Type == type)
                .Include(a => a.Tracks)
                .Include(a => a.Artists)
                .ToList();
        }
    }

}
