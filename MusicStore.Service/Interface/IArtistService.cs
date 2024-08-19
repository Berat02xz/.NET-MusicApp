using MusicStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Service.Interface
{
    public interface IArtistService
    {

        List<Artist> GetAllArtists();

        Artist? GetArtistById(Guid id);

        void AddArtist(Artist artist);

        void UpdateArtist(Artist artist);

        void DeleteArtist(Guid id);

        List<Artist> FindArtistsByCriteria(string criteria);

        List<Artist> GetArtistsByIds(List<Guid> ids);
    }

}
