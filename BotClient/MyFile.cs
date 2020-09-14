using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotClient
{
    class MyFile
    {
        public string FileName { get; set; }
        public long ChatID { get; set; }
        public string FileType { get; set; }

        public MyFile(string FileName, string FileType, long ChatID)
        {
            this.FileName = FileName;
            
            this.FileType = FileType;
            this.ChatID = ChatID;
        }
    }
}
