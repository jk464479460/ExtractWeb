using DataStatistics;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;

namespace ExtractValueData
{
   

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
