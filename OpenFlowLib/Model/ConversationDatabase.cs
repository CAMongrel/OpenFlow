using System;
using System.Collections.Generic;
using MimeKit;

namespace OpenFlowLib.Model
{
	public static class ConversationDatabase
	{
		private static List<Conversation> conversations;

		static ConversationDatabase ()
		{
			conversations = new List<Conversation> ();
		}

		public static int Count
		{
			get
			{
				if (conversations == null)
					return 0;
				
				return conversations.Count;
			}
		}

		public static Conversation GetConversationAtIndex(int index)
		{
			if (conversations == null || index < 0 || index >= Count)
				return null;

			return conversations[index];
		}

		private static Conversation FindConversationForParticipants(MailboxAddress[] participants)
		{
			for (int i = 0; i < conversations.Count; i++)
			{
				if (conversations [i].HasExactParticipants (participants))
					return conversations [i];
			}
			return null;
		}

		public static Conversation GetConversationForParticipants(MailboxAddress[] participants)
		{
			Conversation conv = FindConversationForParticipants (participants);
			if (conv != null)
				return conv;

			conv = new Conversation (participants);
			conversations.Add (conv);
			return conv;
		}
	}
}

