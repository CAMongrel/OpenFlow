using System;
using System.Collections.Generic;
using UIKit;
using MailKit.Net.Imap;
using MailKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using MailKit.Search;
using OpenFlowLib.Network;
using OpenFlowLib.Model;
using OpenFlow.Chat;
using MimeKit;
using Foundation;

namespace OpenFlow
{
	public partial class ViewController : UIViewController
	{
		private MessageLoader loader;
		private MailAccount account;

		public ViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			conversationTable.WeakDelegate = this;
			conversationTable.WeakDataSource = this;

			account = new MailAccount ();
			account.Address.Name = "Hans Meier 1";
			account.Address.Address = "openflowtest1@gmail.com";
			account.ImapAddress = "imap.gmail.com";
			account.ImapPort = 993;
			account.Password = "openflow1";
			account.SmtpAddress = "smtp.gmail.com";
			account.SmtpPort = 587;

			ContactDatabase.AddContact ("Ich", account.Address.Name, account.Address.Address);

			ConversationDatabase.GetConversationForParticipants (new string[] { "hans@meier.de", "zuppel@bla.de" });

			loader = new MessageLoader (account);
			loader.OnMessageReceived += (Message msg) => { HandleMessage(msg); };

			loader.Start();
			Console.WriteLine("Loader.IsActive: " + loader.IsActive);

			this.Title = account.Address.Address;
		}

		[Export ("tableView:numberOfRowsInSection:")]
		public nint RowsInSection (UITableView tableview, nint section)
		{
			return ConversationDatabase.Count;
		}

		[Export ("tableView:cellForRowAtIndexPath:")]
		public UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell ("convCell");
			if (cell == null)
			{
				cell = new UITableViewCell (UITableViewCellStyle.Default, "convCell");
			}

			Conversation conv = ConversationDatabase.GetConversationAtIndex (indexPath.Row);
			if (conv != null)
				cell.TextLabel.Text = conv.ToString () + " (" + conv.NumberOfUnreadMessages() + ")";
			else
				cell.TextLabel.Text = "Invalid conversation";
			return cell;
		}

		[Export ("tableView:didSelectRowAtIndexPath:")]
		public void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			Conversation conv = ConversationDatabase.GetConversationAtIndex (indexPath.Row);
			if (conv != null)
			{
				ChatViewController cvc = new ChatViewController(conv);
				cvc.SenderAccount = account;
				this.NavigationController.PushViewController(cvc, true);
			}
		}

		private void HandleMessage(Message msg)
		{
			Console.WriteLine("Received: " + msg.Text);

			InvokeOnMainThread (() => {
				string[] listOfAllAddresses = new string[msg.Others.Length + 1];
				listOfAllAddresses[0] = msg.From.Address;
				for (int i = 0; i < msg.Others.Length; i++)
				{
					listOfAllAddresses[i + 1] = msg.Others[i].Address;
				}

				Conversation conv = ConversationDatabase.GetConversationForParticipants(listOfAllAddresses);
				if (conv == null)
					throw new InvalidOperationException("Conversation cannot be null");

				ChatMessage newMsg = new ChatMessage ();
				newMsg.Type = ChatMessageType.Incoming;
				newMsg.Sender = ContactDatabase.GetContact(msg.From.Address);
				newMsg.Text = msg.Text;
				newMsg.Unread = true;

				conv.AddMessage(newMsg);

				conversationTable.ReloadData();

				ChatViewController cvc = this.NavigationController.TopViewController as ChatViewController;
				if (cvc != null && cvc.Conversation == conv)
				{
					cvc.HandleMessage (newMsg);
				}
			});
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		partial void UIButton130_TouchUpInside (UIButton sender)
		{
			Contact ctct = ContactDatabase.GetContact("openflowtest1@gmail.com");
			if (ctct == null)
			{
				ContactDatabase.AddContact("openflowtest1@gmail.com", "openflow1", "openflowtest1@gmail.com");
				ctct = ContactDatabase.GetContact("openflowtest1@gmail.com");
			}

			string[] listOfAllAddresses = new string[2];
			listOfAllAddresses[0] = ctct.Address.Address;
			listOfAllAddresses[1] = account.Address.Address;

			Conversation conv = ConversationDatabase.GetConversationForParticipants(listOfAllAddresses);
			if (conv != null)
			{
				ChatViewController cvc = new ChatViewController(conv);
				cvc.SenderAccount = account;
				this.NavigationController.PushViewController(cvc, true);
			}
		}

		partial void UIButton131_TouchUpInside (UIButton sender)
		{
			Contact ctct = ContactDatabase.GetContact("openflowtest2@gmail.com");
			if (ctct == null)
			{
				ContactDatabase.AddContact("openflowtest2@gmail.com", "openflow2", "openflowtest2@gmail.com");
				ctct = ContactDatabase.GetContact("openflowtest2@gmail.com");
			}

			string[] listOfAllAddresses = new string[2];
			listOfAllAddresses[0] = ctct.Address.Address;
			listOfAllAddresses[1] = account.Address.Address;

			Conversation conv = ConversationDatabase.GetConversationForParticipants(listOfAllAddresses);
			if (conv != null)
			{
				ChatViewController cvc = new ChatViewController(conv);
				cvc.SenderAccount = account;
				this.NavigationController.PushViewController(cvc, true);
			}
		}
	}
}

