using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace YaR.SpbEdu
{
    class StudentListRequest : BaseRequest<List<Student>>
    {
        public StudentListRequest(SpbEduApi api) : base(api)
        {
        }

        public override HttpWebRequest CreateRequest(string baseDomain = null)
        {
            var request = base.CreateRequest(baseDomain);
            return request;
        }

        public override string RelationalUri => "/dnevnik/cabinet";

        protected override RequestResponse<List<Student>> DeserializeMessage(string html)
        {
            var res = new List<Student>();
            Match m = Regex.Match(html, _regex);
            while (m.Success)
            {
                res.Add(new Student
                {
                    UID = long.Parse(m.Groups["studid"].Value),
                    FirstName = m.Groups["firstname"].Value,
                    SecondName = m.Groups["secondname"].Value,
                    LastName = m.Groups["lastname"].Value,

                    Institution = new Institution
                    {
                        UID = long.Parse(m.Groups["instid"].Value),
                        Name = m.Groups["instname"].Value
                    },

                    Class = m.Groups["class"].Value,
                    LastUpdateDate = DateTime.Parse(m.Groups["upddate"].Value)
                });

                m = m.NextMatch();
            }

            return new RequestResponse<List<Student>>
            {
                Ok = true,
                Result = res
            };
        }

        private const string _regex = @"(?snx-) 
            <div \s* class=""heading""> \s*
            <span \s* class=""ou-label""> \s*
            <b>(?<class>.*?)</b> \s*
            <a \s* href=""/institution/content/details/UID/(?<instid>\d+)"">\s*(?<instname>.*?)\s*</a> \s*
            </span> \s*
            <div \s* class=""fio""> \s*
            <a \s* href=""/dnevnik/profile/index/student/(?<studid>\d+)"">\s*(?<lastname>.*?)<br \s* /><br \s* />(?<firstname>.*?)<br \s* />(?<secondname>.*?) \s*</a> \s*
            </div> \s*
            <p \s* class=""last-update"">Последнее \s* обновление: (?<upddate>.*?) <a \s* href=""#""></a></p>	";
    }


    public class Student
    {
        public long UID { get; set; }

        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }

        public Institution Institution { get; set; }
        public string Class { get; set; }

        public DateTime LastUpdateDate { get; set; }

        public List<Grad> Grads { get; set; }
    }

    public class Grad
    {
        public DateTime Date { get; set; }
        public string Subject { get; set; }
        public string Number { get; set; }
        public string Title { get; set; }
    }

    public class Institution
    {
        public long UID { get; set; }
        public string Name { get; set; }
    }
}