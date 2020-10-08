using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace BotClient
{
    /// <summary>
    /// Лог сообщений
    /// </summary>
    class MessageHistory
    {
        /// <summary>
        /// История сообщений
        /// </summary>
        public ObservableCollection<MessageRec> Messages { get;}

        /// <summary>
        /// Путь к файлу с историей сообщений
        /// </summary>
        string historyPath = $@"{Directory.GetCurrentDirectory()}\messagelog.json";

        public MessageHistory()
        {
            
            if (File.Exists(historyPath))
            {
                string jsonString;

                try
                {
                    using (StreamReader fs = new StreamReader(historyPath))
                        jsonString = fs.ReadToEnd();
                    this.Messages = JsonConvert.DeserializeObject<ObservableCollection<MessageRec>>(jsonString);

                }
                catch (Exception ex)
                {

                    Debug.WriteLine("Не удалось прочитать файл истории сообщений! " + ex.Message);
                    this.Messages = new ObservableCollection<MessageRec>();
                }
            }
            else
                this.Messages = new ObservableCollection<MessageRec>();
        }

        
        public void Add(MessageRec message)
        {
            this.Messages.Add(message);
        }

        
        public void Save(string path)

        {
            string jsonFile = JsonConvert.SerializeObject(this.Messages);

            try
            {
                using (StreamWriter fs = new StreamWriter(path, false))
                fs.Write(jsonFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Невозможно сохранить файл истории сообщений по указанному пути! " + ex.Message);
               
            }
         }

        public void Save()
        {
            Save(historyPath);
        }

    }
}
