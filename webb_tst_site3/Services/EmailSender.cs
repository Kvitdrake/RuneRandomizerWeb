namespace webb_tst_site3.Services
{
    // Services/EmailSender.cs
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Реализация отправки email через SMTP или другой сервис
            // Например, используя MailKit или SendGrid
            return Task.CompletedTask;
        }
    }
}
