using DataStatistics;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;

namespace ExtractValueData
{
    public class DayData
    {
        public string Date { get; set; }
        public double Val { get; set; }
    }

    public class ExtractDayData
    {
        private string _code = string.Empty;
        private string _url = string.Empty;
        private string _queryUri = string.Empty;

        public IList<DayData> GetData(string sdate, string edate)
        {
            IList<DayData> daysData = new List<DayData>();
            var page = "1";
            var per = "1";

            var pageRecordsInfo = GetPagesAndRecords(GetHtmBody(page,per, sdate, edate));

            page = 1 + "";
            per = pageRecordsInfo.Item1 + "";
           
            var res = GetHtmBody(page, per, sdate, edate);

            var arr = res.Split(new string[] { "<tr>", "</tr>" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in arr)
            {
                var rowData = item.Split(new string[] { "<td>", "</td><td class='tor bold'>" }, StringSplitOptions.RemoveEmptyEntries);
                if (rowData.Length >= 2)
                    daysData.Add(new DayData { Date = rowData[0], Val = Convert.ToDouble(rowData[1]) });
            }

            return daysData;
        }

        public ExtractDayData(string url, string queryUri, string gjCode)
        {
            _code = gjCode;
            _url = url;
            _queryUri = queryUri;
        }

        private string DownLoadString(string url)
        {
            WebClient wc = new WebClient();
            var st = wc.OpenRead(url);
            var sr = new StreamReader(st);
            string res = sr.ReadToEnd();
            sr.Close();
            st.Close();

            return res;
        }

        private string GetHtmBody(string page, string per, string sdate, string edate)
        {
            var url = string.Format("{0}{1}", _url, _queryUri);
            url = url.Replace("{code}", _code).Replace("{page}", page).Replace("{per}", per).Replace("{sdate}", sdate).Replace("{edate}", edate);
            var res = DownLoadString(url);
            return res;
        }

        private Tuple<int, int> GetPagesAndRecords(string res)
        {
            var records = 0;
            var pages = 0;
            var arr = res.Split(new char[] { ',' });
            foreach (var item in arr)
            {
                if (item.Contains("records"))
                {
                    records = Convert.ToInt32(item.Split(':')[1]);
                }
                if (item.Contains("pages"))
                {
                    pages = Convert.ToInt32(item.Split(':')[1]);
                }
            }

            return new Tuple<int, int>(records, pages);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                var url = ConfigurationManager.AppSettings["Url"];
                var queryUri = ConfigurationManager.AppSettings["queryUri"];
                var code = ConfigurationManager.AppSettings["GJCode"];

                var sdate = Console.ReadLine();
                var edate = Console.ReadLine();
                var daysData = new ExtractDayData(url, queryUri, code).GetData(sdate, edate);
               
                Console.WriteLine("data is ready>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                var dataProcess = new DataProcess();
                var dataArr = daysData.Select(x => x.Val).ToArray();
                var minium = dataProcess.GetMinium(dataArr);
                var max = dataProcess.GetMax(dataArr);
                var mean = dataProcess.GetMean(dataArr);
                Console.WriteLine($"Max\tMin\tMean\n{max}\t{minium}\t{mean}");

                var maxPoints = TargetMaxMin(daysData, max);
                Console.Write("Max:\n\t");
                foreach(var item in maxPoints)
                {
                    Console.Write(item.Date+" ");
                }
                Console.Write("\n");

                var minPoints = TargetMaxMin(daysData, minium);
                Console.Write("Min:\n\t");
                foreach (var item in minPoints)
                {
                    Console.Write(item.Date + " ");
                }
                Console.Write("\n");


                Console.WriteLine("down >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            } while (Console.ReadLine() != "Q");
        }

       static IList<DayData> TargetMaxMin(IList<DayData> daysData, double findValue)
       {
            var result = new List<DayData>();
            foreach(var item in daysData)
            {
                if (item.Val == findValue) result.Add(item);
            }
            return result;
        }
    }
}
