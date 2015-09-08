using System;
using System.Collections.Generic;
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

		public static Contact FromProperties(IDictionary<string, object> properties)
		{
			return new Contact () { LocalName = (string)properties["contact.localname"], 
				Address = new MailboxAddress(
					(string)properties["contact.address.name"],
					(string)properties["contact.address.address"]
				) };
		}
	}
}

