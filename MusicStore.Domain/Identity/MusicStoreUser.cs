using Microsoft.AspNetCore.Identity;
using MusicStore.Domain.Models;

namespace MusicStore.Domain.Identity
{
    public class MusicStoreUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string UserAddress { get; set; }

        // Playlist Implementation (For each registered user theres a playlist)
        public List<Playlist> Playlists { get; set; } = new List<Playlist>();


    }
}
