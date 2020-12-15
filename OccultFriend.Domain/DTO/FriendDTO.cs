using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OccultFriend.Domain.DTO
{
    public class FriendDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
    }
}
