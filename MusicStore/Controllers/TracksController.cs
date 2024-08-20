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

        // GET: Tracks/Create
        public IActionResult Create(Guid albumId)
        {
            // Retrieve the album to ensure it exists
            var album = _albumService.GetAlbumById(albumId);
            if (album == null)
            {
                return NotFound();
            }

            // Create a new Track instance if needed
            var track = new Track { AlbumId = albumId }; // Initialize as needed

            // Retrieve all artists
            var artists = _artistRepository.GetAllArtists();

            // Configure JsonSerializerOptions to handle cycles and format the JSON
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                WriteIndented = true // Optional: Makes the output more readable
            };

            // Pass the track, albumId, and the list of artists to the view via ViewBag
            ViewBag.AlbumId = albumId;
            ViewBag.Artists = JsonSerializer.Serialize(artists, options);
            return View(track); // Pass the track to the view
        }



        // POST: Tracks/Create
        [HttpPost]
        public IActionResult Create(Track track, string[] selectedArtistIds)
        {
            // Log the received Track and Artist IDs
            Debug.WriteLine("Received Track Model:");
            Debug.WriteLine($"Title: {track.Title}, Duration: {track.Duration}, AlbumId: {track.AlbumId}");
            Debug.WriteLine("Received Artist IDs:");
            foreach (var id in selectedArtistIds)
            {
                Debug.WriteLine(id);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the selected artists
                    var artistIds = selectedArtistIds.Select(id => Guid.Parse(id)).ToList();
                    var artists = _artistRepository.GetArtistsByIds(artistIds);

                    track.Artists = artists;
                    track.AlbumId = track.AlbumId; // Ensure this is properly set

                    // Save the track to the database
                    _trackService.AddTrack(track);
                    Debug.WriteLine("Track saved successfully.");

                    // Redirect to a suitable page
                    return RedirectToAction("Index", "Albums");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception occurred: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while creating the track.");
                }
            }
            else
            {
                // Log validation errors
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Debug.WriteLine($"Error in {modelState.Key}: {error.ErrorMessage}");
                    }
                }
            }


            return RedirectToAction("Index", "Albums");
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
            var artistIds = selectedArtistIds.Split(',')
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

            // Fetch the selected artists
            var artists = _artistRepository.GetArtistsByIds(artistIds);
            existingTrack.Artists = artists;

            // Save changes
            _trackService.UpdateTrack(existingTrack);

            // Redirect to the index or another view
            return RedirectToAction("Index", "Albums");

        }


    }
}
