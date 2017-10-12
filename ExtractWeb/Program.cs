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
            var markLevel = ConfigurationManager.AppSettings["Level"];
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
                            if (string.IsNullOrEmpty(markLevel) == false)
                            {
                                var levelPrice = Convert.ToDouble(markLevel);
                                var gzRealTime = Convert.ToDouble(resVal);
                                using (var w = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt", true))
                                {
                                    w.WriteLine($"{gzRealTime - levelPrice}");
                                }
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

   
}
