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
        private static string UrlImage => $"https://appdev-occultfriend-api.herokuapp.com/api/Friend/Image?name=";

        #endregion

        public EmailServices(IEmailTemplate emailTemplate, IEmailSettingService emailSettingService)
        {
            _emailTemplate = emailTemplate;
            _emailSettingService = emailSettingService;
        }

        public async Task SendEmailParticipantService(IEnumerable<FriendDTO> friends)
        {
            foreach (var friend in friends)
            {
                var viewModel = new
                {
                    Friend = friend,
                    Image = UrlImage + friend.PathImage,
                    Date = DateTime.UtcNow.ToString("dd/MM/yyyy"),
                };

                var html = _emailTemplate.GenerateTemplateDrawEmail(DrawEmailTemplate, viewModel);

                await SendEmailService(friend.Email, await html);
            }
        }

        public async Task SendEmailAdminService(Dictionary<string, string> dicFriendDuplicate)
        {
            var viewModel = new
            {
                Data = _emailTemplate.GenerateTemplateTable(dicFriendDuplicate),
                Date = DateTime.UtcNow.ToString("dd/MM/yyyy"),
            };

            var html = _emailTemplate.GenerateTemplateDrawEmail(DrawnDuplicateEmailTemplate, viewModel);

            await SendEmailService(null, await html);
        }

        //TODo melhorias, ao menos só testarei o envio sem as crianças.
        public async Task SendEmailResponsibleService(FriendDTO nameDescription, FriendDTO emailFriends)
        {
            var position = nameDescription.Name.IndexOf(",");
            var viewModel = new
            {
                Name = emailFriends.Name,
                MyDescription = emailFriends.Description,
                NameChild = nameDescription.Name[(position + 1)..],
                DrawnName = nameDescription.Name.Substring(0, position),
                Description = nameDescription.Description,
                Date = DateTime.UtcNow.ToString("dd/MM/yyyy"),
                //Image = $"https://localhost:44370/api/Friend/Image?name=Image.jpeg"
            };

            var html = _emailTemplate.GenerateTemplateDrawEmail(ResponsibleEmailTemplate, viewModel);

            await SendEmailService(emailFriends.Email, await html);
        }

        private async Task SendEmailService(string friendEmail, string html)
        {
            await _emailSettingService.SendEmail(friendEmail, html);
        }

        private TimeZoneInfo GetTimeZoneFromBrazil()
        {
            return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        }
    }
}
