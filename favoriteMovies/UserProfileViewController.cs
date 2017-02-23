using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using BigTed;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using MovieFriends;
using SDWebImage;
using UIKit;

namespace FavoriteMovies
{
	public class UserProfileViewController : BaseController
	{

		UIView background = new UIView ();
		UILabel name = new UILabel ();
		UIImageView profileImage = new UIImageView ();
		UILabel location = new UILabel ();
		UILabel followers = new UILabel ();
		UILabel numFollowers = new UILabel ();
		UILabel following = new UILabel ();
		UILabel numFollowing = new UILabel ();
		UILabel sharedLists = new UILabel ();
		UILabel numSharedLists = new UILabel ();
		//UIScrollView scrollView = new UIScrollView ();
		List<CustomListCloud> customLists = new List<CustomListCloud> ();
		UILabel custlistName;
		ContactCard user;
		UIVisualEffectView visualEffectView;
		UserCollectionViewController viewController;
		static CGSize HeaderReferenceSize = new CGSize (50, 50);
		static int MinimumInteritemSpacing = 30;
		static int MinimumLineSpacing = 5;
		static CGSize ItemSize = new CGSize (100, 150);
		ObservableCollection<Movie> userMovies;

		public AzureTablesService postService = AzureTablesService.DefaultService;
		public UserProfileViewController (ContactCard user)
		{
			this.user = user;
			this.profileImage.Image = user.profileImage.Image;

		}

		void Initialize ()
		{

			InvokeOnMainThread (async () => {
				customLists = await postService.GetCustomList (user.id);
				numFollowing.Text = await postService.GetFollowingAsync (user.id);
				numFollowers.Text = await postService.GetFollowersAsync (user.id);
			});


		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Initialize ();

			scrollView.Frame = new CGRect () { X = View.Frame.X, Y = View.Frame.Y, Width = View.Frame.Width, Height = View.Frame.Height };

			BTProgressHUD.Dismiss ();
			if (this.profileImage.Image == null) {
				profileImage.Image = UIImage.FromBundle ("blank.png");
			}
			View.BackgroundColor = UIColor.White;

			name.Frame = new CGRect () { X = 0, Y = 40, Width = View.Bounds.Width, Height = 120 };
			name.Text = user.nameLabel.Text;
			name.TextAlignment = UITextAlignment.Center;
			name.Font = UIFont.FromName (ColorExtensions.PROFILE_NAME, 20);
			name.TextColor = UIColor.Black;

			followers.Frame = new CGRect () { X = 15, Y = 120, Width = View.Bounds.Width, Height = 120 };
			followers.Text = "Followers".ToUpper();
			followers.TextAlignment = UITextAlignment.Left;
			followers.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 10);
			followers.TextColor = UIColor.White;


			numFollowers.Frame = new CGRect () { X = 15, Y = 95, Width = View.Bounds.Width, Height = 120 };
			numFollowers.TextAlignment = UITextAlignment.Left;
			numFollowers.Font = UIFont.FromName (ColorExtensions.PROFILE_NAME, 25);
			numFollowers.TextColor = UIColor.White;

			sharedLists.Frame = new CGRect () { X = 0, Y = 120, Width = View.Bounds.Width, Height = 120 };
			sharedLists.Text = "Movie List(s)".ToUpper();
			sharedLists.TextAlignment = UITextAlignment.Center;
			sharedLists.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 10);
			sharedLists.TextColor = UIColor.White;

			numSharedLists.Frame = new CGRect () { X = 0, Y = 95, Width = View.Bounds.Width, Height = 120 };
			numSharedLists.TextAlignment = UITextAlignment.Center;
			numSharedLists.TextColor = UIColor.White;
			numSharedLists.Font = UIFont.FromName (ColorExtensions.PROFILE_NAME, 25);

