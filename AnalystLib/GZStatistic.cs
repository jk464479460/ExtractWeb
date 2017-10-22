using DataStatistics;
using ExtractValueData;
using System;
using System.Linq;

namespace AnalystLib
{
    public class GZStatistic
    {
        private string _code = string.Empty;
        private string _url = string.Empty;
        

        public GZStatistic(string code, string url)
        {
            _code = code;
            _url = url;
        }



        public GZHistory GetHistory(string queryUri)
        {
            var sdate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            var edate = DateTime.Now.ToString("yyyy-MM-dd");

            var daysData = new ExtractDayData(_url, queryUri, _code).GetData(sdate, edate);

            var dataProcess = new DataProcess();
            var dataArr = daysData.Select(x => x.Val).ToArray();
            var minium = dataProcess.GetMinium(dataArr);
            var max = dataProcess.GetMax(dataArr);
            var mean = dataProcess.GetMean(dataArr);
            return new GZHistory { Max = max, Mean = mean, Min = minium};
        }
    }
}
