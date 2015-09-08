using System;
using MimeKit;
using Couchbase.Lite;
using System.Collections.Generic;

namespace OpenFlowLib.Model
{
	public class MailAccount
	{
		private static Database database;

		private static string accountDatabaseId;

		public MailboxAddress Address { get; } = new MailboxAddress ("", "");
		public string Password { get; set; } = null;

		public string SmtpAddress { get; set; }
		public int SmtpPort { get; set; }

		public string ImapAddress { get; set; }
		public int ImapPort { get; set; }

		public MailAccount ()
		{
			//
		}

		static MailAccount()
		{
			accountDatabaseId = null;
			database = Manager.SharedInstance.GetDatabase ("mailaccount");
		}

		public static MailAccount LoadMailAccount()
		{
			MailAccount result = null;

			Query query = database.CreateAllDocumentsQuery ();
			var rows = query.Run ();
			foreach (var row in rows)
			{
				Document doc = row.Document;
				var properties = doc.Properties;

				result = new MailAccount ();
				result.Address.Name = (string)properties ["mailaccount.address.name"];
				result.Address.Address = (string)properties ["mailaccount.address.address"];
				result.ImapAddress = (string)properties ["mailaccount.imap.address"];
				result.ImapPort = Convert.ToInt32(properties ["mailaccount.imap.port"]);
				result.Password = (string)properties ["mailaccount.password"];
				result.SmtpAddress = (string)properties ["mailaccount.smtp.address"];
				result.SmtpPort = Convert.ToInt32(properties ["mailaccount.smtp.port"]);

				accountDatabaseId = row.DocumentId;
				break;
			}

			return result;
		}

		public void Save()
		{
			Dictionary<string, object> vals = null;
			Document doc = null;

			if (accountDatabaseId != null)
			{
				doc = database.GetExistingDocument (accountDatabaseId);
			} 

			if (doc == null)
			{
				doc = database.CreateDocument ();
				accountDatabaseId = doc.Id;
			}
			vals = new Dictionary<string, object> () { 
				{ "mailaccount.address.name", this.Address.Name },
				{ "mailaccount.address.address", this.Address.Address },
				{ "mailaccount.imap.address", this.ImapAddress },
				{ "mailaccount.imap.port", this.ImapPort },
				{ "mailaccount.password", this.Password },
				{ "mailaccount.smtp.address", this.SmtpAddress },
				{ "mailaccount.smtp.port", this.SmtpPort },
				{ "mailaccount.is_active", true },
			};
			doc.PutProperties (vals);
		}

		public bool IsValid()
		{
			return true;
		}
	}
}

