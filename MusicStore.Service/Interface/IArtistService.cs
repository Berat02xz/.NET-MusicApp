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
        // Method to retrieve all artists
        List<Artist> GetAllArtists();

        // Method to retrieve a specific artist by their unique identifier
        Artist? GetArtistById(Guid id);

        // Method to add a new artist
        void AddArtist(Artist artist);

        // Method to update an existing artist's details
        void UpdateArtist(Artist artist);

        // Method to delete an artist by their unique identifier
        void DeleteArtist(Guid id);

        // Method to find artists by their name, tags, or other criteria
        List<Artist> FindArtistsByCriteria(string criteria);

        // Method to retrieve a list of artists by their unique identifiers
        List<Artist> GetArtistsByIds(List<Guid> ids);
    }

}
