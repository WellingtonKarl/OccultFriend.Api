using OccultFriend.Domain.DTO;
using OccultFriend.Domain.Model;
using System.Collections.Generic;

namespace OccultFriend.Domain.IRepositories
{
    public interface IRepositoriesFriend
    {
        void Create(Friend friend);
        Friend Get(int id);
        Friend Get(string name, string password);
        IEnumerable<FriendDTO> GetAll();
        IEnumerable<FriendDTO> Childdrens();
        void Update(Friend friend);
        void Delete(int id);
    }
}
