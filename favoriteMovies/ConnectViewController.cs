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
using JSQMessagesViewController;
using MovieFriends;
using ObjCRuntime;
using Plugin.Messaging;
using SDWebImage;
using SQLite;
using ToastIOS;
using UIKit;

namespace FavoriteMovies
{
	public class ConnectViewController:MovieFriendsBaseViewController
	{
		protected List<ContactCard> tableItems;
		//UIBarButtonItem following;
		const string cellIdentifier = "UserCloudCells";
		protected List<UserCloud> users;
		protected List<UserFriendsCloud> friends;
		UILabel loading;
		public AzureTablesService postService = AzureTablesService.DefaultService;
		public ConnectViewController()
		{
			loading = new UILabel () { Frame = new CoreGraphics.CGRect () { X = 115, Y = 155, Width = 100, Height = 100 } };
			loading.Text = "Loading...";
		}
		public override void ViewDidLoad ()
		{
			
			base.ViewDidLoad ();
			View.Add (loading);
			//Title = "Connect";

			//tableView.EstimatedRowHeight = 100;
		
			// Creates an instance of a custom View Controller that holds the results
			var searchResultsController = new SearchFriendResultsViewController (this);

			//Creates a search controller updater
			var searchUpdater = new SearchResultsUpdator ();
			searchUpdater.UpdateSearchResults += searchResultsController.Search;

			//add the search controller
			var searchController = new UISearchController (searchResultsController) {
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
			if (tableItems == null) // this means async viewdidload  not finished yet
				BTProgressHUD.Show ();


		}

		protected async Task<List<ContactCard>> GetUserContactsAsync ()
		{
			//if(tableItems == null)
			 
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
					result.moviesInCommon = await postService.MoviesInCommon (GetAllMoviesInCustomLists(), user.Id);
					result.location = user.city + " " + user.state + " " + user.country;
					results.Add (result);
					Console.WriteLine (user.Id	);
				}

			}

			watch.Stop ();
			Console.WriteLine("GetUserContactsAsync Method took " + watch.ElapsedMilliseconds/ 1000.0 + " seconds") ;

			return results.OrderByDescending (x => x.moviesInCommon).ToList ();

		}

		static List<Movie> GetAllMoviesInCustomLists ()
		{

			var retMovies = new List<Movie> ();
			var customLists = MainViewController.GetCustomLists ();
			foreach(var list in customLists)
			{
				var movies = MainViewController.GetMovieList (list);
				retMovies = retMovies.Concat (movies).ToList ();
			}
			return retMovies;
		}

