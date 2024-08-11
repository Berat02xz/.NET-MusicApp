using Microsoft.AspNetCore.Identity;
using MusicStore.Domain.Models;

namespace MusicStore.Domain.Identity
{
    public class MusicStoreUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string UserAddress { get; set; }

        // FUTURE IMPLEMENTATION FOR PLAYLISTS
        //  public List<Playlist> Playlists { get; set; } = new List<Playlist>();

    }
}
