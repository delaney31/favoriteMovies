using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FavoriteMoviesPCL;
using MovieFriends;

namespace FavoriteMovies
{
	public class MovieFriendsViewController:ConnectViewController
	{
		
		async Task<List<UserFriendsCloud>> GetMovieFriends ()
		{
			AzureTablesService userFriendsService = AzureTablesService.DefaultService;
		//	await userFriendsService.InitializeStoreAsync ();

			return  await userFriendsService.GetUserFriends (ColorExtensions.CurrentUser.Id);

		}

		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			tableItems = await GetFriendsContactsAsync ();
			tableSource = new ConnectCloudTableSource (tableItems, this);

			table.Source = tableSource;
			await ((ConnectCloudTableSource)tableSource).updateImages ();
			NavigationController.NavigationBar.Translucent = false;
			View.Add (table);
		

		}
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		
		
		}

	}
}
