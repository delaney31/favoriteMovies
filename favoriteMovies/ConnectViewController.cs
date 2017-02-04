using System;
using System.Collections.Generic;
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
	public class ConnectViewController:BaseBasicListViewController
	{
		List<ContactCard> tableItems;
		//UIBarButtonItem following;
		const string cellIdentifier = "UserCloudCells";
		List<UserCloud> users;
		public AzureTablesService postService = AzureTablesService.DefaultService;

		public override async void ViewDidLoad ()
		{
			
			base.ViewDidLoad ();

			tableItems = await GetUserContactsAsync ();

			tableSource = new ConnectCloudTableSource (tableItems, this, users, tableView);

			tableView.Source = tableSource;
			await ((ConnectCloudTableSource)tableSource).updateImages ();
			tableView.ContentInset = new UIEdgeInsets (0, 0, 64, 0);
			View.Add (tableView);
			NavigationController.NavigationBar.Translucent = false;

		}
		public override void ViewDidAppear (bool animated)
		{
			
			base.ViewDidAppear (animated);
		
		
		}
		public override async void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (tableItems == null) // this means async viewdidload  not finished yet
				BTProgressHUD.Show ();
			
			if (tableSource != null) 
			{
				await ((ConnectCloudTableSource)tableSource).updateCommonMovies ();

			}

		}

		async Task<List<ContactCard>> GetUserContactsAsync ()
		{
			BTProgressHUD.Show ();
			var watch = System.Diagnostics.Stopwatch.StartNew ();

			const string cellId = "UserContacts";
			List<ContactCard> results = new List<ContactCard> ();

			tableView.TableHeaderView = new UIView () {
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
					results.Add (result);
					Console.WriteLine (user.Id	);
				}

			}
			watch.Stop ();
			Console.WriteLine("GetUserContactsAsync Method took " + watch.ElapsedMilliseconds/ 1000.0 + " seconds") ;

			BTProgressHUD.Dismiss ();
			return results.OrderByDescending (x => x.moviesInCommon).ToList ();

		}
	}

	public class ConnectCloudTableSource : UITableViewSource
	{
		List<ContactCard> listItems;
		ConnectViewController controller;
		AzureTablesService postService = AzureTablesService.DefaultService;
		List<UserCloud> users;
		UITableView tableView;

		public ConnectCloudTableSource (List<ContactCard> items, ConnectViewController cont, List<UserCloud> users)
		{
			this.listItems = items;
			this.controller = cont;
			this.users = users;
		}

		public ConnectCloudTableSource (List<ContactCard> items, ConnectViewController cont, List<UserCloud> users, UITableView tableView) : this (items, cont, users)
		{
			this.tableView = tableView;
		}

		public async Task updateImages ()
		{
			foreach (var user in listItems) 
			{
				
				user.profileImage.Image = await BlobUpload.getProfileImage (user.id, 150, 150);
					
			}

		}
		public async Task  updateCommonMovies ()
		{
			int cnt = 0;
			foreach (var user in listItems) 
			{
				user.moviesInCommon = await postService.MoviesInCommon (ColorExtensions.CurrentUser, users[cnt]);
				cnt++;
			}
			listItems.OrderByDescending (x => x.moviesInCommon).ToList ();


		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			
			var cell = listItems [indexPath.Row];
			if (cell.connection)
				cell.addRemove.Image = UIImage.FromBundle ("unfollow.png");
			else
				cell.addRemove.Image = UIImage.FromBundle ("follow.png");
			cell.id = listItems [indexPath.Row].id;
			if (!cell.connection) 
			{
				var tapGesture = new UITapGestureRecognizer ();
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
						MainViewController.NewCustomListToRefresh = 1;
						if (inserted) 
						{
							listItems [indexPath.Row].connection = true;
							controller.tableView.ReloadData ();
							BTProgressHUD.ShowToast ("Following " + cell.nameLabel.Text, false);
							//Toast.MakeText (cell.nameLabel.Text + " is now your Movie Friend.")
							//.SetUseShadow (true)
							//.SetGravity (ToastGravity.Bottom)
							//.SetCornerRadius (10)
							//.SetDuration (1000)
							//.Show (ToastType.Info);

						}
					});
				});
				cell.AddGestureRecognizer (tapGesture);
			} else 
			{
				var tapGesture = new UITapGestureRecognizer ();
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
								MainViewController.NewCustomListToRefresh = 1;
								listItems [indexPath.Row].connection = false;
								controller.tableView.ReloadData ();
								BTProgressHUD.ShowToast ("UnFollowing " + cell.nameLabel.Text ,false);
								// Toast.MakeText (cell.nameLabel.Text + " is no longer your Movie Friend.")
								//.SetUseShadow (true)
								//.SetGravity (ToastGravity.Bottom)
								//.SetCornerRadius (10)
								//.SetDuration (1000)
								//.Show (ToastType.Info);

							}
						}
					});
				});
				cell.AddGestureRecognizer (tapGesture);
			}

			return cell;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return listItems.Count;
		}
	}

}
