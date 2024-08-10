
namespace MusicStore.Domain.Models
{
    public class TrackArtist : BaseEntity
    {
        public Guid TrackId { get; set; }
        public Track Track { get; set; }

        public Guid ArtistId { get; set; }
        public Artist Artist { get; set; }
    }
}
