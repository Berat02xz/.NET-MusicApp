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
        public void DeleteAlbum(Guid albumId)
        {
            var album = _context.Albums
                .Include(a => a.Tracks)
                .SingleOrDefault(a => a.Id == albumId);

            if (album == null)
            {
                throw new ArgumentException("Album not found", nameof(albumId));
            }

            // Remove entries from AlbumArtist table
            var albumArtists = _context.AlbumArtist.Where(aa => aa.AlbumId == albumId).ToList();
            _context.AlbumArtist.RemoveRange(albumArtists);

            // Remove entries from ArtistTrack table
            var trackIds = album.Tracks.Select(t => t.Id).ToList();
            var artistTracks = _context.ArtistTrack.Where(at => trackIds.Contains(at.TrackId)).ToList();
            _context.ArtistTrack.RemoveRange(artistTracks);

            // Remove tracks related to the album
            _context.Tracks.RemoveRange(album.Tracks);

            // Remove the album
            _context.Albums.Remove(album);

            _context.SaveChanges();
        }



        public void SaveChanges() // Implement this method
        {
            _context.SaveChanges();
        }
    }


}
