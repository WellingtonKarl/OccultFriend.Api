using MongoDB.Driver;
using OccultFriend.Domain.DTO;
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
        private readonly IMongoCollection<Friend> _friends;

        public FriendRepository(HostMongoConnection hostMongo)
        {
            var clientMongo = new MongoClient(hostMongo.ConnectionString);
            var dataBaseMongo = clientMongo.GetDatabase(hostMongo.DataBaseName);

            _friends = dataBaseMongo.GetCollection<Friend>(hostMongo.FriendsCollectionName);
        }

        public IEnumerable<FriendDTO> Childdrens()
        {
            throw new NotImplementedException();
        }

        public void Create(Friend friend)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Friend Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FriendDTO> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(Friend friend, int id)
        {
            throw new NotImplementedException();
        }
    }
}
