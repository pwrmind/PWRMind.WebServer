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
        //private static bool flag = true;

        public static void Main()
        {
            //Thread watcher = new Thread(Watch); //Создаем новый объект потока (Thread)
            Thread server = new Thread(StartServer); //Создаем новый объект потока (Thread)

            //watcher.Start();
            server.Start();
            Console.WriteLine("Press \'q\' to quit the sample.");
            while (Console.Read() != 'q') ;
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Watch()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = ".\\sourceFiles";
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.*";
            
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            //watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        // Define the event handlers.
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
            Transform(e.FullPath);
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            //Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            //Transform(e.FullPath);
        }

        private static void Transform(XmlReader reader)
        {
            Stream result = Stream.Null;
            try
            {
                XPathDocument doc = new XPathDocument(reader);
                //XPathNavigator navigator = doc.CreateNavigator();
                //XmlProcessingInstruction instruction = navigator.SelectSingleNode("processing-instruction('xml-stylesheet')") as XmlProcessingInstruction;
                //Directory.CreateDirectory(".\\processedFiles");
                using (XmlWriter writer = XmlWriter.Create(result))
                {
                    XslCompiledTransform transform = new XslCompiledTransform();
                    XsltSettings settings = new XsltSettings();
                    settings.EnableScript = true;

                    transform.Load(".\\sourceFiles\\" + Path.GetFileNameWithoutExtension(source) + ".xslt", settings, null);

                    transform.Transform(doc, writer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // http://csharpprogramming.ru/web/kak-sozdat-veb-server-s-pomoshhyu-klassa-httplistener
        
        private static void StartServer()
        {
            string prefix = "http://*:8080/";
            server = new HttpListener();
            // текущая ос не поддерживается
            if (!HttpListener.IsSupported) return;
            //добавление префикса (say/)
            //обязательно в конце должна быть косая черта
            if (string.IsNullOrEmpty(prefix))
                throw new ArgumentException("prefix");
            server.Prefixes.Add(prefix);
            //запускаем север
            server.Start();
            Console.WriteLine("Сервер запущен!");
            //сервер запущен? Тогда слушаем входящие соединения
            while (server.IsListening)
            {
                //ожидаем входящие запросы
                HttpListenerContext context = server.GetContext();
                //получаем входящий запрос
                HttpListenerRequest request = context.Request;
                //обрабатываем POST запрос
                //запрос получен методом POST (пришли данные формы)
                //if (request.HttpMethod == "POST")
                //{
                //    //показать, что пришло от клиента
                //    ShowRequestData(request);
                //    //завершаем работу сервера
                //    if (!flag) return;
                //}
                //формируем ответ сервера:
                //динамически создаём страницу
                string responseString = @"<!DOCTYPE HTML>
<html><head></head><body>
<form method=""post"" action=""say"">
<p><b>Name: </b><br>
<input type=""text"" name=""myname"" size=""40""></p>
<p><input type=""submit"" value=""send""></p>
</form></body></html>";
                //отправка данных клиенту
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

        private static void GetModel(string model)
        {
            string response = string.Empty;

            using (var connection = new SqlConnection("Data Source=TFS01;Initial Catalog=Rosbank;Integrated Security=True"))
            using (var command = new SqlCommand(model, connection) { CommandType = CommandType.StoredProcedure })
            {
                connection.Open();
                using (XmlReader reader = command.ExecuteXmlReader())
                {
                    Transform(command.ExecuteXmlReader());
                    //while (reader.Read())
                    //{
                    //    response += reader.ReadOuterXml();
                    //}
                }
                connection.Close();
            }
            //return response;
        }
        //private void ShowRequestData(HttpListenerRequest request)
        //{
        //    //есть данные от клиента?
        //    if (!request.HasEntityBody) return;
        //    //смотрим, что пришло
        //    using (Stream body = request.InputStream)
        //    {
        //        using (StreamReader reader = new StreamReader(body))
        //        {
        //            string text = reader.ReadToEnd();
        //            //оставляем только имя
        //            text = text.Remove(0, 7);
        //            //преобразуем %CC%E0%EA%F1 -> Макс
        //            text = System.Web.HttpUtility.UrlDecode(text, Encoding.UTF8);
        //            //выводим имя
        //            MessageBox.Show("Ваше имя: " + text);
        //            flag = true;
        //            //останавливаем сервер
        //            if (text == "stop")
        //            {
        //                server.Stop();
        //                this.Text = "Сервер остановлен!";
        //                flag = false;
        //            }
        //        }
        //    }
        //}
    }    
}