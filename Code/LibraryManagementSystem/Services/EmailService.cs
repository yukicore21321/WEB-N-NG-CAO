using System.Net;
using System.Net.Mail;

namespace LibraryManagementSystem.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string otp, string userName = "Người dùng");
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string otp, string userName = "Người dùng")
        {
            var smtpSettings = _configuration.GetSection("Smtp");
            var host = smtpSettings["Host"];
            var port = int.Parse(smtpSettings["Port"] ?? "587");
            var user = smtpSettings["User"];
            var pass = smtpSettings["Pass"];

            string htmlBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 15px rgba(0,0,0,0.05);'>
                <div style='background-color: #151619; padding: 30px; text-align: center;'>
                    <h1 style='color: white; margin: 0; letter-spacing: 5px; font-size: 28px; font-weight: 900;'>BOOKWORM</h1>
                </div>
                <div style='padding: 40px; background-color: #ffffff;'>
                    <h2 style='color: #151619; margin-top: 0; font-size: 22px;'>Xin chào, {userName}!</h2>
                    <p style='color: #555; line-height: 1.6; font-size: 16px;'>
                        Cảm ơn bạn đã tham gia hệ thống thư viện số của chúng tôi. Để hoàn tất quá trình xác thực, vui lòng sử dụng mã OTP dưới đây:
                    </p>
                    <div style='background-color: #f5f5f5; border-radius: 12px; padding: 30px; text-align: center; margin: 30px 0;'>
                        <span style='font-size: 48px; font-weight: 900; letter-spacing: 15px; color: #151619;'>{otp}</span>
                    </div>
                    <p style='color: #888; font-size: 14px; text-align: center; margin-bottom: 0;'>
                        Mã này có hiệu lực trong vòng <strong>10 phút</strong>. Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.
                    </p>
                </div>
                <div style='background-color: #fafafa; padding: 20px; text-align: center; border-top: 1px solid #eeeeee;'>
                    <p style='margin: 0; color: #aaa; font-size: 12px;'>
                        Đây là email tự động, vui lòng không phản hồi.<br>
                        &copy; 2026 BookWorm Management System
                    </p>
                </div>
            </div>";

            using (var client = new SmtpClient(host, port))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(user, pass);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(user!, "BookWorm System"),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
