using MusicStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Domain.JunctionTables
{
    public class AlbumArtist : BaseEntity
    {
        public Guid AlbumId { get; set; }
        public Album Album { get; set; }

        public Guid ArtistId { get; set; }
        public Artist Artist { get; set; }
    }

}
