using System.Net;

namespace YaR.SpbEdu.Requests
{
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