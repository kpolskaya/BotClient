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
        public string PathToUserFiles { get { return $@"{Directory.GetCurrentDirectory()}"; } }  //первая часть пути
        public ObservableCollection<FileProperties> Files { get; }

        /// <summary>
        /// Путь к json-файлу каталога полученных файлов
        /// </summary>
        string catalogPath = $@"{Directory.GetCurrentDirectory()}\catalog.json";


        public FileCatalog()
        {
            if (File.Exists(catalogPath))
            {

                string json;

                try
                {
                    using (StreamReader fs = new StreamReader(catalogPath))
                        json = fs.ReadToEnd();
                    this.Files = JsonConvert.DeserializeObject<ObservableCollection<FileProperties>>(json);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Не удалось прочитать журнал полученных файлов! " + ex.Message);
                    this.Files = new ObservableCollection<FileProperties>();
                }

            }

            else
                this.Files = new ObservableCollection<FileProperties>();

        }

       
        public void Add(FileProperties f)
        {
            this.Files.Add(f);
        }
        public void Save()
        {
            string json = JsonConvert.SerializeObject(Files);

            try
            {
                using (StreamWriter fs = new StreamWriter(this.catalogPath, false))
                    fs.Write(json);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Невозможно сохранить журнал полученных файлов по указанному пути! " + ex.Message);

            }
            

        }

      
    }
}


