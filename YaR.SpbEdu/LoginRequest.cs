using System;
using System.Net;
using System.Text;

namespace YaR.SpbEdu
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
            request.KeepAlive = true;  //request.Headers.Add("Connection", "keep-alive");
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8"; //request.Headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36"; //request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");
            request.Referer = "https://petersburgedu.ru";
            request.Headers.Add("Origin", "https://petersburgedu.ru");
            request.Host = "petersburgedu.ru"; //request.Headers.Add("Host", "petersburgedu.ru");
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");


            //request.Headers.Add("", "");
            //request.Headers.Add("", "");
            //request.Headers.Add("", "");
            //request.Headers.Add("", "");
            //request.Headers.Add("", "");
            //request.Headers.Add("", "");
            //request.Headers.Add("", "");

            //            Accept: application / json, text / javascript, */*; q=0.01
            //:
            //Accept-Language:ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4
            //Connection:keep-alive
            //Content-Length:50
            //Content-Type:application/x-www-form-urlencoded; charset=UTF-8
            //Cookie:PHPSESSID=b0vkhnv2pe5jvpraes025cq3s4; _ym_uid=1489476172269978420; _ym_isad=2; eSi_state=on; __utmt=1; __utma=263703443.1580002029.1489476172.1489484912.1489491424.3; __utmb=263703443.1.10.1489491424; __utmc=263703443; __utmz=263703443.1489476172.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none); _ym_visorc_25567901=w
            //Host:petersburgedu.ru
            //Origin:https://petersburgedu.ru
            //Referer:https://petersburgedu.ru/
            //User-Agent:Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36
            //X-Requested-With:XMLHttpRequest


            //request.Headers["Cookie"] = "PHPSESSID=0bnbac5vc4st1v3p5nvuqlamu4";

            return request;
        }

        public override string RelationalUri => "/user/auth/login/";

        protected override byte[] CreateHttpContent()
        {
            string data = $"Login={Uri.EscapeDataString(_login)}&Password={Uri.EscapeDataString(_password)}&doLogin=1";

            return Encoding.UTF8.GetBytes(data);


            //ASCIIEncoding encoding = new ASCIIEncoding();
            //byte[] postDataBytes = encoding.GetBytes(data);
            //return postDataBytes;
        }
    }

    class InitRequest : BaseRequest<string>
    {
        public InitRequest(SpbEduApi api) : base(api)
        {
        }

        public override HttpWebRequest CreateRequest(string baseDomain = null)
        {
            var request = base.CreateRequest(baseDomain);
            return request;
        }

        public override string RelationalUri => "/user/auth/login/get-form/1/";


    }
}