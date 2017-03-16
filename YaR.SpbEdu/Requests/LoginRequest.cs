using System;
using System.Net;
using System.Text;

namespace YaR.SpbEdu.Requests
{
    class LoginRequest : BaseRequest<string>
    {
        private readonly string _login;
        private readonly string _password;

        public LoginRequest(SpbEduApi api, string login, string password) : base(api)
        {
            _login = login;
            _password = password;
        }

        public override HttpWebRequest CreateRequest(string baseDomain = null)
        {
            var request = base.CreateRequest(baseDomain);

            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
            request.KeepAlive = true;
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36";
            request.Referer = "https://petersburgedu.ru";
            request.Headers.Add("Origin", "https://petersburgedu.ru");
            request.Host = "petersburgedu.ru";
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");

            return request;
        }

        public override string RelationalUri => "/user/auth/login/";

        protected override byte[] CreateHttpContent()
        {
            string data = $"Login={Uri.EscapeDataString(_login)}&Password={Uri.EscapeDataString(_password)}&doLogin=1";

            return Encoding.UTF8.GetBytes(data);
        }
    }
}