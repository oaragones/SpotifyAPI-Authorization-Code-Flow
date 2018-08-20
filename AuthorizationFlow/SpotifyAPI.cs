using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using SpotifyClasses;
using System.Runtime.Serialization.Json;

namespace AuthorizationFlow
{
    class SpotifyAPI
    {
        private string _ClientID = Environment.GetEnvironmentVariable("SpotifyClientId");
        private string _ClientSecret = Environment.GetEnvironmentVariable("SpotifyClientSecret");
        private string _RedirectURI = "http://localhost:5132/callback";
        
        public string MakeEndpoint(List<string> scopes, AuthTypes type)
        {
            string response_type = (type == AuthTypes.AuthorizationFlow) ? "code" : "token";
            string scope = String.Join("%20", scopes);

            string endPoint = String.Format("https://accounts.spotify.com/authorize/?client_id={0}&response_type={1}&redirect_uri={2}&scope={3}", _ClientID, response_type, _RedirectURI, scope);

            return endPoint;
        }
        
        // no need to use again for the same scopes - use refresh token
        public void GetAccessToken()
        {
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

        public void RefreshAccessToken(string refresh_token)
        {
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
                Console.WriteLine(AccessToken);
            }
        }

        private string GetResponse(string Endpoint, string access_token)
        {
            string resp = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Endpoint);
            request.PreAuthenticate = true;
            request.Headers.Add("Authorization", "Bearer " + access_token);
            request.Accept = "application/json";
            request.Method = "GET";
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Console.WriteLine("ERROR: ", response.StatusCode);
                    }
                    else
                    {
                        using (Stream ResponseStream = response.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(ResponseStream);
                            resp = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return resp;
        }

        public UserProfile GetUserProfile(string access_token)
        {
            string UserEndpoint = "https://api.spotify.com/v1/me";

            string response = GetResponse(UserEndpoint, access_token);
            byte[] byteArray = Encoding.ASCII.GetBytes(response);
            MemoryStream stream = new MemoryStream(byteArray);

            var serializer = new DataContractJsonSerializer(typeof(UserProfile));
            UserProfile userProfile = (UserProfile)serializer.ReadObject(stream);

            return userProfile;
        }
    }
}
