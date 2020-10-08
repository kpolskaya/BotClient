﻿using System;
using System.Collections.Generic;

namespace BotClient
{
    /// <summary>
    /// Структура контакта
    /// </summary>
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
