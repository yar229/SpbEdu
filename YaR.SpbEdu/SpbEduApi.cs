using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using YaR.SpbEdu.Requests;

namespace YaR.SpbEdu
{
    public class SpbEduApi
    {
        private readonly string _login;
        private readonly string _password;

        public IWebProxy Proxy;

        public CookieContainer Cookies => _cookies ?? (_cookies = new CookieContainer());
        private CookieContainer _cookies;

        public readonly CancellationTokenSource CancelToken = new CancellationTokenSource();

        

        public SpbEduApi(string login, string password)
        {
            _login = login;
            _password = password;

            Proxy = WebRequest.DefaultWebProxy;
            //var credentials = System.Net.CredentialCache.DefaultCredentials; ;
            Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

            WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
            //Proxy = WebRequest.DefaultWebProxy;


            //Cookies.Add(new Uri("https://petersburgedu.ru"), new Cookie("eSi_state", "on"));
            //Cookies.Add(new Uri("https://petersburgedu.ru"), new Cookie("__utma", "263703443.206646750.1489499706.1489499706.1489499706.1"));
            //Cookies.Add(new Uri("https://petersburgedu.ru"), new Cookie("__utmc", "263703443"));
            //Cookies.Add(new Uri("https://petersburgedu.ru"), new Cookie("__utmz", "263703443.1489499706.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)"));
            //Cookies.Add(new Uri("https://petersburgedu.ru"), new Cookie("_ym_isad", "2"));
            //Cookies.Add(new Uri("https://petersburgedu.ru"), new Cookie("_ym_uid", "1489499705341173477"));
            //Cookies.Add(new Uri("https://petersburgedu.ru"), new Cookie("eSi_state", "on"));




        }



        public async Task<bool> LoginAsync()
        {
            if (string.IsNullOrEmpty(_login))
                throw new ArgumentException("Login is null or empty.");

            if (string.IsNullOrEmpty(_password))
                throw new ArgumentException("Password is null or empty.");

            await new InitRequest(this).MakeRequestAsync();

            await new LoginRequest(this, _login, _password)
                .MakeRequestAsync();

            return true;
        }


        public async Task<List<Student>> StudentListAsync()
        {
            var res = await new StudentListRequest(this)
                .MakeRequestAsync();

            return res;
        }

        public async Task<List<Grad>> GradListAsync(long studentId)
        {
            var res = await new StudentGradRequest(this, studentId)
                .MakeRequestAsync();

            return res;
        }

    }
}
