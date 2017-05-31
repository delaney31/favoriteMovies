using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using AlertView;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using LoginScreen;
using MovieFriends;
using UIKit;

namespace FavoriteMovies
{

    public class NewsFeedViewController : UIViewController
	{

		List<MDCard> tableItems = new List<MDCard> ();
		public UITableView table;
		NewsFeedTableSource tableSource;
		public AzureTablesService postService= AzureTablesService.DefaultService;
        private bool needLogin = true;

        public override  void ViewDidAppear (bool animated)
        {

            base.ViewDidAppear (animated);
			tableSource = new NewsFeedTableSource (tableItems, this);
			table.Source = tableSource;
			NavigationItem.Title = "Movie Posts";
			View.Add (table);

			
		}

         bool CheckForNewPosts ()
        {
            List<MDCard> items = new List<MDCard> ();
            var ret = false;
           

            try {
                items = MovieNewsFeedService.GetMDCardItems ();
                if (items.Count > 0 && tableItems.Count > 0) {
                    if (items [0].titleLabel == tableItems [0].titleLabel)
                        ret = true; ;
                }

            } catch (Exception e) {
                //first time in no favorites yet.
                Debug.Write (e.Message);
                ret = false;
            }
     

            return ret;
        }

        public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear (animated);
			needLogin = ColorExtensions.CurrentUser.username == null;

			if (needLogin) {
				LoginScreenControl<CredentialsProvider>.Activate (this);
				needLogin = false;

				SideMenuController.ShowTipsController ();
			}
            if (tableItems.Count == 0) 
            {
                tableItems = MovieNewsFeedService.GetMDCardItems ();
                MBHUDView.HudWithBody (
                   body: "Loading",
                   aType: MBAlertViewHUDType.ActivityIndicator,
                   delay: 5.0f,
                   showNow: true
                   );
               
            }

		}


