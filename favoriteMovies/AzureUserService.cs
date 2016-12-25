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
    public class AzureUserService
    {
        static AzureUserService instance = new AzureUserService ();

        const string applicationURL = @"https://moviefriends.azurewebsites.net";

        private MobileServiceClient client;
#if OFFLINE_SYNC_ENABLED
        const string localDbPath    = "localstore.db";

        private IMobileServiceSyncTable<User> todoTable;
#else
        private IMobileServiceTable<User> userTable;
#endif

        private AzureUserService ()
        {
            CurrentPlatform.Init();

            // Initialize the client with the mobile app backend URL.
            client = new MobileServiceClient(applicationURL);

#if OFFLINE_SYNC_ENABLED
            // Initialize the store
            InitializeStoreAsync().Wait();

            // Create an MSTable instance to allow us to work with the TodoItem table
            todoTable = client.GetSyncTable<ToDoItem>();
#else
            userTable = client.GetTable<User>();
#endif
        }

        public static AzureUserService DefaultService {
            get {
                return instance;
            }
        }

        public List<User> Items { get; private set;}

        public async Task InitializeStoreAsync()
        {
#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore(localDbPath);
            store.DefineTable<ToDoItem>();

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
                    await todoTable.PullAsync("allTodoItems", todoTable.CreateQuery()); // query ID is used for incremental sync
                }
            }

            catch (MobileServiceInvalidOperationException e)
            {
                Console.Error.WriteLine(@"Sync Failed: {0}", e.Message);
            }
#endif
        }

        public async Task<List<User>> RefreshDataAsync ()
        {
            try {
#if OFFLINE_SYNC_ENABLED
                // Update the local store
                await SyncAsync(pullData: true);
#endif

                // This code refreshes the entries in the list view by querying the local TodoItems table.
                // The query excludes completed TodoItems
				Items = await userTable
                        .Where (todoItem => todoItem.deleted == false).ToListAsync ();

            } catch (MobileServiceInvalidOperationException e) {
                Console.Error.WriteLine (@"ERROR {0}", e.Message);
                return null;
            }

            return Items;
        }

		public async Task InsertUserAsync (User user)
        {
            try {
                await userTable.InsertAsync (user); // Insert a new TodoItem into the local database.
#if OFFLINE_SYNC_ENABLED
                await SyncAsync(); // Send changes to the mobile app backend.
#endif

                Items.Add (user);

            } catch (MobileServiceInvalidOperationException e) {
                Console.Error.WriteLine (@"ERROR {0}", e.Message);
            }
        }

        public async Task DeleteItemAsync (User item)
        {
            try {
                item.deleted = true;
                await userTable.UpdateAsync (item); // Update todo item in the local database
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

