using System;
using System.Diagnostics;
using System.Linq;
using BigTed;
using CoreGraphics;
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
			
			table.BackgroundColor =  UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f) ;

			await ((ConnectCloudTableSource)tableSource).updateImages ();
            table.ContentInset = new UIEdgeInsets ( 0, 0, 66, 0 );
			View.Add (table);
			
		 	
		}
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
            NavigationController.NavigationBar.Translucent = false;
		}



		[Export ("CustomListChangeReceived:")]
		public async void CustomListChangeReceived (NSNotification n)
		{
			try {
				BTProgressHUD.Show ();
				tableItems = await GetUserContactsAsync ();
				tableSource = new ConnectCloudTableSource (tableItems, this);

				table.Source = tableSource;
				if (tableItems.FirstOrDefault ().id != "0")
					await ((ConnectCloudTableSource)tableSource).updateImages ();
				BTProgressHUD.Dismiss ();
			} catch (Exception e)
			{
				Debug.WriteLine (e.Message);
			}
		}
	}
}
