using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FavoriteMoviesPCL;
using MovieFriends;

namespace FavoriteMovies
{
	public class MovieFriendsViewController:MovieFriendsBaseViewController
	{
		
		async Task<List<UserFriendsCloud>> GetMovieFriends ()
		{
			AzureTablesService userFriendsService = AzureTablesService.DefaultService;
		//	await userFriendsService.InitializeStoreAsync ();

			var retList = await userFriendsService.GetUserFriends (ColorExtensions.CurrentUser.Id);
			return retList;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

		}
		public override async void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			friendsList = await GetMovieFriends ();

			tableSource = new FriendsTableSource (friendsList, this);
			table.Source = tableSource;
			View.Add (table);
			NavigationController.NavigationBar.Translucent = false;
		}

	}
}
