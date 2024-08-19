using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using MusicStore.Domain.Models; 
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
using MusicStore.Service.Implementation;

namespace MusicStore.Web.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly IArtistRepository _artistRepository;
        private readonly IAlbumRepository _albumRepository;
        private readonly ITrackRepository _trackRepository;
        private readonly ITrackService _trackService;
        private readonly ApplicationDbContext _context; 

        public AlbumsController(
            IArtistRepository artistRepository,
            IAlbumRepository albumRepository,
            ITrackRepository trackRepository,
            ITrackService trackService,
            ApplicationDbContext context) 
        {
            _artistRepository = artistRepository;
            _albumRepository = albumRepository;
            _trackRepository = trackRepository;
            _trackService = trackService;
            _context = context; 
        }

        // GET: Album/Create
        public IActionResult Create()
        {
            var artists = _artistRepository.GetAllArtists();
            ViewBag.Artists = JsonSerializer.Serialize(artists);

            return View();
        }


        public IActionResult Details(Guid id)
        {
            var album = _albumRepository.GetAlbumById(id);
            var tracks = _trackRepository.GetAllTracks().Where(t => t.AlbumId == id).ToList();

            if (album == null)
            {
                return NotFound();
            }

            if(tracks == null)
            {
                return NotFound();
            }

            //Debugging
            Debug.WriteLine($"Album: {album.Title}, {album.Description}, {album.Tags}, {album.CoverImageUrl}, {album.Type}, {album.ReleaseDate}");
            foreach (var track in tracks)
            {
                Debug.WriteLine($"Track: {track.Title}, {track.Duration}, {track.YoutubeURL}");
            }

            ViewBag.Album = album;
            ViewBag.Tracks = tracks;

            return View();
        }



        // GET: Albums/Edit/5
        public IActionResult Edit(Guid id)
        {
            var album = _albumRepository.GetAlbumById(id);
            if (album == null)
            {
                return NotFound();
            }
            return View(album);
        }

        // POST: Albums/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, Album album)
        {
            if (id != album.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _albumRepository.UpdateAlbum(album);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }
            }
            return View(album);
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
            for (int i = 0; i < trackList.Count; i++)
            {
                var artistIds = trackSelectedArtistIds[i].Split(',').Select(Guid.Parse).ToList();
                Debug.WriteLine($"Track {i + 1}: Artist IDs - {string.Join(", ", artistIds)}");

                var artists = new List<Artist>();
                foreach (var id in artistIds)
                {
                    Debug.WriteLine($"Fetching artist with ID: {id}");
                    var artist = _artistRepository.GetArtistById(id);
                    if (artist != null)
                    {
                        artists.Add(artist);
                        Debug.WriteLine($"Added artist: {artist.Name} (ID: {artist.Id}) to track {trackList[i].Id}");
                    }
                    else
                    {
                        Debug.WriteLine($"Artist with ID {id} not found for Track {trackList[i].Id}");
                    }
                }

                trackList[i].Artists = artists;
                Debug.WriteLine($"Total artists added to track {trackList[i].Id}: {artists.Count}");

                // Additional Debugging
                foreach (var artist in trackList[i].Artists)
                {
                    Debug.WriteLine($"Track {trackList[i].Id} has artist: {artist.Name} (ID: {artist.Id})");
                }
            }

            // Ensure that the changes are saved to the database
            try
            {
                _context.SaveChanges();
                Debug.WriteLine("Changes saved to the database.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving changes to the database: {ex.Message}");
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Album/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = _albumRepository.GetAlbumById(id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: Album/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                _albumRepository.DeleteAlbum(id);
                return RedirectToAction(nameof(Index)); 
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                // Log the exception and handle error
                Debug.WriteLine($"Exception: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        // GET: Album/Index
        public IActionResult Index()
        {
            var albums = _albumRepository.GetAllAlbums();

            return View(albums);
        }
    }


}
