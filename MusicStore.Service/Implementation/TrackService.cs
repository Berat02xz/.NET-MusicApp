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
            return _context.Tracks
                .Include(t => t.Artists)
                .ToList();
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
            var existingTrack = _context.Tracks
                .Include(t => t.Artists)
                .FirstOrDefault(t => t.Id == track.Id);

            if (existingTrack != null)
            {
                existingTrack.Title = track.Title;
                existingTrack.Duration = track.Duration;
                existingTrack.ListenCount = track.ListenCount;
                existingTrack.YoutubeURL = track.YoutubeURL;
                existingTrack.DateAdded = track.DateAdded;
                existingTrack.AlbumId = track.AlbumId;

                // Update the artists associated with the track
                var existingArtistIds = existingTrack.Artists.Select(a => a.Id).ToList();
                var newArtistIds = track.Artists.Select(a => a.Id).ToList();

                var artistIdsToAdd = newArtistIds.Except(existingArtistIds).ToList();
                var artistIdsToRemove = existingArtistIds.Except(newArtistIds).ToList();

                if (artistIdsToAdd.Any())
                {
                    _context.Database.ExecuteSqlRaw(
                        "INSERT INTO TrackArtists (TrackId, ArtistId) SELECT {0}, ArtistId FROM Artists WHERE ArtistId IN ({1})",
                        track.Id, string.Join(",", artistIdsToAdd));
                }

                if (artistIdsToRemove.Any())
                {
                    _context.Database.ExecuteSqlRaw(
                        "DELETE FROM TrackArtists WHERE TrackId = {0} AND ArtistId IN ({1})",
                        track.Id, string.Join(",", artistIdsToRemove));
                }

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
            _context.Database.ExecuteSqlRaw(
                "INSERT INTO TrackArtists (TrackId, ArtistId) VALUES ({0}, {1})",
                trackId, artistId);
            _context.SaveChanges();
        }

        public void RemoveArtistFromTrack(Guid trackId, Guid artistId)
        {
            _context.Database.ExecuteSqlRaw(
                "DELETE FROM TrackArtists WHERE TrackId = {0} AND ArtistId = {1}",
                trackId, artistId);
            _context.SaveChanges();
        }
    }


}
