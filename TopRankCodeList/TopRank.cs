using Newtonsoft.Json;
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

    public interface ITopRank
    {
        IList<TopResult> GetCodeList();
    }

    public abstract class TopRank
    {
        protected string DownLoadString(string url)
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

    public class TopRank1: TopRank,ITopRank
    {
        private readonly string _url;

        public TopRank1(string url)
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

        
    }

    public class TopRank2 : TopRank, ITopRank
    {
        private readonly string _url;

        public TopRank2(string url)
        {
            _url = url;
        }

        public IList<TopResult> GetCodeList()
        {
            var result = new List<TopResult>();
            var downLoadString = DownLoadString(_url);
            var arr = downLoadString.Split(new string[] { "datas:", ",allRecords" }, StringSplitOptions.RemoveEmptyEntries);
            var codeArr = JsonConvert.DeserializeObject<IList<string>>(arr[1]);
            foreach (var item in codeArr)
            {
                var itemArr = item.Split(',');
                if (itemArr.Length >= 2)
                    result.Add(new TopResult { Code = itemArr[0], Name = itemArr[1] });
            }
            return result;
        }

    }
}
