using AnalystLib;
using System;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ExtractWeb
{
    class Program
    {
        static GZHistory Prepare(string code)
        {
            var urlHistory = "http://fund.eastmoney.com/f10/F10DataApi.aspx?type=lsjz";
            var queryUri = "&code={code}&page={page}&per={per}&sdate={sdate}&edate={edate}&rt=0." + DateTime.Now.Second + "2038" + DateTime.Now.Millisecond + "498954" + DateTime.Now.Second;
            var his = new GZStatistic(code, urlHistory);
            try
            {
                var result = his.GetHistory(queryUri);
                using (var w = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt", true))
                {
                    w.WriteLine($"max: {result.Max} \tmin: {result.Min} \tmean:{result.Mean}");
                }
                return result;
            }
            catch
            {
                return null;
            }
           
        }
        static void Main(string[] args)
        {
            var url = ConfigurationManager.AppSettings["Url"];
            var markLevel = ConfigurationManager.AppSettings["Level"];
            var code = ConfigurationManager.AppSettings["Code"];
            
            var gzHis = Prepare(code);

            var task = Task.Factory.StartNew(()=> {
                while (true)
                {
                    var logIt = true;
                    try
                    {
                        var gsjz = ProcessWeb.GetGSZByClass(code);
                        var resVal = gsjz.Datas[0].gsz;//ProcessWeb.GetHtmlElementById("gz_gsz", url);
                        var resVal2 = gsjz.Datas[0].gztime;//ProcessWeb.GetHtmlElementById("gz_gztime", url);
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
                            var gzRealTime = Convert.ToDouble(resVal);
                            if (string.IsNullOrEmpty(markLevel) == false)
                            {
                                var levelPrice = Convert.ToDouble(markLevel);
                               
                                using (var w = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt", true))
                                {
                                    w.WriteLine($"level: {gzRealTime - levelPrice}");
                                }
                            }

                            if (gzHis != null)
                            {
                                using (var w = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt", true))
                                {
                                    w.WriteLine($"mean: {gzRealTime - gzHis.Mean}");
                                    w.WriteLine($"min: {gzRealTime - gzHis.Min}");
                                    w.WriteLine($"max: {gzRealTime - gzHis.Max}");
                                }
                            }
                        }
                       
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("There are some error");
                    }
                    
                    Thread.Sleep(6000);
                }
            });
           
            Console.Read();
        }
    }

   
}
