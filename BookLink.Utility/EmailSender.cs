// BookLink.Utility/EmailSender.cs
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Threading.Tasks;

namespace BookLink.Utility
{
	/// <summary>
	/// Implements email sending functionality using Gmail SMTP via MailKit.
	/// </summary>
	public class EmailSender : IEmailSender
	{
		private readonly IConfiguration _config;
		private readonly ILogger<EmailSender> _logger;

		/// <summary>
		/// Initializes a new instance of the EmailSender class with configuration and logging dependencies.
		/// </summary>
		public EmailSender(IConfiguration config, ILogger<EmailSender> logger)
		{
			_config = config ?? throw new ArgumentNullException(nameof(config));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <summary>
		/// Sends an email asynchronously using SMTP settings retrieved from configuration.
		/// </summary>
		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			try
			{
				// Retrieve SMTP settings from configuration
				var smtpServer = _config["EmailSettings:SmtpServer"];
				var portString = _config["EmailSettings:Port"];
				var senderEmail = _config["EmailSettings:SenderEmail"];
				var password = _config["EmailSettings:Password"];
				var senderName = _config["EmailSettings:SenderName"];

				// Validate required settings
				if (string.IsNullOrWhiteSpace(smtpServer))
					throw new InvalidOperationException("SMTP server is not configured.");
				if (string.IsNullOrWhiteSpace(portString) || !int.TryParse(portString, out int port))
					throw new InvalidOperationException("Invalid or missing SMTP port.");
				if (string.IsNullOrWhiteSpace(senderEmail))
					throw new InvalidOperationException("Sender email is not configured.");
				if (string.IsNullOrWhiteSpace(password))
					throw new InvalidOperationException("SMTP password is not configured.");
				if (string.IsNullOrWhiteSpace(subject))
					throw new ArgumentException("Subject cannot be empty.", nameof(subject));
				if (string.IsNullOrWhiteSpace(htmlMessage))
					throw new ArgumentException("Message body cannot be empty.", nameof(htmlMessage));
				if (string.IsNullOrWhiteSpace(email))
					throw new ArgumentException("Recipient email cannot be empty.", nameof(email));

				// Use default sender name if not provided
				senderName = string.IsNullOrWhiteSpace(senderName) ? "BookLink App" : senderName;

				_logger.LogInformation("Starting email send process to {Email} with subject '{Subject}'", email, subject);

				// Create the email message
				var message = new MimeMessage();
				message.From.Add(new MailboxAddress(senderName, senderEmail));
				message.To.Add(new MailboxAddress("", email));
				message.Subject = subject;
				message.Body = new TextPart("html") { Text = htmlMessage };

				// Send the email using MailKit
				using (var client = new SmtpClient())
				{
					_logger.LogDebug("Connecting to SMTP server at {SmtpServer}:{Port}", smtpServer, port);
					await client.ConnectAsync(smtpServer, port, MailKit.Security.SecureSocketOptions.StartTls);

					_logger.LogDebug("Authenticating with SMTP server as {SenderEmail}", senderEmail);
					await client.AuthenticateAsync(senderEmail, password);

					_logger.LogDebug("Sending email to {Email}", email);
					await client.SendAsync(message);

					_logger.LogDebug("Disconnecting from SMTP server");
					await client.DisconnectAsync(true);

					_logger.LogInformation("Email sent successfully to {Email}", email);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to send email to {Email} with subject '{Subject}'", email, subject);
				throw; // Re-throw to allow calling code (e.g., Identity pages) to handle the failure
			}
		}
	}
}