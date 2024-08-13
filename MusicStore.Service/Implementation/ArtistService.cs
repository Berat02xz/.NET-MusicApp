using MusicStore.Domain.Models;
using MusicStore.Repository;
using MusicStore.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Service.Implementation
{
    public class ArtistService : IArtistService
    {
        private readonly ApplicationDbContext _context;

        public ArtistService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Artist> GetAllArtists()
        {
            return _context.Artists.ToList();
        }

        public Artist? GetArtistById(Guid id)
        {
            return _context.Artists.FirstOrDefault(a => a.Id == id);
        }

        public void AddArtist(Artist artist)
        {
            _context.Artists.Add(artist);
            _context.SaveChanges();
        }

        public void UpdateArtist(Artist artist)
        {
            var existingArtist = _context.Artists.FirstOrDefault(a => a.Id == artist.Id);
            if (existingArtist != null)
            {
                existingArtist.Name = artist.Name;
                existingArtist.ImageAvatarURL = artist.ImageAvatarURL;
                existingArtist.Description = artist.Description;
                existingArtist.Country = artist.Country;
                existingArtist.City = artist.City;
                existingArtist.Tags = artist.Tags;
                existingArtist.Tracks = artist.Tracks;
                existingArtist.Albums = artist.Albums;

                _context.SaveChanges();
            }
        }

        public void DeleteArtist(Guid id)
        {
            var artist = _context.Artists.FirstOrDefault(a => a.Id == id);
            if (artist != null)
            {
                _context.Artists.Remove(artist);
                _context.SaveChanges();
            }
        }

        public List<Artist> FindArtistsByCriteria(string criteria)
        {
            return _context.Artists
                .Where(a => a.Name.Contains(criteria) || a.Tags.Contains(criteria) || a.Description.Contains(criteria))
                .ToList();
        }

        public List<Artist> GetArtistsByIds(List<Guid> ids)
        {
            return _context.Artists
                .Where(a => ids.Contains(a.Id))
                .ToList();
        }
    }

}
