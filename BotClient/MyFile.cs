using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotClient
{
    class MyFile
    {
        public string FilePath { get; set; }

        public string FileId { get; set; }
        public string FileType { get; set; }

        public string DTmes { get; set; }
        public string ChatFirstN { get; set; }
        public long ChatId { get; set; }

        public bool IsDownloaded { get; set; }
       
        public MyFile(string FilePath, string FileId, string FileType, string DTmes,string ChatFirstN, long ChatId)
        {
            this.FilePath = FilePath;
            this.FileId = FileId;
            this.FileType = FileType;
            this.DTmes = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            this.ChatFirstN = ChatFirstN;
            this.ChatId = ChatId;
            this.IsDownloaded = false;
        }
        //public override string ToString()
        //{
        //    return base.ToString();
        //}
    }
}
