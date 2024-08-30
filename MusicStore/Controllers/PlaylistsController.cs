using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStore.Domain.Identity;
using MusicStore.Service.Interface;
using System.Diagnostics;
using System.Security.Claims;

namespace MusicStore.Web.Controllers
{
    public class PlaylistsController : Controller
    {
        private readonly IPlaylistService _playlistService;
        private readonly ITrackService _trackService;
        private readonly UserManager<MusicStoreUser> _userManager;

        public PlaylistsController(UserManager<MusicStoreUser> userManager,IPlaylistService playlistService, ITrackService trackService)
        {
            _playlistService = playlistService;
            _trackService = trackService;
            _userManager = userManager;
        }

        // GET: Playlist/Index
        // Display the playlist with all its tracks for the current user
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieve user ID from claims

            if (userId == null)
            {
                Debug.WriteLine("UserId not exist");
                return Unauthorized(); // or Redirect to login page
            }

            var playlist = _playlistService.GetPlaylistByUserId(userId);

            if (playlist == null)
            {
                Debug.WriteLine("Playlist is null - Adding a new playlist");
                playlist = _playlistService.CreatePlaylistForUser(userId);
                Debug.WriteLine("Playlist Added to user");
            }

            return View(playlist); // Pass the playlist to the view
        }

        [HttpPost]
        public IActionResult AddTrackToPlaylist(Guid trackId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Retrieve user ID from claims

            if (userId == null)
            {
                return Unauthorized(); // or redirect to login page
            }

            var user = _userManager.Users
                .Include(u => u.Playlists)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(); // Handle case where user is not found
            }

            // Find the first playlist of the user
            var playlist = user.Playlists.FirstOrDefault();

            if (playlist == null)
            {
                return NotFound(); // Handle case where user does not have any playlists
            }

            // Find the track by ID
            var track = _trackService.GetTrackById(trackId);

            if (track == null)
            {
                return NotFound(); // Handle case where track is not found
            }

            // Add the track to the playlist
            playlist.PlaylistTracks.Add(track);

            // Save changes
            _playlistService.UpdatePlaylist(playlist);

            return Ok(); // Return a successful response
        }


        // POST: Playlist/RemoveTrack
        // Remove a track from the playlist
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveTrack(Guid playlistId, Guid trackId)
        {
            var playlist = _playlistService.GetPlaylistById(playlistId);
            if (playlist == null)
            {
                return NotFound();
            }

            var track = playlist.PlaylistTracks.FirstOrDefault(t => t.Id == trackId);
            if (track != null)
            {
                playlist.PlaylistTracks.Remove(track);
                _playlistService.UpdatePlaylist(playlist); // Update the playlist after removing the track
            }

            return RedirectToAction(nameof(Index), new { playlistId = playlistId });
        }

        // GET: Playlist/Checkout
        // Display the checkout page
        public IActionResult Checkout(Guid playlistId)
        {
            var playlist = _playlistService.GetPlaylistById(playlistId);
            if (playlist == null)
            {
                return NotFound();
            }

            // Logic for handling the checkout process goes here
            // For example, creating an order from the playlist tracks

            // Redirect to a confirmation page or return the checkout view
            return View(playlist);
        }
    }
}
