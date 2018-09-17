using System;
using System.Collections.Generic;
using System.IO;
using SpotifyClasses;
using System.Diagnostics;

namespace AuthorizationFlow
{
    class Program
    {
        static void Main(string[] args)
        {
            SpotifyFunctions s = new SpotifyFunctions();

            s.ToggleShuffle(false);
            //spotifyCurrentlyPlaying current = s.GetCurrentlyPlaying();
            //Console.WriteLine(current.item.name);
        }
    }

    public struct Playlists
    {
        public const string Americannn = "5ybgdI5zfh6NcMDQp5NHVk";
        public const string Bangerzzz = "0quqcsWCf5xt8z76kMFMzf";
    };

    public struct AuthTypes
    {
        public const string AuthorizationFlow = "code";
        public const string ImplicitGrant = "token";
        public const string ClientCredentials = "";
    };

   
    public struct Scopes
    {
        // Spotify Connect
        public const string UserReadCurrentlyPlaying = "user-read-currently-playing";
        public const string UserModifyPlaybackState = "user-modify-playback-state";
        public const string UserReadPlaybackState = "user-read-playback-state";

        // Playback
        public const string PlaybackStreaming = "streaming";
        public const string AppRemoteControl = "app-remote-control";

        // Playlists
        public const string PlaylistReadCollaborative = "playlist-read-collaborative";
        public const string PlaylistModifyPublic = "playlist-modify-public";
        public const string PlaylistReadPrivate = "playlist-read-private";
        public const string PlaylistModifyPrivate = "playlist-modify-private";

        // Users
        public const string UserReadPrivate = "user-read-private";
        public const string UserReadBirthdate = "user-read-birthdate";
        public const string UserReadEmail = "user-read-email";

        // Follow
        public const string UserFollowModiy = "user-follow-modify";
        public const string UserFollowRead = "user-follow-read";

        // Library
        public const string UserLibraryRead = "user-library-read";
        public const string UserLibraryModify = "user-library-modify";

        // Listening History
        public const string UserReadRecentlyPlayed = "user-read-recently-played";
        public const string UserTopRead = "user-top-read";
    }

    public enum SpotifyType
    {
        Album,
        Playlist
    };
}
