using System;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using MovieFriends;
using StoreKit;
using UIKit;
using Xamarin.InAppPurchase;

namespace FavoriteMovies
{
	public partial class SettingsViewController : UIViewController, IPurchaseViewController
	{

		public AzureTablesService postService = AzureTablesService.DefaultService;
		public static string ProductId = "com.jadesystemsinc.favoritemovies.productid";
		UIImageView userProfileImage;
		UIImagePickerController imagePicker;
		static UIImage signUpImage;
		InAppPurchaseManager iap;

		bool isEmail;
		public SettingsViewController () : base ("SettingsViewController", null)
		{
			signUpImage = UIImage.FromBundle ("1481507483_compose.png");
			iap = new InAppPurchaseManager ();
            iap.SimulateiTunesAppStore = false;
         
			//iap.SimulatedRestoredPurchaseProducts = "product.nonconsumable,antivirus.nonrenewingsubscription.duration6months,content.nonconsumable.downloadable";
			AttachToPurchaseManager (null, iap);

		}

		public override async void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);


         

			// Perform any additional setup after loading the view, typically from a nib.
			NavigationItem.Title = "Profile";
			NavigationController.NavigationBar.Translucent = true;
			txtLastName.Text = ColorExtensions.CurrentUser.lastname;
			txtFirstName.Text = ColorExtensions.CurrentUser.firstname;
			switchDarktheme.On = ColorExtensions.CurrentUser.darktheme;
			switchDarktheme.ValueChanged += SwitchDarktheme_ValueChanged;
			switchSuggestions.On = ColorExtensions.CurrentUser.suggestmovies;
			switchSuggestions.ValueChanged += SwitchSuggestions_ValueChanged;
			txtPhoneNumber.Text = ColorExtensions.CurrentUser.phone;
			txtEmail.Text = ColorExtensions.CurrentUser.email;
			switchRemoveAds.On = ColorExtensions.CurrentUser.removeAds;
			switchRemoveAds.ValueChanged += SwitchRemoveAds_ValueChanged;
			lblVersion.Text = "Version " + NSBundle.MainBundle.InfoDictionary ["CFBundleShortVersionString"];
			segmentTileSize.SelectedSegment = ColorExtensions.CurrentTileSize;
			segmentTileSize.ValueChanged += SegmentTileSize_ValueChanged;
			userProfileImage = new UIImageView ();
			userProfileImage.Image = signUpImage;

			if (ColorExtensions.CurrentUser.Id != null)
				signUpImage = await BlobUpload.getProfileImage (ColorExtensions.CurrentUser.Id);
			if (signUpImage != null)
				userProfileImage.Image = signUpImage;
			userProfileImage.BackgroundColor = UIColor.Clear;
			userProfileImage.Frame = new RectangleF (105, 65, 120, 120);
			userProfileImage.ContentMode = UIViewContentMode.ScaleAspectFill;
			//userProfileImage.Layer.BorderWidth = 2;
			userProfileImage.Layer.CornerRadius = userProfileImage.Frame.Size.Width / 2;
			userProfileImage.Layer.MasksToBounds = true;

			var profileImageTapGestureRecognizer = new UITapGestureRecognizer ();
			userProfileImage.UserInteractionEnabled = true;
			profileImageTapGestureRecognizer.AddTarget (() => { HandleAction (); });
			userProfileImage.AddGestureRecognizer (profileImageTapGestureRecognizer);
			View.BackgroundColor= UIColor.White;//UIColor.Clear.FromHexString ("#e9755e", 1.0f);

			View.Add (userProfileImage);
         
