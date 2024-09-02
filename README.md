![fcse_logo](https://github.com/BeratAhmetaj/Museudonia/blob/main/Gif%20Animations/Logo_FINKI_UKIM_EN/Logo_FINKI_UKIM_EN_00000.png)


# Early Work In Progress
a scalable music management app built with ASP.NET Core 8.0 and Onion architecture. It supports CRUD operations for albums, tracks, artists, and user playlists, with customizable ordering and data export features. Designed for Azure Cloud deployment.


# API Endpoints

When running the app, use these API Endpoints to get access to data from the database (hosted or local).

| API Endpoint                           | Description |
|----------------------------------------|-------------|
| `GET /api/api/GetAllAlbums`            | Retrieves a list of all albums, including all tracks within each album. |
| `GET /api/api/GetAllArtists`           | Retrieves a list of all artists. |
| `GET /api/api/GetAllTracks`            | Retrieves a list of all existing tracks. |
| `GET /api/api/GetAllPlaylists`         | Retrieves a list of all user-created playlists. |
| `GET /api/api/GetAlbumById/{id}`       | Fetches details of a specific album by its ID, including all associated tracks. |
| `GET /api/api/GetArtistById/{id}`      | Fetches details of a specific artist by their ID. |
| `GET /api/api/GetPlaylistById/{id}`    | Fetches details of a specific playlist by its ID. |
| `GET /api/api/GetTrackById/{id}`       | Fetches details of a specific track by its ID. |
