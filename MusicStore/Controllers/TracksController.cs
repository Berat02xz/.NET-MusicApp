using Microsoft.AspNetCore.Mvc;
using MusicStore.Domain.Models;
using MusicStore.Repository;
using MusicStore.Repository.Interface;
using MusicStore.Service.Implementation;
using MusicStore.Service.Interface;
using System.Text.Json;
using Azure.Core;
using System.Diagnostics;
using NuGet.DependencyResolver;
using MusicStore.Repository.Implementation;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace MusicStore.Web.Controllers
{
    public class TracksController : Controller
    {
        private readonly ITrackService _trackService;
        private readonly IArtistRepository _artistRepository;
        private readonly ApplicationDbContext _context;
        private readonly IAlbumService _albumService;
        public TracksController(ITrackService trackService, IArtistRepository artistRepository, ApplicationDbContext context, IAlbumService albumService)
        {
            _trackService = trackService;
            _artistRepository = artistRepository;
            _context = context;
            _albumService = albumService;
        }



        public IActionResult Create(Guid albumId)
        {
            // Retrieve the album to ensure it exists
            var album = _albumService.GetAlbumById(albumId);
            if (album == null)
            {
                return NotFound();
            }

            // Retrieve all artists
            var artists = _artistRepository.GetAllArtists();

            // Configure JsonSerializerOptions to handle cycles
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            // Serialize the artists list to JSON
            var artistsJson = JsonSerializer.Serialize(artists, options);

            // Pass the albumId and artists JSON to the view via ViewBag
            ViewBag.AlbumId = albumId;
            ViewBag.Artists = artistsJson;

            // Return the view with a blank track model
            return View(new Track());
        }








        // POST: Tracks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string title, string duration, Guid albumId, string[] selectedArtistIds, string youtubeURL)
        {
            Debug.WriteLine("Received Track Data:");
            Debug.WriteLine($"Title: {title}, Duration: {duration}, AlbumId: {albumId}");

            if (selectedArtistIds == null || selectedArtistIds.Length == 0)
            {
                ModelState.AddModelError("", "Please select at least one artist.");
            }

            if (albumId == Guid.Empty)
            {
                ModelState.AddModelError("AlbumId", "The Album field is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string formattedDuration = (duration.Length == 5 && duration[2] == ':') ? duration : $"{duration}:00";

                    // Create a new Track instance and populate it
                    var track = new Track
                    {
                        Title = title,
                        Duration = formattedDuration,
                        YoutubeURL = youtubeURL,
                        AlbumId = albumId,
                        DateAdded = DateTime.Now
                    };

                    // Fetch the album object using AlbumId
                    track.Album = _albumService.GetAlbumById(albumId);

                    // Convert the selected artist IDs to a list of GUIDs
                    var artistIds = selectedArtistIds.Select(id => Guid.Parse(id)).ToList();

                    // Retrieve the corresponding artist entities from the repository
                    var artists = _artistRepository.GetArtistsByIds(artistIds);

                    // Assign the selected artists to the track
                    track.Artists = artists;

                    // Save the track to the database using the service layer
                    _trackService.AddTrack(track);
                    Debug.WriteLine("Track saved successfully.");

                    // Redirect to a suitable page, typically back to the album's details or index
                    return RedirectToAction("Details", "Albums", new { id = albumId });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception occurred: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while creating the track.");
                }
            }

            // Log validation errors
            foreach (var modelState in ModelState)
            {
                foreach (var error in modelState.Value.Errors)
                {
                    Debug.WriteLine($"Error in {modelState.Key}: {error.ErrorMessage}");
                }
            }

            // Re-populate the artist selection list in case of errors
            var artistList = _artistRepository.GetAllArtists();
            ViewBag.Artists = artistList.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToList();

            // Return to the creation view if there are errors
            ViewBag.AlbumId = albumId;
            return View();
        }











        // GET: Tracks/Edit/5
        public IActionResult Edit(Guid id)
        {
            var track = _trackService.GetTrackById(id);
            if (track == null)
            {
                return NotFound();
            }

            var artists = _artistRepository.GetAllArtists();
            var artistIds = track.Artists.Select(a => a.Id).ToList();

            ViewBag.Artists = JsonSerializer.Serialize(artists, new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });
            ViewBag.SelectedArtistIds = JsonSerializer.Serialize(artistIds);

            return View(track);
        }




        // POST: Tracks/Edit
        [HttpPost]
        public IActionResult Edit(Track track, string selectedArtistIds)
        {
            // Log received data for debugging
            Debug.WriteLine("Received Track ID: " + track.Id);
            Debug.WriteLine("Received Selected Artist IDs: " + selectedArtistIds);

            // Convert selectedArtistIds to a list of GUIDs with improved error handling

                var artistIds = new List<Guid>();

            if (string.IsNullOrEmpty(selectedArtistIds) || selectedArtistIds == "0")
            {
              
            }
            else
            {
                artistIds = selectedArtistIds.Split(',')
                                             .Select(id => id.Trim())
                                             .Where(id => !string.IsNullOrEmpty(id))
                                             .Select(id =>
                                             {
                                                 Guid parsedGuid;
                                                 bool success = Guid.TryParse(id, out parsedGuid);
                                                 if (!success)
                                                 {
                                                     Debug.WriteLine($"Failed to parse GUID: {id}");
                                                 }
                                                 return parsedGuid;
                                             })
                                             .Where(guid => guid != Guid.Empty)
                                             .ToList();
            }

            // Log the parsed artist IDs
            Debug.WriteLine("Parsed Artist IDs: " + string.Join(", ", artistIds));

            // Retrieve the track to update
            var existingTrack = _trackService.GetTrackById(track.Id);
            if (existingTrack == null)
            {
                return NotFound();
            }

            // Update track properties
            existingTrack.Title = track.Title;
            existingTrack.Duration = track.Duration;
            existingTrack.YoutubeURL = track.YoutubeURL;
            existingTrack.ListenCount = track.ListenCount;
            existingTrack.DateAdded = DateTime.Now;

            // Fetch the selected artists
            var artists = _artistRepository.GetArtistsByIds(artistIds);
            existingTrack.Artists = artists;

            // Save changes
            _trackService.UpdateTrack(existingTrack);

            // Redirect to the index or another view
            return RedirectToAction("Index", "Albums");

        }





        //MISCLELLANIOUS METHODS
        [HttpPost]
        public IActionResult Play(Guid id)
        {
            var track = _trackService.GetTrackById(id);
            if (track == null)
            {
                return NotFound();
            }

            // Increment the listen count
            track.ListenCount++;
            _trackService.UpdateTrack(track);

            // Return a response (can be empty if no specific response is needed)
            return Ok();
        }


    }
}
