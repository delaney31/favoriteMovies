
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using com.google.i18n.phonenumbers;
using Contacts;
using CoreLocation;
using FavoriteMoviesPCL;
using Foundation;
using LoginScreen;
using MovieFriends;
using Plugin.Geolocator;
using SQLite;

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
				return false;
			}
		}

		public void Login (string userName, string password, Action successCallback, Action<LoginScreenFaultDetails> failCallback)
		{
			// If login was successfully completed
			DelayInvoke (async () => {
				if (password != await GetPassword (userName)) {
					failCallback (new LoginScreenFaultDetails { PasswordErrorMessage = "Password incorrect." });
				} else {
					MainViewController.getUser ();
					successCallback ();

					//SideMenuController.title.SetTitle (ColorExtensions.CurrentUser.username, UIControlState.Normal);
					//SideMenuController.location.SetTitle (ColorExtensions.CurrentUser.city + ", " + ColorExtensions.CurrentUser.state + " " + ColorExtensions.CurrentUser.country, UIControlState.Normal);

				}
			});

		}
		CNContact GetCurrentUser (string email)
		{
			// Define fields to be searched
			var fetchKeys = new NSString [] { CNContactKey.GivenName, CNContactKey.FamilyName, CNContactKey.PhoneNumbers, CNContactKey.EmailAddresses, CNContactKey.ImageDataAvailable, CNContactKey.ThumbnailImageData };

			try {
				var store = new CNContactStore ();
				NSError error;
				CNContainer [] containers = store.GetContainers (null, out error);

				foreach (var container in containers) {
					var fetchPredicate = CNContact.GetPredicateForContactsInContainer (container.Identifier);

					var containerResults = store.GetUnifiedContacts (fetchPredicate, fetchKeys, out error);
					foreach (var contact in containerResults) {
						var userEmail = contact.EmailAddresses.FirstOrDefault ()?.Value;
						if (userEmail == email)
							return contact;

					}

				}
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
				return null;
			}

			return null;
		}
		public void Register (string email, string userName, string password, Action successCallback, Action<LoginScreenFaultDetails> failCallback)
		{
			java.lang.String phoneNumber = "";
			CNContact currentUser = null;
			bool unique = false;
            string CityName = "", State = "", Country = "", zip = "";
			// If registration was successfully completed
			DelayInvoke (async () => 
			{
				try {
					if (password.Length < 4) {
						failCallback (new LoginScreenFaultDetails { PasswordErrorMessage = "Password must be at least 4 chars." });
					} else 
					{
						
                        try {
							postService.InitializeStore ();
                            var locator = CrossGeolocator.Current;
                            locator.DesiredAccuracy = 50;

                            if (CLLocationManager.LocationServicesEnabled) {
                                var position = await locator.GetPositionAsync (timeoutMilliseconds: 10000);

                                Console.WriteLine ("Position Status: {0}", position.Timestamp);
                                Console.WriteLine ("Position Latitude: {0}", position.Latitude);
                                Console.WriteLine ("Position Longitude: {0}", position.Longitude);

                                var url = String.Format ("http://api.geonames.org/findNearbyPostalCodes?lat={0}&lng={1}&username=delaney31", position.Latitude, position.Longitude);
                                WebRequest webRequest = WebRequest.Create (url);
                                WebResponse webResponse = webRequest.GetResponse ();
                                Stream stream = webResponse.GetResponseStream ();
                                XmlDocument xmlDocument = new XmlDocument ();
                                xmlDocument.Load (stream);

                                CityName = xmlDocument.SelectNodes ("geonames") [0].SelectSingleNode ("code").SelectSingleNode ("name").InnerText;
                                State = xmlDocument.SelectNodes ("geonames") [0].SelectSingleNode ("code").SelectSingleNode ("adminCode1").InnerText;
                                Country = xmlDocument.SelectNodes ("geonames") [0].SelectSingleNode ("code").SelectSingleNode ("countryCode").InnerText;
                                zip = xmlDocument.SelectNodes ("geonames") [0].SelectSingleNode ("code").SelectSingleNode ("postalcode").InnerText;
                            }
                            currentUser = GetCurrentUser (email);
                        }
						catch (WebException e) 
						{
							ColorExtensions.NoInternet = true;
						}
                        catch(Exception ex)
                        {
                            Debug.WriteLine ((ex.Message));
                        }
						UserCloud userCloud = null;
						if (!ColorExtensions.NoInternet) {
							if (currentUser != null) {
								if (Country != "") {
									var util = PhoneNumberUtil.getInstance ();
									var number = util.parse (currentUser.PhoneNumbers.FirstOrDefault ().Value.ValueForKey (new NSString ("digits")).ToString (), Country);
									phoneNumber = util.format (number, PhoneNumberUtil.PhoneNumberFormat.E164);
								}
								userCloud = new UserCloud () { firstname = currentUser.GivenName ?? string.Empty, lastname = currentUser.FamilyName ?? string.Empty, phone = phoneNumber ?? string.Empty, email = email, username = userName, city = CityName, state = State, country = Country, zip = zip };

							} else
								userCloud = new UserCloud () { email = email, username = userName, city = CityName, state = State, country = Country, zip = zip };



							unique = await postService.InsertUserAsync (userCloud);
						}
						if (!unique && !ColorExtensions.NoInternet)
							failCallback (new LoginScreenFaultDetails { UserNameErrorMessage = "This username already exits." });
						else 
						{


							var notification = NSNotification.FromName (Constants.CurrentUserSetNotification, new NSObject ());
							NSNotificationCenter.DefaultCenter.PostNotification (notification);

							//inset username and password locally
							ColorExtensions.CurrentUser.tilesize = 1;
							ColorExtensions.CurrentUser.suggestmovies = true;
							ColorExtensions.CurrentUser.darktheme = false;
							if (currentUser != null) {
								ColorExtensions.CurrentUser.lastname = currentUser.FamilyName ?? string.Empty;
								ColorExtensions.CurrentUser.firstname = currentUser.GivenName ?? string.Empty;
							}
							ColorExtensions.CurrentUser.phone = phoneNumber;
							ColorExtensions.CurrentUser.email = email;
							ColorExtensions.CurrentUser.password = password;
							ColorExtensions.CurrentUser.username = userName;
							if(!ColorExtensions.NoInternet)
							   ColorExtensions.CurrentUser.Id = userCloud.Id;
							ColorExtensions.CurrentUser.city = CityName;
							ColorExtensions.CurrentUser.country = Country;
							ColorExtensions.CurrentUser.state = State;
							ColorExtensions.CurrentUser.zip = zip;
							ColorExtensions.CurrentUser.removeAds = false;

							await AddUserAsync (ColorExtensions.CurrentUser);
							successCallback ();
						}
                      
					}

				} catch (Exception ex) {
					Debug.WriteLine (ex.Message);
				}
			});



		}

		async Task<string> GetPassword (string userName)
		{
			string returnValue = string.Empty;
			using (var db = new SQLiteConnection (MovieService.Database)) 
			{
				await Task.Run (() => {
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
			
			}
			return returnValue;
		}
		async Task DeleteAll ()
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

			} catch (Exception e) {
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
				return false;

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
			timer.Elapsed += (object sender, ElapsedEventArgs e) => {
				operation.Invoke ();
				//SideMenuController.title.SetTitle (ColorExtensions.CurrentUser.username, UIControlState.Normal);
			};
			timer.Start ();



		}

	}
}