using System;
using System.Collections.Generic;
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
        public List <MyFile> Files { get; set; }
        
        public FileCatalog (string CatPath, List<MyFile> Files)
        {
            this.CatPath = CatPath;
            this.Files = Files; 

        }

        public FileCatalog()
        {
            this.CatPath = Directory.GetCurrentDirectory();
            this.Files = new List<MyFile>();
        }

       
        public void AddFile (MyFile f)
        {
            
          this.Files.Add(f);
        }
       
    }
}


