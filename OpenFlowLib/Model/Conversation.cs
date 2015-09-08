using System;
using System.Collections.Generic;
using MimeKit;
using System.Text;

namespace OpenFlowLib.Model
{
	public class Conversation
	{
		public Contact[] Contacts { get; private set; }

		public List<ChatMessage> MessagesCache { get; private set; }

		public Conversation (string[] participantAddresses)
		{
			MessagesCache = new List<ChatMessage> ();

			if (participantAddresses == null)
				throw new ArgumentNullException ("setParticipants");

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

		public int NumberOfUnreadMessages()
		{
			int count = 0;
			for (int i = 0; i < MessagesCache.Count; i++)
			{
				if (MessagesCache [i].Unread)
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

		private static bool ArrayContains(MailboxAddress[] array, MailboxAddress item)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array [i] == item)
					return true;
			}
			return false;
		}

		public MailboxAddress[] CopyParticipants(params MailboxAddress[] exclusionList)
		{
			List<MailboxAddress> result = new List<MailboxAddress>();
			for (int i = 0; i < Contacts.Length; i++)
			{
				if (ArrayContains (exclusionList, Contacts [i].Address))
					continue;

				result.Add(Contacts [i].Address);
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

