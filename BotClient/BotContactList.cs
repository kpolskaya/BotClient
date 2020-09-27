using Newtonsoft.Json;
using System.Diagnostics;
using System;
using System.Collections.ObjectModel;
using System.IO;


namespace BotClient
{
    class BotContactList 
    {
        public ObservableCollection<BotContact> Contacts { get; set; } //сеттер можно убрать?

        public BotContactList()
        {
            this.Contacts = new ObservableCollection<BotContact>();
        }
        public BotContactList(string path)
        {
            string jsonString;
            try
            {
                using (StreamReader fs = new StreamReader(path))
                    jsonString = fs.ReadToEnd();
                this.Contacts = JsonConvert.DeserializeObject<ObservableCollection<BotContact>>(jsonString);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Не удалось прочитать файл контактов! " + ex.Message);
                this.Contacts = new ObservableCollection<BotContact>();
            }

        }

        public void Add(BotContact contact)
        {
            this.Contacts.Add(contact);
        }

        public bool Contains(BotContact contact)
        {
            //if (this.Contacts == null || !this.Contacts.Contains(contact))
            //    return false;
            //return true;
            return this.Contacts.Contains(contact);
        }

        public void Save(string path)
        {
            string jsonFile = JsonConvert.SerializeObject(this.Contacts);
            try
            { 
                using (StreamWriter fs = new StreamWriter(path, false))
                fs.Write(jsonFile);

            }
            catch (Exception ex)
            {

                Debug.WriteLine("Невозможно сохранить файл контактов по указанному пути! " + ex.Message);
            }
           

        }
    }
}
