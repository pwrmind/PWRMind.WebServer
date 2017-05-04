using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace NVG.WebServer
{
    class Program
    {
        private static HttpListener server;

        public static void Main()
        {
            Thread server = new Thread(StartServer);
            server.Start();
            Console.WriteLine("Press \'q\' to quit.");
            while (Console.Read() != 'q') ;
        }
        
        private static string GetResponse(string request)
        {
            string response = string.Empty;

            using (var connection = new SqlConnection("Data Source=TFS01;Initial Catalog=Rosbank;Integrated Security=True"))
            using (var command = new SqlCommand(request, connection) { CommandType = CommandType.StoredProcedure })
            {
                connection.Open();
                using (XmlReader reader = command.ExecuteXmlReader())
                {
                    response = Transform(request, reader);
                }
                connection.Close();
            }
            return response;
        }

        private static string Transform(string request, XmlReader reader)
        {
            StringBuilder result = new StringBuilder();
            try
            {
                XPathDocument doc = new XPathDocument(reader);

                using (XmlWriter writer = XmlWriter.Create(result))
                {
                    XslCompiledTransform transform = new XslCompiledTransform();
                    XsltSettings settings = new XsltSettings();
                    settings.EnableScript = true;

                    transform.Load(".\\sourceFiles\\" + request + ".xslt", settings, null);

                    transform.Transform(doc, writer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result.ToString();
        }

        // http://csharpprogramming.ru/web/kak-sozdat-veb-server-s-pomoshhyu-klassa-httplistener
        
        private static void StartServer()
        {
            string prefix = "http://*:8080/";
            server = new HttpListener();

            if (!HttpListener.IsSupported) return;

            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentException("prefix");
            server.Prefixes.Add(prefix);

            server.Start();
            Console.WriteLine("Server started...");

            while (server.IsListening)
            {
                HttpListenerContext context = server.GetContext();
                HttpListenerRequest request = context.Request;
                Console.WriteLine(request.RawUrl);
                string vm = request.Url.Segments[1].TrimEnd(new char[] { '/' });
                if (vm != "favicon.ico")
                {
                    SendResponse(context, GetResponse(vm));
                }               
            }
        }

        private static void SendResponse(HttpListenerContext context, string responseString)
        {
            HttpListenerResponse response = context.Response;
            response.ContentType = "text/html; charset=UTF-8";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            using (Stream output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
            }
        }
    }    
}