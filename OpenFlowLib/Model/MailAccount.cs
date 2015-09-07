using System;
using MimeKit;

namespace OpenFlowLib.Model
{
	public class MailAccount
	{
		public MailboxAddress Address { get; } = new MailboxAddress ("", "");
		public string Password { get; set; } = null;

		public string SmtpAddress { get; set; }
		public int SmtpPort { get; set; }

		public string ImapAddress { get; set; }
		public int ImapPort { get; set; }

		public MailAccount ()
		{
			
		}

		public bool IsValid()
		{
			return true;
		}
	}
}

