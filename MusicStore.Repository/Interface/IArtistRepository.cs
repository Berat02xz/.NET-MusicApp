using MusicStore.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Repository.Interface
{
    public interface IArtistRepository
    {
        List<Artist> GetAllArtists();
        Artist GetArtistById(Guid id);
        void AddArtist(Artist artist);
        void UpdateArtist(Artist artist);
        void DeleteArtist(Guid id);
    }
}
