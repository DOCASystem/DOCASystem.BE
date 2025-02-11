using System.Net;
using System.Net.Mail;

namespace DOCA.API.Utils;

public class SmsUtil
{
    public static bool SendEmail(string toEmail, string subject, string body, IConfiguration configuration)
    {
        try
        {
            var smtpServer = configuration["Email:SmtpServer"];
            var smtpPort = int.Parse(configuration["Email:Port"]);
            var smtpUser = configuration["Email:Username"];
            var smtpPass = configuration["Email:Password"];
            var fromEmail = configuration["Email:From"];

            var smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            smtpClient.Send(mailMessage);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi gửi email: {ex.Message}");
            return false;
        }
    }
}