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
    public class TrackRepository : ITrackRepository
    {
        private readonly ApplicationDbContext _context;

        public TrackRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Track> GetAllTracks()
        {
            return _context.Tracks.ToList();
        }

        public Track GetTrackById(Guid id)
        {
            return _context.Tracks
                .Include(t => t.Artists)
                .FirstOrDefault(t => t.Id == id);
        }

        public void AddTrack(Track track)
        {
            _context.Tracks.Add(track);
            _context.SaveChanges();
        }

        public void UpdateTrack(Track track)
        {
            _context.Tracks.Update(track);
            _context.SaveChanges();
        }

        public void DeleteTrack(Guid id)
        {
            var track = _context.Tracks.Find(id);
            if (track != null)
            {
                _context.Tracks.Remove(track);
                _context.SaveChanges();
            }
        }


        public List<Track> GetTracksByAlbumId(Guid albumId)
        {
            return _context.Tracks
                .Where(t => t.AlbumId == albumId)
                .ToList();
        }



    }

}
