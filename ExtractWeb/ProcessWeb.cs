using HtmlAgilityPack;

namespace ExtractWeb
{
    public class ProcessWeb
    {
        public static string GetHtmlElementById(string elementId, string url = "http://fund.eastmoney.com/000945.html")
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
    }
}
