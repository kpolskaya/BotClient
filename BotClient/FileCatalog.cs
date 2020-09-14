using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotClient
{
    class FileCatalog
    {
        public string CatPath { get; set; }
        public List <MyFile> Files { get; set; }
        
        public FileCatalog (string CatPath, List<MyFile> Files)
        {
            this.CatPath = CatPath;
            this.Files = Files; 

        }

        public FileCatalog ()
        {
            this.CatPath = "";
            this.Files = new List<MyFile>();
        }

        public void AddFile (string FileName,string FileType, long ChatID)
        {
            
            MyFile f = new MyFile(FileName, FileType, ChatID);
            this.Files.Add(f);
        }
    }
}


