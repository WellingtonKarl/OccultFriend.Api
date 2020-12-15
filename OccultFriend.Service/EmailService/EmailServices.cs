using OccultFriend.Domain.Model;
using OccultFriend.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using OccultFriend.Domain.DTO;
using OccultFriend.Domain.IRepositories;
using System.Linq;

namespace OccultFriend.Service.EmailService
{
    public class EmailServices : IEmailService
    {
        private readonly IEmailTemplate _emailTemplate;
        private const string DrawEmailTemplate = "DrawEmailTemplate";
        private const string DrawnDuplicateEmailTemplate = "DrawnDuplicateEmailTemplate";
        private const string ResponsibleEmailTemplate = "ResponsibleEmailTemplate";

        public EmailServices(IEmailTemplate emailTemplate)
        {
            _emailTemplate = emailTemplate;
        }

        public async Task BodyEmail(IEnumerable<FriendDTO> friends, EmailSettings emailSettings)
        {
            foreach (var friend in friends)
            {
                var viewModel = new
                {
                    Friend = friend,
                    Date = DateTime.Now
                };

                var html = _emailTemplate.GenerateTemplateDrawEmail(DrawEmailTemplate, viewModel);

                await emailSettings.SendEmail(emailSettings, friend.Email, await html);
            }
        }

        public async Task BodyEmailAdmin(HashSet<string> names, EmailSettings emailSettings)
        {
            var viewModel = new
            {
                Names = _emailTemplate.GenerateTextNamesDuplicate(names),
                Date = DateTime.Now
            };

            var html = _emailTemplate.GenerateTemplateDrawEmail(DrawnDuplicateEmailTemplate, viewModel);

            await emailSettings.SendEmail(emailSettings, null, await html);
        }

        public async Task BodyEmailResponsible(FriendDTO namesDescription, FriendDTO friend, EmailSettings emailSettings)
        {
            var position = namesDescription.Name.IndexOf(",");
            var viewModel = new
            {
                Name = friend.Name,
                MyDescription = friend.Description,
                NameChild = namesDescription.Name.Substring(position + 1),
                DrawnName = namesDescription.Name.Substring(0, position),
                Description = namesDescription.Description,
                Date = DateTime.Now
            };

            var html = _emailTemplate.GenerateTemplateDrawEmail(ResponsibleEmailTemplate, viewModel);

            await emailSettings.SendEmail(emailSettings, friend.Email, await html);
        }
    }
}
