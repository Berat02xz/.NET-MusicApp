using Microsoft.AspNetCore.Identity;
using MusicStore.Domain.Models;

namespace MusicStore.Domain.Identity
{
    public class MusicStoreUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string UserAddress { get; set; }

        // Navigation property
        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>(); // Initialize with an empty list

    }
}
