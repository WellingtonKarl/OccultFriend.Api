using Dapper;
using OccultFriend.Domain.DTO;
using OccultFriend.Domain.IRepositories;
using OccultFriend.Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace OccultFriend.Repository.Repositories
{
    public class RepositoriesFriend : IRepositoriesFriend
    {
        private readonly SqlConnection _sqlConnection;

        public RepositoriesFriend(SqlConnection connection)
        {
            _sqlConnection = connection;
        }

        public void Create(Friend friend)
        {
            var sql = $"Insert Into Friends (Name, Description, Email, EhCrianca)  Values('{friend.Name}', '{friend.Description}', '{friend.Email}', '{friend.EhCrianca}')";
            _sqlConnection.Query<Friend>(sql);
        }

        public Friend Get(int id)
        {
            var sql = $"Select * from Friends where Id = {id}";
            var friend = _sqlConnection.Query<Friend>(sql).SingleOrDefault();

            return friend;
        }

        public IEnumerable<FriendDTO> GetAll()
        {
            var sql = "Select * from Friends";
            var friends = _sqlConnection.Query<FriendDTO>(sql);

            return friends;
        }

        public IEnumerable<FriendDTO> Childdrens()
        {
            var sql = "Select * from Friends where EhCrianca = 1";
            var friends = _sqlConnection.Query<FriendDTO>(sql);

            return friends;
        }

        public void Update(Friend friend, int id)
        {
            var friendSelected = Get(id);
            var criterion = new Friend
            {
                Name = !string.IsNullOrEmpty(friend.Name) ? friend.Name : friendSelected.Name,
                Description = !string.IsNullOrEmpty(friend.Description) ? friend.Description : friendSelected.Description,
                Email = !string.IsNullOrEmpty(friend.Email) ? friend.Email : friendSelected.Email,
                EhCrianca = friend.EhCrianca != friendSelected.EhCrianca ? friend.EhCrianca : friendSelected.EhCrianca
            };

            var sql = $"Update Friends Set Name = '{criterion.Name}', Description = '{criterion.Description}', Email = '{criterion.Email}' Where Id = {id}";
            _sqlConnection.Query<Friend>(sql);
        }

        public void Delete(int id)
        {
            var sql = $"Delete From Friends Where Id = {id}";
            _sqlConnection.Query<Friend>(sql);
        }
    }
}
