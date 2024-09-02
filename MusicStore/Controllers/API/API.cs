using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Repository.Interface;

namespace MusicStore.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class API : ControllerBase
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IArtistRepository _artistRepository;
        private readonly IPlaylistRepository _playlistRepository;
        private readonly ITrackRepository _trackRepository;

        public API(IAlbumRepository albumRepository, IArtistRepository artistRepository, IPlaylistRepository playlistRepository, ITrackRepository trackRepository)
        {
            _albumRepository = albumRepository;
            _artistRepository = artistRepository;
            _playlistRepository = playlistRepository;
            _trackRepository = trackRepository;
        }


        [HttpGet("GetAllAlbums")]
        public IActionResult GetAllAlbums()
        {
            var albums = _albumRepository.GetAllAlbums();
            return Ok(albums);
        }

        [HttpGet("GetAllArtists")]
        public IActionResult GetAllArtists()
        {
            var artists = _artistRepository.GetAllArtists();
            return Ok(artists);
        }

        [HttpGet("GetAllPlaylists")]
        public IActionResult GetAllPlaylists()
        {
            var playlists = _playlistRepository.GetAllPlaylists();
            return Ok(playlists);
        }

        [HttpGet("GetAllTracks")]
        public IActionResult GetAllTracks()
        {
            var tracks = _trackRepository.GetAllTracks();
            return Ok(tracks);
        }

        [HttpGet("GetAlbumById/{id}")]
        public IActionResult GetAlbumById(Guid id)
        {
            var album = _albumRepository.GetAlbumById(id);
            return Ok(album);
        }

        [HttpGet("GetArtistById/{id}")]
        public IActionResult GetArtistById(Guid id)
        {
            var artist = _artistRepository.GetArtistById(id);
            return Ok(artist);
        }


        [HttpGet("GetPlaylistById/{id}")]
        public IActionResult GetPlaylistById(Guid id)
        {
            var playlist = _playlistRepository.GetPlaylistById(id);
            return Ok(playlist);
        }

        [HttpGet("GetTrackById/{id}")]
        public IActionResult GetTrackById(Guid id)
        {
            var track = _trackRepository.GetTrackById(id);
            return Ok(track);
        }


    }
}
