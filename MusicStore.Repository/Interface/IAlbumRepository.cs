using MusicStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Repository.Interface
{
    public interface IAlbumRepository
    {
        List<Album> GetAllAlbums();
        Album GetAlbumById(Guid id);
        void AddAlbum(Album album);
        void UpdateAlbum(Album album);
        void DeleteAlbum(Guid id);
    }

}