		public override  void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			table = new UITableView (View.Bounds);
			View.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			table.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			table.AutoresizingMask = UIViewAutoresizing.All;
			table.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			table.AllowsSelectionDuringEditing = true;
			
		}


	}

	public class NewFeedPostViewController : UIViewController
	{
		UIImagePickerController imagePicker;
		UITextView comment;
		UIBarButtonItem close;



		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
            
			View.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);

			close = new UIBarButtonItem (UIBarButtonSystemItem.Cancel, (s, e) => {

				this.DismissViewController (true, null);
			});
          

			comment = new UITextView (new RectangleF (20, 0, 270, 400));
			comment.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 15);
			comment.TintColor = UIColor.Black;
			//comment.BackgroundColor = UIColor.Gray;
			//comment.ReturnKeyType = UIReturnKeyType.Done;
			comment.BecomeFirstResponder ();

			NavigationController.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			NavigationController.NavigationBar.TintColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			NavigationController.NavigationBar.Translucent = true;
			NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f)
			};


			var image = new UIBarButtonItem (UIBarButtonSystemItem.Camera, (s, e) => {

				imagePicker = new UIImagePickerController ();
				imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
				imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes (UIImagePickerControllerSourceType.PhotoLibrary);
				imagePicker.FinishedPickingMedia += Handle_FinishedPickingMedia;
				imagePicker.Canceled += Handle_Canceled;
				NavigationController.PresentModalViewController (imagePicker, true);
			});


			var post = new UIBarButtonItem ("Post", UIBarButtonItemStyle.Done, (s, e) => {


			});

			var barButton = new UIBarButtonItem (UIBarButtonSystemItem.Done, (sender, e) =>
			// var barButton = new UIBarButtonItem ("Dismiss",UIBarButtonItemStyle.Plain, (sender, e) => 
			//var barButton = new UIBarButtonItem (icon,UIBarButtonItemStyle.Plain, (sender, e) => 
			{
				comment.ResignFirstResponder ();
			});
			var toolbarItems = new UIBarButtonItem [] { image, new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace), barButton };
			UIToolbar toolbar = new UIToolbar () { Frame = new CGRect () { X = 0, Y = 0, Width = View.Frame.Size.Width, Height = 50 } };
			toolbar.BarStyle = UIBarStyle.Default;
			toolbar.Items = toolbarItems;
			toolbar.SizeToFit ();
			comment.InputAccessoryView = toolbar;



			NavigationItem.Title = "Movie Meme";
			NavigationItem.RightBarButtonItem = close;
			NavigationItem.LeftBarButtonItem = post;

			View.Add (comment);
            this.TabBarItem.Title = "Home";
		}

		void Handle_Canceled (object sender, EventArgs e)
		{
			imagePicker.DismissModalViewController (true);
		}

		void Handle_FinishedPickingMedia (object sender, UIImagePickerMediaPickedEventArgs e)
		{
			// determine what was selected, video or image

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
			var referenceURL = e.Info [new NSString ("UIImagePickerControllerReferenceUrl")] as NSUrl;
			if (referenceURL != null)
				Console.WriteLine ("Url:" + referenceURL.ToString ());

			// if it was an image, get the other image info
			if (isImage) {
				// get the original image
				UIImage originalImage = e.Info [UIImagePickerController.OriginalImage] as UIImage;
				if (originalImage != null) 
				{
					// do something with the image
					Console.WriteLine ("got the original image");
					MFTextAttachment attachImage = new MFTextAttachment ();
					attachImage.Image = originalImage; // display
													   //attachImage.Bounds = new CGRect () {X=50, Y=90,Width = attachImage.Image.Size.Width, Height = attachImage.Image.Size.Height  };

					var initialText = comment.AttributedText;
					var newText = new NSMutableAttributedString (initialText);
					newText.Append (NSAttributedString.CreateFrom (attachImage));
					comment.AttributedText = newText;
					comment.BecomeFirstResponder ();
				}
			} else 
				{ // if it's a video
					 // get video url
				var mediaURL = e.Info [UIImagePickerController.MediaURL] as NSUrl;
				if (mediaURL != null) {
					Console.WriteLine (mediaURL.ToString ());
				}
			}
			// dismiss the picker
			imagePicker.DismissModalViewController (true);
		}

	}

	class MFTextAttachment : NSTextAttachment
	{
		public override CGRect GetAttachmentBounds (NSTextContainer textContainer, CGRect proposedLineFragment, CGPoint glyphPosition, nuint characterIndex)
		{
			nfloat width = proposedLineFragment.Size.Width;

			var height = proposedLineFragment.Size.Height;
			nfloat scalingFactor = 1.0f;

			CGSize imageSize = this.Image.Size;
			if (width < imageSize.Width)
				scalingFactor = width / imageSize.Width;
			var rect = new CGRect () { X = 0, Y = 0, Width = imageSize.Width * scalingFactor, Height = imageSize.Height * scalingFactor };

			return rect;
		}
	}
	public class NewsFeedTableSource : UITableViewSource
	{
		readonly List<MDCard> tableItems;
		NewsFeedViewController newsFeedViewController;

		public NewsFeedTableSource (List<MDCard> tableItems, NewsFeedViewController newsFeedViewController)
		{
			this.tableItems = tableItems;
			this.newsFeedViewController = newsFeedViewController;

		}
		public static void HideTabBar (UIViewController tab,UIColor viewColor)
		{
			var screenRect = UIScreen.MainScreen.Bounds;
			nfloat fHeight = screenRect.Height;
			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft
			   || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight) {
				fHeight = screenRect.Width;
			}

			UIView.BeginAnimations (null);
			UIView.SetAnimationDuration (0.4);
			foreach (UIView view in tab.View.Subviews) {
				if (view is UITabBar) {
					view.Frame = new CGRect (view.Frame.X, fHeight, view.Frame.Width, view.Frame.Height);
				} else {
					view.Frame = new CGRect (view.Frame.X, view.Frame.Y, view.Frame.Width, fHeight);
					view.BackgroundColor = viewColor;
				}
			}
			UIView.CommitAnimations ();
		}

		public static void ShowTabBar (UIViewController tab)
		{
			
			var screenRect = UIScreen.MainScreen.Bounds;
			
			nfloat fHeight = screenRect.Height - 49f;
			if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft
			   || UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight) {
				fHeight = screenRect.Width - 49f;
			}

			UIView.BeginAnimations (null);
			UIView.SetAnimationDuration (0.4);
			foreach (UIView view in tab.View.Subviews) {
				if (view is UITabBar) {
					view.Frame = new CGRect (view.Frame.X, fHeight, view.Frame.Width, view.Frame.Height);
				} else {
					view.Frame = new CGRect (view.Frame.X, view.Frame.Y, view.Frame.Width, fHeight);
				}
			}
			UIView.CommitAnimations ();
		}

		public override void WillEndDragging (UIScrollView scrollView, CGPoint velocity, ref CGPoint targetContentOffset)
		{
			var targetPoint = targetContentOffset;
			var currentPoint = scrollView.ContentOffset;


			if (targetPoint.Y > currentPoint.Y) {
				HideTabBar ((UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.TabController,newsFeedViewController.View.BackgroundColor);
				newsFeedViewController.NavigationController.SetNavigationBarHidden (true, true);
				var statusView = new UIView () { Frame = UIApplication.SharedApplication.StatusBarFrame };
				statusView.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
				newsFeedViewController.View.Add (statusView);
			} else {
				ShowTabBar ((UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.TabController);
				newsFeedViewController.NavigationController.SetNavigationBarHidden (false, true);
			}

		}
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return tableItems.Count;
		}
		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}
		public override nfloat GetHeightForRow (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			//return base.GetHeightForRow (tableView, indexPath);
			return 350;
		}
		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			var webView = new UIWebView () {
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
				BackgroundColor = UIColor.Black,
			};
            webView.AllowsInlineMediaPlayback = true;
            webView.MediaPlaybackRequiresUserAction = true;
			var viewController = new UIViewController ();

			webView.Frame = new CGRect (0, 0, (float)newsFeedViewController.View.Frame.Width, (float)newsFeedViewController.View.Frame.Height);
			//webView.LoadHtmlString (sb.ToString (), null);
			webView.LoadRequest (new NSUrlRequest (new NSUrl (tableItems [indexPath.Row].Link)));
			viewController.View.Add (webView);
			//this.View.AddSubview (webView);
			newsFeedViewController.NavigationController.PushViewController (viewController, true);
			Console.WriteLine (newsFeedViewController.NavigationController.ViewControllers.Length);

			ShowTabBar ((UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.TabController);
			newsFeedViewController.NavigationController.SetNavigationBarHidden (false, true);
		}
		async void  HandleAction (MDCard cell)
		{
			bool liked = false;

			if (tableItems [(int)cell.Tag].likeLabel.Text == "Like") {
				liked = true;
				tableItems [(int)cell.Tag].likeLabel.Text = "Unlike";
			} else {
				tableItems [(int)cell.Tag].likeLabel.Text = "Like";

			}

			cell.likeLabel.Text = tableItems [(int)cell.Tag].likeLabel.Text;

			if (liked)
				tableItems [(int)cell.Tag].likes++;
			else if (tableItems [(int)cell.Tag].likes > 0)
				tableItems [(int)cell.Tag].likes--;

			if (tableItems [(int)cell.Tag].likes > 0)
				cell.numberLikes.Text = tableItems [(int)cell.Tag].likes == 1 ? tableItems [(int)cell.Tag].likes + " Like" : tableItems [(int)cell.Tag].likes + " Likes";
			else
				cell.numberLikes.Text = "";

			var postItem = new PostItem ();
			postItem.Id = tableItems [(int)cell.Tag].id;
			postItem.Title = tableItems [(int)cell.Tag].titleLabel.Text;
			postItem.Like = tableItems [(int)cell.Tag].likeLabel.Text;
			postItem.UserId = ColorExtensions.CurrentUser.Id;
			postItem.Likes = tableItems [(int)cell.Tag].likes;
			await newsFeedViewController.postService.RefreshDataAsync (postItem);

			//cell.likeButton.Image = tableItems [(int)cell.Tag].like == "Unlike" ? UIImage.FromBundle ("unlike.png") : cell.likeButton.Image = UIImage.FromBundle ("like.png");

		}


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableItems [indexPath.Row];

			try {
                //const string CellIdentifier = @"CardCell";
                //var cell = (MDCard)tableView.DequeueReusableCell (CellIdentifier);

                cell.Tag = indexPath.Row;
                var likepress = new UITapGestureRecognizer ();
                likepress.AddTarget ((obj) => HandleAction (cell));
                cell.likeLabel.AddGestureRecognizer (likepress);
                cell.likeLabel.UserInteractionEnabled = true;


                //Console.WriteLine (tableItems [indexPath.Row].Title);
                //cell.profileImage.Image = MovieCell.GetImageUrl (tableItems [indexPath.Row].ImageLink);
                var backGroundColor = MovieDetailViewController.averageColor (cell.profileImage.Image);
                //cell.titleLabel.Text = tableItems [indexPath.Row].Title;
                cell.titleLabel.TextColor = MovieDetailViewController.IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
                //cell.titleLabel.TextColor = UIColor.Black;
                cell.nameLabel.Text = tableItems [indexPath.Row].Creator;
                cell.nameLabel.TextColor = MovieDetailViewController.IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
                //cell.nameLabel.TextColor = UIColor.Black;
                //cell.descriptionLabel.Text = tableItems [indexPath.Row].Description;
                cell.descriptionLabel.TextColor = MovieDetailViewController.IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
                //cell.descriptionLabel.TextColor = UIColor.Black;
                //cell.likeLabel.Text = tableItems [indexPath.Row].like;
                cell.cardView.BackgroundColor = backGroundColor;
                cell.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
                // My father is an English teacher
                if (tableItems [(int)cell.Tag].likes > 0)
                    cell.numberLikes.Text = tableItems [(int)cell.Tag].likes == 1 ? tableItems [(int)cell.Tag].likes + " Like" : tableItems [(int)cell.Tag].likes + " Likes";
                else
                    cell.numberLikes.Text = "";
                cell.likeLabel.BackgroundColor = backGroundColor;
                //cell.likeButton.BackgroundColor = backGroundColor;
                cell.likeLabel.TextColor = MovieDetailViewController.IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
                cell.numberLikes.TextColor = MovieDetailViewController.IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;

                //cell.likeButton.Image= cell.likeButton.Image.WithColor(MovieDetailViewController.IsDarkColor (backGroundColor) ? UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f) : UIColor.Black).ImageWithRenderingMode (UIImageRenderingMode.AlwaysOriginal);
                return cell;
            }
            catch(Exception ex)
            {
                Debug.WriteLine ("Error in GetCell NewsFeedController: " + ex.Message);
                return cell;
            }

		}
	}
}

