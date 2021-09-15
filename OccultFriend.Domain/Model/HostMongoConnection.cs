using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OccultFriend.Domain.Model
{
    public class HostMongoConnection
    {
        public string FriendsCollectionName { get; set ; }
        public string ConnectionString { get; set; }
        public string DataBaseName { get; set; }
    }
}
