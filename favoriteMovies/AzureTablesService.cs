/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=717898
 */
//#define OFFLINE_SYNC_ENABLED

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices;
using FavoriteMoviesPCL;
using System.Collections;
using System.Linq;
using FavoriteMovies;
using System.Globalization;


#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;  // offline sync
using Microsoft.WindowsAzure.MobileServices.Sync;         // offline sync
#endif

namespace MovieFriends
{
	public class AzureTablesService
	{
		static AzureTablesService instance = new AzureTablesService ();
		const string applicationURL = @"https://moviefriends.azurewebsites.net";
		public MobileServiceClient client;
#if OFFLINE_SYNC_ENABLED
		static readonly string localDbPath = MovieService.Database;
		private IMobileServiceSyncTable<PostItem> postTable;
		private IMobileServiceSyncTable<UserCloud> userTable;
		private IMobileServiceSyncTable<UserFriendsCloud> ufTable;
		private IMobileServiceSyncTable<CustomListCloud> clTable;
		private IMobileServiceSyncTable<MovieCloud> mfTable;
#else
		private IMobileServiceTable<PostItem> postTable;
		private IMobileServiceTable<UserCloud> userTable;
		private IMobileServiceTable<UserFriendsCloud> ufTable;
		private IMobileServiceTable<CustomListCloud> clTable;
		private IMobileServiceTable<MovieCloud> mfTable;


#endif

		private AzureTablesService ()
		{
			CurrentPlatform.Init ();
			// Initialize the client with the mobile app backend URL.
			client = new MobileServiceClient (applicationURL);
#if OFFLINE_SYNC_ENABLED

			// Create an MSTable instance to allow us to work with the TodoItem table
			postTable = client.GetSyncTable<PostItem> ();
			userTable = client.GetSyncTable<UserCloud> ();
			ufTable = client.GetSyncTable<UserFriendsCloud>();
			clTable = client.GetSyncTable<CustomListCloud>();
			mfTable = client.GetSyncTable<MovieCloud>();
#else
			postTable = client.GetTable<PostItem> ();
			userTable = client.GetTable<UserCloud> ();
			ufTable = client.GetTable<UserFriendsCloud> ();
			mfTable = client.GetTable<MovieCloud> ();
			clTable = client.GetTable<CustomListCloud> ();
#endif
		}

		public static AzureTablesService DefaultService {
			get {
				return instance;
			}
		}

		public async Task InitializeStoreAsync ()
		{
#if OFFLINE_SYNC_ENABLED
			var store = new MobileServiceSQLiteStore (MovieService.Database);
			store.DefineTable<PostItem> ();
			store.DefineTable<UserCloud> ();
			store.DefineTable<UserFriendsCloud>();
			store.DefineTable<CustomListCloud>();
			store.DefineTable<MovieCloud>();

			// Uses the default conflict handler, which fails on conflict
			// To use a different conflict handler, pass a parameter to InitializeAsync.
			// For more details, see http://go.microsoft.com/fwlink/?LinkId=521416

			await client.SyncContext.InitializeAsync (store);
#endif
		}

		public async Task PostSyncAsync (bool pullData = false)
		{

#if OFFLINE_SYNC_ENABLED
			try {
				await client.SyncContext.PushAsync ();

				if (pullData) 
				{
					await postTable.PullAsync ("allPostItems", postTable.CreateQuery ()); // query ID is used for incremental sync
				}
			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"Sync Failed: {0}", e.Message);
			}
#endif
		}
		public async Task UserSyncAsync (bool pullData = false)
		{
#if OFFLINE_SYNC_ENABLED
			try {
				await client.SyncContext.PushAsync ();

				if (pullData) 
				{
					await userTable.PullAsync ("allUserItems", userTable.CreateQuery ()); // query ID is used for incremental sync
				}
			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"Sync Failed: {0}", e.Message);
			}
#endif
		}
		public async Task UserFriendsSyncAsync (bool pullData = false)
		{
#if OFFLINE_SYNC_ENABLED
			try {
				await client.SyncContext.PushAsync ();

				if (pullData) 
				{
					await ufTable.PullAsync ("allUserItems", ufTable.CreateQuery ()); // query ID is used for incremental sync

				}
			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"Sync Failed: {0}", e.Message);
			}
