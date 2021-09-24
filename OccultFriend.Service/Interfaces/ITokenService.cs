using OccultFriend.Domain.Model;

namespace OccultFriend.Service.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Friend friend);
    }
}
