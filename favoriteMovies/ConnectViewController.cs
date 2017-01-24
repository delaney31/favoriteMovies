using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using MovieFriends;
using ToastIOS;
using UIKit;

namespace FavoriteMovies
{
	public class ConnectViewController:BaseBasicListViewController
	{
		List<ContactCard> tableItems;

		const string cellIdentifier = "UserCloudCells";
		public AzureTablesService postService = AzureTablesService.DefaultService;

		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			//InvokeOnMainThread (async () => {
			tableItems = await GetUserContactsAsync ();
			tableSource = new ConnectCloudTableSource (tableItems, this, true);
			tableView.Source = tableSource;
			tableView.TableHeaderView = new UIView () {
				Frame = new CGRect () { X = 0.0f, Y = 0.0f, Width = View.Layer.Frame.Width, Height = 20f }
			};

			View.Add (tableView);
			//});

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewDidAppear (animated);


		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);



		}

		async Task<List<ContactCard>> GetUserContactsAsync ()
		{
			var watch = System.Diagnostics.Stopwatch.StartNew ();

			const string cellId = "UserContacts";
			List<ContactCard> results = new List<ContactCard> ();
			var users = await postService.GetUserCloud ();

			foreach (var user in users) 
			{
				if (user.Id != ColorExtensions.CurrentUser.Id)
				{
					var result = new ContactCard (UITableViewCellStyle.Default, cellId);
					result.nameLabel.Text = user.username;
					result.connection = user.Friend;
					result.id = user.Id;
					result.moviesInCommon = await postService.MoviesInCommon (ColorExtensions.CurrentUser.Id, user.Id);
					results.Add (result);

				}

			}
			watch.Stop ();
			Console.WriteLine("ViewWillAppear Method took " + watch.ElapsedMilliseconds + " milli seconds");
			return results.OrderByDescending (x => x.moviesInCommon).ToList() ;

		}
	}

	public class ConnectCloudTableSource : UITableViewSource
	{
		public List<ContactCard> listItems;
		ConnectViewController controller;
		public ConnectCloudTableSource (List<ContactCard> items, ConnectViewController cont, bool updateProfiles)
		{
			this.listItems = items;
			this.controller = cont;
			if(updateProfiles)
			   updateImages ();
		}
		public async void updateImages ()
		{
			InvokeOnMainThread (async () => 
			{
				foreach (var user in listItems) 
				{
					user.profileImage.Image =await BlobUpload.getProfileImage (user.id, 150, 150);
				} 
			});
		}
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = listItems [indexPath.Row];
			if (cell.connection)
				cell.addRemove.Image = UIImage.FromBundle ("ic_person_remove.png");
			else
				cell.addRemove.Image = UIImage.FromBundle ("ic_person_add.png");
			
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
						if (inserted) {
							listItems [indexPath.Row].connection = true;
							controller.tableView.ReloadData ();
							Toast.MakeText (cell.nameLabel.Text + " is now your Movie Friend.")
							.SetUseShadow (true)
							.SetGravity (ToastGravity.Center)
							.SetCornerRadius (10)
							.SetDuration (3000)
							.Show (ToastType.Info);
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
								listItems [indexPath.Row].connection = false;
								controller.tableView.ReloadData ();
								 Toast.MakeText (cell.nameLabel.Text + " is no longer your Movie Friend.")
								.SetUseShadow (true)
								.SetGravity (ToastGravity.Center)
								.SetCornerRadius (10)
								.SetDuration (3000)
								.Show (ToastType.Info);
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
