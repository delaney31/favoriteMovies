﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FavoriteMoviesPCL;
using MovieFriends;

namespace FavoriteMovies
{
	public class MovieFriendsViewController:MovieFriendsBaseViewController
	{

		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			friendsList = await GetMovieFriends ();

			tableSource = new FriendsTableSource (friendsList, this);
			table.Source = tableSource;
		}

		async Task<List<UserFriendsCloud>> GetMovieFriends ()
		{
			AzureTablesService userFriendsService = AzureTablesService.DefaultService;
			await userFriendsService.InitializeStoreAsync ();

			var retList = await userFriendsService.GetUserFriends (ColorExtensions.CurrentUser.Id);
			return retList;
		}
}
}
