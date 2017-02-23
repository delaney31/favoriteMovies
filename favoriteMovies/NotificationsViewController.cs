using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BigTed;
using FavoriteMoviesPCL;
using Foundation;
using MovieFriends;
using UIKit;

namespace FavoriteMovies
{
	public class NotificationsViewController: MovieFriendsBaseViewController
	{
		List<NotificationsCloud> notificationsList = new List<NotificationsCloud> ();
		public AzureTablesService postService = AzureTablesService.DefaultService;

		public override async void ViewDidAppear (bool animated)
		{
			try {
				
				base.ViewDidAppear (animated);
				BTProgressHUD.Show ();
				notificationsList = await getNotifications ();
				tableSource = new NotificationsCloudTableSource (notificationsList, this);
				BTProgressHUD.Dismiss ();

				table.Source = tableSource;

				View.Add (table);
				NavigationController.NavigationBar.Translucent = false;
			}catch(Exception ex)
			{
				Console.WriteLine (ex.Message);
			}
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		
		}


		async Task<List<NotificationsCloud>> getNotifications ()
		{
			return await postService.GetNotifications ();
		}
	}

	public class NotificationsCloudTableSource : UITableViewSource
	{
		List<NotificationsCloud> listItems;
		UIViewController controller;
		string cellIdentifier = "NotificationCell";
		public NotificationsCloudTableSource (List<NotificationsCloud> items, UIViewController cont)
		{
			this.listItems = items;
			this.controller = cont;

		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{

			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);
		

			var cellStyle = UITableViewCellStyle.Default;

			// if there are no cells to reuse, create a new one
			if (cell == null) {
				cell = new UITableViewCell (cellStyle, cellIdentifier);

			}

			cell.TextLabel.Text = listItems [indexPath.Row].notification;
			cell.TextLabel.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);


			return cell;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{

			return listItems.Count;
		}


	}
}
