using MongoDB.Driver;
using OccultFriend.Domain.DTO;
using OccultFriend.Domain.Enum;
using OccultFriend.Domain.IRepositories;
using OccultFriend.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OccultFriend.Repository.MongoRepository
{
    public class FriendRepository : IRepositoriesFriend
    {
        private static int Id { get; set; }
        private readonly IMongoCollection<Friend> _friends;

        public FriendRepository(HostMongoConnection hostMongo)
        {
            var clientMongo = new MongoClient(hostMongo.ConnectionString);
            var dataBaseMongo = clientMongo.GetDatabase(hostMongo.DataBaseName);
            
            _friends = dataBaseMongo.GetCollection<Friend>(hostMongo.FriendsCollectionName);

            if (Id <= 0)
                GetCountIdFriends();
        }

        public IEnumerable<FriendDto> Childdrens()
        {
            return _friends.Find(f => f.IsChildreen == Convert.ToBoolean(Childreen.ISCHILDREEN))
                .ToEnumerable()
                .Select(f => new FriendDto 
                {
                    Description = f.Description,
                    Email = f.Email,
                    Id = f.Id,
                    IsChildreen = f.IsChildreen,
                    Name = f.Name,
                    Password = f.Password,
                    Data = f.Data,
                    ImagePath = f.ImagePath
                });
        }

        public void Create(Friend friend)
        {
            friend.Id = Id + 1;
            _friends.InsertOne(friend);
        }

        public void Delete(int id)
        {
            _friends.DeleteOne(f => f.Id == id);
        }

        public Friend Get(int id)
        {
            return _friends.Find(f => f.Id == id).FirstOrDefault();
        }

        public Friend Get(string name, string password)
        {
            return _friends.Find(f => f.Name == name && f.Password == password).FirstOrDefault();
        }

        public IEnumerable<FriendDto> GetAll()
        {
            return _friends.AsQueryable().Select(f => new FriendDto 
            {
                Description = f.Description,
                Email = f.Email,
                Id = f.Id,
                IsChildreen = f.IsChildreen,
                Name = f.Name,
                Password = f.Password,
                Data = f.Data,
                ImagePath = f.ImagePath
            });
        }

        public void Update(Friend friend)
        {
            _friends.ReplaceOne(f => f.Id == friend.Id, friend);
        }

        private void GetCountIdFriends()
        {
            try
            {
                Id += _friends.AsQueryable().Count();
            }
            catch (Exception ex)
            {
                var exception = new ApplicationException(ex.Message);
                throw exception;
            }
        }
    }
}
