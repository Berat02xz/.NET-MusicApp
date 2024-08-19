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

namespace MusicStore.Web.Controllers
{
    public class TracksController : Controller
    {
        private readonly ITrackService _trackService;
        private readonly IArtistRepository _artistRepository;
        private readonly ApplicationDbContext _context;

        public TracksController(ITrackService trackService, IArtistRepository artistRepository, ApplicationDbContext context)
        {
            _trackService = trackService;
            _artistRepository = artistRepository;
            _context = context;
        }

        // GET: Tracks/Edit/5
        public IActionResult Edit(Guid id)
        {
            var track = _trackService.GetTrackById(id);
            var artists = _artistRepository.GetAllArtists();

            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                WriteIndented = true // Optional: Makes the output more readable
            };

            ViewBag.Artists = JsonSerializer.Serialize(artists, options);
            ViewBag.Track = track;

            return View();
        }



        // POST: Tracks/Edit

        [HttpPost]
        public IActionResult Edit(Track model, Guid[] selectedArtistIds)
        {
            Debug.WriteLine("Edit POST action started.");

            // Log the received model
            Debug.WriteLine($"Received model: Id={model.Id}, Title={model.Title}, AlbumId={model.AlbumId}");

            // Check if model state is valid
            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model state is invalid.");

                // Log detailed validation errors
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Debug.WriteLine($"Error for {modelState.Key}: {error.ErrorMessage}");
                    }
                }

                return View(model);
            }

            Debug.WriteLine($"Model state is valid. Track ID: {model.Id}");

            var track = _trackService.GetTrackById(model.Id);
            if (track == null)
            {
                Debug.WriteLine("Track not found.");
                return NotFound(); // Or handle the null case appropriately
            }

            Debug.WriteLine($"Track found. Title: {track.Title}");

            // Update track properties
            track.Title = model.Title;
            track.Duration = model.Duration;
            track.YoutubeURL = model.YoutubeURL;
            track.AlbumId = model.AlbumId; // Ensure the AlbumId is updated

            Debug.WriteLine("Updating track artists.");

            // Fetch selected artists
            var selectedArtists = _artistRepository.GetArtistsByIds(selectedArtistIds);

            // Check if the number of selected artists matches the number of IDs
            if (selectedArtists.Count != selectedArtistIds.Length)
            {
                Debug.WriteLine("Mismatch between selected artist IDs and fetched artists.");
            }

            track.Artists = selectedArtists;

            // Update the track
            try
            {
                _trackService.UpdateTrack(track);
                Debug.WriteLine("Track updated successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating track: {ex.Message}");
                // Handle the exception appropriately
                return StatusCode(500, "Internal server error"); // Or another appropriate response
            }

            // Redirect or return a different view
            return RedirectToAction("Index"); // Or another appropriate action
        }


    }
}
