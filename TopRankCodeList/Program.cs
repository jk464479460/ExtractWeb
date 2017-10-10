using DataStatistics;
using ExtractValueData;
using ExtractWeb;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;

namespace TopRankCodeList
{
    class ConsoleEx
    {
        public static void Write(bool file,string txt)
        {
            if (file)
                using (var w = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + $"{DateTime.Now.ToString("yyyy-MM-dd")}.txt", true))
                {
                    w.Write(txt);
                }
            else
                Console.Write(txt);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var url = ConfigurationManager.AppSettings["Url"];
            var top = new TopRank(url);
            var markcode = ConfigurationManager.AppSettings["MarkCode"];
            var codeList = new List<TopResult>();
            if (string.IsNullOrEmpty(markcode))
            codeList = top.GetCodeList().ToList();
            else
            {
                var arr = markcode.Split(',');
                codeList.AddRange(arr.Select(x => new TopResult { Code = x, Name = "" }));
            }

            var urlHistory = "http://fund.eastmoney.com/f10/F10DataApi.aspx?type=lsjz";
            var index = codeList.Count;
            foreach(var code in codeList)
            {
                var queryUri = "&code={code}&page={page}&per={per}&sdate={sdate}&edate={edate}&rt=0."+ DateTime.Now.Second + "2038"+ DateTime.Now.Millisecond+ "498954"+DateTime.Now.Second;

                var sdate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                var edate = DateTime.Now.ToString("yyyy-MM-dd") ;
                var daysData = new ExtractDayData(urlHistory, queryUri, code.Code).GetData(sdate, edate);

                var resVal = ProcessWeb.GetHtmlElementById("gz_gsz", "http://fund.eastmoney.com/"+code.Code+".html");
                ConsoleEx.Write(true, resVal);
                ConsoleEx.Write(true," One Month\n");
                Output(daysData, code);

                daysData = new ExtractDayData(urlHistory, queryUri, code.Code).GetData(DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd"), edate);
                ConsoleEx.Write(true,"3 Month\n");
                Output(daysData, code);

                daysData = new ExtractDayData(urlHistory, queryUri, code.Code).GetData(DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd"), edate);
                ConsoleEx.Write(true, "6 Month\n");
                Output(daysData, code);

                daysData = new ExtractDayData(urlHistory, queryUri, code.Code).GetData(DateTime.Now.AddMonths(-12).ToString("yyyy-MM-dd"), edate);
                ConsoleEx.Write(true,"12 Month\n");
                Output(daysData, code);
                Thread.Sleep(1000*DateTime.Now.Second<5?10: DateTime.Now.Second);
                ConsoleEx.Write(true,"+++++++++++++++++++++++++++++++++\n");
                Console.WriteLine((index--)+"+++++++++++++++++++++++++++++++++"+code.Code);
            }
        }

        static void Output(IList<DayData> daysData, TopResult code)
        {
            var dataProcess = new DataProcess();
            var dataArr = daysData.Select(x => x.Val).ToArray();
            var minium = dataProcess.GetMinium(dataArr);
            var max = dataProcess.GetMax(dataArr);
            var mean = dataProcess.GetMean(dataArr);

            ConsoleEx.Write(true, $"{code.Code} {code.Name}\n");
            ConsoleEx.Write(true, $"Max\tMin\tMean\n{max}\t{minium}\t{mean}\n");

            var maxPoints = TargetMaxMin(daysData, max);

            ConsoleEx.Write(true, "Max:\t");
            foreach (var item in maxPoints)
            {
                ConsoleEx.Write(true, item.Date + " ");
            }
            ConsoleEx.Write(true, "\n");

            var minPoints = TargetMaxMin(daysData, minium);
            ConsoleEx.Write(true, "Min:\t");
            foreach (var item in minPoints)
            {
                ConsoleEx.Write(true, item.Date + " ");
            }
            ConsoleEx.Write(true, "\n");
            Thread.Sleep(2000);
        }

        static IList<DayData> TargetMaxMin(IList<DayData> daysData, double findValue)
        {
            var result = new List<DayData>();
            foreach (var item in daysData)
            {
                if (item.Val == findValue) result.Add(item);
            }
            return result;
        }
    }
}
