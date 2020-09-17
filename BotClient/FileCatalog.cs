using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            this.CatPath = "";
            this.Files = new List<MyFile>();
        }

       
        public void AddFile (string FullPath, string Type, long ChatID)
        {
            
            MyFile f = new MyFile(FullPath,Type,ChatID);
            Debug.WriteLine($"{FullPath }{Type}{ChatID}");
            this.Files.Add(f);
        }
       
    }
}


