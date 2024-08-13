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

        public AlbumsController(
            IArtistRepository artistRepository,
            IAlbumRepository albumRepository,
            ITrackRepository trackRepository,
            ITrackService trackService)
        {
            _artistRepository = artistRepository;
            _albumRepository = albumRepository;
            _trackRepository = trackRepository;
            _trackService = trackService;
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
        public IActionResult Create(Album model, string selectedArtistIds, int numberOfTracksInput, string[] trackTitles, string[] trackDurations, string[] trackYoutubeURLs, string[] trackSelectedArtistIds)
        {
            Debug.WriteLine("Create action invoked.");
            Debug.WriteLine("===================================================");
            Debug.WriteLine("Form data:");
            foreach (var key in Request.Form.Keys)
            {
                Debug.WriteLine($"{key}: {Request.Form[key]}");
            }

            Debug.WriteLine($"Number of tracks from input: {numberOfTracksInput}");
            Debug.WriteLine($"Track Titles: {string.Join(", ", trackTitles)}");
            Debug.WriteLine($"Track Durations: {string.Join(", ", trackDurations)}");
            Debug.WriteLine($"Track Youtube URLs: {string.Join(", ", trackYoutubeURLs)}");
            Debug.WriteLine($"Track Selected Artist IDs: {string.Join(", ", trackSelectedArtistIds)}");
            Debug.WriteLine("===================================================");

            if (ModelState.IsValid)
            {
                Debug.WriteLine("Model state is valid.");
                Debug.WriteLine("===================================================");

                // Parse and set the selected artists for the album
                if (!string.IsNullOrEmpty(selectedArtistIds))
                {
                    Debug.WriteLine($"Selected artist IDs: {selectedArtistIds}");
                    Debug.WriteLine("===================================================");

                    var artistIds = selectedArtistIds.Split(',').Select(Guid.Parse).ToList();
                    model.Artists = _artistRepository.GetAllArtists().Where(a => artistIds.Contains(a.Id)).ToList();
                }
                Debug.WriteLine("===================================================");
                Debug.WriteLine("Album model:");
                Debug.WriteLine(JsonSerializer.Serialize(model));
                Debug.WriteLine("===================================================");

                // Save the album first to generate its Id
                _albumRepository.AddAlbum(model);

                // Create and add tracks
                var trackList = new List<Track>();
                Debug.WriteLine("=====================New Track List Created======================");

                for (int i = 0; i < numberOfTracksInput; i++)
                {
                    var title = i < trackTitles.Length ? trackTitles[i] : null;
                    var durationStr = i < trackDurations.Length ? trackDurations[i] : null;
                    var youtubeURL = i < trackYoutubeURLs.Length ? trackYoutubeURLs[i] : null;
                    var artistIdsStr = i < trackSelectedArtistIds.Length ? trackSelectedArtistIds[i] : null;

                    Debug.WriteLine($"Processing track {i}: Title = {title}, Duration = {durationStr}, YoutubeURL = {youtubeURL}, ArtistIds = {artistIdsStr}");

                    if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(durationStr))
                    {
                        ModelState.AddModelError(string.Empty, "Invalid track information.");
                        return View(model);
                    }

                    // Format the duration
                    string formattedDuration = (durationStr.Length == 5 && durationStr[2] == ':') ? durationStr : $"{durationStr}:00";

                    var track = new Track
                    {
                        Title = title,
                        Duration = formattedDuration,
                        YoutubeURL = youtubeURL,
                        DateAdded = DateTime.Now,
                        Artists = new List<Artist>()
                    };

                    trackList.Add(track);

                    _trackRepository.AddTrack(track);
                }

                // Save all tracks to generate their Ids
                // You might want to re-fetch the tracks if you're using in-memory list
                var tracks = _trackRepository.GetAllTracks();

                // Create TrackArtist entries
                foreach (var track in trackList)
                {
                    var index = trackList.IndexOf(track);
                    var artistIdsStr = index < trackSelectedArtistIds.Length ? trackSelectedArtistIds[index] : null;

                    Debug.WriteLine($"TrackId: {track.Id}, ArtistIdsStr: {artistIdsStr}");

                    if (!string.IsNullOrEmpty(artistIdsStr))
                    {
                        var trackArtistIds = artistIdsStr.Split(',').Select(Guid.Parse).ToList();

                        foreach (var artistId in trackArtistIds)
                        {
                            _trackService.AddArtistToTrack(track.Id, artistId);
                        }
                    }
                }

                return RedirectToAction(nameof(Index)); // Redirect to avoid form resubmission
            }

            // If we reach here, something went wrong and the model is invalid
            var artistList = _artistRepository.GetAllArtists();

            ViewBag.Artists = artistList.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToList();

            return View(model); // Return the view with the model and any validation messages
        }

        // GET: Album/Index
        public IActionResult Index()
        {
            var albums = _albumRepository.GetAllAlbums();

            return View(albums);
        }
    }


}
