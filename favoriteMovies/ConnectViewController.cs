using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BigTed;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using MovieFriends;
using SDWebImage;
using ToastIOS;
using UIKit;

namespace FavoriteMovies
{
	public class ConnectViewController:MovieFriendsBaseViewController
	{
		List<ContactCard> tableItems;
		//UIBarButtonItem following;
		const string cellIdentifier = "UserCloudCells";
		List<UserCloud> users;
		public AzureTablesService postService = AzureTablesService.DefaultService;

		public override async void ViewDidLoad ()
		{
			
			base.ViewDidLoad ();
						//BTProgressHUD.Show ();
			tableItems = await GetUserContactsAsync ();
			//Title = "Connect";
			tableSource = new ConnectCloudTableSource (tableItems, this);

			table.Source = tableSource;
			await ((ConnectCloudTableSource)tableSource).updateImages ();
			//tableView.EstimatedRowHeight = 100;
			table.RowHeight = 55;
			table.ContentInset = new UIEdgeInsets (0, 0, 64, 0);
			View.Add (table);
			NavigationController.NavigationBar.Translucent = false;

			// Creates an instance of a custom View Controller that holds the results
			var searchResultsController = new SearchFriendResultsViewController (this);

			//Creates a search controller updater
			var searchUpdater = new SearchResultsUpdator ();
			searchUpdater.UpdateSearchResults += searchResultsController.Search;

			//add the search controller
			var searchController = new UISearchController (searchResultsController) 
			{
				SearchResultsUpdater = searchUpdater,

				WeakDelegate = searchUpdater,
				WeakSearchResultsUpdater = searchUpdater,
			};

			searchResultsController.searchController = searchController;

			//format the search bar
			searchController.SearchBar.SizeToFit ();
			searchController.SearchBar.SearchBarStyle = UISearchBarStyle.Prominent;
			searchController.SearchBar.Placeholder = "Find a friend";

			//searchResultsController.TableView.WeakDelegate = this;
			searchController.SearchBar.WeakDelegate = searchResultsController;

			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).TextColor = UIColor.White;
			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);
			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, BackGroundColorAlpha);
			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).ResignFirstResponder ();
			//the search bar is contained in the navigation bar, so it should be visible
			searchController.HidesNavigationBarDuringPresentation = false;
			NavigationItem.TitleView = searchController.SearchBar;
			//Ensure the searchResultsController is presented in the current View Controller 
			DefinesPresentationContext = true;

			BTProgressHUD.Dismiss ();


		}
		public override void ViewDidAppear (bool animated)
		{
			
			base.ViewDidAppear (animated);
		
		
		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (tableItems == null) // this means async viewdidload  not finished yet
				BTProgressHUD.Show ();
		}

		async Task<List<ContactCard>> GetUserContactsAsync ()
		{
			BTProgressHUD.Show ();
			var watch = System.Diagnostics.Stopwatch.StartNew ();

			const string cellId = "UserContacts";
			List<ContactCard> results = new List<ContactCard> ();

			table.TableHeaderView = new UIView () {
				Frame = new CGRect () { X = 0.0f, Y = 0.0f, Width = View.Layer.Frame.Width, Height = 20f }
			};
			users = await postService.GetUserCloud ();
			foreach (var user in users) 
			{
				if (user.Id != ColorExtensions.CurrentUser.Id)
				{
					var result = new ContactCard (UITableViewCellStyle.Default, cellId);
					result.nameLabel.Text = user.username;
					result.connection = user.connection;
					result.id = user.Id;
					result.moviesInCommon = await postService.MoviesInCommon (ColorExtensions.CurrentUser, user);
					result.location = user.City + " " + user.State + " " + user.Country;
					results.Add (result);
					Console.WriteLine (user.Id	);
				}

			}
			watch.Stop ();
			Console.WriteLine("GetUserContactsAsync Method took " + watch.ElapsedMilliseconds/ 1000.0 + " seconds") ;


			return results.OrderByDescending (x => x.moviesInCommon).ToList ();

		}
	}

	public class ConnectCloudTableSource : UITableViewSource
	{
		List<ContactCard> listItems;
		ConnectViewController controller;

		public ConnectCloudTableSource (List<ContactCard> items, ConnectViewController cont)
		{
			this.listItems = items;
			this.controller = cont;

		}

		public async Task updateImages ()
		{
			foreach (var user in listItems) 
			{
				
				user.profileImage.Image = await BlobUpload.getProfileImage (user.id, 150, 150);
					
			}

		}


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			
			var cell = listItems [indexPath.Row];
			if (cell.connection)
				cell.addRemove.Image = UIImage.FromBundle ("ic_remove_circle_outline.png");
			else
				cell.addRemove.Image = UIImage.FromBundle ("follow.png");
			cell.id = listItems [indexPath.Row].id;
			var tapGesture = new UITapGestureRecognizer ();
			if (!cell.connection) 
			{
				
				tapGesture.AddTarget (() =>
				{
					bool inserted= false;
					var userfriend = new UserFriendsCloud ();
					userfriend.userid = ColorExtensions.CurrentUser.Id;
					userfriend.friendid = cell.id;
					userfriend.friendusername = cell.nameLabel.Text;
					InvokeOnMainThread (async () => 
					{
						inserted = await controller.postService.InsertUserFriendAsync (userfriend);

						if (inserted) 
						{
							listItems [indexPath.Row].connection = true;
							controller.table.ReloadData ();
							BTProgressHUD.ShowToast ("Following " + cell.nameLabel.Text, false);
						}
					});
				});

			} else 
			{
				
				tapGesture.AddTarget(() =>
				{
					
					var userfriend = new UserFriendsCloud ();
					userfriend.userid = ColorExtensions.CurrentUser.Id;
					userfriend.friendid = cell.id;
					userfriend.friendusername = cell.nameLabel.Text;
					bool deleted = false;
					InvokeOnMainThread (async () => 
					{
						bool accepted = await BaseCollectionViewController.ShowAlert ("Confirm", "Are you sure you want to remove this friend?");
					    Console.WriteLine ("Selected button {0}", accepted ? "Accepted" : "Canceled");
						if (accepted) 
						{
							deleted = await controller.postService.DeleteItemAsync (userfriend);
							if (deleted) 
							{
								listItems [indexPath.Row].connection = false;
								controller.table.ReloadData ();
								BTProgressHUD.ShowToast ("Unfollowing " + cell.nameLabel.Text ,false);
							}
						}
					});
				});

			}

			var userProfile = new UITapGestureRecognizer ();
			var profile = new UserProfileViewController (listItems [indexPath.Row]);
			userProfile.AddTarget (() => 
			{
				
				controller.NavigationController.PushViewController (profile,true);

			});
			cell.addRemove.UserInteractionEnabled = true;
			cell.addRemove.AddGestureRecognizer (tapGesture);

			cell.UserInteractionEnabled = true;
			cell.AddGestureRecognizer (userProfile);

			if (cell.moviesInCommon == 0)
				cell.descriptionLabel.Text = listItems [indexPath.Row].location;
			else 
			{
				cell.descriptionLabel.Text = "You have " + cell.moviesInCommon + " movie" + (cell.moviesInCommon==0||cell.moviesInCommon > 1 ?"s":"") + " in common";;
				cell.locationLabel.Text= listItems [indexPath.Row].location;
			}

			return cell;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			
			return listItems.Count;
		}
	}
	public class SearchFriendResultsViewController : UITableViewController
	{
		static readonly string friendItemCellID = "friendItemCellId";
		public UISearchController searchController;
		ConnectViewController connectViewController;

		public ObservableCollection<ContactCard> Friends = new ObservableCollection<ContactCard> ();



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

			Friends.Clear ();
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
		public SearchFriendResultsViewController ()
		{
			Friends = new ObservableCollection<ContactCard> ();

		}

		public SearchFriendResultsViewController (ConnectViewController connectViewController)
		{
			this.connectViewController = connectViewController;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return Friends.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{

			var cell = Friends [indexPath.Row];
			if (cell.connection)
				cell.addRemove.Image = UIImage.FromBundle ("ic_remove_circle_outline.png");
			else
				cell.addRemove.Image = UIImage.FromBundle ("follow.png");
			cell.id = Friends [indexPath.Row].id;
			var tapGesture = new UITapGestureRecognizer ();
			if (!cell.connection) {

				tapGesture.AddTarget (() => {
					bool inserted = false;
					var userfriend = new UserFriendsCloud ();
					userfriend.userid = ColorExtensions.CurrentUser.Id;
					userfriend.friendid = cell.id;
					userfriend.friendusername = cell.nameLabel.Text;
					InvokeOnMainThread (async () => {
						inserted = await connectViewController.postService.InsertUserFriendAsync (userfriend);

						if (inserted) {
							Friends [indexPath.Row].connection = true;
							connectViewController.table.ReloadData ();
							BTProgressHUD.ShowToast ("Following " + cell.nameLabel.Text, false);
						}
					});
				});

			} else {

				tapGesture.AddTarget (() => {

					var userfriend = new UserFriendsCloud ();
					userfriend.userid = ColorExtensions.CurrentUser.Id;
					userfriend.friendid = cell.id;
					userfriend.friendusername = cell.nameLabel.Text;
					bool deleted = false;
					InvokeOnMainThread (async () => {
						bool accepted = await BaseCollectionViewController.ShowAlert ("Confirm", "Are you sure you want to remove this friend?");
						Console.WriteLine ("Selected button {0}", accepted ? "Accepted" : "Canceled");
						if (accepted) {
							deleted = await  connectViewController.postService.DeleteItemAsync (userfriend);
							if (deleted) {
								Friends [indexPath.Row].connection = false;
								connectViewController.table.ReloadData ();
								BTProgressHUD.ShowToast ("Unfollowing " + cell.nameLabel.Text, false);
							}
						}
					});
				});

			}

			var userProfile = new UITapGestureRecognizer ();
			var profile = new UserProfileViewController (Friends [indexPath.Row]);
			userProfile.AddTarget (() => {

				connectViewController.NavigationController.PushViewController (profile, true);

			});
			cell.addRemove.UserInteractionEnabled = true;
			cell.addRemove.AddGestureRecognizer (tapGesture);

			cell.UserInteractionEnabled = true;
			cell.AddGestureRecognizer (userProfile);

			if (cell.moviesInCommon == 0)
				cell.descriptionLabel.Text = Friends [indexPath.Row].location;
			else {
				cell.descriptionLabel.Text = "You have " + cell.moviesInCommon + " movie" + (cell.moviesInCommon == 0 || cell.moviesInCommon > 1 ? "s" : "") + " in common"; 
				cell.locationLabel.Text = Friends [indexPath.Row].location;
			}

			return cell;
		}


		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			try {
				//this.searchController.Active = false;
				var row = Friends [indexPath.Row];


				connectViewController.NavigationController.PushViewController (new UserProfileViewController (row), true);

				//*****this fixes a problem with the uitableview adding space at the top after each selection*****

				Debug.Write (this.TableView.ContentInset);
				this.TableView.ContentInset = new UIEdgeInsets (64, 0, 49, 0);

				//this.searchController.Active = true;
			} catch (Exception e) {
				Debug.WriteLine (e.Message);
			}
		}

		public async void Search (string forSearchString)
		{
			AzureTablesService postService = AzureTablesService.DefaultService;
			try {
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
				// perform search
				if (forSearchString.Length > 0) 
				{
					Friends = await postService.FriendSearch (forSearchString);
					TableView.ReloadData ();
				}
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			} catch (Exception ex) {
				Debug.WriteLine (ex.Message);
			}

		}
	}
}
