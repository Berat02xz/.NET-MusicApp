namespace MusicStore.Domain.Models
{
    public class Album : BaseEntity
    {
        public required string Title { get; set; }
        public required string CoverImageUrl { get; set; }
        public required string Description { get; set; }
        public required string Tags { get; set; } // For tagging, can be a comma-separated string or JSON
        public required AlbumType Type { get; set; } // Enum for album type

        // Navigation properties
        public List<TrackArtist>? TrackArtists { get; set; } = new List<TrackArtist>();
        public List<AlbumArtist>? AlbumArtists { get; set; } = new List<AlbumArtist>();
        public List<AlbumTrack>? AlbumTracks { get; set; } = new List<AlbumTrack>(); // Added this line

    }

    public enum AlbumType
    {
        EP, // Extended Play
        LP // Long Play
    }
}
