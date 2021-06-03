using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OccultFriend.Service.Interfaces
{
    public interface IEmailSettingService
    {
        Task SendEmail(string friendEmail, string html);
    }
}
