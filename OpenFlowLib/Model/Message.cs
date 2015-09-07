using System;
using MimeKit;

namespace OpenFlowLib.Model
{
	public class Message
	{
		public string Text { get; set; }

		public MailboxAddress From;
		public MailboxAddress[] Others;

		public Message ()
		{
		}
	}
}

