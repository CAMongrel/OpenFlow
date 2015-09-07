using System;
using System.Collections.Generic;
using MimeKit;

namespace OpenFlowLib.Model
{
	public static class ContactDatabase
	{
		private static Dictionary<string, Contact> knownContacts;

		static ContactDatabase ()
		{
			knownContacts = new Dictionary<string, Contact> ();
		}

		public static int Count
		{
			get
			{
				return knownContacts.Count;
			}
		}

		public static Contact GetContact(string eMailAddress)
		{
			string key = eMailAddress.ToLowerInvariant ();
			if (knownContacts.ContainsKey (key))
				return knownContacts [key];

			return null;
		}

		public static Contact AddContact(string setLocalName, MailboxAddress setAddress)
		{
			string key = setAddress.Address.ToLowerInvariant ();

			if (knownContacts.ContainsKey (key))
			{
				Contact contact = knownContacts [key];
				contact.Address = setAddress;
				contact.LocalName = setLocalName;
				return contact;
			} else
			{
				Contact newContact = new Contact () { LocalName = setLocalName, Address = setAddress };
				knownContacts.Add (key, newContact);
				return newContact;
			}
		}

		public static void RemoveContact(string eMailAdress)
		{
			string key = eMailAdress.ToLowerInvariant ();
			if (knownContacts.ContainsKey (key))
				knownContacts.Remove (key);
		}
	}
}

