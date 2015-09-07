using System;
using OpenFlowLib.Model;

namespace OpenFlowLib.Model
{
	public class ChatMessage
	{
		public ChatMessageType Type { get; set; } = ChatMessageType.Incoming;
		public Contact Sender { get; set; } = null;
		public string Text { get; set; } = string.Empty;
		public bool Unread { get; set; } = true;
	}
}

