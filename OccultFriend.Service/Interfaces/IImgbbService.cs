using Microsoft.AspNetCore.Http;
using OccultFriend.Domain.Model;
using System.Threading.Tasks;

namespace OccultFriend.Service.Interfaces
{
    public interface IImgbbService
    {
        Task<ResponseImgbbService> UploadImage(IFormFile file);
    }
}
