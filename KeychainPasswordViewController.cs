using System;
using System.Drawing;

using Foundation;
using UIKit;

namespace KeychainPassword
{
	public partial class KeychainPasswordViewController : UIViewController
	{
		public KeychainPasswordViewController (IntPtr handle) : base (handle)
		{
		}

		[Action("UnwindSegue:")]
		public void UnwindSegue(UIStoryboardSegue source) {
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if (TouchIdUtils.IsPhoneSecured ()) {
				TouchIdUtils.SetupTouchIdAuthentication ();
			} else {
				var alert = new UIAlertView ("oops", "no touch id or passcode set. Might want to fix that", null, "Ok");
				alert.Show ();
			}

		}

		partial void PressToLogin (UIButton sender)
		{
			if (TouchIdUtils.RequestAuthentication("For Secrets!")) {
				PerformSegue("LoggedIn", null);
			} else {
				var alert = new UIAlertView ("nope", "you still have your fingers, yes?", null, "Ok");
				alert.Show ();
			}


		}


	}
}

