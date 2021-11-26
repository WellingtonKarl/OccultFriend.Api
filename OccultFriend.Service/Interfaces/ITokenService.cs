using OccultFriend.Domain.Model;
using System.Security.Claims;

namespace OccultFriend.Service.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Friend friend, ClaimsPrincipal user);
    }
}
