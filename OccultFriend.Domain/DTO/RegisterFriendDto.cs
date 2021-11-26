using System.ComponentModel.DataAnnotations;

namespace OccultFriend.Domain.DTO
{
    public class RegisterFriendDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Password { get; set; }

        [EmailAddress(ErrorMessage = "Email inválido!")]
        [Required(ErrorMessage = "O email é obrigatório.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A Descrição é obrigatória.")]
        public string Description { get; set; }

        public bool? IsChildren { get; set; }
    }
}
