using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using SpotifyClasses;

namespace AuthorizationFlow
{
    class Token
    {
        private HttpServer _server { get; set; }

        private string _scope { get; set; }
        private string _ClientID = Environment.GetEnvironmentVariable("SpotifyClientId");
        private string _ClientSecret = Environment.GetEnvironmentVariable("SpotifyClientSecret");
        private string _RedirectURI = "http://localhost:5132/callback";

        public Token(List<string> scopes)
        {
            _scope = String.Join("%20", scopes);
        }

        ~Token()
        {
            _server.close();
        }


        // no need to use again for the same scopes - use refresh token for the same scopes
        public void GetAccessToken()
        {
            MakeAuthorizationCode();
            var encodeIdSecret = Convert.ToBase64String(Encoding.UTF8.GetBytes(_ClientID + ":" + _ClientSecret));
            string AccessCode;
            try
            {
                AccessCode = File.ReadAllLines("AuthorizationCode.txt")[0];
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://accounts.spotify.com/api/token");
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Accept = "application/json";
                webRequest.Headers.Add("Authorization: Basic " + encodeIdSecret);

                var request = String.Format("grant_type=authorization_code&code={0}&redirect_uri={1}", AccessCode, _RedirectURI);
                byte[] requestBytes = Encoding.ASCII.GetBytes(request);
                webRequest.ContentLength = requestBytes.Length;

                Stream stream = webRequest.GetRequestStream();
                stream.Write(requestBytes, 0, requestBytes.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                spotifyToken token;
                using (Stream responseStream = response.GetResponseStream())
                {
                    var serializer = new DataContractJsonSerializer(typeof(spotifyToken));
                    token = (spotifyToken)serializer.ReadObject(responseStream);
                    string AccessToken = token.access_token;
                    string RefreshToken = token.refresh_token;
                    File.WriteAllText("AccessToken.txt", AccessToken);
                    File.WriteAllText("RefreshToken.txt", RefreshToken);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // refresh the token every hour
        public void RefreshAccessToken()
        {
            string refresh_token = File.ReadAllText("RefreshToken.txt");

            var encodeIdSecret = Convert.ToBase64String(Encoding.UTF8.GetBytes(_ClientID + ":" + _ClientSecret));

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://accounts.spotify.com/api/token");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Accept = "application/json";
            webRequest.Headers.Add("Authorization: Basic " + encodeIdSecret);

            var request = String.Format("grant_type=refresh_token&refresh_token={0}", refresh_token);
            byte[] requestBytes = Encoding.ASCII.GetBytes(request);
            webRequest.ContentLength = requestBytes.Length;

            Stream stream = webRequest.GetRequestStream();
            stream.Write(requestBytes, 0, requestBytes.Length);
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            spotifyToken token;
            using (Stream responseStream = response.GetResponseStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(spotifyToken));
                token = (spotifyToken)serializer.ReadObject(responseStream);
                string AccessToken = token.access_token;
                File.WriteAllText("AccessToken.txt", AccessToken);
            }
        }

        // make the token from the given scopes
        private void MakeAuthorizationCode()
        {
            string response_type = AuthTypes.AuthorizationFlow;
            string endpoint = String.Format("https://accounts.spotify.com/authorize/?client_id={0}&response_type={1}&redirect_uri={2}&scope={3}", _ClientID, response_type, _RedirectURI, _scope);

            _server = new HttpServer();
            _server.run();
            System.Diagnostics.Process.Start(endpoint);
        }
    }
}
