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

        public IEnumerable<FriendDTO> Childdrens()
        {
            return _friends.Find(f => f.IsChildreen == Convert.ToBoolean(Childreen.ISCHILDREEN))
                .ToEnumerable()
                .Select(f => new FriendDTO 
                {
                    Description = f.Description,
                    Email = f.Email,
                    Id = f.Id,
                    IsChildreen = f.IsChildreen,
                    Name = f.Name
                });
        }

        public void Create(Friend friend)
        {
            friend.Id = Id++;
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

        public IEnumerable<FriendDTO> GetAll()
        {
            return _friends.AsQueryable().Select(f => new FriendDTO 
            {
                Description = f.Description,
                Email = f.Email,
                Id = f.Id,
                IsChildreen = f.IsChildreen,
                Name = f.Name
            });
        }

        public void Update(Friend friend, int id)
        {
            _friends.ReplaceOne(f => f.Id == id, friend);
        }

        private void GetCountIdFriends()
        {
            Id += _friends.AsQueryable().Count();
        }
    }
}
