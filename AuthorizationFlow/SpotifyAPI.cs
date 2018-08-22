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
        // method to send data e.g. add track to playlist
        public void PostData (string Endpoint, string access_token)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Endpoint);
            request.PreAuthenticate = true;
            request.Method = "POST";
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + access_token);
            request.ContentLength = 0;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        }

        // method to get data e.g. get user profile
        public string GetData(string Endpoint, string access_token)
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
    }
}
