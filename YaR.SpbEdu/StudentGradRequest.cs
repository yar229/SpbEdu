using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace YaR.SpbEdu
{
    class StudentGradRequest : BaseRequest<List<Grad>>
    {
        private readonly long _studentId;

        public StudentGradRequest(SpbEduApi api, long studentId) : base(api)
        {
            _studentId = studentId;
        }

        public override HttpWebRequest CreateRequest(string baseDomain = null)
        {
            var request = base.CreateRequest(baseDomain);
            return request;
        }

        public override string RelationalUri => $"/dnevnik/lessons/index/student/{_studentId}";

        protected override RequestResponse<List<Grad>> DeserializeMessage(string htmlstring)
        {
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(htmlstring);

            var subjects = html.DocumentNode
                .SelectNodes("/html[1]/body[1]/div[1]/div[4]/div[2]/div[3]/div[2]/div[1]//div/a")
                .Select(n => new
                {
                    Name = n.InnerText,
                    Href = n.Attributes.First(a => a.Name == "href")?.Value
                })
                .ToList();


            var tablenodes = html
                .GetElementbyId("marks")
                .ChildNodes;

            var months = tablenodes
                .FindFirst("thead")
                .ChildNodes
                .FindFirst("tr")
                .ChildNodes
                .Where(n => n.Name == "th")
                .Select(n => new
                {
                    Month = n.InnerText,
                    Days = int.Parse(n.Attributes.First(a => a.Name == "colspan").Value)
                })
                .ToList();

            var monthPeriod = months
                .Select((n, index) => new
                {
                    Month = n.Month,
                    From = months.Where((w, ind) => ind < index).Sum(z => z.Days),
                    Days = n.Days
                })
                .Select((n, index) => new
                {
                    Month = n.Month,
                    From = n.From,
                    To = n.From + n.Days - 1
                });


            var days = tablenodes
                .FindFirst("thead")
                .ChildNodes
                .Where(n => n.Name == "tr")
                .ElementAt(1)
                .ChildNodes
                .Where(n => n.Name == "th")
                .Select(n => n.ChildNodes.First(cn => cn.Name == "i").InnerText);

            var dates = monthPeriod
                .SelectMany(m => days
                    .Where((d, index) => m.From <= index && m.To >= index)
                    .Select(z => $"{z} {m.Month}")
                )
                .Select(DateTime.Parse)
                .ToList();


            try
            {
                var marks = tablenodes
                    .FindFirst("tbody").ChildNodes
                    .Where(node => node.Name == "tr")
                    .SelectMany(node => node.ChildNodes.Where(cn => cn.Name == "td"))
                    .SelectMany((node, index) =>
                    {
                        var cns = node.ChildNodes
                            .Where(icn => icn.Name == "span") // оценка
                            .Union(node.ChildNodes // присутствовал
                                    .Where(icn => icn.Name == "i")
                                    .Select(cn => cn.ChildNodes.FirstOrDefault(icn => icn.Name == "span")))
                            .Union(node.ChildNodes  // отсутствовал
                                    .Where(icn => icn.Name == "i" && icn.Attributes.Any(a => a.Name == "class" && a.Value == "skip")))
                            .Select(gr => gr == null
                                            ? null
                                            : new Grad
                                            {
                                                Number = gr.InnerText,
                                                Title = gr.Attributes.FirstOrDefault(a => a.Name == "title")?.Value ?? String.Empty,
                                                Date = dates[index % dates.Count],
                                                Subject = subjects[index / dates.Count].Name
                                            });
                        
                        return cns;
                    })
                    .Where(n => n != null)
                    .ToList();

            return new RequestResponse<List<Grad>>
            {
                Ok = true,
                Result = marks
            };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


    }


}