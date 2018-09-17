using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using SpotifyClasses;
using System.Linq;

namespace AuthorizationFlow
{
    class Token
    {
        private HttpServer _server { get; set; }

        private HashSet<string> _scopes { get; set; }
        private string _ClientID = Environment.GetEnvironmentVariable("SpotifyClientID");
        private string _ClientSecret = Environment.GetEnvironmentVariable("SpotifyClientSecret");
        private string _RedirectURI = "http://localhost:5132/callback";

        public Token()
        {
            _scopes = new HashSet<string>();
        }

        public Token(List<string> scopes)
        {
            _scopes = new HashSet<string>();
            foreach (string s in scopes)
            {
                _scopes.Add(s);
            }
        }

        private void MakeAuthorizationCode()
        {
            string response_type = AuthTypes.AuthorizationFlow;
            string scopes = String.Join("%20", _scopes);
            string endpoint = String.Format("https://accounts.spotify.com/authorize/?client_id={0}&response_type={1}&redirect_uri={2}" + ((_scopes.Count == 0) ? "" : "&scope=" + scopes), _ClientID, response_type, _RedirectURI);

            _server = new HttpServer();
            _server.run();
            Console.WriteLine(endpoint);
            System.Diagnostics.Process.Start(endpoint);
            //_server.wait();
        }


        public void GetAccessToken()
        {
            bool new_token = _scopes.Count > 0;
            string[] lines = new string[3];
            DateTime prev_time = DateTime.Now;
            try
            {
                lines = File.ReadAllLines("AccessToken.txt");
                string[] previous_tokens = lines[1].Split(new string[] { "%20" }, StringSplitOptions.RemoveEmptyEntries);
                if (previous_tokens.Length > 0)
                {
                    foreach (string s in _scopes)
                    {
                        if (!previous_tokens.Contains(s))
                        {
                            new_token = true;
                            break;
                        }
                        else { new_token = false; }
                    }
                }

                prev_time = Convert.ToDateTime(lines[2]);
            }
            catch (Exception e) { }

            int c = (DateTime.Now - prev_time).CompareTo(new TimeSpan(1, 0, 0));
            if (!new_token)
            {
                if (c > 0)
                {
                    RefreshAccessToken();
                }
                return;
            }
            else
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

                        File.WriteAllLines("AccessToken.txt", new string[] { AccessToken, String.Join("%20", _scopes), DateTime.Now.ToString() });
                        File.WriteAllText("RefreshToken.txt", RefreshToken);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                _server.close();
                _server = null;
            }
        }


        private void RefreshAccessToken()
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
                File.WriteAllLines("AccessToken.txt", new string[] { AccessToken, String.Format("%20", _scopes), DateTime.Now.ToString() });
            }
        }
    }
}
