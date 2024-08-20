namespace MusicStore.Domain.Models
{
    public class Track : BaseEntity
    {
        public  string Title { get; set; }
        public  string Duration { get; set; }
        public int ListenCount { get; set; } = 0; // Number of times the track has been listened to, initialized to 0 than incrementet whenever a user clicks on track
        public string? YoutubeURL { get; set; }
        public DateTime DateAdded { get; set; }

        // Foreign key for Album
        public Guid AlbumId { get; set; }
        public Album Album { get; set; }

        // Many-to-many relationship with Artist (Handled via a junction table)
        public List<Artist> Artists { get; set; } = new List<Artist>();

    }
}
