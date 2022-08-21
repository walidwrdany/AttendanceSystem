using System.Net.Mail;

namespace ServiceDemo1.Utilities
{
    public class EmailUtility
    {
        public static void SendEmail(string fromMail, string fromMailName, string toMail, string ccMail, string bccMail, string subject, string body)
        {

            var message = new MailMessage
            {
                From = new MailAddress(fromMail, fromMailName),
                Sender = null,
                Priority = MailPriority.Normal,
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            foreach (var c in toMail.Split(',')) message.To.Add(new MailAddress(c));
            foreach (var c in ccMail.Split(',')) message.CC.Add(new MailAddress(c));
            foreach (var c in bccMail.Split(',')) message.Bcc.Add(new MailAddress(c));

            var smtp = new SmtpClient
            {
                EnableSsl = true
            };

            smtp.Send(message);
        }
    }
}