			following.Frame = new CGRect () { X = 10, Y = 120, Width = View.Bounds.Width - 25, Height = 120 };
			following.Text = "Following".ToUpper();
			following.TextAlignment = UITextAlignment.Right;
			following.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT,10);
			following.TextColor = UIColor.White;


			numFollowing.Frame = new CGRect () { X = 10, Y = 95, Width = View.Bounds.Width - 25, Height = 120 };
			numFollowing.TextAlignment = UITextAlignment.Right;
			numFollowing.Font = UIFont.FromName (ColorExtensions.PROFILE_NAME, 25);
			numFollowing.TextColor = UIColor.White;

			location.Frame = new CGRect () { X = 0, Y = 60, Width = View.Bounds.Width, Height = 120 };
			location.Text = user.location;
			location.TextAlignment = UITextAlignment.Center;
			location.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 10);
			location.TextColor = UIColor.Black;

			background.Frame = new CGRect () { X = 0, Y = 140, Width = View.Bounds.Width, Height = 50 };
			background.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);

			profileImage.Layer.BorderWidth = 3.0f;
			profileImage.BackgroundColor = UIColor.Clear;
			profileImage.Frame = new RectangleF (100, -40, 120, 120);
			profileImage.ContentMode = UIViewContentMode.ScaleAspectFill;
			//userProfileImage.Layer.BorderWidth = 2;
			profileImage.Layer.CornerRadius = profileImage.Frame.Size.Width / 2;
			profileImage.Layer.MasksToBounds = true;
			profileImage.Layer.BorderColor =UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f).CGColor;


		}


		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);


		}
		public override async void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			MovieDetailViewController.DeleteAllSubviews (scrollView);


			int cnt = 0;
			CGRect lastLabelFrame = new CGRect ();
			CGRect lastCollectionFrame = new CGRect ();
			if (customLists.Count > 0) {
				foreach (var list in customLists) {

					userMovies = new ObservableCollection<Movie> (await postService.GetCustomListMovies (list.Id));



					if (cnt == 0) {

						custlistName = new UILabel () {
							TextColor = UIColor.Black, Frame = new CGRect (16, background.Frame.Y + 70, 180, 20),
							//BackgroundColor = View.BackgroundColor,
							Font = UIFont.FromName (ColorExtensions.TITLE_FONT, 15),
							Text = list.Name
						};

					} else {
						custlistName = new UILabel () {
							TextColor = UIColor.Black, Frame = new CGRect (16, lastLabelFrame.Y + viewController.CollectionView.Frame.Height + 45, 180, 20),
							Font = UIFont.FromName (ColorExtensions.TITLE_FONT, 15),
							Text = list.Name
						};
					}
					lastLabelFrame = custlistName.Frame;
					viewController = new UserCollectionViewController (new UICollectionViewFlowLayout () {
						MinimumInteritemSpacing = MinimumInteritemSpacing, MinimumLineSpacing = MinimumLineSpacing,
						HeaderReferenceSize = HeaderReferenceSize, ItemSize = ItemSize,
						ScrollDirection = UICollectionViewScrollDirection.Horizontal
					}, userMovies, this, user.nameLabel.Text);

					viewController.CollectionView.BackgroundColor = View.BackgroundColor;
					viewController.CollectionView.RegisterClassForCell (typeof (MovieCell), UserCollectionViewController.movieCellId);
					if (cnt == 0) {
						viewController.CollectionView.Frame = new CGRect (-35, custlistName.Frame.Y + custlistName.Frame.Height + 5, 345, 150);

					} else
						viewController.CollectionView.Frame = new CGRect (-35, lastCollectionFrame.Y + lastCollectionFrame.Height + 45, 345, 150);


					lastCollectionFrame = viewController.CollectionView.Frame;
					scrollView.AddSubview (custlistName);
					scrollView.AddSubview (viewController.CollectionView);
					cnt++;
				}

			}


			numSharedLists.Text = customLists.Count.ToString ();

			//scrollView.SizeToFit ();

			//For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
			scrollView.ContentSize = new CGSize (320, lastCollectionFrame.Y + lastCollectionFrame.Height + 120);
			//scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);
			scrollView.Bounces = true;


			scrollView.AddSubview (background);
			scrollView.AddSubview (name);
			scrollView.AddSubview (profileImage);
			scrollView.AddSubview (location);
			scrollView.AddSubview (followers);
			scrollView.AddSubview (following);
			scrollView.AddSubview (numFollowers);
			scrollView.AddSubview (numFollowing);
			scrollView.AddSubview (sharedLists);
			scrollView.AddSubview (numSharedLists);

			View.AddSubview (scrollView);

		}



	}



}


