using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace PWRMind.WebServer
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
        
        private static string GetResponse(string request, NameValueCollection parameters)
        {
            string response = string.Empty;

            using (var connection = new SqlConnection(Properties.Settings.Default.DatabaseConnectingString))
            using (var command = new SqlCommand(request, connection) { CommandType = CommandType.StoredProcedure })
            {
                if (parameters.Count > 0)
                {
                    foreach (string name in parameters)
                    {
                        if (name != null)
                        {
                            SqlParameter parameter = new SqlParameter("@" + name.ToUpper(), parameters[name]);
                            command.Parameters.Add(parameter);
                        }
                    }                    
                }

                connection.Open();
                using (XmlReader reader = command.ExecuteXmlReader())
                {
                    response = Transform(request, reader, parameters);
                }
                connection.Close();
            }
            return response;
        }

        private static string Transform(string request, XmlReader reader, NameValueCollection parameters)
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

                    if (parameters.Count > 0)
                    {
                        XsltArgumentList arguments = new XsltArgumentList();
                        foreach (string name in parameters)
                        {
                            if (name != null)
                            {
                                arguments.AddParam(name.ToUpper(), string.Empty, parameters[name]);
                            }
                        }

                        transform.Transform(doc, arguments, writer);
                    }
                    else
                    {
                        transform.Transform(doc, writer);
                    }
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
            server.AuthenticationSchemes = AuthenticationSchemes.Ntlm;
            server.Start();
            Console.WriteLine("Server started...");

            while (server.IsListening)
            {
                HttpListenerContext context = server.GetContext();
                HttpListenerRequest request = context.Request;
                if (request.HttpMethod == "POST")
                {

                }
                Console.WriteLine(request.RawUrl);
                string vm = request.Url.Segments[1].TrimEnd(new char[] { '/' });

                var queryString = HttpUtility.ParseQueryString(request.Url.Query);

                if (vm != "favicon.ico")
                {
                    SendResponse(context, GetResponse(vm, queryString));
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