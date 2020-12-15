using OccultFriend.Domain.DTO;
using OccultFriend.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace OccultFriend.Domain.IRepositories
{
    public interface IRepositoriesFriend
    {
        void Create(Friend friend);
        Friend Get(int id);
        IEnumerable<FriendDTO> GetAll();
        IEnumerable<FriendDTO> Childdrens();
        void Update(Friend friend, int id);
        void Delete(int id);
    }
}
