using Microsoft.EntityFrameworkCore;
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
    public class TrackService : ITrackService
    {
        private readonly ApplicationDbContext _context;

        public TrackService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Track> GetAllTracks()
        {
            return _context.Tracks.Include(t => t.Artists).ToList();
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
            var existingTrack = _context.Tracks.FirstOrDefault(t => t.Id == track.Id);
            if (existingTrack != null)
            {
                existingTrack.Title = track.Title;
                existingTrack.Duration = track.Duration;
                existingTrack.ListenCount = track.ListenCount;
                existingTrack.YoutubeURL = track.YoutubeURL;
                existingTrack.DateAdded = track.DateAdded;

                // Update the artists associated with the track
                existingTrack.Artists.Clear();
                existingTrack.Artists.AddRange(track.Artists);

                _context.SaveChanges();
            }
        }

        public void DeleteTrack(Guid id)
        {
            var track = _context.Tracks.FirstOrDefault(t => t.Id == id);
            if (track != null)
            {
                _context.Tracks.Remove(track);
                _context.SaveChanges();
            }
        }

        public List<Track> GetTracksByArtistId(Guid artistId)
        {
            return _context.Tracks
                .Where(t => t.Artists.Any(a => a.Id == artistId))
                .Include(t => t.Artists)
                .ToList();
        }



        public void AddArtistToTrack(Guid trackId, Guid artistId)
        {
            var trackArtist = new TrackArtist
            {
                TrackId = trackId,
                ArtistId = artistId
            };

            _context.TrackArtists.Add(trackArtist);
            _context.SaveChanges();
        }

        public void RemoveArtistFromTrack(Guid trackId, Guid artistId)
        {
            var trackArtist = _context.TrackArtists
                .FirstOrDefault(ta => ta.TrackId == trackId && ta.ArtistId == artistId);

            if (trackArtist != null)
            {
                _context.TrackArtists.Remove(trackArtist);
                _context.SaveChanges();
            }
        }

    }

}
