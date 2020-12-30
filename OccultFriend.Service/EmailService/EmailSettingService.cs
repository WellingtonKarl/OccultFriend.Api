using OccultFriend.Domain.Model;
using OccultFriend.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OccultFriend.Service.EmailService
{
    public class EmailSettingService : IEmailSettingService
    {
        private EmailSetting _emailSettingDto;
        public EmailSettingService(EmailSetting emailSettingDto)
        {
            _emailSettingDto = emailSettingDto;
        }

        public async Task SendEmail(string friendEmail, string html)
        {
            using (var smtp = new SmtpClient())
            {
                try
                {
                    var mailMessage = new MailMessage
                    {
                        // Remetente
                        From = new MailAddress(_emailSettingDto.Username)
                    };

                    //Constrói o MailMessage
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

                    //CONFIGURAÇÃO COM PORTA
                    var _smtpClient = new SmtpClient(_emailSettingDto.HostName, _emailSettingDto.Port)
                    {
                        // Credencial para envio por SMTP Seguro (Quando o servidor exige autenticação)
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(_emailSettingDto.Username, _emailSettingDto.Password),

                        EnableSsl = _emailSettingDto.UseSSL
                    };

                    await _smtpClient.SendMailAsync(mailMessage);

                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }

    }
}
