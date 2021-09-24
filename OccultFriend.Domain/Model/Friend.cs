namespace OccultFriend.Domain.Model
{
    public class Friend
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public bool IsChildreen { get; set; }
    }
}
