using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using MovieFriends;
using UIKit;

namespace FavoriteMovies
{

	public class NewsFeedViewController : UIViewController
	{
		void HandleAction ()
		{

		}

		List<FeedItem> tableItems = new List<FeedItem> ();
		UITableView table;
		NewsFeedTableSource tableSource;
		UIBarButtonItem add;
		public AzurePostService postService;

		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			postService = AzurePostService.DefaultService;
			await postService.InitializeStoreAsync ();


			var task = Task.Run (async () => {
				tableItems = MovieNewsFeed.GetFeedItems ();
			});
			TimeSpan ts = TimeSpan.FromMilliseconds (3000);
			task.Wait (ts);
			if (!task.Wait (ts))
				Console.WriteLine ("The timeout interval elapsed.");

			table = new UITableView (View.Bounds);
			table.AutoresizingMask = UIViewAutoresizing.All;
			tableSource = new NewsFeedTableSource (tableItems, this);
			table.Source = tableSource;
			table.AllowsSelectionDuringEditing = true;



			add = new UIBarButtonItem (UIBarButtonSystemItem.Add, (s, e) => {

				var newPost = new UINavigationController (new NewFeedPostViewController ());

				newPost.View.Frame = new CGRect () { X = 10, Y = 15, Width = 300, Height = 300 };
				NavigationController.PresentViewController (newPost, true, null);
			});


			NavigationItem.RightBarButtonItem = add;


			View.Add (table);
		}


	}

	public class NewFeedPostViewController : UIViewController
	{
		UIImagePickerController imagePicker;
		UITextView comment;
		UIBarButtonItem close;
		private AzurePostService postService;



		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			postService = AzurePostService.DefaultService;
			await postService.InitializeStoreAsync ();

			View.BackgroundColor = UIColor.White;

			close = new UIBarButtonItem (UIBarButtonSystemItem.Cancel, (s, e) => {

				this.DismissViewController (true, null);
			});


			comment = new UITextView (new RectangleF (20, 0, 270, 400));
			comment.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, 15);
			comment.TintColor = UIColor.Black;
			//comment.BackgroundColor = UIColor.Gray;
			//comment.ReturnKeyType = UIReturnKeyType.Done;
			comment.BecomeFirstResponder ();

			NavigationController.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			NavigationController.NavigationBar.TintColor = UIColor.White;
			NavigationController.NavigationBar.Translucent = true;
			NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.White
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
			var icon = UIImage.FromBundle ("432920-200(1).png");
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



			NavigationItem.Title = "Post to Feed";
			NavigationItem.RightBarButtonItem = close;
			NavigationItem.LeftBarButtonItem = post;

			//this.SetToolbarItems(toolbarItems, true);
			//NavigationController.ToolbarHidden = false;
			View.Add (comment);


		}

		private async Task RefreshAsync ()
		{
			

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
			NSUrl referenceURL = e.Info [new NSString ("UIImagePickerControllerReferenceUrl")] as NSUrl;
			if (referenceURL != null)
				Console.WriteLine ("Url:" + referenceURL.ToString ());

			// if it was an image, get the other image info
			if (isImage) {
				// get the original image
				UIImage originalImage = e.Info [UIImagePickerController.OriginalImage] as UIImage;
				if (originalImage != null) {
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
			} else { // if it's a video
					 // get video url
				NSUrl mediaURL = e.Info [UIImagePickerController.MediaURL] as NSUrl;
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
			CGRect rect = new CGRect () { X = 0, Y = 0, Width = imageSize.Width * scalingFactor, Height = imageSize.Height * scalingFactor };

			return rect;
		}
	}
	public class NewsFeedTableSource : UITableViewSource
	{
		List<FeedItem> tableItems;
		NewsFeedViewController newsFeedViewController;
		UINavigationBar nvBar;
		public NewsFeedTableSource (List<FeedItem> tableItems, NewsFeedViewController newsFeedViewController)
		{
			this.tableItems = tableItems;
			this.newsFeedViewController = newsFeedViewController;

		}
		public static void HideTabBar (UIViewController tab)
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
					view.BackgroundColor = UIColor.Black;
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
				HideTabBar ((UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.TabController);
				newsFeedViewController.NavigationController.SetNavigationBarHidden (true, true);

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
			if (tableItems [(int)cell.Tag].like == "Like") {
				liked = true;
				tableItems [(int)cell.Tag].like = "Unlike";
			} else {
				tableItems [(int)cell.Tag].like = "Like";

			}

			cell.likeLabel.Text = tableItems [(int)cell.Tag].like;

			if (liked)
				tableItems [(int)cell.Tag].likes++;
			else if (tableItems [(int)cell.Tag].likes > 0)
				tableItems [(int)cell.Tag].likes--;

			if (tableItems [(int)cell.Tag].likes > 0)
				cell.numberLikes.Text = tableItems [(int)cell.Tag].likes == 1 ? tableItems [(int)cell.Tag].likes + " Like" : tableItems [(int)cell.Tag].likes + " Likes";
			else
				cell.numberLikes.Text = "";

			var postItem = new Post ();
			postItem.FeedId = tableItems [(int)cell.Tag].id.ToString();
			postItem.Like = tableItems [(int)cell.Tag].like;
			postItem.UserId = "1";

			await newsFeedViewController.postService.InsertPostItemAsync (postItem);


		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			const string CellIdentifier = @"CardCell";
			var cell = (MDCard)tableView.DequeueReusableCell (CellIdentifier);

			if (cell == null)
				cell = new MDCard (UITableViewCellStyle.Default, CellIdentifier);

			cell.Tag = indexPath.Row;
			var likepress = new UITapGestureRecognizer ();
			likepress.AddTarget ((obj) => HandleAction (cell));
			cell.likeButton.AddGestureRecognizer (likepress);
			cell.likeButton.UserInteractionEnabled = true;

			Console.WriteLine (tableItems [indexPath.Row].Title);
			cell.profileImage.Image = MovieCell.GetImageUrl (tableItems [indexPath.Row].ImageLink);
			//var backGroundColor = MovieDetailViewController.averageColor (cell.profileImage.Image);
			cell.titleLabel.Text = tableItems [indexPath.Row].Title;
			//cell.titleLabel.TextColor = MovieDetailViewController.IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
			cell.titleLabel.TextColor = UIColor.Black;
			cell.nameLabel.Text = tableItems [indexPath.Row].Creator;
			//cell.nameLabel.TextColor = MovieDetailViewController.IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
			cell.nameLabel.TextColor = UIColor.Black;
			cell.descriptionLabel.Text = tableItems [indexPath.Row].Description;
			//cell.descriptionLabel.TextColor = MovieDetailViewController.IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
			cell.descriptionLabel.TextColor = UIColor.Black;
			cell.likeLabel.Text = tableItems [indexPath.Row].like;
			cell.cardView.BackgroundColor = UIColor.White;
			cell.BackgroundColor = UIColor.White;
			// My father is an English teacher
			if (tableItems [(int)cell.Tag].likes > 0)
				cell.numberLikes.Text = tableItems [(int)cell.Tag].likes == 1 ? tableItems [(int)cell.Tag].likes + " Like" : tableItems [(int)cell.Tag].likes + " Likes";
			else
				cell.numberLikes.Text = "";


			return cell;
		}
	}
}

