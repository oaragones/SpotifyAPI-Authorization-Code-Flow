using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace AuthorizationFlow
{
    class HttpServer
    {
        static HttpListener _httpListener = new HttpListener();
        static string html { get; set; }
        static Thread _ResponseThread { get; set; }

        public HttpServer()
        {
            html = File.ReadAllText(@"E:\Ram\Spotify Project\AuthorizationFlow\HTMLpage.txt");
        }

        public HttpServer(string HTML)
        {
            html = HTML;
        }

        static void ResponseThread()
        {
            while (true)
            {
                HttpListenerContext context = _httpListener.GetContext();
                HttpListenerRequest request = context.Request;

                string AuthCode = request.Url.Query.Substring(6);
                File.WriteAllText("AuthorizationCode.txt", AuthCode);

                byte[] byteArray = Encoding.UTF8.GetBytes(html);
                context.Response.OutputStream.Write(byteArray, 0, byteArray.Length);
                context.Response.KeepAlive = false;

                context.Response.Close();
                Console.WriteLine("Request given to response");
            }
        }

        public void run()
        {
            Console.WriteLine("Starting Server...");
            _httpListener.Prefixes.Add("http://localhost:5132/");
            _httpListener.Start();
            Console.WriteLine("Server Started");
            _ResponseThread = new Thread(ResponseThread);
            _ResponseThread.Start();
        }

        public void close()
        {
            _ResponseThread.Abort();
            _httpListener.Stop();
        }
    }
}
