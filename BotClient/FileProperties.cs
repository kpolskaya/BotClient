using System;
using System.Text;


namespace BotClient
{
    struct FileProperties
    {
        public string FilePath { get; set; }

        public string FileId { get; set; }
        public string FileType { get; set; }

        public string DTmes { get; set; }
        public string ChatFirstN { get; set; }
        public long ChatId { get; set; }

        public bool IsDownloaded { get; set; }
       
        public FileProperties(string FilePath, string FileId, string FileType, string DTmes,string ChatFirstN, long ChatId)
        {
            this.FilePath = FilePath;
            this.FileId = FileId;
            this.FileType = FileType;
            this.DTmes = DTmes;
            this.ChatFirstN = ChatFirstN;
            this.ChatId = ChatId;
            this.IsDownloaded = false;
        }
        
    }
}
