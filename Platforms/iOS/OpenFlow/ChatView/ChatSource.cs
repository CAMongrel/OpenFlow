using System;

using UIKit;
using System.Collections.Generic;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using OpenFlowLib.Model;

namespace OpenFlow.Chat
{
	public class ChatSource : UITableViewSource
	{
		static readonly NSString IncomingCellId = new NSString ("Incoming");
		static readonly NSString OutgoingCellId = new NSString ("Outgoing");

		IList<ChatMessage> messages;

		readonly BubbleCell[] sizingCells;

		public ChatSource (IList<ChatMessage> messages)
		{
			if (messages == null)
				throw new ArgumentNullException ("messages");

			this.messages = messages;
			sizingCells = new BubbleCell[2];
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return messages.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			BubbleCell cell = null;
			ChatMessage msg = messages [indexPath.Row];

			cell = (BubbleCell)tableView.DequeueReusableCell (GetReuseId (msg.Type));
			cell.Message = msg;

			msg.Unread = false;

			return cell;
		}

		public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			ChatMessage msg = messages [indexPath.Row];
			return CalculateHeightFor (msg, tableView);
		}

		public override nfloat EstimatedHeight (UITableView tableView, NSIndexPath indexPath)
		{
			ChatMessage msg = messages [indexPath.Row];
			return CalculateHeightFor (msg, tableView);
		}

		nfloat CalculateHeightFor (ChatMessage msg, UITableView tableView)
		{
			var index = (int)msg.Type;
			BubbleCell cell = sizingCells [index];
			if (cell == null)
				cell = sizingCells [index] = (BubbleCell)tableView.DequeueReusableCell (GetReuseId (msg.Type));

			cell.Message = msg; 

			cell.SetNeedsLayout ();
			cell.LayoutIfNeeded ();
			CGSize size = cell.ContentView.SystemLayoutSizeFittingSize (UIView.UILayoutFittingCompressedSize);
			return NMath.Ceiling (size.Height) + 1;
		}

		NSString GetReuseId (ChatMessageType msgType)
		{
			return msgType == ChatMessageType.Incoming ? IncomingCellId : OutgoingCellId;
		}
	}
}