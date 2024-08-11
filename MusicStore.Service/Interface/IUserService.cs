using MusicStore.Domain.Identity;
using MusicStore.Domain.Models;
using System.Collections.Generic;

namespace MusicStore.Service.Interface
{
    public interface IUserService
    {
        IEnumerable<MusicStoreUser> GetAllUsers();
        MusicStoreUser GetUserById(string id);
        void AddUser(MusicStoreUser user);
        void UpdateUser(MusicStoreUser user);
        void DeleteUser(string id);

        // Methods for managing playlists
    }
}
