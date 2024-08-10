namespace MusicStore.Domain.Models
{
    public class Track : BaseEntity
    {
        public required string Title { get; set; }
        public TimeSpan Duration { get; set; }
        public int ListenCount { get; set; } // New property to count listens
        public string YoutubeURL { get; set; }
        public DateTime DateAdded { get; set; }

        // FOR FUTURE IMPLEMENTATION
        // public string Mp3FilePath { get; set; } // Path to the uploaded MP3 file


        // Navigation properties
        public ICollection<TrackArtist>? TrackArtists { get; set; } // Changed to many-to-many relationship
        public ICollection<AlbumTrack>? AlbumTracks { get; set; } // Added this line
    }
}
