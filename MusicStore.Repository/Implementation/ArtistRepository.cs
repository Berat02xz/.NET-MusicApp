using MusicStore.Domain.Models;
using MusicStore.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Repository.Implementation
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly ApplicationDbContext _context;

        public ArtistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Artist> GetAllArtists()
        {
            return _context.Artists.ToList();
        }

        public Artist GetArtistById(Guid id)
        {
            return _context.Artists.Find(id);
        }

        public void AddArtist(Artist artist)
        {
            _context.Artists.Add(artist);
            _context.SaveChanges();
        }

        public void UpdateArtist(Artist artist)
        {
            _context.Artists.Update(artist);
            _context.SaveChanges();
        }

        public void DeleteArtist(Guid id)
        {
            var artist = _context.Artists.Find(id);
            if (artist != null)
            {
                _context.Artists.Remove(artist);
                _context.SaveChanges();
            }
        }

        public List<Artist> GetArtistsByIds(IEnumerable<Guid> ids)
        {
            return _context.Artists
                .Where(artist => ids.Contains(artist.Id))
                .ToList();
        }
    }


}
