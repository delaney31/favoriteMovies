using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BigTed;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace FavoriteMovies
{
	public class FindFriendsViewController:ConnectViewController
	{
        private bool userInteraction = true;

        public FindFriendsViewController ()
		{
		}

        public FindFriendsViewController (bool userinteraction)
        {
            this.userInteraction = userinteraction;
        }

        public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if(users==null)
               users = await postService.GetUserCloud ();
			tableItems = await GetUserContactsAsync ();
		
			var name = new NSString (Constants.CustomListChange);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new Selector (Constants.CustomListChangeReceived), name, null);

			var followers = new NSString (Constants.ModifyFollowerNotification);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new Selector (Constants.CustomListChangeReceived), followers, null);

			//Title = "Connect";
			tableSource = new ConnectCloudTableSource (tableItems, this);

			table.Source = tableSource;
			
            table.BackgroundColor = UIColor.White;
			await ((ConnectCloudTableSource)tableSource).updateImages ();
            table.UserInteractionEnabled = userInteraction;
           
			View.Add (table);
			
		 	
		}

        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);
			if (!userInteraction)
				NavigationItem.TitleView = null;
        }
        public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);	
            NavigationController.NavigationBar.Translucent = false;
			table.Frame = new CGRect () { X = 0.0f, Y = 0.0f, Width = View.Layer.Frame.Width, Height = View.Layer.Frame.Height };
            table.ContentInset = new UIEdgeInsets (0, 0, 66, 0);
			
		}



		[Export ("CustomListChangeReceived:")]
		public async void CustomListChangeReceived (NSNotification n)
		{
			try {
				BTProgressHUD.Show ();
                users = await postService.GetUserCloud ();
				tableItems = await GetUserContactsAsync ();
				tableSource = new ConnectCloudTableSource (tableItems, this);

				table.Source = tableSource;
				if (tableItems.FirstOrDefault ().id != "0")
					await ((ConnectCloudTableSource)tableSource).updateImages ();
                InvokeOnMainThread (async () => { table.ReloadData (); });
               
				BTProgressHUD.Dismiss ();
			} catch (Exception e)
			{
				Debug.WriteLine (e.Message);
			}
		}
	}
}
