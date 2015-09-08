using System;
using System.Collections.Generic;
using MimeKit;
using Couchbase.Lite;
using Newtonsoft.Json;

namespace OpenFlowLib.Model
{
	public static class ContactDatabase
	{
		private static Database database;
		private static View lookupView;
		private static LiveQuery lookupViewLiveQuery;
		private static Dictionary<string, string> emailLookup;

		static ContactDatabase ()
		{
			database = Manager.SharedInstance.GetDatabase ("contacts");
			emailLookup = new Dictionary<string, string> ();

			lookupView = database.GetView ("addresses");
			lookupView.SetMap ((document, emit) => {
				var val = document["contact.address.address"];
				emit(document["_id"], val);
			}, "1");

			Query lookupQuery = lookupView.CreateQuery ();

			BuildLookup(lookupQuery.Run ());

			lookupViewLiveQuery = lookupQuery.ToLiveQuery();
			lookupViewLiveQuery.Changed += (object sender, QueryChangeEventArgs e) => { BuildLookup(e.Rows); };
			lookupViewLiveQuery.Start ();
		}

		public static int Count
		{
			get
			{
				return database.DocumentCount;
			}
		}

		private static void BuildLookup(QueryEnumerator rows)
		{
			lock (emailLookup)
			{
				emailLookup.Clear ();

				foreach (var row in rows)
				{
					string key = (string)row.Value;

					if (emailLookup.ContainsKey(key))
						emailLookup [key] = (string)row.Key;
					else
						emailLookup.Add (key, (string)row.Key);
				}
			}
		}

		private static string GetDatabaseId(string eMailAddress)
		{
			string key = eMailAddress.ToLowerInvariant ();
			lock (emailLookup)
			{
				if (emailLookup.ContainsKey (key))
				{
					return emailLookup [key];
				}
			}

			return null;
		}

		public static List<Contact> GetAllContacts()
		{
			List<Contact> result = new List<Contact> ();

			Query query = database.CreateAllDocumentsQuery ();
			var rows = query.Run ();
			foreach (var row in rows)
			{
				object key = row.Key;
				Document doc = row.Document;
				var properties = doc.Properties;
				var contact = Contact.FromProperties(properties);
				result.Add (contact);
			}

			return result;
		}

		public static Contact GetContact(string eMailAddress)
		{
			string docId = GetDatabaseId (eMailAddress);
			Document doc = database.GetExistingDocument (docId);
			if (doc == null)
				return null;

			return Contact.FromProperties (doc.Properties);
		}

		public static Contact AddContact(string setLocalName, string setAddressName, string setAddress)
		{
			string docId = GetDatabaseId (setAddress);
			Document doc = database.GetExistingDocument (docId);

			if (doc != null)
			{
				Dictionary<String, Object> newProperties = new Dictionary<String, Object>(doc.Properties);
				newProperties["contact.address.name"] = setAddressName;
				newProperties["contact.address.address"] = setAddress;
				newProperties["contact.localname"] = setLocalName;
				doc.PutProperties (newProperties);
				return Contact.FromProperties (newProperties);
			} else
			{
				Contact newContact = new Contact () { LocalName = setLocalName, Address = new MailboxAddress(setAddressName, setAddress) };
				doc = database.CreateDocument ();
				Dictionary<string, object> vals = new Dictionary<string, object> { 
					{ "contact.localname", newContact.LocalName },
					{ "contact.address.name", newContact.Address.Name },
					{ "contact.address.address", newContact.Address.Address } 
				};
				doc.PutProperties (vals);
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

