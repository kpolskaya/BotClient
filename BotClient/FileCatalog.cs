using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotClient
{
    class FileCatalog
    {
        public string CatPath { get; set; } //первая часть пути
        public ObservableCollection<MyFile> Files { get; set; }

        public FileCatalog(string CatPath, ObservableCollection<MyFile> Files)
        {
            this.CatPath = CatPath;
            this.Files = Files;

        }

        public FileCatalog()
        {
            this.CatPath = Directory.GetCurrentDirectory();
            this.Files = new ObservableCollection<MyFile>();
        }

        public FileCatalog(string path)
        {
            this.CatPath = Directory.GetCurrentDirectory();
            string json;
            using (StreamReader fs = new StreamReader(path))
                json = fs.ReadToEnd();
            Files = JsonConvert.DeserializeObject<ObservableCollection<MyFile>>(json);


        }


        public void Add(MyFile f)
        {
            this.Files.Add(f);
        }
        public void Save(string path)
        {
            string json = JsonConvert.SerializeObject(Files);
            using (StreamWriter fs = new StreamWriter(path, false))
                fs.WriteAsync(json);

        }

      
    }
}


