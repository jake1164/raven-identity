using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace CreativeColon.Raven.Identity.Domain
{
    public class EmailService : IIdentityMessageService
    {
        public virtual async Task SendAsync(IdentityMessage message)
        {
            string Text = message.Body;
            string Html = message.Body;

            var SmtpInfo = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            using (var Message = new MailMessage(SmtpInfo.From, message.Destination))
            {
                Message.Subject = message.Subject;
                Message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(Text, null, MediaTypeNames.Text.Plain));
                Message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(Html, null, MediaTypeNames.Text.Html));

                using (var SmtpClient = new SmtpClient())
                {
                    SmtpClient.Host = SmtpInfo.Network.Host;
                    SmtpClient.EnableSsl = SmtpInfo.Network.EnableSsl;
                    var Credentials = new NetworkCredential(SmtpInfo.Network.UserName, SmtpInfo.Network.Password);
                    SmtpClient.UseDefaultCredentials = true;
                    SmtpClient.Credentials = Credentials;
                    SmtpClient.Port = SmtpInfo.Network.Port;
                    await SmtpClient.SendMailAsync(Message);
                }
            }
        }
    }
}
