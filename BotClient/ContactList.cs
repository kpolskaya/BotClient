using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BotClient
{
    struct ContactList //структура или класс?
    {
        public ObservableCollection<BotContact> Contacts { get; set; } // геттер нужно убрать

        public ContactList(string path)
        {
            string jsonString;
            using (StreamReader fs = new StreamReader(path))
                jsonString = fs.ReadToEnd();
            this.Contacts = JsonConvert.DeserializeObject<ObservableCollection<BotContact>>(jsonString);
        }

        public void Add(BotContact contact)
        {
            this.Contacts.Add(contact);
        }

        public bool Exists(BotContact contact)
        {
            return (this.Contacts.Contains(contact));
        }

        public void Save(string path)
        {
            string jsonFile = JsonConvert.SerializeObject(this.Contacts);
            using (StreamWriter fs = new StreamWriter(path, false))
                fs.WriteAsync(jsonFile);

        }
    }
}
