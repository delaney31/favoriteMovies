using System;
using BigTed;
using UIKit;

namespace FavoriteMovies
{
	public class FindFriendsViewController:ConnectViewController
	{
		public FindFriendsViewController ()
		{
		}

		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();	
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
			BTProgressHUD.Dismiss ();
		}
	}
}
