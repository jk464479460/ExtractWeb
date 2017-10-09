using HtmlAgilityPack;
using System;
using System.Threading;

namespace ExtractWeb
{
    public class ProcessWeb
    {
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
                Thread.Sleep(1000 * (10 + DateTime.Now.Second));
                return GetHtmlElementById(elementId, url);
            }
            
        }
    }
}
