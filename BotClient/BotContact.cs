using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotClient
{
    struct BotContact
    {
        public string FirstName { get; set; }
        public long Id { get; set; }

        public BotContact(string FirstName, long Id)
        {
            this.FirstName = FirstName;
            this.Id = Id;
        }

    }
}
