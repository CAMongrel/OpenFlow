using System;
using MimeKit;

namespace OpenFlowLib.Model
{
	public class Contact
	{
		public string LocalName { get; set; } = "";
		public MailboxAddress Address { get; set; } = null;

		public Contact ()
		{
			
		}
	}
}

