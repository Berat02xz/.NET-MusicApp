using MusicStore.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Domain.Models
{
    public class Playlist : BaseEntity
    {
        public string Name { get; set; } 
        public string UrlPicture { get; set; } 

        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Track> PlaylistTracks { get; set; } = new List<Track>();
    }
}
