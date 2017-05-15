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
       
        public FindFriendsViewController ()
		{
		}

       
        public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if(users==null)
               users = await postService.GetUserCloud ();
			tableItems = await GetUserContactsAsync ();
		
			var name = new NSString (Constants.CustomListChange);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new Selector (Constants.CustomListChangeReceived), name, null);

			//Title = "Connect";
			tableSource = new ConnectCloudTableSource (tableItems, this);

			table.Source = tableSource;
			
			table.BackgroundColor =  UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f) ;

			await ((ConnectCloudTableSource)tableSource).updateImages ();
           
			View.Add (table);
			
		 	
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
				BTProgressHUD.Dismiss ();
			} catch (Exception e)
			{
				Debug.WriteLine (e.Message);
			}
		}
	}
}
