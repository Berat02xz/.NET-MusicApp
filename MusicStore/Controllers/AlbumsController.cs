using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Assuming you are using Entity Framework
using MusicStore.Domain.Models; // Replace with the correct namespace
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicStore.Repository;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;
using System.Diagnostics;
using NuGet.DependencyResolver;
using MusicStore.Service.Interface;
using MusicStore.Repository.Interface;

namespace MusicStore.Web.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly IArtistRepository _artistRepository;
        private readonly IAlbumRepository _albumRepository;
        private readonly ITrackRepository _trackRepository;
        private readonly ITrackService _trackService;
        private readonly ApplicationDbContext _context; // Add DbContext here

        public AlbumsController(
            IArtistRepository artistRepository,
            IAlbumRepository albumRepository,
            ITrackRepository trackRepository,
            ITrackService trackService,
            ApplicationDbContext context) // Inject DbContext
        {
            _artistRepository = artistRepository;
            _albumRepository = albumRepository;
            _trackRepository = trackRepository;
            _trackService = trackService;
            _context = context; // Initialize DbContext
        }

        // GET: Album/Create
        public IActionResult Create()
        {
            var artists = _artistRepository.GetAllArtists();
            ViewBag.Artists = JsonSerializer.Serialize(artists);

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(
          Album album,
          string selectedArtistIds,
          int numberOfTracksInput,
          string[] trackTitles,
          string[] trackDurations,
          string[] trackYoutubeURLs,
          string[] trackSelectedArtistIds)
        {
            Debug.WriteLine("Create POST action invoked.");

            // Debug information
            Debug.WriteLine($"Album Model: {album.Title}, {album.Description}, {album.Tags}, {album.CoverImageUrl}, {album.Type}, {album.ReleaseDate}");
            Debug.WriteLine($"Selected Artist IDs: {selectedArtistIds}");
            Debug.WriteLine($"Number of Tracks Input: {numberOfTracksInput}");

            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model state is not valid.");

                // Re-populate artists for the view
                var artistList = _artistRepository.GetAllArtists();
                ViewBag.Artists = artistList.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList();

                // Log validation errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Debug.WriteLine($"ModelState Error: {error.ErrorMessage}");
                }

                return View(album);
            }

            // Parse and set the selected artists for the album
            if (!string.IsNullOrEmpty(selectedArtistIds))
            {
                Debug.WriteLine("Selected artist IDs: " + selectedArtistIds);
                var artistIds = selectedArtistIds.Split(',').Select(Guid.Parse).ToList();
                album.Artists = _artistRepository.GetAllArtists().Where(a => artistIds.Contains(a.Id)).ToList();
            }

            // Save the album
            Debug.WriteLine("Saving album...");
            _albumRepository.AddAlbum(album);
            _albumRepository.SaveChanges();

            Debug.WriteLine($"Album saved with Id: {album.Id}");

            // Create and add tracks
            var trackList = new List<Track>();
            for (int i = 0; i < numberOfTracksInput; i++)
            {
                var title = i < trackTitles.Length ? trackTitles[i] : null;
                var durationStr = i < trackDurations.Length ? trackDurations[i] : null;
                var youtubeURL = i < trackYoutubeURLs.Length ? trackYoutubeURLs[i] : null;
                var artistIdsStr = i < trackSelectedArtistIds.Length ? trackSelectedArtistIds[i] : null;

                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(durationStr))
                {
                    ModelState.AddModelError(string.Empty, "Invalid track information.");
                    Debug.WriteLine("Track information is invalid.");
                    return View(album);
                }

                // Format the duration
                string formattedDuration = (durationStr.Length == 5 && durationStr[2] == ':') ? durationStr : $"{durationStr}:00";
                Debug.WriteLine($"Track Title: {title}, Duration: {formattedDuration}, YouTube URL: {youtubeURL}");

                var track = new Track
                {
                    Title = title,
                    Duration = formattedDuration,
                    YoutubeURL = youtubeURL,
                    DateAdded = DateTime.Now,
                    Artists = new List<Artist>(),
                    AlbumId = album.Id // Set the AlbumId here
                };

                _trackRepository.AddTrack(track);
                trackList.Add(track);
                Debug.WriteLine($"Track added with Id: {track.Id}");
            }

            // Handle TrackArtist entries
            foreach (var track in trackList)
            {
                var index = trackList.IndexOf(track);
                if (index >= trackSelectedArtistIds.Length)
                {
                    Debug.WriteLine($"Index out of bounds: Track Id {track.Id} does not have a corresponding artist list.");
                    continue;
                }

                var artistIdsStr = trackSelectedArtistIds[index];
                if (string.IsNullOrEmpty(artistIdsStr))
                {
                    Debug.WriteLine($"No artist IDs provided for Track Id: {track.Id}");
                    continue;
                }

                Debug.WriteLine($"Track Id: {track.Id}, Artist IDs: {artistIdsStr}");

                List<Guid> trackArtistIds;
                try
                {
                    trackArtistIds = artistIdsStr.Split(',').Select(Guid.Parse).ToList();
                }
                catch (FormatException ex)
                {
                    Debug.WriteLine($"Error parsing artist IDs for Track Id: {track.Id}. Exception: {ex.Message}");
                    continue;
                }

                foreach (var artistId in trackArtistIds)
                {
                    try
                    {
                        _trackService.AddArtistToTrack(track.Id, artistId);
                        Debug.WriteLine($"Added Artist Id: {artistId} to Track Id: {track.Id}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error adding Artist Id: {artistId} to Track Id: {track.Id}. Exception: {ex.Message}");
                    }
                }
            }


            return RedirectToAction(nameof(Index));
        }







        // GET: Album/Index
        public IActionResult Index()
        {
            var albums = _albumRepository.GetAllAlbums();

            return View(albums);
        }
    }


}
