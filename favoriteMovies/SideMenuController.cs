using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using LoginScreen;
using ObjCRuntime;
using UIKit;

namespace FavoriteMovies
{
	public partial class SideMenuController : BaseController
	{
		
		UIImageView userProfileImage;
		UIImagePickerController imagePicker;
		public static UIButton title;
		public static UIButton location;
		public static UIImage signUpImage;
		UILabel loading;

		public SideMenuController() : base()
		{
			//signUpImage = UIImage.FromBundle ("124817-matte-white-square-icon-business-signature1.png");
			signUpImage = UIImage.FromBundle ("1481507483_compose.png");
			loading = new UILabel () { Frame = new CoreGraphics.CGRect () { X = 55, Y = 55, Width = 100, Height = 100 } };
			loading.Text = "Loading..";
			loading.TextColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
		}

		void HandleAction ()
		{
			imagePicker = new UIImagePickerController ();
			imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes (UIImagePickerControllerSourceType.PhotoLibrary);
			imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
			imagePicker.Canceled += Handle_Canceled;
			NavController.PresentModalViewController (imagePicker, true);

		}
		[Export ("CurrentUserSetNotificationReceived:")]
		public void CurrentUserSetNotificationReceived (NSNotification n)
		{
			InvokeOnMainThread (() => {
                title.SetTitle (ColorExtensions.CurrentUser.username, UIControlState.Normal);
                var locationIcon = new UIImageView () { Image = UIImage.FromBundle ("location-100.png") };
                if (ColorExtensions.CurrentUser.city != null) {

                    locationIcon.Frame = new CGRect () { X = 110, Y = 243, Width = 10, Height = 10 };
                    location = new UIButton (new RectangleF (20, 220, 192, 20));
                    location.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 13);
                    //title.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
                    location.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
                    location.SetTitleColor (UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f), UIControlState.Normal);
                    location.SetTitle (ColorExtensions.CurrentUser.city + " " + ColorExtensions.CurrentUser.state + " " + ColorExtensions.CurrentUser.country, UIControlState.Normal);
                    location.TouchUpInside += (sender, e) => {
                        NavController.PopToRootViewController (false);
                        SidebarController.CloseMenu ();
                    };
                    View.Add (location);
                    View.Add (locationIcon);
                }
            });
		}

		public override async void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			userProfileImage = new UIImageView ();
			userProfileImage.Image = signUpImage;

			if (ColorExtensions.CurrentUser.Id != null)
				signUpImage = await BlobUpload.getProfileImage (ColorExtensions.CurrentUser.Id);
			if (signUpImage != null)
				userProfileImage.Image = signUpImage;
			userProfileImage.BackgroundColor = UIColor.Clear;
			userProfileImage.Frame = new RectangleF (30, 40, 150, 150);
			userProfileImage.ContentMode = UIViewContentMode.ScaleAspectFill;
			//userProfileImage.Layer.BorderWidth = 2;
			userProfileImage.Layer.CornerRadius = userProfileImage.Frame.Size.Width / 2;
			userProfileImage.Layer.MasksToBounds = true;

			var profileImageTapGestureRecognizer = new UITapGestureRecognizer ();
			userProfileImage.UserInteractionEnabled = true;
			profileImageTapGestureRecognizer.AddTarget (() => { HandleAction (); });
			userProfileImage.AddGestureRecognizer (profileImageTapGestureRecognizer);
			View.Add (userProfileImage);
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			View.Add (loading);

			var name = new NSString (Constants.CurrentUserSetNotification);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new Selector (Constants.CurrentUserSetNotificationReceived), name, null);

			title = new UIButton (new RectangleF (10, 200, 192, 20));
			title.Font = UIFont.FromName (ColorExtensions.TITLE_FONT, 20);
			////title.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			title.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
			//title.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, 15);
			title.SetTitleColor (UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f), UIControlState.Normal);
			title.SetTitle (ColorExtensions.CurrentUser.username, UIControlState.Normal);
			//title.Lines = 2;
			title.TouchUpInside += (sender, e) => {
				NavController.PopToRootViewController (false);
				SidebarController.CloseMenu ();
			};
			var locationIcon = new UIImageView () { Image = UIImage.FromBundle ("location-100.png") };
			if (ColorExtensions.CurrentUser.city != null) 
			{
				
				locationIcon.Frame = new CGRect () { X = 100, Y = 243, Width = 10, Height = 10 };
				location = new UIButton (new RectangleF (10, 220, 192, 20));
				location.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 13);
				//title.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
				location.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
				location.SetTitleColor (UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f), UIControlState.Normal);
				location.SetTitle (ColorExtensions.CurrentUser.city + " " + ColorExtensions.CurrentUser.state + " " + ColorExtensions.CurrentUser.country, UIControlState.Normal);
				location.TouchUpInside += (sender, e) => 
				{
					NavController.PopToRootViewController (false);
					SidebarController.CloseMenu ();
				};
			}
			UIImageView profileImage = new UIImageView () { Image = UIImage.FromBundle ("1481450570_05-myhouse.png") };
			profileImage.Frame = new RectangleF (50, 275, 25, 30);
			var introButton = new UIButton (UIButtonType.System);
			introButton.Frame = new RectangleF (80, 282, 230, 20);
			introButton.SetTitle ("Movies", UIControlState.Normal);
			introButton.SetTitleColor (UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f), UIControlState.Normal);
			introButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			introButton.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 16);
			introButton.TouchUpInside += (sender, e) => {
				NavController.PopToRootViewController (false);
				SidebarController.CloseMenu ();
			};
			//UIImageView customImage = new UIImageView () { Image = UIImage.FromBundle ("1481443482_document.png") };
			//customImage.Frame = new RectangleF (40, 315, 40, 40);
			//var contentButton = new UIButton (UIButtonType.System);
			//contentButton.Frame = new RectangleF (80, 330, 230, 20);
			//contentButton.SetTitle ("Notifications", UIControlState.Normal);
			//contentButton.SetTitleColor (UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f), UIControlState.Normal);
			//contentButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			//contentButton.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 16);

			//contentButton.TouchUpInside += (sender, e) => 
			//{
				
			//	NavController.PushViewController (new NotificationsViewController (), true);

			//	SidebarController.CloseMenu ();
			//};

			//UIImageView friendsImage = new UIImageView () { Image = UIImage.FromBundle ("1481444239_AddFriends") };
			//friendsImage.Frame = new RectangleF (60, 355, 40, 40);
			//var connectionsButton = new UIButton (UIButtonType.System);
			//connectionsButton.Frame = new RectangleF (100, 370, 230, 20);
			//connectionsButton.SetTitle ("Notifications", UIControlState.Normal);
			//connectionsButton.SetTitleColor (UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f), UIControlState.Normal);
			//connectionsButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			//connectionsButton.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 16);
			//connectionsButton.TouchUpInside += (sender, e) => {
			//	NavController.PushViewController (new NotificationsViewController (), true);
			//	SidebarController.CloseMenu ();
			//};
			UIImageView settingsImage = new UIImageView () { Image = UIImage.FromBundle ("1481445346_tools.png") };
			settingsImage.Frame = new RectangleF (40, 355, 40, 40);
			var SettingsButton = new UIButton (UIButtonType.System);
			SettingsButton.Frame = new RectangleF (80, 370, 230, 20);
			SettingsButton.SetTitle ("Profile", UIControlState.Normal);
			SettingsButton.SetTitleColor (UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f), UIControlState.Normal);
			SettingsButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			SettingsButton.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 16);
			SettingsButton.TouchUpInside += (sender, e) => 
			{
				NavController.PushViewController (new SettingsViewController (), true);
				SidebarController.CloseMenu ();
			};


			UIImageView showTipsImage = new UIImageView () { Image = UIImage.FromBundle ("tips.png") };
			showTipsImage.Frame = new RectangleF (40, 395, 40, 40);
			var showTipsButton = new UIButton (UIButtonType.System);
			showTipsButton.Frame = new RectangleF (80, 410, 230, 20);
			showTipsButton.SetTitle ("Tips", UIControlState.Normal);
			showTipsButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			showTipsButton.SetTitleColor (UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f), UIControlState.Normal);
			showTipsButton.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 16);
			showTipsButton.TouchUpInside += (sender, e) => 
			{
				NavController.PushViewController (new TipsViewController (), true);
				SidebarController.CloseMenu ();
			};
			UIImageView logOut = new UIImageView () { Image = UIImage.FromBundle ("ic_exit_to_app_3x.png") };
			logOut.Frame = new RectangleF (50, 435, 30, 30);
			var logoutButton = new UIButton (UIButtonType.System);
			logoutButton.Frame = new RectangleF (80, 450, 230, 20);
			logoutButton.SetTitle ("Sign Out", UIControlState.Normal);
			logoutButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			logoutButton.SetTitleColor (UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f), UIControlState.Normal);
			logoutButton.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 16);
			logoutButton.TouchUpInside += (sender, e) => {
				//NavController.PushViewController(new ContentController(), false);
				//NavController.PushViewController (new MovieListPickerViewController (null, false), false);
				//SidebarController.ChangeContentView (new MovieListPickerViewController (null, true));
				LoginScreenControl<CredentialsProvider>.Activate (this);
			};

			View.Add (title);
			if(location!=null)
			   View.Add (location);
			//View.Add (introButton);
		//	View.Add (contentButton);
		//	View.Add (connectionsButton);
			//View.Add (SettingsButton);
			//View.Add (profileImage);
		//	View.Add (customImage);
		//	View.Add (friendsImage);
			//View.Add (settingsImage);
			if(locationIcon!=null)
			  View.Add (locationIcon);

			//View.Add (showTipsImage);
			//View.Add (showTipsButton);
			//View.Add (logoutButton);
			//View.Add (logOut);
			loading.RemoveFromSuperview ();

		}

		internal static void ShowTipsController ()
		{
			(UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.NavController.NavigationItem.BackBarButtonItem = new UIBarButtonItem ("Skip",UIBarButtonItemStyle.Plain, null);
			(UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.NavController.PushViewController (new TipsViewController (), false);
			(UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.SidebarController.CloseMenu ();
		}

		void Handle_Canceled (object sender, EventArgs e)
		{
			imagePicker.DismissModalViewController (true);
		}

		 void Handle_FinishedPickingMedia (object sender, UIImagePickerMediaPickedEventArgs e)
		{
			// determine what was selected, video or image
			UIImage originalImage = new UIImage();
			bool isImage = false;
			switch (e.Info [UIImagePickerController.MediaType].ToString ()) {
			case "public.image":
				Console.WriteLine ("Image selected");
				isImage = true;
				break;
			case "public.video":
				Console.WriteLine ("Video selected");
				break;
			}

			// get common info (shared between images and video)
			NSUrl referenceURL = e.Info [new NSString ("UIImagePickerControllerReferenceUrl")] as NSUrl;
			if (referenceURL != null)
				Console.WriteLine ("Url:" + referenceURL.ToString ());

			// if it was an image, get the other image info
			if (isImage) {
				// get the original image
				originalImage = e.Info [UIImagePickerController.OriginalImage] as UIImage;
				if (originalImage != null) {
					// do something with the image
					userProfileImage.Image = originalImage;

				}
			} else { // if it's a video
					 // get video url
				NSUrl mediaURL = e.Info [UIImagePickerController.MediaURL] as NSUrl;
				if (mediaURL != null) {
					Console.WriteLine (mediaURL.ToString ());
				}
			}

			try {


				var task = Task.Run (async () => {
					var byteArray = ColorExtensions.ConvertImageToByteArray (ColorExtensions.MaxResizeImage(originalImage,150f,150f));

					await BlobUpload.createContainerAndUpload (byteArray);
				});
				TimeSpan ts = TimeSpan.FromMilliseconds (500);
				task.Wait (ts);
				if (!task.Wait (ts))
					Console.WriteLine ("The timeout interval elapsed uploading Profile image.");
			} catch (Exception f) {
				Debug.WriteLine (f.Message);

			}

			// dismiss the picker
			imagePicker.DismissViewController (true, null);

		}

		 void UpdateImage (UIImage image)
		{
			try {
				

				var task = Task.Run (async () => {
					var byteArray = ColorExtensions.ConvertImageToByteArray (userProfileImage.Image);

					await BlobUpload.createContainerAndUpload (byteArray);
				});
				TimeSpan ts = TimeSpan.FromMilliseconds (3000);
				task.Wait (ts);
				if (!task.Wait (ts))
					Console.WriteLine ("The timeout interval elapsed uploading Profile image.");
			} catch (Exception e) {
				Debug.WriteLine (e.Message);

			}

		}


}


}

