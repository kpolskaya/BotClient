using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Diagnostics;

namespace BotClient
{
    class FileCatalog
    {
        public string PathToUserFiles { get; } //первая часть пути
        public ObservableCollection<MyFile> Files { get; set; }

        public FileCatalog(string CatPath, ObservableCollection<MyFile> Files)
        {
            this.PathToUserFiles = CatPath;
            this.Files = Files;

        }

        public FileCatalog()
        {
            this.PathToUserFiles = Directory.GetCurrentDirectory();
            this.Files = new ObservableCollection<MyFile>();
        }

        public FileCatalog(string path)
        {
            this.PathToUserFiles = Directory.GetCurrentDirectory();
            string json;

            try
            {
                using (StreamReader fs = new StreamReader(path))
                    json = fs.ReadToEnd();
                this.Files = JsonConvert.DeserializeObject<ObservableCollection<MyFile>>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Не удалось прочитать журнал полученных файлов! " + ex.Message);
                this.Files = new ObservableCollection<MyFile>();
            }
  
        }


        public void Add(MyFile f)
        {
            this.Files.Add(f);
        }
        public void Save(string path)
        {
            string json = JsonConvert.SerializeObject(Files);

            try
            {
                using (StreamWriter fs = new StreamWriter(path, false))
                    fs.Write(json);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Невозможно сохранить журнал полученных файлов по указанному пути! " + ex.Message);

            }
            

        }

      
    }
}


