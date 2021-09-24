using Dapper;
using OccultFriend.Domain.DTO;
using OccultFriend.Domain.Enum;
using OccultFriend.Domain.IRepositories;
using OccultFriend.Domain.Model;
using System.Collections.Generic;
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
            var sql = @"Insert Into Friends (Name, Password, Description, Email, IsChildreen)  
                        Values(@Name, @Password, @Description, @Email, @IsChildreen)";

            _sqlConnection.Query<Friend>(
                    sql,
                    new
                    {
                        Name = friend.Name,
                        Password = friend.Password,
                        Description = friend.Description,
                        Email = friend.Email,
                        IsChildreen = friend.IsChildreen
                    });
        }

        public Friend Get(int id)
        {
            var sql = @"Select * from Friends where Id = @Id";

            var friend = _sqlConnection.Query<Friend>(sql, new { Id = id }).SingleOrDefault();

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
            var sql = "Select * from Friends where IsChildreen = @Childreen";
            var friends = _sqlConnection.Query<FriendDTO>(sql, new { Childreen = Childreen.ISCHILDREEN });

            return friends;
        }

        public void Update(Friend friend)
        {
            var sql = new StringBuilder();
            sql.Append("Update Friends Set ");

            if (!string.IsNullOrEmpty(friend.Name))
                sql.Append("Name = @Name, ");

            if (!string.IsNullOrWhiteSpace(friend.Password))
                sql.Append("Password = @Password, ");

            if (!string.IsNullOrEmpty(friend.Description))
                sql.Append("Description = @Description, ");

            if (!string.IsNullOrEmpty(friend.Email))
                sql.Append("Email = @Email, ");

            sql.Append("IsChildreen = @IsChildreen Where Id = @Id");

            _sqlConnection.Query<Friend>(
                sql.ToString(),
                new
                {
                    Name = friend.Name,
                    Password = friend.Password,
                    Description = friend.Description,
                    Email = friend.Email,
                    IsChildreen = friend.IsChildreen,
                    Id = friend.Id
                });
        }

        public void Delete(int id)
        {
            var sql = @"Delete From Friends Where Id = @Id";
            _sqlConnection.Query<Friend>(sql, new { Id = id });
        }
    }
}
