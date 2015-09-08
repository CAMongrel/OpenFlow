using System;
using System.Collections.Generic;
using MimeKit;
using Couchbase.Lite;

namespace OpenFlowLib.Model
{
	public static class ConversationDatabase
	{
		private static Database database;
		private static View databaseView;
		private static List<Conversation> conversations;

		static ConversationDatabase ()
		{
			conversations = new List<Conversation> ();

			database = Manager.SharedInstance.GetDatabase ("conversations");
			databaseView = database.GetView ("conversations");

			LoadConversations ();
		}

		private static void LoadConversations()
		{
			Query query = database.CreateAllDocumentsQuery ();
			var rows = query.Run ();
			foreach (var row in rows)
			{
				Document doc = row.Document;
				var properties = doc.Properties;
				Conversation conv = Conversation.FromProperties (properties);
				conversations.Add (conv);
			}
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

		private static Conversation FindConversationForParticipants(string[] participantAddresses)
		{
			for (int i = 0; i < conversations.Count; i++)
			{
				if (conversations [i].HasExactParticipants (participantAddresses))
					return conversations [i];
			}
			return null;
		}

		public static Conversation GetConversationForParticipants(string[] participantAddresses)
		{
			Conversation conv = FindConversationForParticipants (participantAddresses);
			if (conv != null)
				return conv;

			conv = new Conversation (participantAddresses);
			conversations.Add (conv);

			Dictionary<string, object> vals = new Dictionary<string, object> {
				{ "conversation.participants", conv.CopyParticipants() },
			};
			Document doc = database.CreateDocument ();
			doc.PutProperties (vals);
			return conv;
		}
	}
}

