using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace PustokApp.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("mahabbatag@code.edu.az"));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = htmlContent };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync("mahabbatag@code.edu.az", "lexk eiml satu syqx");
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Email uğurla göndərildi: {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Email göndərilməsi xətası: {ex.Message}");
                return false;
            }
        }

        public async Task<string> LoadEmailTemplateAsync(string templatePath, Dictionary<string, string> replacements)
        {
            try
            {
                using var reader = new StreamReader(templatePath);
                string html = await reader.ReadToEndAsync();

                foreach (var replacement in replacements)
                {
                    html = html.Replace($"{{{{{replacement.Key}}}}}", replacement.Value);
                }

                return html;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Email template yüklənməsi xətası: {ex.Message}");
                return string.Empty;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string username, string resetLink)
        {
            var replacements = new Dictionary<string, string>
            {
                { "username", username },
                { "resetLink", resetLink }
            };

            string htmlContent = await LoadEmailTemplateAsync("wwwroot/templates/ResetPasswordTemplate.html", replacements);
            
            if (string.IsNullOrEmpty(htmlContent))
                return false;

            return await SendEmailAsync(toEmail, "PustokApp - şifrə Sıfırlama Tələbi", htmlContent);
        }

        public async Task<bool> SendEmailConfirmationAsync(string toEmail, string username, string confirmationLink)
        {
            var replacements = new Dictionary<string, string>
            {
                { "username", username },
                { "link", confirmationLink }
            };

            string htmlContent = await LoadEmailTemplateAsync("wwwroot/templates/EmailConfirmTemplate.html", replacements);
            
            if (string.IsNullOrEmpty(htmlContent))
                return false;

            return await SendEmailAsync(toEmail, "PustokApp - Email Təsdiqi", htmlContent);
        }
    }
}