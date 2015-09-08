using System;
using System.Collections.Generic;
using MimeKit;
using System.Text;
using Couchbase.Lite;

namespace OpenFlowLib.Model
{
	public class Conversation
	{
		private static Database database;
		private static View defaultView;

		public Contact[] Contacts { get; private set; }

		private List<ChatMessage> messagesCache;

		static Conversation()
		{
			database = Manager.SharedInstance.GetDatabase ("conversations");
			defaultView = database.GetView ("conversation-messages");
			defaultView.SetMap ((document, emit) => {
				//
			}, "1");
		}

		public Conversation (string[] participantAddresses)
		{
			messagesCache = new List<ChatMessage> ();

			if (participantAddresses == null)
				throw new ArgumentNullException ("participantAddresses");

			Contacts = new Contact[participantAddresses.Length];
			for (int i = 0; i < participantAddresses.Length; i++)
			{
				Contact cntct = ContactDatabase.GetContact (participantAddresses[i]);
				if (cntct == null)
				{
					cntct = ContactDatabase.AddContact (participantAddresses[i], participantAddresses[i], participantAddresses[i]);
				}
				Contacts [i] = cntct;
			}
		}

		public static Conversation FromProperties(IDictionary<string, object> properties)
		{
			string[] parts = ((Newtonsoft.Json.Linq.JArray)properties ["conversation.participants"]).ToObject<string[]>();
			return new Conversation (parts);
		}

		public void AddMessage(ChatMessage msg)
		{
			messagesCache.Add (msg);
		}

		public ChatMessage[] CopyMessageCache()
		{
			return messagesCache.ToArray ();
		}

		public int NumberOfUnreadMessages()
		{
			int count = 0;
			for (int i = 0; i < messagesCache.Count; i++)
			{
				if (messagesCache [i].Unread)
				{
					count++;
				}
			}
			return count;
		}

		private bool HasEmailAddressInParticipants(string eMailAddress)
		{
			if (Contacts == null || eMailAddress == null)
				return false;

			eMailAddress = eMailAddress.ToLowerInvariant ();
			for (int i = 0; i < Contacts.Length; i++)
			{
				if (Contacts [i].Address.Address.ToLowerInvariant () == eMailAddress)
					return true;
			}

			return false;
		}

		public bool HasExactParticipants(string[] setParticipantAddresses)
		{
			if (Contacts == null || Contacts.Length == 0)
				return false;

			if (setParticipantAddresses.Length != Contacts.Length)
				return false;

			for (int i = 0; i < setParticipantAddresses.Length; i++)
			{
				if (HasEmailAddressInParticipants (setParticipantAddresses[i]) == false)
					return false;
			}

			return true;
		}

		private static bool ArrayContains(string[] array, string item)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array [i] == item)
					return true;
			}
			return false;
		}

		public string[] CopyParticipants(params string[] exclusionList)
		{
			List<string> result = new List<string>();
			for (int i = 0; i < Contacts.Length; i++)
			{
				if (ArrayContains (exclusionList, Contacts [i].Address.Address))
					continue;

				result.Add(Contacts [i].Address.Address);
			}
			return result.ToArray();
		}

		public override string ToString ()
		{
			StringBuilder builder = new StringBuilder ();
			if (Contacts.Length > 0)
			{
				builder.Append (Contacts [0].LocalName);

				for (int i = 1; i < Contacts.Length; i++)
				{
					builder.Append (", ");
					builder.Append (Contacts [i].LocalName);
				}
			} else
			{
				builder.Append ("Empty conversation");
			}
			return builder.ToString ();
		}
	}
}

