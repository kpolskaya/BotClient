using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Diagnostics;

namespace BotClient
{
    /// <summary>
    /// Каталог присланных пользователями файлов (тип которых поддерживается ботом)
    /// </summary>
    class UpploadedFilesCatalog
    {
        
        /// <summary>
        /// Первая часть пути к папкам с файлами 
        /// </summary>
        public string PathToUserFiles { get { return $@"{Directory.GetCurrentDirectory()}"; } } 
        
        
        public ObservableCollection<FileProperties> Files { get; }

        /// <summary>
        /// Путь к json-файлу в котором хранится каталог полученных файлов
        /// </summary>
         private string catalogJsonPath = $@"{Directory.GetCurrentDirectory()}\catalog.json";


        public UpploadedFilesCatalog()
        {
            if (File.Exists(catalogJsonPath))
            {

                string json;

                try
                {
                    using (StreamReader fs = new StreamReader(catalogJsonPath))
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
                using (StreamWriter fs = new StreamWriter(this.catalogJsonPath, false))
                    fs.Write(json);

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Невозможно сохранить журнал полученных файлов по указанному пути! " + ex.Message);

            }
 
        }

    }
}


