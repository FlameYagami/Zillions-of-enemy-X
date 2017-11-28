using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Wrapper.Constant;

namespace WebCrawler
{
    public class AnalyseUtils
    {
        public static int AnalysePageCount(HtmlNodeCollection nodes)
        {
            var pageInnerText = nodes
                .First(x => x.Attributes["class"] != null && x.Attributes["class"].Value.Equals("countRule"))
                .InnerText;
            var pageCountText = pageInnerText.Substring(
                pageInnerText.IndexOf("全", StringComparison.Ordinal) + 1,
                pageInnerText.LastIndexOf("件", StringComparison.Ordinal) - pageInnerText.IndexOf("全", StringComparison.Ordinal) - 1);
            return int.Parse(pageCountText);
        }

        public static int AnalyseCost(HtmlNodeCollection nodes)
        {
            int cost;
            return int.TryParse(nodes[2].InnerText, out cost) ? cost : -1;
        }

        public static int AnalysePower(HtmlNodeCollection nodes)
        {
            int power;
            return int.TryParse(nodes[3].InnerText, out power) ? power : -1;
        }

        public static string AnalyseJName(HtmlNodeCollection nodes)
        {
            return nodes[1].InnerText;
        }

        public static string AnalyseNumber(HtmlNodeCollection nodes)
        {
            return nodes[0].InnerText;
        }

        public static string AnalyseIllust(HtmlNodeCollection nodes)
        {
            return string.IsNullOrWhiteSpace(nodes[3].InnerText.Trim()) ? "" : nodes[3].InnerText.Trim().Replace("&nbsp;", "");
        }

        public static string AnalyseSign(HtmlNodeCollection nodes)
        {
            var firstOrDefault = nodes[3].SelectNodes(@"//img")
                .FirstOrDefault(
                    x =>
                        x.Attributes["src"].Value.Contains("ignition") ||
                        x.Attributes["src"].Value.Contains("evolseed"));
            if (firstOrDefault == null) return StringConst.Hyphen;
            var srcValue = firstOrDefault.Attributes["src"].Value;
            return srcValue.Contains("ignition") ? StringConst.SignIg : StringConst.AbilityEvo;
        }

        public static string AnalyseCamp(HtmlDocument frame)
        {
            var campText = frame.DocumentNode.SelectNodes(@"//img")
                .First(x => null != x.Attributes["src"] && x.Attributes["src"].Value.Contains("w_"))
                .Attributes["src"].Value;
            campText = campText.Substring(campText.LastIndexOf("/", StringComparison.Ordinal) + 1);
            campText = campText.Substring(campText.IndexOf("_", StringComparison.Ordinal) + 1,
                campText.LastIndexOf("_", StringComparison.Ordinal) - campText.IndexOf("_", StringComparison.Ordinal) - 1).ToUpper();
            return  CampDic[campText];
        }

        public static string AnalyseRare(HtmlDocument frame)
        {
            var rareText = frame.DocumentNode.SelectNodes(@"//img")
                .First(y => null != y.Attributes["class"] && y.Attributes["class"].Value.Equals("gauge"))
                .Attributes["src"].Value;
            rareText = rareText.Substring(rareText.LastIndexOf("/", StringComparison.Ordinal) + 1);
            return rareText.Substring(rareText.IndexOf("_", StringComparison.Ordinal) + 1,
                rareText.LastIndexOf("_", StringComparison.Ordinal) - rareText.IndexOf("_", StringComparison.Ordinal) - 1).ToUpper();
        }

        public static string AnalyseType(HtmlDocument frame)
        {
            var typeText = frame.DocumentNode.SelectNodes(@"//div")
                .First(x => null != x.Attributes["class"] && x.Attributes["class"].Value.Equals("mainArea"))
                .SelectSingleNode(@"//thead")
                .SelectNodes(@"//tr")[1]
                .SelectNodes(@"//td")[0]
                .InnerText;
            return TypeDic[typeText];
        }

        public static string AnalyseRace(HtmlNodeCollection nodes)
        {
            return string.IsNullOrWhiteSpace(nodes[1].InnerText) ? "" : nodes[1].InnerText;
        }

        public static string AnalyseLines(HtmlDocument frame)
        {
            var exTbodyInnerHtml = frame.DocumentNode.SelectNodes(@"//div")
                .First(y => null != y.Attributes["class"] && y.Attributes["class"].Value.Equals("mainArea"))
                .SelectSingleNode(@"//tbody")
                .InnerHtml;
            var exDoc = new HtmlDocument();
            exDoc.LoadHtml(exTbodyInnerHtml);
            return exDoc.DocumentNode.SelectNodes($"//tr")[1].InnerText.Trim().Replace("&nbsp;", "");
        }

        public static Dictionary<string, string> CampDic = new Dictionary<string, string>
        {
            {"RED","红"},
            {"BLUE","蓝"},
            {"WHITE","白"},
            {"BLACK","黑"},
            {"GREEN","绿"},
            {"MU","无"}
        };

        public static Dictionary<string, string> TypeDic = new Dictionary<string, string>
        {
            {"ゼクス","Z/X"},
            {"イベント","事件"},
            {"プレイヤー","玩家"},
            {"ゼクス エクストラ","Z/X EX"},
        };
    }
}
