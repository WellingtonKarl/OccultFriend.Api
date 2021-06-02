using System;
using System.Collections.Generic;
using System.Text;

namespace OccultFriend.Domain.Model
{
    public class EmailSetting
    {
        public string HostName { get; set; }
        public string Username { get; set; }
        public string UserAdmin { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
    }
}
