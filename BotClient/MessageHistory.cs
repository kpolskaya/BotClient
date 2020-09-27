﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace BotClient
{
    class MessageHistory
    {
        /// <summary>
        /// История сообщений
        /// </summary>
        public ObservableCollection<MessageRec> Messages { get; set; } //геттер убрать?

        /// <summary>
        /// Пустой конструктор
        /// </summary>
        public MessageHistory()
        {
            this.Messages = new ObservableCollection<MessageRec>();
        }

        public MessageHistory(string path)
        {
            string jsonString;

            try
            {
                using (StreamReader fs = new StreamReader(path))
                jsonString = fs.ReadToEnd();
                this.Messages = JsonConvert.DeserializeObject<ObservableCollection<MessageRec>>(jsonString);

            }
            catch (Exception ex)
            {

                Debug.WriteLine("Не удалось прочитать файл истории сообщений! " + ex.Message);
                this.Messages = new ObservableCollection<MessageRec>();
            }
            
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

       


    }
}
