using OccultFriend.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OccultFriend.Service.Interfaces
{
    public interface IEmailService
    {
        Task BodyEmail(IEnumerable<FriendDTO> friends);
        Task BodyEmailAdmin(HashSet<string> names);
        Task BodyEmailResponsible(FriendDTO nameDescription, FriendDTO emailFriends);
    }
}
