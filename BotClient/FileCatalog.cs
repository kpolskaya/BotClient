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
        public string PathToUserFiles { get; } //первая часть пути
        public ObservableCollection<FileProperties> Files { get; set; }

        public FileCatalog(string CatPath, ObservableCollection<FileProperties> Files)
        {
            this.PathToUserFiles = CatPath;
            this.Files = Files;

        }

        public FileCatalog()
        {
            this.PathToUserFiles = Directory.GetCurrentDirectory();
            this.Files = new ObservableCollection<FileProperties>();
        }

        public FileCatalog(string path)
        {
            this.PathToUserFiles = Directory.GetCurrentDirectory();
            string json;
            using (StreamReader fs = new StreamReader(path))
                json = fs.ReadToEnd();
            Files = JsonConvert.DeserializeObject<ObservableCollection<FileProperties>>(json);


        }


        public void Add(FileProperties f)
        {
            this.Files.Add(f);
        }
        public void Save(string path)
        {
            string json = JsonConvert.SerializeObject(Files);
            using (StreamWriter fs = new StreamWriter(path, false))
                fs.Write(json);

        }

      
    }
}


