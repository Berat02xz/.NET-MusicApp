using MusicStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Repository.Interface
{
    public interface ITrackRepository
    {
        List<Track> GetAllTracks();
        Track GetTrackById(Guid id);
        void AddTrack(Track track);
        void UpdateTrack(Track track);
        void DeleteTrack(Guid id);
    }

}
