using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace TopRankCodeList
{
    public class TopResult
    {
        public string Code { get; set; }
        public string Name { get; set; }
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
            var arr = downLoadString.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries);
            var codeArr = arr[1].Split(',');
            foreach (var item in codeArr)
            {
                var itemArr = item.Split('|');
                if(itemArr.Length>=2)
                    result.Add(new TopResult { Code = itemArr[0].Replace("\"",""),Name=itemArr[1] });
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
}
