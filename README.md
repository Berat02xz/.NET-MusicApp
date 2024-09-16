![fcse_logo](https://github.com/BeratAhmetaj/Museudonia/blob/main/Gif%20Animations/Logo_FINKI_UKIM_EN/Logo_FINKI_UKIM_EN_00000.png)

# Music App Platform
This project is a fully-featured music streaming platform built with ASP.NET Core, following the principles of Clean Architecture (Onion Architecture). It allows users to manage artists, albums, and tracks with complete CRUD (Create, Read, Update, Delete) functionality. The platform also provides API endpoints for integration with external systems, including integration with the [Integrated Systems Restaurant App](https://github.com/JordanovaAntoaneta/Integrated-Systems-Restaurant-App).

![Gif](https://github.com/BeratAhmetaj/MusicApp/blob/master/.github/1%20.gif)

## Features
- **Artists/Albums/Tracks Management:** 
  - Add, edit, and delete artists, albums, and tracks.
  - Albums consist of multiple tracks, and tracks can be linked to multiple artists.
- **API Endpoints:**
  - The platform provides API endpoints for external system integrations.
  - Integrated with the Restaurant App.
- **Playlists:**
  - Users can create personal playlists, favorite songs and contribute to track listens.
  - Users can checkout and purchase all their playlist tracks using Stripe payment integration.
  - Export playlists as Excel sheets for easy sharing.


![Gif](https://github.com/BeratAhmetaj/MusicApp/blob/master/.github/2%20.gif)

# API Endpoints
When running the app, use these API Endpoints to get access to data from the database (hosted or local).

| API Endpoint                           | Description |
|----------------------------------------|-------------|
| `GET /api/api/GetAllAlbums`            | Retrieves a list of all albums, including all tracks within each album. |
| `GET /api/api/GetAllArtists`           | Retrieves a list of all artists. |
| `GET /api/api/GetAllTracks`            | Retrieves a list of all existing tracks. |
| `GET /api/api/GetAllPlaylists`         | Retrieves a list of all user-created playlists. |
| `GET /api/api/GetAlbumById/{id}`       | Fetches details of a specific album by its ID, including all its tracks. |
| `GET /api/api/GetArtistById/{id}`      | Fetches details of a specific artist by their ID. |
| `GET /api/api/GetPlaylistById/{id}`    | Fetches details of a specific playlist by its ID. |
| `GET /api/api/GetTrackById/{id}`       | Fetches details of a specific track by its ID. |

# Running The Project
Docker Building in progress
