using System;

namespace OccultFriend.Domain.DTO
{
    public class FriendDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public DateTime? Data { get; set; }
        public string ImagePath { get; set; }
        public bool IsChildreen { get; set; }
    }
}
