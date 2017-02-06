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
using UIKit;
using BigTed;


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
		private IMobileServiceTable<NotificationsCloud> nfTable;

#else
		private IMobileServiceTable<PostItem> postTable;
		private IMobileServiceTable<UserCloud> userTable;
		private IMobileServiceTable<UserFriendsCloud> ufTable;
		private IMobileServiceTable<CustomListCloud> clTable;
		private IMobileServiceTable<MovieCloud> mfTable;
		private IMobileServiceTable<NotificationsCloud> nfTable;


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
			nfTable = client.GetSyncTable<NotificationsCloud>();
#else
			postTable = client.GetTable<PostItem> ();
			userTable = client.GetTable<UserCloud> ();
			ufTable = client.GetTable<UserFriendsCloud> ();
			mfTable = client.GetTable<MovieCloud> ();
			clTable = client.GetTable<CustomListCloud> ();
			nfTable = client.GetTable<NotificationsCloud> ();
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
		public async Task<List<CustomListCloud>> GetCustomList (string userId)
		{
			try {

				var customList =
					from custom in await clTable.Where (item => item.UserId == userId && item.shared == true).ToListAsync ()
					select new CustomListCloud {
						Id = custom.Id, Name = custom.Name, order = custom.order, shared = custom.shared
					};

				return new List<CustomListCloud> (customList);

			} catch (Exception e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return new List<CustomListCloud> ();
			}
		}
		public async Task<List<MovieCloud>> GetCustomListMovies (string customListId)
		{
			try {

				var movieList =
					from movies in await mfTable.Where (item => item.CustomListID == customListId).ToListAsync ()
					select new MovieCloud { id = movies.id, name = movies.name,
					BackdropPath = movies.BackdropPath, CustomListID = movies.CustomListID,
					Favorite = movies.Favorite, HighResPosterPath = movies.HighResPosterPath,
					OriginalLanguage = movies.OriginalLanguage, Overview = movies.Overview,
					Popularity = movies.Popularity, PosterPath = movies.PosterPath, ReleaseDate = movies.ReleaseDate,
					VoteAverage = movies.VoteAverage, UserReview = movies.UserReview, UserRating = movies.UserRating,
					OriginalId = movies.OriginalId, order= movies.order};
				
				return new List<MovieCloud> (movieList);

			} catch (Exception e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return new List<MovieCloud> ();
			}
		}
		public async Task<List<CustomListCloud>> GetUserFriendsLists (string userid)
		{
			try 
			{
				var customLists =
					from friends in await ufTable.Where (item => item.userid == userid).ToListAsync ()
					join customlist in await clTable.ToListAsync () on friends.userid equals customlist.UserId
					//join movies in await mfTable.ToListAsync () on customlist.Id equals movies.CustomListID
					where customlist.shared == true
					select new CustomListCloud{ Id = customlist.Id, Name= customlist.Name, order = customlist.order, shared= customlist.shared };

				return new List<CustomListCloud> (customLists);

			} catch (Exception e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return new List<CustomListCloud> ();
			}
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
				var notification = new NotificationsCloud ();
				notification.notification = ColorExtensions.CurrentUser.username + " is now following : " + user.friendusername;
				notification.userid = ColorExtensions.CurrentUser.Id;
				await nfTable.InsertAsync (notification);

				return true;

#if OFFLINE_SYNC_ENABLED
				await UserFriendsSyncAsync (); // Send changes to the mobile app backend.
#endif

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return false;
			}

		}

		public async Task<int> MoviesInCommon (UserCloud user1, UserCloud user2)
		{
			int inCommon = 0;

			try {


				if (user2.username == "delaney51")
					Console.WriteLine (user2.username);
				
				var user2Movies =
					from movies in await mfTable.ToListAsync ()
					join customlist in await clTable.ToListAsync () on movies.CustomListID equals customlist.Id
					where customlist.UserId == user2.Id
					select new { movies.name};

				var userMovies =
					from movies in await mfTable.ToListAsync ()
					join customlist in await clTable.ToListAsync () on movies.CustomListID equals customlist.Id
					where customlist.UserId==user1.Id           
					select new {movies.name};
				
				var common = from list1 in userMovies
							 join list2 in user2Movies
					on list1.name equals list2.name
							 select new 
							 {
								 list1.name
							 };
				inCommon = common.Distinct().Count ();

			} catch (Exception ex) 
			{
				Console.Error.WriteLine (@"ERROR {0}", ex.Message);
				return 0;
			}
			return inCommon;
		}

		internal  async Task DeleteCustomList (CustomListCloud customList)
		{
			try 
			{
				await clTable.DeleteAsync (customList);
				var notification = new NotificationsCloud ();
				notification.notification = ColorExtensions.CurrentUser.username + " deleted Custom List: " + customList.Name;
				notification.userid = ColorExtensions.CurrentUser.Id;
				await nfTable.InsertAsync (notification);
				
			} catch (Exception ex) 
			{
				Console.WriteLine (@"ERROR{0}", ex.Message);
			}

		}

		public async Task DeleteAll (string userid)
		{
			try {
				var customList = await clTable.Where (item => item.UserId == userid).ToListAsync ();

				foreach (var cl in customList) 
				{
					//never delete list only movies
					//await clTable.DeleteAsync (cl);
					var movies = await mfTable.Where (Movie => Movie.CustomListID == cl.Id).ToListAsync();
					foreach (var movie in movies) 
					{
						await mfTable.DeleteAsync (movie);
					}
						

				}


			} catch (Exception e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}
		public async Task UpdatedShared (CustomListCloud list)
		{
			try {
				//await clTable.DeleteAsync (list);
				await clTable.UpdateAsync (list);
				var notification = new NotificationsCloud ();
				if (list.shared)
				   notification.notification = ColorExtensions.CurrentUser.username + " shared Custom List: " + list.Name;
				notification.userid = ColorExtensions.CurrentUser.Id;
				await nfTable.InsertAsync (notification);
			} catch (Exception e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}
		public async Task<bool> InsertMovieAsync (MovieCloud movie)
		{
			bool retValue = false;
			try {
				
				await mfTable.InsertAsync (movie);

			retValue = true;


#if OFFLINE_SYNC_ENABLED
				await MovieSyncAsync (); // Send changes to the mobile app backend.
#endif

			} catch (Exception e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return false;
			}
			return retValue;


		}

		internal async Task DeleteItemAsync (string name, string currentUserId)
		{
			//onetime cleanup
			foreach (var item in await clTable.ToListAsync()) 
			{
				var orphan = await mfTable.Where (x => x.CustomListID == item.Id).ToListAsync ();
				if (orphan.Count () == 0)
					await clTable.DeleteAsync (item);
			}
			var list = await clTable.Where (x => x.Name == name && x.UserId == currentUserId).ToListAsync();
			if (list.Count () > 0)
				await clTable.DeleteAsync (list [0]);

		}

		public async Task<bool> InsertCustomListAsync (CustomListCloud list)
		{
			try {
				var exists = await clTable.Where (i => i.Name == list.Name && i.UserId == ColorExtensions.CurrentUser.Id).ToListAsync();
				if (exists.Count() ==0)
				   await clTable.InsertAsync (list);
				if (list.shared) 
				{
					var notification = new NotificationsCloud ();
					notification.notification = ColorExtensions.CurrentUser.username + " shared Custom List: " + list.Name;
					notification.userid = ColorExtensions.CurrentUser.Id;
					await nfTable.InsertAsync (notification);
				}


#if OFFLINE_SYNC_ENABLED
				await CustomListSyncAsync (); // Send changes to the mobile app backend.
#endif

			} catch (Exception e) {
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

			} catch (Exception e) {
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

		public async Task<List<UserCloud>> GetUserCloud ()
		{
			
			try {
				
				var friends = await ufTable.ToListAsync ();

				var userfriends =
					from u in await userTable.ToListAsync ()                        
					let friend = friends.Count (x => x.friendid == u.Id && x.userid == ColorExtensions.CurrentUser.Id) >0
					select new UserCloud ()
					{ 
						username = u.username, 
						State = u.State,
						City = u.City,
						Country = u.Country,
						connection = friend,
						Id= u.Id
					};


				return userfriends.Take(50).ToList ();



			} catch (Exception e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return new List<UserCloud> ();
			}


		}
		public async Task DeleteMovieItemAsync (Movie movie, string customListName)
		{
			try 
			{
				var customListId =  await clTable.Where (x => x.Name == customListName && x.UserId == ColorExtensions.CurrentUser.Id).ToListAsync();

				var items = await mfTable.Where (item => item.CustomListID == customListId.FirstOrDefault().Id && item.name == movie.name && item.ReleaseDate == movie.ReleaseDate.Value.ToString ("MM/dd/yyyy", CultureInfo.InvariantCulture)).ToListAsync();

				if (items.Count > 0)
					foreach (var item in items) 
					{
					   await mfTable.DeleteAsync (item);
					}


#if OFFLINE_SYNC_ENABLED
				await UserSyncAsync (); // Send changes to the mobile app backend.
#endif

				// Items.Remove (item);

			} catch (Exception e) {
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

			} catch (Exception e) {
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

				var notification = new NotificationsCloud ();
				notification.notification = ColorExtensions.CurrentUser.username + " is no longer following : " + item.friendusername;
				notification.userid = ColorExtensions.CurrentUser.Id;
				await nfTable.InsertAsync (notification);

				return true;
#if OFFLINE_SYNC_ENABLED
				await UserFriendsSyncAsync (); // Send changes to the mobile app backend.
#endif

				// Items.Remove (item);

			} catch (Exception e) {
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

			} catch (Exception e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return false;
			}
		}

	}
}

