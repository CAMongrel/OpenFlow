using System;
using OpenFlowLib.Model;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading.Tasks;

namespace OpenFlowLib.iOS
{
	public class MessageSender
	{
		public MessageSender ()
		{
		}

		public async Task SendMessage(MailAccount fromAccount, string messageText, params string[] receipients)
		{
			MimeKit.MimeMessage message = new MimeKit.MimeMessage();
			message.From.Add(fromAccount.Address);
			for (int i = 0; i < receipients.Length; i++)
			{
				message.To.Add (new MailboxAddress(receipients [i], receipients [i]));
			}

			message.Subject = string.Format("[OpenFlow {0}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss zz"));
			message.Body = new MimeKit.TextPart("plain") {
				Text = messageText
			};

			SmtpClient client = new SmtpClient();
			await client.ConnectAsync(fromAccount.SmtpAddress, fromAccount.SmtpPort, SecureSocketOptions.StartTls);
			try
			{
				client.AuthenticationMechanisms.Remove ("XOAUTH2");
				await client.AuthenticateAsync(fromAccount.Address.Address, fromAccount.Password);
				await client.SendAsync(message);
			}
			finally
			{
				await client.DisconnectAsync(true);	
			}
		}
	}
}

