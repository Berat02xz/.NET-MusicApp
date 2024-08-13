namespace MusicStore.Domain.Models
{
    public class Track : BaseEntity
    {
        public required string Title { get; set; }
        public required string Duration { get; set; }
        public int ListenCount { get; set; } = 0; // Number of times the track has been listened to, initialized to 0 than incrementet whenever a user clicks on track
        public string? YoutubeURL { get; set; }
        public DateTime DateAdded { get; set; }

        public List<Artist> Artists { get; set; } = new List<Artist>();
        // List of artists on the track
    }
}
