using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OccultFriend.Service.EmailService
{
    public class EmailSettings
    {
        public string HostName { get; set; }
        public string Username { get; set; }
        public string UserAdmin { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }

        public async Task SendEmail(EmailSettings emailSettings, string friendEmail, string html)
        {
            using (var smtp = new SmtpClient())
            {
                try
                {
                    var mailMessage = new MailMessage();
                    // Remetente
                    mailMessage.From = new MailAddress(emailSettings.Username);

                    //Contrói o MailMessage
                    if(friendEmail != null)
                    {
                        mailMessage.CC.Add(friendEmail);
                    }
                    else
                    {
                        mailMessage.CC.Add(emailSettings.UserAdmin);
                    }

                    mailMessage.Subject = "Testando Email para o App Amigo oculto da família.";
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = html;

                    //CONFIGURAÇÃO COM PORTA
                    var _smtpClient = new SmtpClient(emailSettings.HostName, emailSettings.Port)
                    {
                        // Credencial para envio por SMTP Seguro (Quando o servidor exige autenticação)
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password),

                        EnableSsl = emailSettings.UseSSL
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
