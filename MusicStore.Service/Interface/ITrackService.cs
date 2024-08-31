using MusicStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Service.Interface
{
    public interface ITrackService
    {
        List<Track> GetAllTracks();
        Track GetTrackById(Guid id);
        void AddTrack(Track track);
        void UpdateTrack(Track track);
        void DeleteTrack(Track track);
        List<Track> GetTracksByArtistId(Guid artistId);

        // New methods for managing TrackArtist relationships
        void AddArtistToTrack(Guid trackId, Guid artistId);
        void RemoveArtistFromTrack(Guid trackId, Guid artistId);

        List<Track> GetTracksByAlbumId(Guid albumId);
    }

}
