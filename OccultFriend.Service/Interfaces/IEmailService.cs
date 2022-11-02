using OccultFriend.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OccultFriend.Service.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailParticipantService(IEnumerable<FriendDto> friends);
        Task SendEmailAdminService(IEnumerable<FriendDto> friendsRepeateds);
        Task SendEmailResponsibleService(FriendDto nameDescription, FriendDto emailFriends);
    }
}
