using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;

namespace ExtractValueData
{
    class DayData
    {
        public string Date { get; set; }
        public string Val { get; set; }
    }
    class Program
    {
        static IList<DayData> daysData = new List<DayData>();

        static string queryUri="&code={code}&page={page}&per={per}&sdate={sdate}&edate={edate}&rt=0.24914203854989547";
        static void Main(string[] args)
        {
            var url = ConfigurationManager.AppSettings["Url"];
            url = string.Format("{0}{1}",url,queryUri);

            var code = "000977";
            var page = "1";
            var per = "1";
            var sdate = "2017-08-01";
            var edate = "2017-09-28";

            url = url.Replace("{code}", code).Replace("{page}",page).Replace("{per}", per).Replace("{sdate}",sdate).Replace("{edate}", edate);

            WebClient wc = new WebClient();
            var st = wc.OpenRead(url);
            var sr = new StreamReader(st);
            string res = sr.ReadToEnd();
            sr.Close();
            st.Close();

            var records = 0;
            var pages = 0;
            var arr = res.Split(new char[] { ','});
            foreach(var item in arr)
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

            //for(var i = 1; i <= pages; i++)
            {
                url = ConfigurationManager.AppSettings["Url"]; ;
                url = string.Format("{0}{1}", url, queryUri);

                code = "000977";
                page = 1+"";
                per = records+"";
              

                url = url.Replace("{code}", code).Replace("{page}", page).Replace("{per}", per).Replace("{sdate}", sdate).Replace("{edate}", edate);
                wc = new WebClient();
                st = wc.OpenRead(url);
                sr = new StreamReader(st);
                res = sr.ReadToEnd();
                sr.Close();
                st.Close();

                arr = res.Split(new string[] { "<tr>","</tr>"},StringSplitOptions.RemoveEmptyEntries);

                foreach(var item in arr)
                {
                    var rowData = item.Split(new string[] { "<td>", "</td><td class='tor bold'>" }, StringSplitOptions.RemoveEmptyEntries);
                    if(rowData.Length>=2)
                    daysData.Add(new DayData { Date = rowData[0], Val = rowData[1] });
                }
            }

            foreach(var item in daysData)
            {
                Console.WriteLine(item.Date+"\t"+item.Val);
            }
            Console.Read();
        }

       
    }
}
