using OccultFriend.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OccultFriend.Domain.DTO;

namespace OccultFriend.Service.EmailService
{
    public class EmailServices : IEmailService
    {
        #region Attributes

        private readonly IEmailTemplate _emailTemplate;
        private readonly IEmailSettingService _emailSettingService;

        #endregion

        #region Properties

        private static string DrawEmailTemplate => "DrawEmailTemplate";
        private static string DrawnDuplicateEmailTemplate => "DrawnDuplicateEmailTemplate";
        private static string ResponsibleEmailTemplate => "ResponsibleEmailTemplate";

        #endregion

        public EmailServices(IEmailTemplate emailTemplate, IEmailSettingService emailSettingService)
        {
            _emailTemplate = emailTemplate;
            _emailSettingService = emailSettingService;
        }

        public async Task SendEmailParticipantService(IEnumerable<FriendDto> friends)
        {
            foreach (var friend in friends)
            {
                var viewModel = new
                {
                    Friend = friend,
                    Date = DateTime.Now.ToString("dd/MM/yyyy")
                };

                var html = _emailTemplate.GenerateTemplateDrawEmail(DrawEmailTemplate, viewModel);

                await SendEmailService(friend.Email, await html);
            }
        }

        public async Task SendEmailAdminService(HashSet<string> names)
        {
            var viewModel = new
            {
                Names = _emailTemplate.GenerateTextNamesDuplicate(names),
                Date = DateTime.Now.ToString("dd/MM/yyyy")
            };

            var html = _emailTemplate.GenerateTemplateDrawEmail(DrawnDuplicateEmailTemplate, viewModel);

            await SendEmailService(null, await html);
        }

        public async Task SendEmailResponsibleService(FriendDto nameDescription, FriendDto emailFriends)
        {
            var position = nameDescription.Name.IndexOf(",");
            var viewModel = new
            {
                Name = emailFriends.Name,
                MyDescription = emailFriends.Description,
                NameChild = nameDescription.Name[(position + 1)..],
                DrawnName = nameDescription.Name.Substring(0, position),
                Description = nameDescription.Description,
                Date = DateTime.Now.ToString("dd/MM/yyyy")
            };

            var html = _emailTemplate.GenerateTemplateDrawEmail(ResponsibleEmailTemplate, viewModel);

            await SendEmailService(emailFriends.Email, await html);
        }

        private async Task SendEmailService(string friendEmail, string html)
        {
            await _emailSettingService.SendEmail(friendEmail, html);
        }
    }
}
