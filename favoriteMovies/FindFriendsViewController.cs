using System;
using System.Linq;
using BigTed;
using Foundation;
using ObjCRuntime;
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
			var name = new NSString (Constants.CustomListChange);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new Selector (Constants.CustomListChangeReceived), name, null);

			//Title = "Connect";
			tableSource = new ConnectCloudTableSource (tableItems, this);

			table.Source = tableSource;
			await ((ConnectCloudTableSource)tableSource).updateImages ();
			table.ContentInset = new UIEdgeInsets (64, 0, 49, 0);
			View.Add (table);
			//NavigationController.NavigationBar.Translucent = false;
		 	
		}




		[Export ("CustomListChangeReceived:")]
		public async void CustomListChangeReceived (NSNotification n)
		{
			BTProgressHUD.Show ();
			tableItems = await GetUserContactsAsync ();
			tableSource = new ConnectCloudTableSource (tableItems, this);

			table.Source = tableSource;
			if (tableItems.FirstOrDefault ().id != "0")
				await ((ConnectCloudTableSource)tableSource).updateImages ();
			BTProgressHUD.Dismiss ();
		}
	}
}
