﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using BotConsole.Properties;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;


namespace BotConsole
{
    class Program
    {
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var bot = new Telegram.Bot.TelegramBotClient(Settings.Default.BotToken)
            {
                WebProxy = WebRequest.DefaultWebProxy
            };
            bot.WebProxy.Credentials = CredentialCache.DefaultCredentials;
            WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
            bot.OnMessage += BotOnOnMessage;
            bot.StartReceiving();


            var timer = new Timer(10 * 60 * 1000);
            ElapsedEventHandler fire = (sender, eventArgs) =>
            {
                Logger.Info("Started processing");
                Process(bot);
                Logger.Info("Finished processing");
            };
            fire(timer, null); // fire at start
            timer.Elapsed += fire;
            timer.Enabled = true;

            while (true)
            {
                char key = Console.ReadKey().KeyChar;
                switch (key)
                {
                    case 'p': fire(timer, null);
                        break;
                    case 'c':
                        Storage.Clear(DateTime.Now.Date);
                        break;

                }

            }

        }

        private static async void Process(Telegram.Bot.TelegramBotClient bot)
        {
            try
            {

                foreach (var data in Storage.Datas.Values)
                {
                    Logger.Info($"Processing User: {data.Login}");
                    var connector = ConnectorCache.GetConnector(data.Login, data.Password);
                    if (null == connector) continue;

                    var sts =  connector.StudentListAsync().Result;
                    foreach (var st in sts)
                    {
                        Logger.Info($"Quering Student ID: {st.UID}");
                        var grads = connector.GradListAsync(st.UID).Result;
                        //grads.Add(new Grad { Date = DateTime.Now, Subject = "Английский язык", Number = "10", Title = "zРабота на уроке" });
                        st.Grads = grads;
                    }

                    if (sts.Count > 0)
                    {
                        foreach (var st in sts)
                        {
                            Logger.Info($"Processing Student ID: {st.UID}");
                            int cntNewMarks = 0;
                            var msg = new StringBuilder();
                            msg.Append($"\r\n 👤{st.LastName} {st.FirstName} {st.SecondName}");

                            foreach (var grad in st.Grads)
                            {
                                var markExists = data.Students?
                                                     .FirstOrDefault(s => s.UID == st.UID)?
                                                     .Grads?
                                                     .Any(gr =>
                                                         gr.Date == grad.Date && gr.Number == grad.Number &&
                                                         gr.Subject == grad.Subject && gr.Title == grad.Title) ?? false;

                                if (!markExists)
                                {
                                    cntNewMarks++;
                                    msg.Append($"\r\n {grad.Date:dd MMM yyyy}, {grad.Subject} • <b>{grad.Number}</b> •");
                                    if (!string.IsNullOrWhiteSpace(grad.Title))
                                        msg.Append($" {grad.Title}");
                                }
                            }
                            Logger.Info($"New grades found: {cntNewMarks}");

                            if (cntNewMarks > 0)
                            {
                                var splitted = ChunksUpto(msg.ToString(), 4000).ToList();
                                foreach (var chatid in data.ChatIds)
                                foreach (var chunk in splitted)
                                    await bot.SendTextMessageAsync(chatid, chunk, parseMode: ParseMode.Html);
                            }
                        }

                        data.Students = sts;
                    }
                }
                Storage.Save();
            }
            catch (Exception e)
            {
                Logger.Error("Process!", e);
            }
        }

        static IEnumerable<string> ChunksUpto(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }

        private static void BotOnOnMessage(object sender, MessageEventArgs messageEventArgs)
        {
            try
            {
                var bot = sender as Telegram.Bot.TelegramBotClient;
                if (bot == null) return;

                var chatid = messageEventArgs.Message.Chat.Id;
                var text = messageEventArgs.Message.Text;

                var splitted = text.Split(' ');
                if (3 == splitted.Length && "/register" == splitted[0])
                {
                    Logger.Info($"Registering new user {splitted[1]} from chatid={chatid}");
                    string login = splitted[1];
                    string password = splitted[2];

                    bool registered = Storage.Register(login, password, chatid);
                    bot.SendTextMessageAsync(chatid, (registered ? "" : "Not ") +  "Registered");
                }

                if (1 == splitted.Length && "/avg" == splitted[0])
                {
                    Logger.Info($"Processing /avg from chatid={chatid}");
                    var z = Storage.Avg(chatid);
                    foreach (var avgGrade in z)
                    {
                        StringBuilder msg = new StringBuilder();
                        msg.Append($"\r\n 👤{avgGrade.Student.LastName} {avgGrade.Student.FirstName} {avgGrade.Student.SecondName}");
                        foreach (var grad in avgGrade.SubjectAverageGrades)
                        {
                            msg.Append($"\r\n {grad.Subject} • <b>{grad.Number}</b> •");
                        }
                        bot.SendTextMessageAsync(chatid, msg.ToString(), parseMode: ParseMode.Html);
                    }
                }

            }
            catch (Exception e)
            {
                Logger.Error("OnBotMessage!", e);
            }

        }
    }
}
