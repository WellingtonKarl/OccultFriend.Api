namespace OccultFriend.Domain.DTO
{
    public class RegisterFriendDto
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public bool? IsChildren { get; set; }
    }
}
