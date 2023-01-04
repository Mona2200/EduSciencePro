using MailKit.Net.Smtp;
using MimeKit;

namespace EduSciencePro.Data.Services
{
    public class EmailService
    {
      public async Task SendEmailAsync(string email, string subject, string message)
      {
         var emailMessage = new MimeMessage();

         emailMessage.From.Add(new MailboxAddress("EduSciencePro", "admin@sraka.top"));
         emailMessage.To.Add(new MailboxAddress("", email));
         emailMessage.Subject = subject;
         emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
         {
            Text = message
         };

         using (var client = new SmtpClient())
         {
            await client.ConnectAsync("188.42.151.19", 465, false);
            await client.AuthenticateAsync("admin@sraka.top", "w01i1ApT7p");
            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
         }
      }
   }
}
