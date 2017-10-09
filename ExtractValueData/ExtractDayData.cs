using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;

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

            var pageRecordsInfo = GetPagesAndRecords(GetHtmBody(page, per, sdate, edate));

            page = 1 + "";
            per = pageRecordsInfo.Item1 + "";

            var res = GetHtmBody(page, per, sdate, edate);

            var arr = res.Split(new string[] { "<tr>", "</tr>" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in arr)
            {
                var rowData = item.Split(new string[] { "<td>", "</td><td class='tor bold'>" }, StringSplitOptions.RemoveEmptyEntries);
                if (rowData.Length >= 2)
                {
                    try
                    {
                        daysData.Add(new DayData { Date = rowData[0], Val = Convert.ToDouble(rowData[1]) });
                    }
                    catch
                    {

                    }
                    
                }
                    
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
            try
            {
                WebClient wc = new WebClient();
                var st = wc.OpenRead(url);
                var sr = new StreamReader(st);
                string res = sr.ReadToEnd();
                sr.Close();
                st.Close();

                return res;
            }catch
            {
                Thread.Sleep(1000*(10+DateTime.Now.Second));
                return DownLoadString(url);
            }
            
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
}
