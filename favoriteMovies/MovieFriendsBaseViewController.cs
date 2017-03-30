using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using MovieFriends;
using UIKit;

namespace FavoriteMovies
{
	public class MovieFriendsBaseViewController:BaseController
	{
		public UITableView table;
		protected List<UserFriendsCloud> friendsList;
		protected UITableViewSource tableSource;
		const string movieFriends = "Movie Friends";
		const string findFriends = "Find Friends";


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			table = new UITableView (View.Bounds);
			table.AutoresizingMask = UIViewAutoresizing.All;
			table.RowHeight = 55;
			table.AllowsSelectionDuringEditing = true;


		}



	}

	public class FriendSearchResultsUpdator : UISearchResultsUpdating
	{
		public event Action<string> UpdateSearchResults = delegate { };

		public override void UpdateSearchResultsForSearchController (UISearchController searchController)
		{
			this.UpdateSearchResults (searchController.SearchBar.Text);

		}
	}
	public class FriendSearchResultsViewController : UITableViewController
	{
		static readonly string friendItemCellId = "friendsCellId";
		public UISearchController searchController;
		public List<UserFriendsCloud> userFriends { get; set; }



		// provide access to the navigation controller to all inheriting controllers
		public UINavigationController NavController {
			get {
				//return (UIApplication.SharedApplication.Delegate as AppDelegate).NavController;
				return (UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.NavController;
			}
		}
		[Export ("scrollViewWillEndDragging:withVelocity:targetContentOffset:")]
		public void WillEndDragging (UIScrollView scrollView, CGPoint velocity, ref CGPoint targetContentOffset)
		{
			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).ResignFirstResponder ();
		}

		[Export ("searchBarCancelButtonClicked:")]
		public void searchBarCancelButtonClicked (UISearchBar searchBar)
		{
			Console.WriteLine ("The default search bar cancel button was tapped.");
			//var size = ((UIScrollView)navigationController.TopViewController.View.Subviews [0]).ContentSize;

			searchBar.ResignFirstResponder ();

			userFriends.Clear ();
			TableView.ReloadData ();
		}
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			//View.Frame = new CGRect () { X = 0, Y = 60, Width = 320, Height = 568 };
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

		}
		public FriendSearchResultsViewController ()
		{
			userFriends = new List<UserFriendsCloud> ();

		}
		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return userFriends.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (friendItemCellId);

			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Value1, friendItemCellId);
			}
			if (userFriends.Count > indexPath.Row) 
			{
				cell.TextLabel.Text = userFriends [indexPath.Row].friendusername;
				cell.TextLabel.Font = UIFont.FromName (ColorExtensions.TITLE_FONT, ColorExtensions.HEADER_FONT_SIZE);
				var profileImage = UIImage.FromBundle ("1481507483_compose.png"); //default image
				cell.ImageView.Image = profileImage;
				Task.Run (async () => 
				{
					profileImage = await BlobUpload.getProfileImage (userFriends [indexPath.Row].friendid, 50,50);
				});
				if (profileImage != null)
					cell.ImageView.Image = profileImage;
			}	
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			try {
				//this.searchController.Active = false;
				var row = userFriends [indexPath.Row];


			} catch (Exception e) {
				Debug.WriteLine (e.Message);
			}
		}

		public async void Search (string forSearchString){
			//var test = NavController.NavigationController.PopViewController (true);
			try {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
				// perform search
				AzureTablesService userFriendsService = AzureTablesService.DefaultService;
				await userFriendsService.InitializeStoreAsync ();

				userFriends = await userFriendsService.GetSearchUserFriends (forSearchString);
				TableView.ReloadData ();
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
				}
				
			 catch (Exception ex) 
			{
				Debug.WriteLine (ex.Message);
			}

		}
	}
	class FriendsTableSource : UITableViewSource
	{
		MovieFriendsBaseViewController movieFriendsBaseViewController;
		List<UserFriendsCloud> userFriends;
		const string  moviefriendsCell = "MovieFriendsCell";
		public FriendsTableSource (List<UserFriendsCloud> userFriends, MovieFriendsBaseViewController movieFriendsBaseViewController)
		{
			this.userFriends = userFriends;
			this.movieFriendsBaseViewController = movieFriendsBaseViewController;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (moviefriendsCell) as UITableViewCell;
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Value1, moviefriendsCell);

			cell.TextLabel.Text = userFriends [indexPath.Row].friendusername;
			var profileImage = UIImage.FromBundle ("1481507483_compose.png"); //default image
			cell.ImageView.Image = profileImage;

			Task.Run (async () => 
				{
					profileImage = await BlobUpload.getProfileImage (userFriends [indexPath.Row].userid, 150, 150);
				});

			if (profileImage != null)
				cell.ImageView.Image = profileImage;

			return cell;
		}
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			//var row = userFriends [indexPath.Row];
			//movieFriendsBaseViewController.NavigationController.PushViewController (new MovieChatViewController (row), true);
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return userFriends.Count;
		}
	}
}
