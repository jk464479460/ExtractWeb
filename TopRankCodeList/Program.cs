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
        public static void Write(bool file, string txt)
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

        static IList<TopResult> GetCodeList()
        {
            var url = ConfigurationManager.AppSettings["Url"];
            var top = new TopRank1(url);
            return top.GetCodeList().ToList();
        }

        static IList<TopResult> GetCodeList2()
        {
            return new TopRank2(ConfigurationManager.AppSettings["CodeSource2"]).GetCodeList().ToList();
        }

        static void Main(string[] args)
        {

            var markcode = ConfigurationManager.AppSettings["MarkCode"];
            var codeList = new List<TopResult>();
            if (string.IsNullOrEmpty(markcode))
                codeList = GetCodeList2().ToList();//GetCodeList().ToList()
            else
            {
                var arr = markcode.Split(',');
                codeList.AddRange(arr.Select(x => new TopResult { Code = x, Name = "" }));
            }
            Analyst(codeList);
            FilterAnalyst();
            LogMeanVal(codeList, 1);
        }

        static void LogMeanVal(List<TopResult> codeList, int monthCnt)
        {
            var urlHistory = "http://fund.eastmoney.com/f10/F10DataApi.aspx?type=lsjz";
            var index = codeList.Count;
            foreach (var code in codeList)
            {
                var queryUri = "&code={code}&page={page}&per={per}&sdate={sdate}&edate={edate}&rt=0." + DateTime.Now.Second + "2238" + DateTime.Now.Millisecond + "490954" + DateTime.Now.Second;

                var sdate = DateTime.Now.AddMonths(-1 * monthCnt).ToString("yyyy-MM-dd");
                var edate = DateTime.Now.ToString("yyyy-MM-dd");
                var daysData = new ExtractDayData(urlHistory, queryUri, code.Code).GetData(sdate, edate);

                var dataProcess = new DataProcess();
                var dataArr = daysData.Select(x => x.Val).ToArray();
                var minium = dataProcess.GetMinium(dataArr);
                var max = dataProcess.GetMax(dataArr);
                var mean = dataProcess.GetMean(dataArr);

                using (var w = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + $"Mean_{DateTime.Now.ToString("yyyy-MM-dd")}.txt", true))
                {
                    w.WriteLine($"{code.Code} \t{mean}");
                }
            }
        }

        static void Analyst(List<TopResult> codeList)
        {
            var urlHistory = "http://fund.eastmoney.com/f10/F10DataApi.aspx?type=lsjz";
            var index = codeList.Count;
            foreach (var code in codeList)
            {
                var resVal = ProcessWeb.GetHtmlElementById("gz_gsz", "http://fund.eastmoney.com/" + code.Code + ".html");
                ConsoleEx.Write(true, resVal);

                var queryUri = "&code={code}&page={page}&per={per}&sdate={sdate}&edate={edate}&rt=0." + DateTime.Now.Second + "2038" + DateTime.Now.Millisecond + "498954" + DateTime.Now.Second;

                var sdate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                var edate = DateTime.Now.ToString("yyyy-MM-dd");

                var daysData = new ExtractDayData(urlHistory, queryUri, code.Code).GetData(sdate, edate);
                ConsoleEx.Write(true, " One Month\n");
                Output(daysData, code, resVal, 1);

                daysData = new ExtractDayData(urlHistory, queryUri, code.Code).GetData(DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd"), edate);
                ConsoleEx.Write(true, "3 Month\n");
                Output(daysData, code, resVal, 3);

                daysData = new ExtractDayData(urlHistory, queryUri, code.Code).GetData(DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd"), edate);
                ConsoleEx.Write(true, "6 Month\n");
                Output(daysData, code, resVal, 6);

                //daysData = new ExtractDayData(urlHistory, queryUri, code.Code).GetData(DateTime.Now.AddMonths(-12).ToString("yyyy-MM-dd"), edate);
                //ConsoleEx.Write(true,"12 Month\n");
                //Output(daysData, code, resVal);
                //Thread.Sleep(1000*DateTime.Now.Second<5?10: DateTime.Now.Second);
                ConsoleEx.Write(true, "+++++++++++++++++++++++++++++++++\n");
                Console.WriteLine((index--) + "+++++++++++++++++++++++++++++++++" + code.Code);
            }
        }

        static void Output(IList<DayData> daysData, TopResult code, string curPrice, int flag)
        {
            var dataProcess = new DataProcess();
            var dataArr = daysData.Select(x => x.Val).ToArray();
            var minium = dataProcess.GetMinium(dataArr);
            var max = dataProcess.GetMax(dataArr);
            var mean = dataProcess.GetMean(dataArr);

            ConsoleEx.Write(true, $"{code.Code} {code.Name}\n");
            ConsoleEx.Write(true, $"\tMax\tMin\tMean\n\t{max}\t{minium}\t{mean}\n");
            try
            {
                var cur = Convert.ToDouble(curPrice);
                var meanPrice = Convert.ToDouble(mean);
                if (cur <= meanPrice) ConsoleEx.Write(true, "\tTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT ");
            }
            catch
            {

            }

            var maxPoints = TargetMaxMin(daysData, max);

            ConsoleEx.Write(true, "\tMax:\t");
            foreach (var item in maxPoints)
            {
                ConsoleEx.Write(true, "\t" + item.Date + " ");
            }
            ConsoleEx.Write(true, "\n");

            var minPoints = TargetMaxMin(daysData, minium);
            ConsoleEx.Write(true, "\tMin:\t");
            foreach (var item in minPoints)
            {
                ConsoleEx.Write(true, "\t" + item.Date + " ");
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

        static void FilterAnalyst()
        {
            var text = string.Empty; 
            using (var read = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + $"{DateTime.Now.ToString("yyyy-MM-dd")}.txt", true))
            {
                text = read.ReadToEnd();
            }
            var arr = text.Split(new string[] { "+++++++++++++++++++++++++++++++++" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in arr)
            {
                if (item.Contains("TTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT"))
                {
                    using (var w = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + $"Filter_{DateTime.Now.ToString("yyyy-MM-dd")}.txt", true))
                    {
                        w.WriteLine(item);
                    }
                }
            }
        }
    }
}
