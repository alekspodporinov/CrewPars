using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace UkrCrewPars
{
    class Program
    {
        public static List<string> Emails = new List<string>();
        static void Main(string[] args)
        {

            var web = new HtmlWeb();
            Console.WriteLine("Details:");

            var pageNext = "/p1";
            var page = 0;
            HtmlAttribute pageNextNode = null;

            do
            {
                
                var urlSite = "http://ukrcrewing.com.ua";
                var urlAgency = "/agency";
                var urlParams = "?on_page=100";
                var url = urlSite + urlAgency + pageNext + urlParams;

                var document = web.Load(url);
                page++;
                var table = document.DocumentNode.ChildNodes.FindFirst("table");

                pageNextNode =
                    document.DocumentNode.SelectSingleNode("/body[1]/div[3]/div[2]/div[9]/a[2]").Attributes["href"];

                if (pageNextNode != null)
                    pageNext = pageNextNode.Value.Replace(urlAgency, "");

                foreach (var tr in table.ChildNodes.Where(tr => tr.Name == "tr"))
                {
                    var link = tr.ChildNodes.FindFirst("a");

                    if (link == null) continue;

                    var linkText = "Компания:" + link.InnerText;
                    var linkUrl = urlSite + link.Attributes["href"].Value;
                    var details = GetDetails(linkUrl, linkText);
                    foreach (var detail in details)
                    {
                        Console.WriteLine(detail);
                    }
                    Console.WriteLine("");

                }
                Console.WriteLine("^--------------------------------------> Page: " + page);
                Thread.Sleep(20000);
            } while (pageNextNode != null);

            Console.WriteLine("");
            Console.WriteLine("Emails:");
            Console.WriteLine("");

            foreach (var email in Emails)
            {
                Console.WriteLine(email);
            }
            Console.WriteLine("All emails:" + Emails.Count);
            Console.WriteLine("");
            Console.WriteLine("");
        }

        static List<string> GetDetails(string url, string companyName)
        {
            List<String> companyDetails = new List<string>();
            var web = new HtmlWeb();
            var document = web.Load(url);
            var nodeLeft = document.GetElementbyId("left");
            companyDetails.Add(companyName);
            foreach (var nodeDiv in nodeLeft.ChildNodes.Where(x => x.Name == "div").Where(x => x.Attributes["class"].Value == "colmn"))
            {
                companyDetails.Add(nodeDiv.InnerText);
                var firstOrDefault = nodeDiv.ChildNodes.FirstOrDefault(x => x.Name == "a");
                if (firstOrDefault != null && firstOrDefault.InnerText.Contains("@"))
                {
                    Emails.Add(firstOrDefault.InnerText);
                }
            }


            return companyDetails;
        }
    }
}
