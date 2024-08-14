namespace MusicStore.Domain.Models
{
    public class Album : BaseEntity
    {
        public required string Title { get; set; }
        public required string CoverImageUrl { get; set; }
        public required string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public required string Tags { get; set; } // For tagging genres, will be comma seperated in frontend using JS
        public required AlbumType Type { get; set; } // Enum for album type


        // One-to-many relationship with Track
        public List<Track> Tracks { get; set; } = new List<Track>();

        // Many-to-many relationship with Artist (Handled via a junction table)
        public List<Artist> Artists { get; set; } = new List<Artist>();

    }

    public enum AlbumType
    {
        EP, // Extended Play
        LP // Long Play
    }
}
