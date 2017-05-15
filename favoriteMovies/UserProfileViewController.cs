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
		UIScrollView scrollView = new UIScrollView ();
		List<CustomListCloud> customLists = new List<CustomListCloud> ();
		UILabel custlistName;
		ContactCard user;
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

			Task.Run (async () =>  {
				customLists = await postService.GetCustomList (user.id);

			}).Wait();

			InvokeOnMainThread (async () => 
			{
				numFollowing.Text = await postService.GetFollowingAsync (user.id);
				numFollowers.Text = await postService.GetFollowersAsync (user.id);
			});
		}
        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);
            NavigationController.NavigationBar.Translucent = true;
        }
		public override  void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Initialize ();
			int cnt = 0;
			CGRect lastLabelFrame = new CGRect ();
			CGRect lastCollectionFrame = new CGRect ();
            nfloat lastViewPostion = 0;
        	
			BTProgressHUD.Dismiss ();
			if (this.profileImage.Image == null) {
				profileImage.Image = UIImage.FromBundle ("blank.png");
			}
			View.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);

			name.Frame = new CGRect () { X = 0, Y = 90, Width = View.Bounds.Width, Height = 120 };
			name.Text = user.nameLabel.Text;
			name.TextAlignment = UITextAlignment.Center;
			name.Font = UIFont.FromName (ColorExtensions.PROFILE_NAME, 20);
			name.TextColor = UIColor.Black;

			followers.Frame = new CGRect () { X = 15, Y = 170, Width = 180, Height = 120 };
			followers.Text = "Follower(s)".ToUpper ();
			followers.TextAlignment = UITextAlignment.Left;
			followers.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 10);
			followers.TextColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);


			numFollowers.Frame = new CGRect () { X = 15, Y = 150, Width = View.Bounds.Width, Height = 120 };
			numFollowers.TextAlignment = UITextAlignment.Left;
			numFollowers.Font = UIFont.FromName (ColorExtensions.PROFILE_NAME, 25);
			numFollowers.TextColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);

			sharedLists.Frame = new CGRect () { X = 0, Y = 170, Width = View.Bounds.Width, Height = 120 };
			sharedLists.Text = "Shared Movie List(s)".ToUpper ();
			sharedLists.TextAlignment = UITextAlignment.Center;
			sharedLists.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 10);
			sharedLists.TextColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);

			numSharedLists.Frame = new CGRect () { X = 0, Y = 150, Width = View.Bounds.Width, Height = 120 };
			numSharedLists.TextAlignment = UITextAlignment.Center;
			numSharedLists.TextColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			numSharedLists.Font = UIFont.FromName (ColorExtensions.PROFILE_NAME, 25);

			following.Frame = new CGRect () { X = 130, Y = 170, Width = 180, Height = 120 };
			following.Text = "Following".ToUpper ();
			following.TextAlignment = UITextAlignment.Right;
			following.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 10);
			following.TextColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);


			numFollowing.Frame = new CGRect () { X = 10, Y = 150, Width = View.Bounds.Width - 25, Height = 120 };
			numFollowing.TextAlignment = UITextAlignment.Right;
			numFollowing.Font = UIFont.FromName (ColorExtensions.PROFILE_NAME, 25);
			numFollowing.TextColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);

			location.Frame = new CGRect () { X = 0, Y = 110, Width = View.Bounds.Width, Height = 120 };
			location.Text = user.location;
			location.TextAlignment = UITextAlignment.Center;
			location.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 10);
			location.TextColor = UIColor.Black;

			background.Frame = new CGRect () { X = 0, Y = 190, Width = View.Bounds.Width, Height = 50 };
			background.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);

			profileImage.Layer.BorderWidth = 3.0f;
			profileImage.BackgroundColor = UIColor.Clear;
			profileImage.Frame = new RectangleF (70, -40, 175, 175);
			profileImage.ContentMode = UIViewContentMode.ScaleAspectFill;
			//userProfileImage.Layer.BorderWidth = 2;
			profileImage.Layer.CornerRadius = profileImage.Frame.Size.Width / 2;
			profileImage.Layer.MasksToBounds = true;
			profileImage.Layer.BorderColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f).CGColor;

            //make followers and following clickable for yourself
            if (user.id == ColorExtensions.CurrentUser.Id)
            {
               

                followers.UserInteractionEnabled = true;
                followers.TextColor = UIColor.FromRGB (74, 212, 231);
				var followersGesture = new UITapGestureRecognizer ();
                followersGesture.AddTarget (() => { GetFollowers (); });
                followers.AddGestureRecognizer (followersGesture);


				following.UserInteractionEnabled = true;
                following.TextColor = UIColor.FromRGB (74, 212, 231);
				var followingGesture = new UITapGestureRecognizer ();
				followingGesture.AddTarget (() => { GetFollowing (); });

				following.AddGestureRecognizer (followingGesture);

			}
			if (customLists.Count > 0) {
				foreach (var list in customLists) 
				{

					Task.Run (async () =>  {
						userMovies = new ObservableCollection<Movie> (await postService.GetCustomListMovies (list.Id));
					}).Wait();


					if (cnt == 0) {

						custlistName = new UILabel () {
							TextColor = ColorExtensions.DarkTheme ? UIColor.White : UIColor.Black, Frame = new CGRect (16, background.Frame.Y + 70, 180, 20),
							//BackgroundColor = View.BackgroundColor,
							Font = UIFont.FromName (ColorExtensions.TITLE_FONT, 15),
							Text = list.Name
						};

					} else {
						custlistName = new UILabel () {
							TextColor = ColorExtensions.DarkTheme ? UIColor.White : UIColor.Black, Frame = new CGRect (16, lastLabelFrame.Y + viewController.CollectionView.Frame.Height + 45, 180, 20),
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
                        viewController.CollectionView.Frame = new CGRect (-35, custlistName.Frame.Y + custlistName.Frame.Height + 5, 375, 205);
                        lastViewPostion += viewController.CollectionView.Frame.Y + viewController.CollectionView.Frame.Height;
                    } else 
                    {
                        viewController.CollectionView.Frame = new CGRect (-35, lastCollectionFrame.Y + lastCollectionFrame.Height + 45, 375, 205);
						lastViewPostion += viewController.CollectionView.Frame.Height;
                    }
                  
					lastCollectionFrame = viewController.CollectionView.Frame;

					scrollView.AddSubview (custlistName);
					scrollView.AddSubview (viewController.CollectionView);
					cnt++;
				}

			}
			numSharedLists.Text = customLists.Count.ToString ();

			//scrollView.SizeToFit ();
			scrollView.Frame = new CGRect () { X = View.Frame.X, Y = 60, Width = View.Frame.Width, Height = View.Frame.Height };
            //For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
           
            scrollView.ContentSize = new CGSize (320, lastCollectionFrame.Height * (userMovies!=null?userMovies.Count: 1)+ 300);
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

        private void GetFollowers ()
        {
			var followr = new FindFriendsViewController ();
			Task.Run (async () => {
					followr.users= await postService.GetFollowersAccountsAsync (user.id);
				}).Wait();

            if (followr.users.Count>0)
			   NavigationController.PushViewController (followr, true);
        }

        private void GetFollowing ()
        {
			var followng= new FindFriendsViewController ();
			Task.Run (async () => {
					followng.users= await postService.GetFollowingAccountAsync (user.id);
				}).Wait();
            if (followng.users.Count>0)
			   NavigationController.PushViewController (followng, true);
     
        }
    }



}


