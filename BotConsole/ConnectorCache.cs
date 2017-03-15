using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaR.SpbEdu;

namespace BotConsole
{
    public static class ConnectorCache
    {
        public static SpbEduApi GetConnector(string login, string password)
        {
            if (Connectors.ContainsKey(login + password))
            {
                var z = Connectors[login + password];
                if (z.CreatedDate < DateTime.Now.AddDays(-_days))
                {
                    Connectors.Remove(login + password);
                    z = AddConnector(login, password);
                }
                return z?.Api;
            }
            return AddConnector(login, password)?.Api;
        }

        
        private static int _days = 4;

        private static ConnectorA AddConnector(string login, string password)
        {
            var api = new SpbEduApi(login, password);
            var connected = api.LoginAsync().Result;
            if (connected)
            {
                var ncn = new ConnectorA
                {
                    Api = api,
                    CreatedDate = DateTime.Now
                };
                Connectors[login + password] = ncn;
                return ncn;
            }
                
            return null;
        }

        private static readonly Dictionary<string, ConnectorA> Connectors = new Dictionary<string, ConnectorA>();
    
    }

    internal class ConnectorA
    {
        public SpbEduApi Api { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
