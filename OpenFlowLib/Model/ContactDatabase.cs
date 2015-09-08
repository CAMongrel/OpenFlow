using System;
using System.Collections.Generic;
using MimeKit;
using Couchbase.Lite;

namespace OpenFlowLib.Model
{
	public static class ContactDatabase
	{
		private static Database database;
		private static Dictionary<string, string> emailLookup;

		static ContactDatabase ()
		{
			database = Manager.SharedInstance.GetDatabase ("Contacts");
			emailLookup = new Dictionary<string, string> ();
		}

		public static int Count
		{
			get
			{
				return database.DocumentCount;
			}
		}

		private static string GetDatabaseId(string eMailAddress)
		{
			string key = eMailAddress.ToLowerInvariant ();
			string docId = null;
			if (emailLookup.ContainsKey (key))
			{
				return emailLookup [key];
			}

			if (docId == null)
			{
				// TODO: Lookup in database
			}

			return null;
		}

		public static Contact GetContact(string eMailAddress)
		{
			string docId = GetDatabaseId (eMailAddress);
			Document doc = database.GetExistingDocument (docId);
			if (doc == null)
				return null;

			Dictionary<String, Object> newProperties = new Dictionary<String, Object>(doc.Properties);
			return newProperties["contact"] as Contact;
		}

		public static Contact AddContact(string setLocalName, MailboxAddress setAddress)
		{
			string docId = GetDatabaseId (setAddress.Address);
			Document doc = database.GetExistingDocument (docId);

			if (doc != null)
			{
				Dictionary<String, Object> newProperties = new Dictionary<String, Object>(doc.Properties);
				Contact contact = newProperties["contact"] as Contact;
				contact.Address = setAddress;
				contact.LocalName = setLocalName;
				doc.PutProperties (newProperties);
				return contact;
			} else
			{
				Contact newContact = new Contact () { LocalName = setLocalName, Address = setAddress };
				doc = database.CreateDocument ();
				Dictionary<string, object> vals = new Dictionary<string, object> { { "contact", newContact } };
				doc.PutProperties (vals);
				emailLookup.Add (setAddress.Address, doc.Id);
				return newContact;
			}
		}

		public static void RemoveContact(string eMailAddress)
		{
			string docId = GetDatabaseId (eMailAddress);
			Document doc = database.GetExistingDocument (docId);
			if (doc != null)
				doc.Delete ();
		}
	}
}

