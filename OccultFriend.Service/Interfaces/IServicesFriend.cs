using OccultFriend.Domain.DTO;
using OccultFriend.Service.EmailService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OccultFriend.Service.Interfaces
{
    public interface IServicesFriend
    {
        Task Draw(EmailSettings emailSettings);
    }
}
