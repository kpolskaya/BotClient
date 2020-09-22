using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Requests;

namespace BotClient
{
    class MessageHistory
    {
        /// <summary>
        /// История сообщений
        /// </summary>
        public ObservableCollection<MessageRec> Messages { get; set; } //геттер убрать?

        /// <summary>
        /// возможно этот конструктор не нужен - проверить или переделать в структуру!
        /// </summary>
        public MessageHistory()
        {
            this.Messages = new ObservableCollection<MessageRec>();
        }

        public MessageHistory(string path)
        {
            string jsonString;
            using (StreamReader fs = new StreamReader(path))
                jsonString = fs.ReadToEnd();
            this.Messages = JsonConvert.DeserializeObject<ObservableCollection<MessageRec>>(jsonString);
        }

        public void Add(MessageRec message)
        {
            this.Messages.Add(message);
        }

        public void Save(string path)
        {
            string jsonFile = JsonConvert.SerializeObject(this.Messages);
            using (StreamWriter fs = new StreamWriter(path, false))
                fs.WriteAsync(jsonFile);

        }

       


    }
}
