using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BigTed;
using FavoriteMoviesPCL;
using Foundation;
using MovieFriends;
using ObjCRuntime;
using UIKit;

namespace FavoriteMovies
{
	public class MovieFriendsViewController:ConnectViewController
	{
		
		async Task<List<UserFriendsCloud>> GetMovieFriends ()
		{
			AzureTablesService userFriendsService = AzureTablesService.DefaultService;
			return  await userFriendsService.GetUserFriends (ColorExtensions.CurrentUser.Id);
		}

		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var name = new NSString (Constants.ModifyFollowerNotification);
			NSNotificationCenter.DefaultCenter.AddObserver (this, new Selector (Constants.ModifyFollowerNotificationReceived), name, null);

			tableItems = await GetFriendsContactsAsync ();
			tableSource = new ConnectCloudTableSource (tableItems, this);

			table.Source = tableSource;
			if (tableItems.FirstOrDefault ().id != "0")
				await ((ConnectCloudTableSource)tableSource).updateImages ();


			NavigationController.NavigationBar.Translucent = false;
			View.Add (table);

		}
		[Export ("FollowerModifiedNotificationReceived:")]
		public async void FollowerModifiedNotificationReceived (NSNotification n)
		{
			tableItems = await GetFriendsContactsAsync ();
			tableSource = new ConnectCloudTableSource (tableItems, this);

			table.Source = tableSource;
			if (tableItems.FirstOrDefault ().id != "0")
				await ((ConnectCloudTableSource)tableSource).updateImages ();

		}
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			if (tableItems == null)
				BTProgressHUD.Show ();
		}


	}
}
