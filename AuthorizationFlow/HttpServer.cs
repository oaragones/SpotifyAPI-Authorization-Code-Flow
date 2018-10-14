using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace AuthorizationFlow
{
    class HttpServer
    {
        static HttpListener _httpListener { get; set; }
        private static string html;
        static Thread _ResponseThread { get; set; }
        private static ManualResetEvent _resetEvent;


        public HttpServer()
        {
            html = File.ReadAllText(@"C:\Arjun\Visual Studio 2017\Projects\AuthorizationFlow\HTMLpage.txt");
            _httpListener = new HttpListener();
            _resetEvent = new ManualResetEvent(false);
        }

        static void ResponseThread()
        {
            try
            {
                HttpListenerContext context = _httpListener.GetContext();
                HttpListenerRequest request = context.Request;

                string AuthCode = request.Url.Query.Substring(6);
                File.WriteAllText("AuthorizationCode.txt", AuthCode);
                Console.WriteLine("Code Recieved, Event Set to Go");

                _resetEvent.Set();

                byte[] byteArray = Encoding.UTF8.GetBytes(html);
                context.Response.OutputStream.Write(byteArray, 0, byteArray.Length);
                context.Response.KeepAlive = false;

                context.Response.Close();
                Console.WriteLine("Request given to response");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void run()
        {
            Console.WriteLine("About to Listen");
            _httpListener.Prefixes.Add("http://localhost:5132/");
            _httpListener.Start();
            Console.WriteLine("Server Started");
            _ResponseThread = new Thread(ResponseThread);
            _ResponseThread.Start();
        }

        public void wait()
        {
            Console.WriteLine("Waiting for event to signal...");
            _resetEvent.WaitOne();
            Console.WriteLine("Event signalled, ready to go");
        }

        public void close()
        {
            _ResponseThread = null;
            Console.WriteLine("Closing HttpListener");
            _httpListener.Abort();
        }
    }
}
