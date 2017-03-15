using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BotConsole.Properties;
using Newtonsoft.Json;
using YaR.SpbEdu;

namespace BotConsole
{
    public static class Storage
    {
        static Storage()
        {
            string data = "";
            try
            {
                data = File.ReadAllText(Settings.Default.DataFilePathName);
            }
            catch (Exception)
            {
                //ignore
            }
            if (!string.IsNullOrEmpty(data))
                Datas = JsonConvert.DeserializeObject<Dictionary<string, Data>>(data);
        }

        public static void Save()
        {
            string data = JsonConvert.SerializeObject(Datas);
            File.WriteAllText(Settings.Default.DataFilePathName, data);
        }


        public static bool Register(string login, string password, long chatid)
        {
            if (Datas.ContainsKey(login + password))
            {
                var z = Datas[login + password];
                if (null == z.ChatIds) z.ChatIds = new List<long>();
                if (!z.ChatIds.Contains(chatid)) z.ChatIds.Add(chatid);
                Save();
                return true;
            }

            var data = new Data
            {
                Login = login,
                Password = password,
                ChatIds = new List<long>{chatid}
            };
            var connector = ConnectorCache.GetConnector(login, password);
            if (connector != null)
            {
                Datas.Add(login + password, data);
                Save();
                return true;
            }

            return false;
        }


        public static readonly Dictionary<string, Data> Datas = new Dictionary<string, Data>();
    }

    public class Data
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public List<long> ChatIds { get; set; }

        public List<Student> Students;
    }
}
