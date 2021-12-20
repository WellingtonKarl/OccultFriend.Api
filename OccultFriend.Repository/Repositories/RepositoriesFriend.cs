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
            var sql = @"Insert Into Friends (Name, Password, Description, Email, Data, ImagePath, IsChildreen)  
                        Values(@Name, @Password, @Description, @Email, @Data, @ImagePath, @IsChildreen)";

            _sqlConnection.Query<Friend>(
                    sql,
                    new
                    {
                        Name = friend.Name,
                        Password = friend.Password,
                        Description = friend.Description,
                        Email = friend.Email,
                        Data = friend.Data,
                        ImagePath = friend.ImagePath,
                        IsChildreen = friend.IsChildreen
                    });
        }

        public Friend Get(int id)
        {
            var sql = @"Select * from Friends where Id = @Id";

            return _sqlConnection.Query<Friend>(sql, new { Id = id }).SingleOrDefault();
        }

        public Friend Get(string name, string password)
        {
            var sql = @"Select * from Friends Where Name = @Name And Password = @Password";

            return _sqlConnection.Query<Friend>(sql, new { Name = name, Password = password }).SingleOrDefault();
        }

        public IEnumerable<FriendDto> GetAll()
        {
            var sql = "Select * from Friends";

            return _sqlConnection.Query<FriendDto>(sql);
        }

        public IEnumerable<FriendDto> Childdrens()
        {
            var sql = "Select * from Friends where IsChildreen = @Childreen";

            return _sqlConnection.Query<FriendDto>(sql, new { Childreen = Childreen.ISCHILDREEN });
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

            if (!string.IsNullOrWhiteSpace(friend.ImagePath))
                sql.Append("ImagePath = @ImagePath");

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
                    ImagePath = friend.ImagePath,
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
