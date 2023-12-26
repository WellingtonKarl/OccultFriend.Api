using System.ComponentModel.DataAnnotations;

namespace OccultFriend.Domain.DTO
{
    public class RequestLogin
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Password { get; set; }
    }
}
