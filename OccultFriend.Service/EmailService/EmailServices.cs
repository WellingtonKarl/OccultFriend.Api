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
        private EmailSettings _emailSettings;
        private const string DrawEmailTemplate = "DrawEmailTemplate";
        private const string DrawnDuplicateEmailTemplate = "DrawnDuplicateEmailTemplate";
        private const string ResponsibleEmailTemplate = "ResponsibleEmailTemplate";
        private IEmailTemplate _emailTemplate;
        private IEmailTemplate EmailTemplate
        {
            get => _emailTemplate ?? (_emailTemplate = new EmailTemplate());
            set => _emailTemplate = value;
        }

        public EmailServices(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public async Task BodyEmail(IEnumerable<FriendDTO> friends)
        {
            foreach (var friend in friends)
            {
                var viewModel = new
                {
                    Friend = friend,
                    Date = DateTime.Now
                };

                var html = EmailTemplate.GenerateTemplateDrawEmail(DrawEmailTemplate, viewModel);

                await _emailSettings.SendEmail(_emailSettings, friend.Email, await html);
            }
        }

        public async Task BodyEmailAdmin(HashSet<string> names)
        {
            var viewModel = new
            {
                Names = _emailTemplate.GenerateTextNamesDuplicate(names),
                Date = DateTime.Now
            };

            var html = EmailTemplate.GenerateTemplateDrawEmail(DrawnDuplicateEmailTemplate, viewModel);

            await _emailSettings.SendEmail(_emailSettings, null, await html);
        }

        public async Task BodyEmailResponsible(FriendDTO namesDescription, FriendDTO friend)
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

            var html = EmailTemplate.GenerateTemplateDrawEmail(ResponsibleEmailTemplate, viewModel);

            await _emailSettings.SendEmail(_emailSettings, friend.Email, await html);
        }
    }
}
