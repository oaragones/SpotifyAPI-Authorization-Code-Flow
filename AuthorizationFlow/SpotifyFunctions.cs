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
            Token token = new Token(scopes);
            token.GetAccessToken();
            string access_token = File.ReadAllText("AccessToken.txt");

            string UserEndpoint = "https://api.spotify.com/v1/me";

            string response = GetData(UserEndpoint, access_token);

            byte[] byteArray = Encoding.ASCII.GetBytes(response);
            MemoryStream stream = new MemoryStream(byteArray);

            var serializer = new DataContractJsonSerializer(typeof(UserProfile));
            UserProfile userProfile = (UserProfile)serializer.ReadObject(stream);

            return userProfile;
        }

        public void AddTracksToPlaylist(string playlist_id, string[] trackIDs, int position = 0)
        {
            List<string> scopes = new List<string> { Scopes.PlaylistModifyPublic, Scopes.PlaylistModifyPrivate };
            Token token = new Token(scopes);
            token.GetAccessToken();
            string access_token = File.ReadAllText("AccessToken.txt");

            trackIDs = Array.ConvertAll(trackIDs, track => track = "spotify:track:" + track);
            string IDs_string = HttpUtility.UrlEncode(String.Join(",", trackIDs));
            string AddTrackEndpoint = String.Format("https://api.spotify.com/v1/playlists/{0}/tracks?position={1}&uris={2}", playlist_id, position, IDs_string);

            PostData(AddTrackEndpoint, access_token);
        }

        public artistList GetTopArtists(int limit = 10)
        {
            List<string> scopes = new List<string> { Scopes.UserTopRead };
            Token token = new Token(scopes);
            token.GetAccessToken();
            string access_token = File.ReadAllText("AccessToken.txt");

            string TopArtistsEndpoint = "https://api.spotify.com/v1/me/top/artists?limit=" + limit.ToString();

            string response = GetData(TopArtistsEndpoint, access_token);

            byte[] byteArray = Encoding.ASCII.GetBytes(response);
            MemoryStream stream = new MemoryStream(byteArray);

            var serializer = new DataContractJsonSerializer(typeof(artistList));
            artistList TopArtists = (artistList)serializer.ReadObject(stream);

            return TopArtists;
        }
    }
}