			// validation
			txtEmail.EditingDidEnd += (object sender, EventArgs e) => 
			{
				
				isEmail = Regex.IsMatch (txtEmail.Text, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

				 if (!isEmail)
					{


						InvokeOnMainThread ( () =>
						{
							txtEmail.Layer.BorderColor =UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f).CGColor;
							txtEmail.Layer.BorderWidth = 4;
							txtEmail.Text = "Invalid Email";
						} );

					}
					else 
					{
						txtEmail.Layer.BorderColor = UIColor.Clear.CGColor;
						txtEmail.Layer.BorderWidth = 0;
					}
				};
		}

		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();

		}
		void Handle_Canceled (object sender, EventArgs e)
		{
			imagePicker.DismissModalViewController (true);
		}

		void Handle_FinishedPickingMedia (object sender, UIImagePickerMediaPickedEventArgs e)
		{
			// determine what was selected, video or image
			UIImage originalImage = new UIImage ();
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
					var byteArray = ColorExtensions.ConvertImageToByteArray (ColorExtensions.MaxResizeImage (originalImage, 150f, 150f));

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
		void HandleAction ()
		{
			imagePicker = new UIImagePickerController ();
			imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
			imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes (UIImagePickerControllerSourceType.PhotoLibrary);
			imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
			imagePicker.Canceled += Handle_Canceled;
			NavigationController.PresentModalViewController (imagePicker, true);
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			var touch = touches.AnyObject as UITouch;

			Console.WriteLine (touch);
			this.View.EndEditing (true);
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		void SwitchRemoveAds_ValueChanged (object sender, EventArgs e)
		{
            switchRemoveAds.ValueChanged -= SwitchRemoveAds_ValueChanged;
			if (switchRemoveAds.Enabled) 
            {
                if (iap.CanMakePayments) 
                {
                    // initiate payment
                    iap.BuyProduct (ProductId);
                  
                }
				else 
                {
					switchRemoveAds.Enabled = false;
				}
			} 
		}

		void SwitchDarktheme_ValueChanged (object sender, EventArgs e)
		{
			ColorExtensions.CurrentUser.darktheme = !ColorExtensions.CurrentUser.darktheme;
			//MainViewController.NewCustomListToRefresh = 1;
		}

		void SwitchSuggestions_ValueChanged (object sender, EventArgs e)
		{
			ColorExtensions.CurrentUser.suggestmovies = !ColorExtensions.CurrentUser.suggestmovies;
			MainViewController.NewCustomListToRefresh = -1;
		}

		void SegmentTileSize_ValueChanged (object sender, EventArgs e)
		{

			ColorExtensions.CurrentTileSize = segmentTileSize.SelectedSegment;
			MainViewController.NewCustomListToRefresh = -1;
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			ColorExtensions.CurrentUser.lastname = txtLastName.Text;
			ColorExtensions.CurrentUser.firstname = txtFirstName.Text;
			ColorExtensions.CurrentUser.phone = txtPhoneNumber.Text;
			if(isEmail)
			   ColorExtensions.CurrentUser.email = txtEmail.Text;
            InsertCurrentUser ();
		}


		void InsertCurrentUser ()
		{
			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					var task = Task.Run (() => {
						// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
						db.DeleteAll<User> ();
						db.InsertOrReplace (ColorExtensions.CurrentUser, typeof (User));


					});
					task.Wait ();



				}
			}
			catch(Exception e)
			{
				Debug.WriteLine (e.Message);
			}
		}



		public void AttachToPurchaseManager (UIStoryboard Storyboard, InAppPurchaseManager purchaseManager)
		{
				iap = purchaseManager;
				iap.InAppProductPurchased += (SKPaymentTransaction transaction, InAppProduct product) => 		
				{
					// Update list to remove any non-consumable products that were
					// purchased
					ColorExtensions.CurrentUser.removeAds = true;
					switchRemoveAds.On = true;
				};

			iap.InAppPurchasesRestored+= (int count) => 		
				{
					// Update list to remove any non-consumable products that were
					// purchased
					iap.RestorePreviousPurchases ();
                    ColorExtensions.CurrentUser.removeAds = false;
                    switchRemoveAds.On = false;
				};

			iap.InAppPurchaseProcessingError += (string message) => 
			{
				Debug.WriteLine (message);
				ColorExtensions.CurrentUser.removeAds = false;
                switchRemoveAds.On = false;
			};
		}


	}
}

