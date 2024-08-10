using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Domain.Models
{
    public class AlbumTrack
    {
        public Guid AlbumId { get; set; }
        public Album Album { get; set; }

        public Guid TrackId { get; set; }
        public Track Track { get; set; }
    }
}
