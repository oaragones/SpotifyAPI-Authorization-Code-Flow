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
        static string html { get; set; }
        static Thread _ResponseThread { get; set; }

        public HttpServer()
        {
            html = File.ReadAllText(@"C:\Arjun\Visual Studio 2017\Projects\AuthorizationFlow\HTMLpage.txt");
            _httpListener = new HttpListener();
        }

        public HttpServer(string HTML)
        {
            html = HTML;
            _httpListener = new HttpListener();
        }

        static void ResponseThread()
        {
            try
            {
                HttpListenerContext context = _httpListener.GetContext();
                HttpListenerRequest request = context.Request;

                string AuthCode = request.Url.Query.Substring(6);
                File.WriteAllText("AuthorizationCode.txt", AuthCode);
                Console.WriteLine("Code = {0}", AuthCode);

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

        public void wait()
        {
            Console.WriteLine("Waiting for response thread to finish");
            _ResponseThread.Join();
            Console.WriteLine("Response thread finished");
        }

        public void run()
        {
            _httpListener.Prefixes.Add("http://localhost:5132/");
            _httpListener.Start();
            Console.WriteLine("Server Started");
            _ResponseThread = new Thread(ResponseThread);
            _ResponseThread.Start();
        }

        public void close()
        {
            _ResponseThread = null;
            _httpListener = null;
        }
    }
}
