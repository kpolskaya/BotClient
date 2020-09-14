using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot;


namespace BotClient
{
    class TgMesClient
    {
        string token = File.ReadAllText(@"token.txt");
        private MainWindow w;

        private TelegramBotClient bot;
        public System.Collections.ObjectModel.ObservableCollection<MessageRec> BotMessageLog { get; set; }

        private void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            Console.WriteLine("---");
            Debug.WriteLine("+++---");

            string text = $"{DateTime.Now.ToLongTimeString()} | {e.Message.Chat.FirstName} | {e.Message.Chat.Id}: {e.Message.Text} | {e.Message.Type}";

            Debug.WriteLine($"{text} TypeMessage: {e.Message.Type}");

            if (e.Message.Text == null) return;

            var messageText = e.Message.Text;

            w.Dispatcher.Invoke(() =>
            {
                BotMessageLog.Add(
                new MessageRec(
                    DateTime.Now.ToLongTimeString(), e.Message.Chat.Id, e.Message.Chat.FirstName, e.Message.Text, e.Message.Type.ToString()));
            });
        }
        public TgMesClient(MainWindow W, string PathToken = @"C:\Users\kpols\Desktop\ДЗ\БОТ\token.txt")
        {
            this.BotMessageLog = new System.Collections.ObjectModel.ObservableCollection<MessageRec>();
            this.w = W;

            bot = new TelegramBotClient(File.ReadAllText(PathToken));

            bot.OnMessage += MessageListener;

            bot.StartReceiving();
        }
    }
}
