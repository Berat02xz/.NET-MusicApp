using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Domain.Models
{
    public class AlbumArtist
    {
        public Guid AlbumId { get; set; }
        public Album Album { get; set; }

        public Guid ArtistId { get; set; }
        public Artist Artist { get; set; }
    }
}
