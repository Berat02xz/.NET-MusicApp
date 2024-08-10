using MusicStore.Domain.Identity;
using MusicStore.Domain.Models;
using MusicStore.Repository.Interface;
using MusicStore.Service.Interface;

namespace MusicStore.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IEnumerable<MusicStoreUser> GetAllUsers()
        {
            return _userRepository.GetAll();
        }

        public MusicStoreUser GetUserById(string id)
        {
            return _userRepository.Get(id);
        }

        public void AddUser(MusicStoreUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            _userRepository.Insert(user);
        }

        public void UpdateUser(MusicStoreUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            _userRepository.Update(user);
        }

        public void DeleteUser(string id)
        {
            var user = _userRepository.Get(id);
            if (user != null)
            {
                _userRepository.Delete(user);
            }
        }

        // Methods for managing playlists

        public IEnumerable<Playlist> GetUserPlaylists(string userId)
        {
            return _userRepository.GetUserPlaylists(userId);
        }

        public Playlist GetPlaylistById(Guid playlistId)
        {
            return _userRepository.GetPlaylistById(playlistId);
        }

        public void AddPlaylist(Playlist playlist)
        {
            if (playlist == null)
            {
                throw new ArgumentNullException(nameof(playlist));
            }
            _userRepository.AddPlaylist(playlist);
        }

        public void UpdatePlaylist(Playlist playlist)
        {
            if (playlist == null)
            {
                throw new ArgumentNullException(nameof(playlist));
            }
            _userRepository.UpdatePlaylist(playlist);
        }

        public void DeletePlaylist(Guid playlistId)
        {
            _userRepository.DeletePlaylist(playlistId);
        }
    }
}
