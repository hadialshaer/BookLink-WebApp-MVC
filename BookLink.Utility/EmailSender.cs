using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

namespace BookLink.Utility
{
	public class EmailSettings
	{
		public string SmtpServer { get; set; }
		public int Port { get; set; }
		public string SenderEmail { get; set; }
		public string SenderName { get; set; }
		public string Password { get; set; }
	}

	public class EmailSender : IEmailSender
	{
		private readonly EmailSettings _emailSettings;

		public EmailSender(IOptions<EmailSettings> emailSettings)
		{
			_emailSettings = emailSettings.Value;
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			var message = new MimeMessage();
			message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
			message.To.Add(new MailboxAddress("", email));
			message.Subject = subject;

			message.Body = new TextPart("html") { Text = htmlMessage };

			using var client = new SmtpClient();
			await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
			await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
			await client.SendAsync(message);
			await client.DisconnectAsync(true);
		}
	}
}