using System;
using System.Drawing;
using UIKit;

namespace FavoriteMovies
{
	public partial class SideMenuController : BaseController
	{
		public SideMenuController() : base(null, null)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			View.BackgroundColor = UIColor.FromRGB(.9f, .9f, .9f);

			var title = new UILabel(new RectangleF(0, 50, 230, 20));
			title.Font = UIFont.SystemFontOfSize(24.0f);
			title.TextAlignment = UITextAlignment.Center;
			title.TextColor = UIColor.Blue;
			title.Text = "Menu";

			var body = new UILabel(new RectangleF(30, 70, 170, 100));
			body.Font = UIFont.SystemFontOfSize(12.0f);
			body.TextAlignment = UITextAlignment.Center;
			body.Lines = 0;
			//body.Text = @"This is the side menu. You can use any UIViewController to put whatever you want here!";

			var introButton = new UIButton(UIButtonType.System);
			introButton.Frame = new RectangleF(0, 180, 230, 20);
			introButton.SetTitle("Profile", UIControlState.Normal);
			introButton.TouchUpInside += (sender, e) => {
				NavController.PopToRootViewController(false);
				SidebarController.CloseMenu();
			};

			var contentButton = new UIButton(UIButtonType.System);
			contentButton.Frame = new RectangleF(0, 220, 230, 20);
			contentButton.SetTitle("Movie Lists", UIControlState.Normal);
			contentButton.TouchUpInside += (sender, e) => {
				//NavController.PushViewController(new ContentController(), false);
				NavController.PushViewController (new MovieListPickerViewController (null, false), false);
				//SidebarController.ChangeContentView (new MovieListPickerViewController (null, true));
				SidebarController.CloseMenu();
			};

			var connectionsButton = new UIButton (UIButtonType.System);
			connectionsButton.Frame = new RectangleF (0, 260, 230, 20);
			connectionsButton.SetTitle ("Flix Friends", UIControlState.Normal);
			connectionsButton.TouchUpInside += (sender, e) => {
				//NavController.PushViewController(new ContentController(), false);
				//NavController.PushViewController (new MovieListPickerViewController (null, false), false);
				//SidebarController.ChangeContentView (new MovieListPickerViewController (null, true));
				SidebarController.CloseMenu ();
			};

			var SettingsButton = new UIButton (UIButtonType.System);
			SettingsButton.Frame = new RectangleF (0, 300, 230, 20);
			SettingsButton.SetTitle ("Settings", UIControlState.Normal);
			SettingsButton.TouchUpInside += (sender, e) => {
				//NavController.PushViewController(new ContentController(), false);
				//NavController.PushViewController (new MovieListPickerViewController (null, false), false);
				//SidebarController.ChangeContentView (new MovieListPickerViewController (null, true));
				SidebarController.CloseMenu ();
			};

			var AddFriendsButton = new UIButton (UIButtonType.System);
			AddFriendsButton.Frame = new RectangleF (0, 300, 230, 20);
			AddFriendsButton.SetTitle ("Add Friends", UIControlState.Normal);
			AddFriendsButton.TouchUpInside += (sender, e) => {
				//NavController.PushViewController(new ContentController(), false);
				//NavController.PushViewController (new MovieListPickerViewController (null, false), false);
				//SidebarController.ChangeContentView (new MovieListPickerViewController (null, true));
				SidebarController.CloseMenu ();
			};

			View.Add(title);
			View.Add(body);
			View.Add(introButton);
			View.Add(contentButton);
			View.Add(connectionsButton);
			View.Add (AddFriendsButton);

		}
	}
}

