using System;
using OpenFlowLib.Model;
using MailKit.Net.Imap;
using MailKit.Security;
using MailKit.Search;
using MailKit;
using System.Collections.Generic;
using System.Threading.Tasks;
using MimeKit;

namespace OpenFlowLib.Network
{
	public delegate void MessageReceived(Message msg);

	public class MessageLoader
	{
		private ImapClient client = null;
		public bool IsActive { get; private set; }

		public MailAccount Account { get; private set; }

		public event MessageReceived OnMessageReceived;

		public MessageLoader (MailAccount setAccountToCheck)
		{
			Account = setAccountToCheck;
			IsActive = false;
		}

		public void Start()
		{
			if (Account.IsValid () == false)
				return;

			client = new ImapClient ();

			IsActive = true;

			Task.Run (async () => {
				while (IsActive)
				{
					await ReadMessages();
					await Task.Delay(500);
				}

				if (client.IsConnected)
				{
					Console.WriteLine("Disconnect is required");
					await client.DisconnectAsync(true);	
				}
			});
		}

		public void Stop()
		{
			IsActive = false;
		}

		private async Task PerformConnect()
		{
		}

		private async Task ReadMessages()
		{
			try
			{
				if (client.IsConnected == false)
				{
					Console.WriteLine("Connect is required");
					await client.ConnectAsync (Account.ImapAddress, Account.ImapPort, SecureSocketOptions.SslOnConnect);
				}

				if (client.IsAuthenticated == false)
				{
					Console.WriteLine("Auth is required");
					client.AuthenticationMechanisms.Remove ("XOAUTH");
					await client.AuthenticateAsync (Account.Address.Address, Account.Password);
				}

				// The Inbox folder is always available on all IMAP servers...
				var inbox = client.Inbox;

				FolderAccess access = await inbox.OpenAsync(FolderAccess.ReadWrite);
				Console.WriteLine("Got folder access: " + access);

				var query = SearchQuery.SubjectContains("[OpenFlow").And(SearchQuery.NotSeen);

				IList<UniqueId> list = (await inbox.SearchAsync(query));

				Console.WriteLine(list.Count + " new message(s)");
				for (int i = 0; i < list.Count; i++) 
				{
					var message = await inbox.GetMessageAsync (list[i]);
					string msg = message.TextBody;
					await inbox.SetFlagsAsync(list[i], MessageFlags.Seen, true);

					MailboxAddress[] receipients = new MailboxAddress[message.To.Count];
					for (int j = 0; j < receipients.Length; j++)
						receipients[j] = message.To[j] as MailboxAddress;

					Message newMsg = new Message();
					newMsg.From = message.From[0] as MailboxAddress;
					newMsg.Others = receipients;
					newMsg.Text = msg;

					if (newMsg.Text.EndsWith("\r\n"))
						newMsg.Text = newMsg.Text.Substring(0, newMsg.Text.Length - "\r\n".Length);

					OnMessageReceived?.Invoke(newMsg);
				}
			}
			finally
			{
				
			}
		}
	}
}

