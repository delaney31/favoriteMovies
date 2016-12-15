using System;
using System.Drawing;
using UIKit;

namespace FavoriteMovies
{
	public partial class SideMenuController : BaseController
	{

		public static UIImage signUpImage;
		public SideMenuController() : base(null, null)
		{
			//signUpImage = UIImage.FromBundle ("124817-matte-white-square-icon-business-signature1.png");
			//signUpImage = UIImage.FromBundle ("profile.png");
			signUpImage = UIImage.FromBundle ("1481507483_compose.png");
		}


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			View.BackgroundColor =  UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);

			UIImageView userProfileImage = new UIImageView();
			userProfileImage.Image = signUpImage;
			userProfileImage.BackgroundColor = UIColor.Clear;
			userProfileImage.Frame = new RectangleF (30, 55, 150, 150);
			//userProfileImage.Layer.BorderWidth = 2;
			userProfileImage.Layer.CornerRadius = userProfileImage.Frame.Size.Width / 2;
			userProfileImage.Layer.MasksToBounds = true;
			var title = new UIButton(new RectangleF(10, 220, 192, 20));
			title.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 20);
			////title.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			title.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			//title.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, 15);
			title.SetTitleColor (UIColor.White, UIControlState.Normal);
			title.SetTitle ("Sign-In/ Sign-Up", UIControlState.Normal);
			//title.Lines = 2;
			title.TouchUpInside += (sender, e) => {
				NavController.PopToRootViewController (false);
				SidebarController.CloseMenu ();
			};

			UIImageView profileImage = new UIImageView () { Image = UIImage.FromBundle ("1481450570_05-myhouse.png")};
			profileImage.Frame = new RectangleF (50, 275, 25, 30);
			var introButton = new UIButton(UIButtonType.System);
			introButton.Frame = new RectangleF(90, 282, 230, 20);
			introButton.SetTitle("Home", UIControlState.Normal);
			introButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			introButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			introButton.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, 18);
			introButton.TouchUpInside += (sender, e) => {
				NavController.PopToRootViewController(false);
				SidebarController.CloseMenu();
			};
			UIImageView customImage = new UIImageView () { Image = UIImage.FromBundle ("1481443482_document.png") };
			customImage.Frame = new RectangleF (40, 325, 40, 40);
			var contentButton = new UIButton(UIButtonType.System);
			contentButton.Frame = new RectangleF(90, 340, 230, 20);
			contentButton.SetTitle("Movie Lists", UIControlState.Normal);
			contentButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			contentButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			contentButton.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, 18);

			contentButton.TouchUpInside += (sender, e) => {
				//NavController.PushViewController(new ContentController(), false);
				NavController.PushViewController (new MovieListPickerViewController (null, false), false);
				//SidebarController.ChangeContentView (new MovieListPickerViewController (null, true));
				SidebarController.CloseMenu();
			};

			UIImageView friendsImage = new UIImageView () { Image = UIImage.FromBundle ("1481444239_Myspace.png") };
			friendsImage.Frame = new RectangleF (40, 365, 40, 40);
			var connectionsButton = new UIButton (UIButtonType.System);
			connectionsButton.Frame = new RectangleF (90, 380, 230, 20);
			connectionsButton.SetTitle ("Friends", UIControlState.Normal);
			connectionsButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			connectionsButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			connectionsButton.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, 18);
			connectionsButton.TouchUpInside += (sender, e) => {
				//NavController.PushViewController(new ContentController(), false);
				//NavController.PushViewController (new MovieListPickerViewController (null, false), false);
				//SidebarController.ChangeContentView (new MovieListPickerViewController (null, true));
				SidebarController.CloseMenu ();
			};
			UIImageView settingsImage = new UIImageView () { Image = UIImage.FromBundle ("1481445346_tools.png") };
			settingsImage.Frame = new RectangleF (40, 405, 40, 40);
			var SettingsButton = new UIButton (UIButtonType.System);
			SettingsButton.Frame = new RectangleF (90, 420, 230, 20);
			SettingsButton.SetTitle ("Settings", UIControlState.Normal);
			SettingsButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			SettingsButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			SettingsButton.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, 18);
			SettingsButton.TouchUpInside += (sender, e) => {
				//NavController.PushViewController(new ContentController(), false);
				//NavController.PushViewController (new MovieListPickerViewController (null, false), false);
				//SidebarController.ChangeContentView (new MovieListPickerViewController (null, true));
				SidebarController.CloseMenu ();
			};
			UIImageView addFriendsImage = new UIImageView () { Image = UIImage.FromBundle ("1481444239_AddFriends.png") };
			addFriendsImage.Frame = new RectangleF (40, 445, 40, 40);
			var AddFriendsButton = new UIButton (UIButtonType.System);
			AddFriendsButton.Frame = new RectangleF (90, 460, 230, 20);
			AddFriendsButton.SetTitle ("Add Friends", UIControlState.Normal);
			AddFriendsButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			AddFriendsButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			AddFriendsButton.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, 18);
			AddFriendsButton.TouchUpInside += (sender, e) => {
				//NavController.PushViewController(new ContentController(), false);
				//NavController.PushViewController (new MovieListPickerViewController (null, false), false);
				//SidebarController.ChangeContentView (new MovieListPickerViewController (null, true));
				SidebarController.CloseMenu ();
			};

			UIImageView inviteFriendsImage = new UIImageView () { Image = UIImage.FromBundle ("1481506307_mail.png") };
			inviteFriendsImage.Frame = new RectangleF (40, 486, 40, 40);
			var inviteFriendsButton = new UIButton (UIButtonType.System);
			inviteFriendsButton.Frame = new RectangleF (90, 500, 230, 20);
			inviteFriendsButton.SetTitle ("Invite Friends", UIControlState.Normal);
			inviteFriendsButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			inviteFriendsButton.SetTitleColor (UIColor.White, UIControlState.Normal);
			inviteFriendsButton.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, 18);
			inviteFriendsButton.TouchUpInside += (sender, e) => {
				//NavController.PushViewController(new ContentController(), false);
				//NavController.PushViewController (new MovieListPickerViewController (null, false), false);
				//SidebarController.ChangeContentView (new MovieListPickerViewController (null, true));
				SidebarController.CloseMenu ();
			};


			View.Add(title);
			View.Add(introButton);
			View.Add(contentButton);
			View.Add(connectionsButton);
			View.Add (SettingsButton);
			View.Add (AddFriendsButton);
			View.Add (profileImage);
			View.Add (customImage);
			View.Add (friendsImage);
			View.Add (settingsImage);
			View.Add (addFriendsImage);
			View.Add (userProfileImage);
			View.Add (inviteFriendsImage);
			View.Add (inviteFriendsButton);

		}
	}
}

