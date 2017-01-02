/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=717898
 */
//#define OFFLINE_SYNC_ENABLED

using System;
using System.Net.Http;
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
    public class AzurePostService
    {
        static AzurePostService instance = new AzurePostService ();

        const string applicationURL = @"https://moviefriends.azurewebsites.net";

        private MobileServiceClient client;
#if OFFLINE_SYNC_ENABLED
        const string localDbPath    = "localstore.db";

        private IMobileServiceSyncTable<Posts> postTable;
#else
        private IMobileServiceTable<Posts> postTable;
#endif

        private AzurePostService ()
        {
            CurrentPlatform.Init();

            // Initialize the client with the mobile app backend URL.
            client = new MobileServiceClient(applicationURL);

#if OFFLINE_SYNC_ENABLED
            // Initialize the store
            InitializeStoreAsync().Wait();

            // Create an MSTable instance to allow us to work with the TodoItem table
            postTable = client.GetSyncTable<Posts>();
#else
            postTable = client.GetTable<Posts>();
#endif
        }

        public static AzurePostService DefaultService {
            get {
                return instance;
            }
        }

        public List<Posts> Items { get; private set;}

        public async Task InitializeStoreAsync()
        {
#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(localDbPath);
            store.DefineTable<Posts>();

            // Uses the default conflict handler, which fails on conflict
            // To use a different conflict handler, pass a parameter to InitializeAsync.
			// For more details, see http://go.microsoft.com/fwlink/?LinkId=521416
            await client.SyncContext.InitializeAsync(store);
#endif
        }

        public async Task SyncAsync(bool pullData = false)
        {
#if OFFLINE_SYNC_ENABLED
            try
            {
                await client.SyncContext.PushAsync();

                if (pullData) {
                    await postTable.PullAsync("allPostItems", postTable.CreateQuery()); // query ID is used for incremental sync
                }
            }

            catch (MobileServiceInvalidOperationException e)
            {
                Console.Error.WriteLine(@"Sync Failed: {0}", e.Message);
            }
#endif
        }

        public async Task<List<Posts>> RefreshDataAsync ()
        {
            try {
#if OFFLINE_SYNC_ENABLED
                // Update the local store
                await SyncAsync(pullData: true);
#endif
				
                // This code refreshes the entries in the list view by querying the local TodoItems table.
                // The query excludes completed TodoItems
				Items = await postTable
                        .Where (postItem => postItem.deleted == false).ToListAsync ();

            } catch (MobileServiceInvalidOperationException e) {
                Console.Error.WriteLine (@"ERROR {0}", e.Message);
                return null;
            }

            return Items;
        }

		public async Task InsertUserAsync (Posts postItem)
        {
            try {
				await postTable.InsertAsync (postItem);
#if OFFLINE_SYNC_ENABLED
                await SyncAsync(); // Send changes to the mobile app backend.
#endif

				Items.Add (postItem);

            } catch (MobileServiceInvalidOperationException e) {
                Console.Error.WriteLine (@"ERROR {0}", e.Message);
            }
        }

        public async Task CompleteItemAsync (Posts item)
        {
            try {
                item.deleted = true;
				await postTable.UpdateAsync (item);
#if OFFLINE_SYNC_ENABLED
                await SyncAsync(); // Send changes to the mobile app backend.
#endif

                Items.Remove (item);

            } catch (MobileServiceInvalidOperationException e) {
                Console.Error.WriteLine (@"ERROR {0}", e.Message);
            }
        }
    }
}

