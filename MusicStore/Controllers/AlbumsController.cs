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

namespace MusicStore.Web.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly ApplicationDbContext _context; // Assuming ApplicationDbContext is your DbContext

        public AlbumsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Album/Create
        public async Task<IActionResult> Create()
        {
            var artists = await _context.Artists
                .Select(a => new { a.Id, a.Name })
                .ToListAsync();

            ViewBag.Artists = JsonSerializer.Serialize(artists);

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Album model, string selectedArtistIds, string[] trackTitles, string[] trackDurations, string[] trackYoutubeURLs, string[] trackSelectedArtistIds)
        {
            Debug.WriteLine("Create action invoked.");
            Debug.WriteLine("===================================================");
            // Log all form data
            Debug.WriteLine("Form data:");
            foreach (var key in Request.Form.Keys)
            {
                Debug.WriteLine($"{key}: {Request.Form[key]}");
            }

            Debug.WriteLine($"trackSelectedArtistIds length: {trackSelectedArtistIds?.Length ?? 0}");
            Debug.WriteLine("===================================================");

            if (ModelState.IsValid)
            {
                Debug.WriteLine("Model state is valid.");
                Debug.WriteLine("===================================================");

                // Parse and set the selected artists for the album
                if (!string.IsNullOrEmpty(selectedArtistIds))
                {
                    var artistIds = selectedArtistIds.Split(',').Select(Guid.Parse).ToList();
                    Debug.WriteLine($"Selected artist IDs: {string.Join(", ", artistIds)}");
                    Debug.WriteLine("===================================================");

                    model.Artists = await _context.Artists
                        .Where(a => artistIds.Contains(a.Id))
                        .ToListAsync();

                    Debug.WriteLine($"Artists associated with album: {string.Join(", ", model.Artists.Select(a => a.Name))}");
                    Debug.WriteLine("===================================================");

                }

                // Save the album first to generate its Id
                _context.Albums.Add(model);
                await _context.SaveChangesAsync();

                Debug.WriteLine($"Album saved with Id: {model.Id}");
                Debug.WriteLine("===================================================");

                // Initialize tracks list
                var trackList = new List<Track>();

                if (trackTitles != null && trackTitles.Length > 0)
                {
                    Debug.WriteLine($"Processing {trackTitles.Length} tracks.");
                    Debug.WriteLine("===================================================");

                    for (int i = 0; i < trackTitles.Length; i++)
                    {
                        var title = trackTitles[i];
                        var durationStr = trackDurations[i];
                        var youtubeURL = trackYoutubeURLs[i];
                        var artistIdsStr = i < trackSelectedArtistIds.Length ? trackSelectedArtistIds[i] : null;

                        Debug.WriteLine($"Processing track {i + 1}: Title = {title}, Duration = {durationStr}, YouTube URL = {youtubeURL}, Selected Artist IDs = {artistIdsStr}");
                        Debug.WriteLine("===================================================");

                        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(durationStr))
                        {
                            ModelState.AddModelError(string.Empty, "Invalid track information.");
                            Debug.WriteLine("Track validation failed.");
                            Debug.WriteLine("===================================================");

                            // Redisplay the form with the existing model and validation errors
                            return View(model);
                        }

                        // Format the duration
                        string formattedDuration;
                        if (durationStr.Length == 5 && durationStr[2] == ':')
                        {
                            formattedDuration = durationStr;
                        }
                        else
                        {
                            formattedDuration = $"{durationStr}:00";
                        }
                        Debug.WriteLine($"Formatted duration: {formattedDuration}");
                        Debug.WriteLine("===================================================");

                        var track = new Track
                        {
                            Title = title,
                            Duration = formattedDuration,
                            YoutubeURL = youtubeURL,
                            DateAdded = DateTime.Now,
                            AlbumId = model.Id, // Now that the album is saved, this will have a valid Id
                            Artists = new List<Artist>() // Initialize Artists list here
                        };

                        // Parse and set the selected artists for each track
                        if (!string.IsNullOrEmpty(artistIdsStr))
                        {
                            var trackArtistIds = artistIdsStr.Split(',').Select(Guid.Parse).ToList();
                            Debug.WriteLine($"Selected artist IDs for track {i + 1}: {string.Join(", ", trackArtistIds)}");
                            Debug.WriteLine("===================================================");

                            var trackArtists = await _context.Artists
                                .Where(a => trackArtistIds.Contains(a.Id))
                                .ToListAsync();

                            track.Artists.AddRange(trackArtists);
                        }

                        trackList.Add(track);
                        Debug.WriteLine($"Track {i + 1} added to the list.");
                        Debug.WriteLine("===================================================");

                    }

                    // Add new tracks to the context
                    _context.Tracks.AddRange(trackList);
                    await _context.SaveChangesAsync();
                    Debug.WriteLine("Tracks saved to the database.");
                    Debug.WriteLine("===================================================");

                    // Insert entries into TrackArtist table
                    foreach (var track in trackList)
                    {
                        foreach (var artist in track.Artists)
                        {
                            var trackArtistInsertCommand = $"INSERT INTO TrackArtist (TrackId, ArtistId) VALUES ('{track.Id}', '{artist.Id}')";
                            await _context.Database.ExecuteSqlRawAsync(trackArtistInsertCommand);
                            Debug.WriteLine($"TrackArtist entry added: TrackId = {track.Id}, ArtistId = {artist.Id}");
                            Debug.WriteLine("===================================================");

                        }
                    }
                }
                else
                {
                    Debug.WriteLine("No tracks to process.");
                    Debug.WriteLine("===================================================");

                }

                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed, redisplay the form
            Debug.WriteLine("Model state is not valid. Redisplaying form.");
            Debug.WriteLine("===================================================");

            var artistList = await _context.Artists
                .Select(a => new { a.Id, a.Name })
                .ToListAsync();

            ViewBag.Artists = artistList.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToList();

            return View(model); // Return the view with validation errors
        }



        // GET: Album/Index
        public async Task<IActionResult> Index()
        {
            var albums = await _context.Albums
                .Include(a => a.Artists)
                .Include(a => a.Tracks)
                .ThenInclude(t => t.Artists) // Include artists for each track
                .ToListAsync();

            return View(albums);
        }
    }
}
