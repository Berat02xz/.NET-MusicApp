using MusicStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Domain.JunctionTables
{
    public class ArtistTrack
    {
        public Guid ArtistId { get; set; }
        public Artist Artist { get; set; }

        public Guid TrackId { get; set; }
        public Track Track { get; set; }
    }

}
