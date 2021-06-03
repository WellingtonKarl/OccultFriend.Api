using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OccultFriend.Domain.Model
{
    public class Friend
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public bool EhCrianca { get; set; }
    }
}