		protected async Task<List<ContactCard>> GetFriendsContactsAsync ()
		{
			//if (tableItems == null) // this means async viewdidload  not finished yet
				
			var watch = System.Diagnostics.Stopwatch.StartNew ();

			const string cellId = "UserContacts";
			List<ContactCard> results = new List<ContactCard> ();

			table.TableHeaderView = new UIView () {
				Frame = new CGRect () { X = 0.0f, Y = 0.0f, Width = View.Layer.Frame.Width, Height = 20f }
			};
			friends = await postService.GetUserFriends (ColorExtensions.CurrentUser.Id);
			if (friends.Count == 0) 
			{
				friends.Add (new UserFriendsCloud () { friendusername = "          Follow friends and chat with them here.", Id = "0", friendid = "0" });
			}
		
			foreach (var user in friends) 
			{
				ContactCard result;

				result = new ContactCard (UITableViewCellStyle.Default, cellId);
				result.nameLabel.Text = user.friendusername;
				result.connection = null;
				result.id = user.friendid;
				result.moviesInCommon = await postService.MoviesInCommon (GetAllMoviesInCustomLists (), user.Id);
				var location = await postService.GetUser (user.friendid);
				if (location != null) 
				{
					result.location = location.city + " " + location.state + " " + location.country;
				}
				results.Add (result);
				Console.WriteLine (user.Id);


			}
			watch.Stop ();
			Console.WriteLine ("GetUserContactsAsync Method took " + watch.ElapsedMilliseconds / 1000.0 + " seconds");


			return results.OrderByDescending (x => x.moviesInCommon).ToList ();

		}
	
	}

	public class ConnectCloudTableSource : UITableViewSource
	{
		List<ContactCard> listItems;
		ConnectViewController controller;
		List<UserFriendsCloud> friends;
		string cellIdentifier = "ConnectCell";
		public ConnectCloudTableSource (List<ContactCard> items, ConnectViewController cont)
		{
			this.listItems = items;
			this.controller = cont;
		}

		public ConnectCloudTableSource (List<ContactCard> items, ConnectViewController cont, List<UserFriendsCloud> friends) : this (items, cont)
		{
			this.friends = friends;
		}

		public async Task updateImages ()
		{
			//BTProgressHUD.Show ();
			foreach (var user in listItems) 
			{
				
				user.profileImage.Image = await BlobUpload.getProfileImage (user.id, 200, 200);
					
			}
			//BTProgressHUD.Dismiss ();
		}


		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{

			//UITableViewCell cell;


			if (listItems [indexPath.Row].id == "0") {
				var cell = tableView.DequeueReusableCell (cellIdentifier);
				var cellStyle = UITableViewCellStyle.Value2;
				// if there are no cells to reuse, create a new one
				if (cell == null) {
					cell = new UITableViewCell (cellStyle, cellIdentifier);
					cell.DetailTextLabel.Text= listItems [indexPath.Row].nameLabel.Text;
					cell.DetailTextLabel.TextAlignment = UITextAlignment.Justified;
					cell.DetailTextLabel.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);
					cell.DetailTextLabel.TextColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);

				}
				return cell;
			} else 
			{
				var cell = listItems [indexPath.Row];




				//cell.id = listItems [indexPath.Row].id;
				var tapGesture = new UITapGestureRecognizer ();
				if (cell.connection != null) {
					if ((bool)cell.connection)
						cell.addRemove.Image = UIImage.FromBundle ("ic_remove_circle_outline.png");
					else
						cell.addRemove.Image = UIImage.FromBundle ("follow.png");

					if ((bool)!cell.connection) {

						tapGesture.AddTarget (() => {
							bool inserted = false;
							var userfriend = new UserFriendsCloud ();
							userfriend.userid = ColorExtensions.CurrentUser.Id;
							userfriend.friendid = cell.id;
							userfriend.friendusername = cell.nameLabel.Text;
							InvokeOnMainThread (async () => {
								inserted = await controller.postService.InsertUserFriendAsync (userfriend);

								if (inserted) {
									listItems [indexPath.Row].connection = true;
									controller.table.ReloadData ();
									BTProgressHUD.ShowToast ("Following " + cell.nameLabel.Text, false);
									var notification = NSNotification.FromName (Constants.ModifyFollowerNotification, new NSObject ());
									NSNotificationCenter.DefaultCenter.PostNotification (notification);
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
									deleted = await controller.postService.DeleteItemAsync (userfriend);
									if (deleted) {
										listItems [indexPath.Row].connection = false;
										controller.table.ReloadData ();
										BTProgressHUD.ShowToast ("Unfollowing " + cell.nameLabel.Text, false);
										var notification = NSNotification.FromName (Constants.ModifyFollowerNotification, new NSObject ());
										NSNotificationCenter.DefaultCenter.PostNotification (notification);

									}
								}
							});
						});

					}
				} else {
					if (cell.id != "0")
						cell.addRemove.Image = UIImage.FromBundle ("chat.png");

					tapGesture.AddTarget (async () => {
						var row = listItems [indexPath.Row];
						//controller.NavigationController.PushViewController (new MovieChatViewController (row,friends [indexPath.Row]), true);

						//SystemSoundPlayer.PlayMessageSentSound ();

						//var message = new Message (row.id, friends[indexPath.Row].friendusername, NSDate.Now, "");
						//messages.Add (message);
						var smsMessenger = CrossMessaging.Current.SmsMessenger;
						if (smsMessenger.CanSendSms) {
							var user = await controller.postService.GetUser (friends [indexPath.Row].friendid);
							//smsMessenger.SendSms ("+27213894839493", "Well hello there from Xam.Messaging.Plugin");
							smsMessenger.SendSms ("+" + user.phone, "");
						}
						//var emailMessenger = CrossMessaging.Current.EmailMessenger;
						//if (emailMessenger.CanSendEmail) {
						//	// Send simple e-mail to single receiver without attachments, bcc, cc etc.
						//	//emailMessenger.SendEmail ("to.plugins@xamarin.com", "Xamarin Messaging Plugin", "Well hello there from Xam.Messaging.Plugin");
						//	emailMessenger.SendEmail ("tldelaney@gmail.com", "Xamarin Messaging Plugin", "Well hello there from Xam.Messaging.Plugin");
						//	// Alternatively use EmailBuilder fluent interface to construct more complex e-mail with multiple recipients, bcc, attachments etc. 
						//	var email = new EmailMessageBuilder ()
						//	  .To ("tldelaney@gmail.com")
						//	  .Cc ("tldelaney@gmail.com")
						//	  //.Bcc (new [] { "bcc1.plugins@xamarin.com", "bcc2.plugins@xamarin.com" })
						//	  .Subject ("Xamarin Messaging Plugin")
						//	  .Body ("Well hello there from Xam.Messaging.Plugin")
						//	  .Build ();

						//	emailMessenger.SendEmail (email);
						//}
						//FinishSendingMessage (true);

						//await Task.Delay (500);

						//await SimulateDelayedMessageReceived ();
					});

				}

				var userProfile = new UITapGestureRecognizer ();
				var profile = new UserProfileViewController (listItems [indexPath.Row]);

				userProfile.AddTarget (() => {

					controller.NavigationController.PushViewController (profile, true);

				});
				cell.addRemove.UserInteractionEnabled = true;
				cell.addRemove.AddGestureRecognizer (tapGesture);

				cell.UserInteractionEnabled = true;
				cell.AddGestureRecognizer (userProfile);

				if (cell.moviesInCommon == 0)
					cell.descriptionLabel.Text = listItems [indexPath.Row].location;
				else {
					cell.descriptionLabel.Text = "You have " + cell.moviesInCommon + " movie" + (cell.moviesInCommon == 0 || cell.moviesInCommon > 1 ? "s" : "") + " in common"; ;
					cell.locationLabel.Text = listItems [indexPath.Row].location;
				}
				return cell;
			}

		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			
			return listItems.Count;
		}
	}
	public class SearchFriendResultsViewController : UITableViewController
	{
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
			cell.id = Friends [indexPath.Row].id;

			var profileImageTapGesture = new UITapGestureRecognizer ();

			var followTapGesture = new UITapGestureRecognizer ();
			if (cell.connection != null)
			{
				if ((bool)cell.connection)
					cell.addRemove.Image = UIImage.FromBundle ("ic_remove_circle_outline.png");
				else
					cell.addRemove.Image = UIImage.FromBundle ("follow.png");


				if ((bool)!cell.connection) {

					followTapGesture.AddTarget (() => {
						bool inserted = false;
						var userfriend = new UserFriendsCloud ();
						userfriend.userid = ColorExtensions.CurrentUser.Id;
						userfriend.friendid = cell.id;
						userfriend.friendusername = cell.nameLabel.Text;
						InvokeOnMainThread (async () => {
							inserted = await connectViewController.postService.InsertUserFriendAsync (userfriend);

							if (inserted) {
								Friends [indexPath.Row].connection = true;
								BTProgressHUD.ShowToast ("Following " + cell.nameLabel.Text, false);
								var notification = NSNotification.FromName (Constants.ModifyFollowerNotification, new NSObject ());
								NSNotificationCenter.DefaultCenter.PostNotification (notification);
								var listNotification = NSNotification.FromName (Constants.CustomListChange, new NSObject ());
								NSNotificationCenter.DefaultCenter.PostNotification (listNotification);
								tableView.ReloadData ();
							}
						});
					});

				} else {

					followTapGesture.AddTarget (() => {

						var userfriend = new UserFriendsCloud ();
						userfriend.userid = ColorExtensions.CurrentUser.Id;
						userfriend.friendid = cell.id;
						userfriend.friendusername = cell.nameLabel.Text;
						bool deleted = false;
						InvokeOnMainThread (async () => {
							bool accepted = await BaseCollectionViewController.ShowAlert ("Confirm", "Are you sure you want to remove this friend?");
							Console.WriteLine ("Selected button {0}", accepted ? "Accepted" : "Canceled");
							if (accepted) {
								deleted = await connectViewController.postService.DeleteItemAsync (userfriend);
								if (deleted) {
									Friends [indexPath.Row].connection = false;
									BTProgressHUD.ShowToast ("Unfollowing " + cell.nameLabel.Text, false);
									var notification = NSNotification.FromName (Constants.ModifyFollowerNotification, new NSObject ());
									NSNotificationCenter.DefaultCenter.PostNotification (notification);
									var listNotification = NSNotification.FromName (Constants.CustomListChange, new NSObject ());
									NSNotificationCenter.DefaultCenter.PostNotification (listNotification);
									tableView.ReloadData ();
								}
							}
						});
					});

				}
			}

			var userProfile = new UITapGestureRecognizer ();
			var profile = new UserProfileViewController (Friends [indexPath.Row]);
			userProfile.AddTarget (() => {

				connectViewController.NavigationController.PushViewController (profile, true);

			});
			cell.addRemove.UserInteractionEnabled = true;
			cell.addRemove.AddGestureRecognizer (followTapGesture);

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

				if(row.id !="0")
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
