
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using FavoriteMoviesPCL;
using LoginScreen;
using MovieFriends;
using SQLite;
using UIKit;

namespace FavoriteMovies
{
	public class CredentialsProvider : ICredentialsProvider
	{
		AzureTablesService postService;
		// Constructor without parameters is required
		public CredentialsProvider ()
		{
			postService = AzureTablesService.DefaultService;

		}
		public bool NeedLoginAfterRegistration {
			get {
				// If you want your user to login after he/she has been registered
				return true;

				// Otherwise you can:
				// return false;
			}
		}

		public void Login (string userName, string password, Action successCallback, Action<LoginScreenFaultDetails> failCallback)
		{
			// Do some operations to login user

			// If login was successfully completed
			DelayInvoke (async () => {
				if (password!=await GetPassword(userName)) 
				{
					failCallback (new LoginScreenFaultDetails { PasswordErrorMessage = "Password incorrect." });
				} else {
					successCallback ();
				}
			});
			MainViewController.getUser ();
			SideMenuController.title.SetTitle (ColorExtensions.CurrentUser.username, UIControlState.Normal);
			SideMenuController.location.SetTitle (ColorExtensions.CurrentUser.email, UIControlState.Normal);




			// Otherwise
			// failCallback(new LoginScreenFaultDetails {
			// 	CommonErrorMessage = "Some error message relative to whole form",
			// 	UserNameErrorMessage = "Some error message relative to user name form field",
			// 	PasswordErrorMessage = "Some error message relative to password form field"
			// });
		}

		public void Register (string email, string userName, string password, Action successCallback, Action<LoginScreenFaultDetails> failCallback)
		{
			// Do some operations to register user


			// If registration was successfully completed
			DelayInvoke (async () => {
				if (password.Length < 4) {
					failCallback (new LoginScreenFaultDetails { PasswordErrorMessage = "Password must be at least 4 chars." });
				} else 
				{
					await postService.InitializeStoreAsync ();

					var userCloud = new UserCloud () { email = email, username = userName };
					//insert email and username in cloud
					var unique = await postService.InsertUserAsync (userCloud);
					if (!unique)
						failCallback (new LoginScreenFaultDetails { UserNameErrorMessage = "This username already exits." });
					else 
					{
						ColorExtensions.CurrentUser = userCloud;

						var user = new User () { email = email, password = password, username = userName, Id = userCloud.Id };
						//inset username and password locally
						await AddUserAsync (user);

						successCallback ();
					}

				}
			});



			// Otherwise
			// failCallback(new LoginScreenFaultDetails {
			// 	CommonErrorMessage = "Some error message relative to whole form",
			// 	EmailErrorMessage = "Some error message relative to e-mail form field",
			// 	UserNameErrorMessage = "Some error message relative to user name form field",
			// 	PasswordErrorMessage = "Some error message relative to password form field"
			// });
		}

		async Task<string> GetPassword (string userName)
		{
			string returnValue= string.Empty;
			using (var db = new SQLiteConnection (MovieService.Database)) 
			{
				var task = Task.Run (() => {
					try {
						// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
						var query = db.Query<User> ("SELECT * FROM [User] WHERE [username] = '" + userName + "'");

						if (query.Count > 0) {
							returnValue = query [0].password;
						}


					} catch (SQLiteException e) {
						Debug.WriteLine (e.Message);

					}
				});
				task.Wait ();
			}
			return returnValue;
		}
		async Task  DeleteAll ()
		{
			
				try {
					using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
						// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);

						db.Query<Movie> ("DELETE FROM [User]");

					}
				} catch (SQLite.SQLiteException e) {
					//first time in no favorites yet.
					Debug.Write (e.Message);
				}
		
		}
		async Task AddUserAsync (User user)
		{
			try {

				using (var db = new SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/
					await DeleteAll ();	
					db.InsertOrReplace (user, typeof (User));




				}

			} catch (SQLiteException e) {
				Debug.WriteLine (e.Message);

				using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) {
					conn.CreateTable<User> ();

				}

				using (var db = new SQLiteConnection (MovieService.Database)) {
					
					db.InsertOrReplace (user, typeof (User));
				

				}
			}

		}



		public void ResetPassword (string email, Action successCallback, Action<LoginScreenFaultDetails> failCallback)
		{
			// Do some operations to reset user's password

			// If password was successfully reset
			successCallback ();

			// Otherwise
			// failCallback(new LoginScreenFaultDetails {
			// 	CommonErrorMessage = "Some error message relative to whole form",
			// 	EmailErrorMessage = "Some error message relative to e-mail form field"
			// });
		}

		public bool ShowPasswordResetLink {
			get {
				// If you want your login screen to have a forgot password button
				return true;

				// Otherwise you can:
				// return false;
			}
		}

		public bool ShowRegistration {
			get {
				// If you want your login screen to have a register new user button
				return true;

				// Otherwise you can:
				// return false;
			}
		}
		private void DelayInvoke (Action operation)
		{
			Timer timer = new Timer ();
			timer.AutoReset = false;
			timer.Interval = 3000;
			timer.Elapsed += (object sender, ElapsedEventArgs e) => operation.Invoke ();
			timer.Start ();
		}

	}
}