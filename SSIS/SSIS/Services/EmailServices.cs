using System.Net.Mail;
using System.Threading;

namespace SSIS.Services
{
    public class EmailServices
    {
        public void SentEmailTo(string emailTo, string subject, string body)
        {
            var sendMailThread = new Thread(() =>
            {
                MailMessage mail = new MailMessage();
                using (SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com"))
                {
                    mail.From = new MailAddress("team3lovely@gmail.com");
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = body + "<br><br>This is an automatically generated email, please do not reply.";
                    mail.IsBodyHtml = true;

                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("team3lovely", "te@mthree");
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                }
            });
            sendMailThread.Start();
        }
    }
}