#endif
		}
		public async Task MovieSyncAsync (bool pullData = false)
		{
#if OFFLINE_SYNC_ENABLED
			try {
				await client.SyncContext.PushAsync ();

				if (pullData) {
					await mfTable.PullAsync ("allUserItems", mfTable.CreateQuery ()); // query ID is used for incremental sync

				}
			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"Sync Failed: {0}", e.Message);
			}
#endif
		}

		public async Task CustomListSyncAsync (bool pullData = false)
		{
#if OFFLINE_SYNC_ENABLED
			try {
				await client.SyncContext.PushAsync ();

				if (pullData) {
					await clTable.PullAsync ("allUserItems", clTable.CreateQuery ()); // query ID is used for incremental sync

				}
			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"Sync Failed: {0}", e.Message);
			}
#endif
		}
		public async Task RefreshDataAsync (PostItem postItem)
		{
			try {
#if OFFLINE_SYNC_ENABLED
				// Update the local store
				await PostSyncAsync (pullData: true);
#endif
				if (postItem.Id != null)
					await DeleteItemAsync (postItem);
				await InsertPostItemAsync (postItem);
				Console.WriteLine ("Saved to the cloud!");

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);

			}
		}

		public async Task RefreshDataAsync (UserCloud postItem)
		{
			try {
#if OFFLINE_SYNC_ENABLED
				// Update the local store
				await UserSyncAsync (pullData: true);
#endif
				//if (postItem.Id != null)
				await DeleteItemAsync (postItem);
				await InsertUserAsync (postItem);
				Console.WriteLine ("Saved to the cloud!");

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);

			}
		}

		public async Task RefreshDataAsync (UserFriendsCloud userFriend)
		{
			try {
#if OFFLINE_SYNC_ENABLED
				// Update the local store
				await UserFriendsSyncAsync (pullData: true);
#endif
				if (userFriend.id != null)
					await DeleteItemAsync (userFriend);
				await InsertUserFriendAsync (userFriend);
				Console.WriteLine ("Saved to the cloud!");

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);

			}
		}
		public async Task<bool> InsertUserAsync (UserCloud user)
		{
			try {
				var exists = await userTable.Where (item => item.username.ToLower () == user.username.ToLower ()).ToListAsync ();
				if (exists.Count > 0)
					return false;
				await userTable.InsertAsync (user);
				return true;

#if OFFLINE_SYNC_ENABLED
				await UserSyncAsync (); // Send changes to the mobile app backend.
#endif

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return false;
			}
		}
		public async Task<bool> InsertUserFriendAsync (UserFriendsCloud user)
		{
			try {

				await ufTable.InsertAsync (user);
				return true;

#if OFFLINE_SYNC_ENABLED
				await UserFriendsSyncAsync (); // Send changes to the mobile app backend.
#endif

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return false;
			}

		}

		public async Task<int> MoviesInCommon (string user1, string user2)
		{
			int inCommon = 0;

			try {
				
				var user2Movies =
					from movie in await mfTable.ToListAsync ()
					join customlist in await clTable.ToListAsync () on movie.CustomListID equals customlist.Id
					join moviesUser2 in await mfTable.ToListAsync () on customlist.UserId equals user2                       
					select moviesUser2.name;

				var userMovies =
					from movies in await mfTable.ToListAsync ()
					join customlist in await clTable.ToListAsync () on movies.CustomListID equals customlist.Id
					join moviesUser1 in await mfTable.ToListAsync () on customlist.UserId equals user1
					where user2Movies.ToList().Contains (movies.name)                
					select new {movies.name, customlist.UserId};
				

				inCommon = userMovies.Distinct().Count ();

			} catch (Exception ex) 
			{
				Console.Error.WriteLine (@"ERROR {0}", ex.Message);
				return 0;
			}
			return inCommon;
		}
		public async Task DeleteAll (string userid)
		{
			try {
				var customList = await clTable.Where (item => item.UserId == userid).ToListAsync ();

				foreach (var cl in customList) {
					await clTable.DeleteAsync (cl);
				}
			} catch (Exception e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}
		public async Task UpdatedShared (CustomListCloud list, bool shared)
		{
			try {
				list.shared = shared;
				await clTable.UpdateAsync (list);
			} catch (Exception e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}
		public async Task<bool> InsertMovieAsync (MovieCloud movie, CustomListCloud list)
		{
			bool retValue = false;
			try {

				var customList = await clTable.Where (item => item.Name.ToLower () == list.Name.ToLower ()).Where (item => item.UserId == list.UserId).ToListAsync ();
				if (customList.Count > 0) {
					movie.CustomListID = customList [0].Id;
					await mfTable.InsertAsync (movie);
					retValue = true;
				} else
					retValue = false;


#if OFFLINE_SYNC_ENABLED
				await MovieSyncAsync (); // Send changes to the mobile app backend.
#endif

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return false;
			}
			return retValue;
		}
		public async Task<bool> InsertCustomListAsync (CustomListCloud list)
		{
			try {

				await clTable.InsertAsync (list);



#if OFFLINE_SYNC_ENABLED
				await CustomListSyncAsync (); // Send changes to the mobile app backend.
#endif

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return false;
			}
			return true;
		}
		public async Task InsertPostItemAsync (PostItem postItem)
		{
			try {

				await postTable.InsertAsync (postItem);


#if OFFLINE_SYNC_ENABLED
				await PostSyncAsync (); // Send changes to the mobile app backend.
#endif

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}

		internal async Task<List<UserFriendsCloud>> GetUserFriends (string userid)
		{
			try {

				List<UserFriendsCloud> items = await ufTable
					.Where (item => item.userid == userid)
				  .ToListAsync ();

				return new List<UserFriendsCloud> (items);

			} catch (Exception e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return new List<UserFriendsCloud> ();
			}

		}
		internal async Task<List<UserFriendsCloud>> GetSearchUserFriends (string search)
		{
			try {

				List<UserFriendsCloud> items = await ufTable
					.Where (item => item.friendusername == search)
				  .ToListAsync ();

				return new List<UserFriendsCloud> (items);

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return new List<UserFriendsCloud> ();
			}

		}
		public async Task<List<PostItem>> GetCloudLike (string title)
		{
			try {

				List<PostItem> items = await postTable
					.Where (post => post.Title == title)
				  .ToListAsync ();

				return new List<PostItem> (items);

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return new List<PostItem> ();
			}


		}

		public async Task<List<UserFriend>> GetUserCloud ()
		{
			try {
				List<UserFriend> items = new List<UserFriend> ();
				var userFriends = await ufTable.ToListAsync ();
				var users = await userTable.ToListAsync ();
				foreach (var user in users) {
					var userFriend = new UserFriend ();
					userFriend.email = user.email;
					userFriend.Id = user.Id;
					userFriend.username = user.username;
					foreach (var uf in userFriends) {
						if (user.Id == uf.friendid && uf.userid == ColorExtensions.CurrentUser.Id)
							userFriend.Friend = true;
					}
					if (user.Id != ColorExtensions.CurrentUser.Id)
						items.Add (userFriend);

				}
				return new List<UserFriend> (items);

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return new List<UserFriend> ();
			}


		}
		public async Task DeleteMovieItemAsync (Movie movie)
		{
			try 
			{
				var items = await mfTable.Where (item => item.name == movie.name && item.ReleaseDate == movie.ReleaseDate.Value.ToString ("MM/dd/yyyy", CultureInfo.InvariantCulture)).ToListAsync();

				await mfTable.DeleteAsync (items.FirstOrDefault ());

#if OFFLINE_SYNC_ENABLED
				await UserSyncAsync (); // Send changes to the mobile app backend.
#endif

				// Items.Remove (item);

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);


			}
		}

		public async Task<bool> DeleteItemAsync (UserCloud item)
		{
			try {
				//item.deleted = true;
				await userTable.DeleteAsync (item);
				return true;
#if OFFLINE_SYNC_ENABLED
				await UserSyncAsync (); // Send changes to the mobile app backend.
#endif

				// Items.Remove (item);

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return false;

			}

		}
		public async Task<bool> DeleteItemAsync (UserFriendsCloud item)
		{
			try {
				//item.deleted = true;
				var user = await ufTable.Where (post => post.userid == item.userid && post.friendid == item.friendid).ToListAsync ();
				if (user.Count == 0)
					return false;
				item.id = user.First ().id;
				await ufTable.DeleteAsync (item);
				return true;
#if OFFLINE_SYNC_ENABLED
				await UserFriendsSyncAsync (); // Send changes to the mobile app backend.
#endif

				// Items.Remove (item);

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return false;

			}
		}
		public async Task<bool> DeleteItemAsync (PostItem item)
		{
			try {
				//item.deleted = true;
				await postTable.DeleteAsync (item);
				return true;
#if OFFLINE_SYNC_ENABLED
				await PostSyncAsync (); // Send changes to the mobile app backend.
#endif

				// Items.Remove (item);

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return false;
			}
		}

	}
}

