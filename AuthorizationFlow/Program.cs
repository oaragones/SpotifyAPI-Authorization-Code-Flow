using System;
using System.Collections.Generic;
using System.IO;
using SpotifyClasses;

namespace AuthorizationFlow
{
    class Program
    {
        static void Main(string[] args)
        {
            SpotifyAPI spotify = new SpotifyAPI();

            //---------------------------------
            //access token to read user profile
            //---------------------------------

            //List<string> scopes = new List<string> { "user-read-private", "user-read-birthdate", "user-read-email" }; // change scopes
            //string endpoint = spotify.MakeEndpoint(scopes, AuthTypes.AuthorizationFlow);

            //HttpServer server = new HttpServer();
            //server.run();
            //System.Diagnostics.Process.Start(endpoint);

            //spotify.GetAccessToken();
            //--------------------------------
            
            string AccessToken = File.ReadAllText("AccessToken.txt");
            UserProfile userProfile = spotify.GetUserProfile(AccessToken);
            Console.WriteLine(userProfile.birthdate);
            Console.WriteLine(userProfile.product);
            Console.WriteLine(userProfile.email);
        }
    }

    public enum AuthTypes
    {
        AuthorizationFlow,
        ImplicitGrant,
        ClientCredentials
    };

   
    public struct Scopes
    {
        public const string UserReadPrivate = "user-read-private";
        public const string UserReadBirthdate = "user-read-birthdate";
        public const string UserReadEmail = "user-read-email";

        public const string PlaylistReadCollaborative = "playlist-read-collaborative";
        public const string PlaylistModifyPublic = "playlist-modify-public";
        public const string PlaylistReadPrivate = "playlist-read-private";
        public const string PlaylistModifyPrivate = "playlist-modify-private";

        public const string UserReadCurrentlyPlaying = "user-read-currently-playing";
        public const string UserModifyPlaybackState = "user-modify-playback-state";
        public const string UserReadPlaybackState = "user-read-playback-state";
    }
}
