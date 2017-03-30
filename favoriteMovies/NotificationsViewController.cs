using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BigTed;
using FavoriteMoviesPCL;
using Foundation;
using MovieFriends;
using ObjCRuntime;
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
				if (tableSource == null)
					BTProgressHUD.Show ();
				
				notificationsList = await getNotifications ();
				if (notificationsList.Count == 0) 
				{
					var noNotifications = new NotificationsCloud ();
					noNotifications.notification = "          Follow someone to see notifications here.";
					notificationsList.Add (noNotifications);
				}

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
			var name = new NSString (Constants.ModifyFollowerNotification);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new Selector (Constants.ModifyFollowerNotificationReceived), name, null);
		}

		[Export ("FollowerModifiedNotificationReceived:")]
		public async void FollowerModifiedNotificationReceived (NSNotification n)
		{
			notificationsList = await getNotifications ();
			if (notificationsList.Count == 0) {
				var noNotifications = new NotificationsCloud ();
				noNotifications.notification = "          Follow someone to see notifications here.";
				notificationsList.Add (noNotifications);
			}
		}

		async Task<List<NotificationsCloud>> getNotifications ()
		{
			return await postService.GetNotifications ();
		}
	}

	public class NotificationsCloudTableSource : UITableViewSource
	{
		List<NotificationsCloud> listItems;
		string cellIdentifier = "NotificationCell";
		public NotificationsCloudTableSource (List<NotificationsCloud> items, UIViewController cont)
		{
			this.listItems = items;
		

		}
		public void updateImage (UITableViewCell cell, NotificationsCloud user)
		{
			
			//cell.ImageView.Image = await BlobUpload.getProfileImage (user.userid, 150, 150);


		}
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{

			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);


			var cellStyle = UITableViewCellStyle.Value2;

			// if there are no cells to reuse, create a new one
			if (cell == null) {
				cell = new UITableViewCell (cellStyle, cellIdentifier);

			}

			cell.DetailTextLabel.Text = listItems [indexPath.Row].notification;
			cell.DetailTextLabel.TextAlignment = UITextAlignment.Justified;
			cell.DetailTextLabel.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);
			cell.DetailTextLabel.TextColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			//cell.ImageView.Image = await BlobUpload.getProfileImage (listItems [indexPath.Row].userid, 150, 150);
		
			return cell;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{

			return listItems.Count;
		}


	}
}
