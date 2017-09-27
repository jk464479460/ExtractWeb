using HtmlAgilityPack;
using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ExtractWeb
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = ConfigurationManager.AppSettings["Url"];
            var task = Task.Factory.StartNew(()=> {
                while (true)
                {
                    var logIt = true;
                    try
                    {
                        var resVal = ProcessWeb.GetHtmlElementById("gz_gsz", url);
                        var resVal2 = ProcessWeb.GetHtmlElementById("gz_gztime", url);
                        if(File.Exists(AppDomain.CurrentDomain.BaseDirectory + $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt"))
                        using (var reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt"))
                        {
                            if (reader.ReadToEnd().Contains(resVal2))
                            {
                                logIt = false;
                            }
                            else logIt = true;
                        }
                        if (logIt)
                        {
                            using (var w = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt", true))
                            {
                                w.WriteLine($"{resVal} \t{resVal2}");
                            }
                        }
                       
                    }
                    catch(Exception ex)
                    {
                        Thread.Sleep(6000 * 10);
                    }
                    
                    Thread.Sleep(6000*5);
                }
            });
           
            Console.Read();
        }
    }

    public class ProcessWeb
    {
        public static string GetHtmlElementById(string elementId,string url = "http://fund.eastmoney.com/000945.html")
        {
            var result = string.Empty;
            var htmlWeb = new HtmlWeb();
            var document = htmlWeb.Load(url);
            HtmlNode someNode = document.GetElementbyId(elementId);

            if (someNode != null)
            {
                var find = someNode.InnerText;
                result = find;
            }

            return result;
        }
    }
}
