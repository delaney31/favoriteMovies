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
		private MobileServiceClient client;
#if OFFLINE_SYNC_ENABLED
		static readonly string localDbPath = MovieService.Database;
		private IMobileServiceSyncTable<PostItem> postTable;
		private IMobileServiceSyncTable<UserCloud> userTable;
#else
        private IMobileServiceTable<PostItem> postTable;
		private IMobileServiceTable<UserCloud> userTable;
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
#else
            postTable = client.GetTable<PostItem>();
			userTable = client.GetTable<UserCloud> ();
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

				if (pullData) {
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

				if (pullData) {
					await userTable.PullAsync ("allUserItems", userTable.CreateQuery ()); // query ID is used for incremental sync
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
				if (postItem.Id != null)
					await DeleteItemAsync (postItem);
				await InsertUserAsync (postItem);
				Console.WriteLine ("Saved to the cloud!");

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);

			}
		}
		public async Task InsertUserAsync (UserCloud user)
		{
			try {

				await userTable.InsertAsync (user);


#if OFFLINE_SYNC_ENABLED
				await UserSyncAsync (); // Send changes to the mobile app backend.
#endif

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
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

		public async Task<List<PostItem>> GetCloudLike (string title)
		{
			try {

				List<PostItem> items = await postTable
					.Where (post => post.Title == title)
				  .ToListAsync ();

				return new List<PostItem> (items);

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
				return null;
			}


		}
		public async Task DeleteItemAsync (UserCloud item)
		{
			try {
				//item.deleted = true;
				await userTable.DeleteAsync (item);
#if OFFLINE_SYNC_ENABLED
				await UserSyncAsync (); // Send changes to the mobile app backend.
#endif

				// Items.Remove (item);

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}

		public async Task DeleteItemAsync (PostItem item)
		{
			try {
				//item.deleted = true;
				await postTable.DeleteAsync (item);
#if OFFLINE_SYNC_ENABLED
				await PostSyncAsync (); // Send changes to the mobile app backend.
#endif

				// Items.Remove (item);

			} catch (MobileServiceInvalidOperationException e) {
				Console.Error.WriteLine (@"ERROR {0}", e.Message);
			}
		}
	}
}

