using SpotifyClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AuthorizationFlow
{
    class SpotifyFunctions : SpotifyAPI
    {
        public UserProfile GetUserProfile()
        {
            List<string> scopes = new List<string> { Scopes.UserReadBirthdate, Scopes.UserReadEmail, Scopes.UserReadPrivate };
            string UserEndpoint = "https://api.spotify.com/v1/me";

            UserProfile user = GetData<UserProfile>(UserEndpoint, scopes);
            return user;
        }

        public artistList GetTopArtists(int limit = 10)
        {
            List<string> scopes = new List<string> { Scopes.UserTopRead };
            string TopArtistsEndpoint = "https://api.spotify.com/v1/me/top/artists?limit=" + limit.ToString();

            artistList artists = GetData<artistList>(TopArtistsEndpoint, scopes);
            return artists;
        }

        public spotifyTrack GetTrack(string TrackID)
        {
            List<string> scopes = new List<string>();
            string GetTrackEndpoint = "https://api.spotify.com/v1/tracks/" + TrackID;

            spotifyTrack track = GetData<spotifyTrack>(GetTrackEndpoint, scopes);
            return track;
        }

        public spotifyAlbum GetAlbum(string AlbumID)
        {
            List<string> scopes = new List<string>();
            string GetAlbumEndpoint = "https://api.spotify.com/v1/albums/" + AlbumID;

            spotifyAlbum album = GetData<spotifyAlbum>(GetAlbumEndpoint, scopes);
            return album;
        }

        public artistInfo GetArtist(string ArtistID)
        {
            List<string> scopes = new List<string>();
            string GetArtistEndpoint = "https://api.spotify.com/v1/artists/" + ArtistID;

            artistInfo artist = GetData<artistInfo>(GetArtistEndpoint, scopes);
            return artist;
        }

        public tracks_list GetArtistTopTracks(string ArtistID)
        {
            List<string> scopes = new List<string>();
            string GetArtistTopEndpoint = String.Format("https://api.spotify.com/v1/artists/{0}/top-tracks?country=GB", ArtistID);

            tracks_list tracks = GetData<tracks_list>(GetArtistTopEndpoint, scopes);
            return tracks;
        }

        public spotifyCurrentlyPlaying GetCurrentlyPlaying()
        {
            List<string> scopes = new List<string> { Scopes.UserReadCurrentlyPlaying };
            string CurrentlyPlayingEndpoint = "https://api.spotify.com/v1/me/player/currently-playing";

            spotifyCurrentlyPlaying track = GetData<spotifyCurrentlyPlaying>(CurrentlyPlayingEndpoint, scopes);
            return track;
        }

        public void Pause()
        {
            List<string> scopes = new List<string> { Scopes.UserModifyPlaybackState };
            string PausePlaybackEndpoint = "https://api.spotify.com/v1/me/player/pause";

            PutData(PausePlaybackEndpoint, scopes);
        }

        public void Play()
        {
            List<string> scopes = new List<string> { Scopes.UserModifyPlaybackState };
            string PlayPlaybackEndpoint = "https://api.spotify.com/v1/me/player/play?device_id=a42f20d5f7ae8e2b65851fb7c5ed2fdbce496564";

            PutData(PlayPlaybackEndpoint, scopes);
        }

        public void Play(SpotifyType type, string id)
        {
            List<string> scopes = new List<string> { Scopes.UserModifyPlaybackState };
            string PlayPlaybackEndpoint = "https://api.spotify.com/v1/me/player/play?device_id=a42f20d5f7ae8e2b65851fb7c5ed2fdbce496564";

            PutData(PlayPlaybackEndpoint, scopes, type, id);
        }

        public void ToggleShuffle(bool state)
        {
            List<string> scopes = new List<string> { Scopes.UserModifyPlaybackState };
            string ShuffleEndpoint = "https://api.spotify.com/v1/me/player/shuffle?state=" + state;

            PutData(ShuffleEndpoint, scopes);
        }

        public void Skip()
        {
            List<string> scopes = new List<string> { Scopes.UserModifyPlaybackState };
            string SkipEndpoint = "https://api.spotify.com/v1/me/player/next";

            PostData(SkipEndpoint, scopes);
        }

        public void Previous()
        {
            List<string> scopes = new List<string> { Scopes.UserModifyPlaybackState };
            string PreviousEndpoint = "https://api.spotify.com/v1/me/player/previous";

            PostData(PreviousEndpoint, scopes);
        }

        public void AddTracksToPlaylist(string playlist_id, string[] trackIDs, int position = 0)
        {
            List<string> scopes = new List<string> { Scopes.PlaylistModifyPublic, Scopes.PlaylistModifyPrivate };

            trackIDs = Array.ConvertAll(trackIDs, track => track = "spotify:track:" + track);
            string IDs_string = HttpUtility.UrlEncode(String.Join(",", trackIDs));
            string AddTrackEndpoint = String.Format("https://api.spotify.com/v1/playlists/{0}/tracks?position={1}&uris={2}", playlist_id, position, IDs_string);

            PostData(AddTrackEndpoint, scopes);
        }

        public void DeleteTracksFromPlaylist(string playlist_id, Dictionary<string, int> track_ids)
        {
            List<string> scopes = new List<string> { Scopes.PlaylistModifyPrivate, Scopes.PlaylistModifyPublic };
            string DeleteTrackEndpoint = String.Format("https://api.spotify.com/v1/playlists/{0}/tracks", playlist_id);

            DeleteData(DeleteTrackEndpoint, scopes, track_ids);
        }
    }
}
