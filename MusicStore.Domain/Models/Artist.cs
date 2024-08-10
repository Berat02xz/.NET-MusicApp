namespace MusicStore.Domain.Models
{
    public class Artist : BaseEntity
    {
        public required string Name { get; set; }
        public required string ImageAvatarURL { get; set; }
        public required string Description { get; set; }



        // Navigation property
        public List<TrackArtist>? TrackArtists { get; set; }
        public List<AlbumArtist>? AlbumArtists { get; set; } // Added this line

    }
}
