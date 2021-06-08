using OccultFriend.Domain.Model;
using OccultFriend.Service.Interfaces;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OccultFriend.Service.EmailService
{
    public class EmailSettingService : IEmailSettingService
    {
        private readonly EmailSetting _emailSettingDto;

        public EmailSettingService(EmailSetting emailSettingDto)
        {
            _emailSettingDto = emailSettingDto;
        }

        public async Task SendEmail(string friendEmail, string html)
        {
            using var smtp = new SmtpClient();
            try
            {
                var mailMessage = new MailMessage
                {
                    //Sender
                    From = new MailAddress(_emailSettingDto.Username)
                };

                //Builder the MailMessage
                if (friendEmail != null)
                {
                    mailMessage.CC.Add(friendEmail);
                }
                else
                {
                    mailMessage.CC.Add(_emailSettingDto.UserAdmin);
                }

                mailMessage.Subject = "Testando Email para o App Amigo oculto da família.";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = html;

                //Configuration with Port
                var _smtpClient = new SmtpClient(_emailSettingDto.HostName, _emailSettingDto.Port)
                {
                    // Credential for send SMTP Security (When the Server require authentication)
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_emailSettingDto.Username, _emailSettingDto.Password),

                    EnableSsl = _emailSettingDto.UseSSL
                };

                await _smtpClient.SendMailAsync(mailMessage);

            }
            catch (Exception ex)
            {
                throw new ArgumentNullException(ex.Message);
            }
        }

    }
}
