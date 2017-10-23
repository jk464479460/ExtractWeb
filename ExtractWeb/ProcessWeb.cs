using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace ExtractWeb
{
    public class Rootobject
    {
        public Data[] Datas { get; set; }
        public int ErrCode { get; set; }
        public object ErrMsg { get; set; }
        public int TotalCount { get; set; }
        public object Expansion { get; set; }
    }

    public class Data
    {
        public string fundcode { get; set; }
        public string dwjz { get; set; }
        public string gsz { get; set; }
        public string gszzl { get; set; }
        public string gszze { get; set; }
        public string gztime { get; set; }
    }
    public class ProcessWeb
    {
        protected static string DownLoadString(string url)
        {
            WebClient wc = new WebClient();
            var st = wc.OpenRead(url);
            var sr = new StreamReader(st);
            string res = sr.ReadToEnd();
            sr.Close();
            st.Close();

            return res;
        }

        //https://fundmobapi.eastmoney.com/FundMApi/FundValuationDetail.ashx?FCODE=180031&deviceid=Wap&plat=Wap&product=EFund&version=2.0.0
        public static Rootobject GetGSZByClass(string code)
        {
            try
            {
                var url = "https://fundmobapi.eastmoney.com/FundMApi/FundValuationDetail.ashx?FCODE={code}&deviceid=Wap&plat=Wap&product=EFund&version=2.0.0";
                url = url.Replace("{code}", code);
                var result = string.Empty;
                var document = DownLoadString(url);
                var nodes = JsonConvert.DeserializeObject<Rootobject>(document);


                return nodes;
            }
            catch
            {
                return GetGSZByClass(code);
            }
        }

        public static string GetHtmlElementById(string elementId, string url = "http://fund.eastmoney.com/000945.html")
        {
            try
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
            catch
            {
                Thread.Sleep(5000);
                return GetHtmlElementById(elementId, url);
            }

        }
    }
}
