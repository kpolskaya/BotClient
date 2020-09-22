using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotClient
{
    struct MessageRec
    {
        public string Time { get; set; }
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public MessageRec(string Time, long Id, string FirstName, string Text, string Type)
        {
            this.Time = Time;
            this.Id = Id;
            this.FirstName = FirstName;
            this.Text = Text;
            this.Type = Type;

        }
        public MessageRec(string Time, long Id, string FirstName, string Type)
        {
            this.Time = Time;
            this.Id = Id;
            this.FirstName = FirstName;
            this.Type = Type;
            this.Text = "";

        }
    }
}
