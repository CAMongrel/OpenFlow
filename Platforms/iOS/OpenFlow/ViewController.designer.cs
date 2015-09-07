// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace OpenFlow
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView conversationTable { get; set; }

		[Action ("UIButton130_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UIButton130_TouchUpInside (UIButton sender);

		[Action ("UIButton131_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UIButton131_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (conversationTable != null) {
				conversationTable.Dispose ();
				conversationTable = null;
			}
		}
	}
}
