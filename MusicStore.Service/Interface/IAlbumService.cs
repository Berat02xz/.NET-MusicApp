using MusicStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Service.Interface
{
    public interface IAlbumService
    {
        List<Album> GetAllAlbums();
        Album GetAlbumById(Guid id);
        void AddAlbum(Album album);
        void UpdateAlbum(Album album);
        void DeleteAlbum(Guid id);
        List<Album> GetAlbumsByArtistId(Guid artistId);
        List<Album> GetAlbumsByTag(string tag);
        List<Album> GetAlbumsByType(AlbumType type);
    }

}
