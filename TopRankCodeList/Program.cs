using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;

namespace TopRankCodeList
{
    public class TopResult
    {
        public string Code { get; set; }
    }
    public class TopRank
    {
        private readonly string _url;

        public TopRank(string url)
        {
            _url = url;
        }

        public IList<TopResult> GetCodeList()
        {
            var result = new List<TopResult>();
            var downLoadString = DownLoadString(_url);
            var arr = downLoadString.Split(new string[] { "[", "]"},StringSplitOptions.RemoveEmptyEntries);
            var codeArr = arr[1].Split(',');
            foreach(var item in codeArr)
            {
                var itemArr = item.Split('|'); ;
                result.Add(new TopResult { Code = itemArr[0]});
            }
            return result;
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
    }

    class Program
    {
        static void Main(string[] args)
        {
            var url = ConfigurationManager.AppSettings["Url"];
            var top = new TopRank(url);
            var codeList = top.GetCodeList();
        }
    }
}
