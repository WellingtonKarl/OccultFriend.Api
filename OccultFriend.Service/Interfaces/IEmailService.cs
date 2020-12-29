﻿using OccultFriend.Domain.DTO;
using OccultFriend.Domain.Model;
using OccultFriend.Service.EmailService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OccultFriend.Service.Interfaces
{
    public interface IEmailService
    {
        Task BodyEmail(IEnumerable<FriendDTO> friends);

        Task BodyEmailAdmin(HashSet<string> names);

        Task BodyEmailResponsible(FriendDTO nameDescription, FriendDTO emailFriends);
    }
}
