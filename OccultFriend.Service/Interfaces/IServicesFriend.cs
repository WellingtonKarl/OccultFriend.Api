using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace OccultFriend.Service.Interfaces
{
    public interface IServicesFriend
    {
        Task Draw(bool childWillPlay);
        Task<string> CreateUploadImage(IFormFile file, string path);
    }
}
