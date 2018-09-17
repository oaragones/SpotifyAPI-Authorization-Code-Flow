using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using SpotifyClasses;
using System.Runtime.Serialization.Json;

namespace AuthorizationFlow
{
    class SpotifyAPI
    {
        protected void DeleteData(string Endpoint, List<string> scopes, Dictionary<string, int> track_ids)
        {
            Token token = new Token(scopes);
            token.GetAccessToken();
            token = null;

            string access_token = File.ReadAllLines("AccessToken.txt")[0];

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Endpoint);
            request.PreAuthenticate = true;
            request.Method = "DELETE";
            request.Accept = "application/json";
            request.Headers.Add("Authorization", "Bearer " + access_token);
            request.ContentType = "application/json";
            
            List<string> tracks = new List<string>();
            foreach (string id in track_ids.Keys)
            {
                string r = "{\"uri\":\"spotify:track:" + id + "\",\"positions\":[" + track_ids[id] + "]}";
                tracks.Add(r);
            }
            string request_body = "{\"tracks\":[" + String.Join(",", tracks) + "]}";

            byte[] bytes = Encoding.ASCII.GetBytes(request_body);
            request.ContentLength = bytes.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        protected void PutData(string Endpoint, List<string> scopes, SpotifyType type, string id)
        {
            Token token = new Token(scopes);
            token.GetAccessToken();
            token = null;

            string access_token = File.ReadAllLines("AccessToken.txt")[0];

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Endpoint);
            request.PreAuthenticate = true;
            request.Method = "PUT";
            request.Accept = "application/json";
            request.Headers.Add("Authorization", "Bearer " + access_token);
            request.ContentType = "application/json";

            string request_body = "";
            switch (type)
            {
                case SpotifyType.Album:
                    request_body = "{\"context_uri\":\"spotify:album:" + id + "\",\"offset\":{\"position\":0},\"position_ms\":0}";
                    break;
                case SpotifyType.Playlist:
                    request_body = "{\"context_uri\":\"spotify:user:ram_marwaha:playlist:" + id + "\",\"offset\":{\"position\":1},\"position_ms\":0}";
                    break;
            }
            byte[] bytes = Encoding.ASCII.GetBytes(request_body);
            request.ContentLength = bytes.Length;
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        protected void PutData(string Endpoint, List<string> scopes)
        {
            Token token = new Token(scopes);
            token.GetAccessToken();
            token = null;

            string access_token = File.ReadAllLines("AccessToken.txt")[0];

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Endpoint);
            request.PreAuthenticate = true;
            request.Method = "PUT";
            request.Accept = "application/json";
            request.Headers.Add("Authorization", "Bearer " + access_token);
            request.ContentType = "application/json";
            request.ContentLength = 0;

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // method to send data e.g. add track to playlist
        protected void PostData (string Endpoint, List<string> scopes)
        {
            Token token = new Token(scopes);
            token.GetAccessToken();
            token = null;

            string access_token = File.ReadAllLines("AccessToken.txt")[0];

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Endpoint);
            request.PreAuthenticate = true;
            request.Method = "POST";
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + access_token);
            request.ContentLength = 0;

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // method to get data from spotify API (Generic Method)
        protected T GetData<T>(string endpoint, List<string> scopes)
        {
            Token token = new Token(scopes);
            token.GetAccessToken();
            token = null;

            string access_token = File.ReadAllLines("AccessToken.txt")[0];
            
            string resp = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
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

            T item = default(T);

            if (resp.Length > 0)
            {
                byte[] byteArray = Encoding.ASCII.GetBytes(resp);
                MemoryStream stream = new MemoryStream(byteArray);

                var serializer = new DataContractJsonSerializer(typeof(T));
                item = (T)serializer.ReadObject(stream);
            }

            return item;
        }
    }
}
