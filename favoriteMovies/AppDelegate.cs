using System.IO;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using SidebarNavigation;
using UIKit;
using WindowsAzure.Messaging;
using Pushwoosh;
using Firebase.Analytics;
using Google.MobileAds;

namespace FavoriteMovies
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		private SBNotificationHub Hub { get; set; }
		public RootViewController rootViewController { get; private set;}
		public override UIWindow Window {
			get;
			set;
		}

		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			PushNotificationManager.PushManager.HandlePushRegistration (deviceToken);
		}

		public override void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
		{
			PushNotificationManager.PushManager.HandlePushRegistrationFailure (error);
		}

		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			PushNotificationManager.PushManager.HandlePushReceived (userInfo);
		}
        public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
        {
            
            App.Configure ();

			// Get your Application Id here: https://apps.admob.com/#account/appmgmt:
			MobileAds.Configure ("ca-app-pub-3328591715743369~6456302736");
		
			MovieService.Database = Path.Combine (FileHelper.GetLocalStoragePath (), "MovieEntries.db3");

            MainViewController.getUser ();
            ColorExtensions.DarkTheme = ColorExtensions.CurrentUser.darktheme;
            ColorExtensions.CurrentTileSize = ColorExtensions.CurrentUser.tilesize;
            rootViewController = new RootViewController ();
            var settings = UIUserNotificationSettings.GetSettingsForTypes (
              UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound
              , null);
            UIApplication.SharedApplication.RegisterUserNotificationSettings (settings);




            // make the window visible
            Window = new UIWindow (UIScreen.MainScreen.Bounds);
            Window.RootViewController = rootViewController;

            var frame = UIScreen.MainScreen.Bounds;
            Window.Frame = new CGRect () { X = 0, Y = 0, Width = frame.Size.Width + 0.000001f, Height = frame.Size.Height + 0.000001f } ;
      
			Window.MakeKeyAndVisible ();
			
			
			// check for a local notification
			if (launchOptions != null) {
				if (launchOptions.ContainsKey (UIApplication.LaunchOptionsLocalNotificationKey)) {
					var localNotification = launchOptions [UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
					if (localNotification != null) {
						UIAlertController okayAlertController = UIAlertController.Create (localNotification.AlertAction, localNotification.AlertBody, UIAlertControllerStyle.Alert);
						okayAlertController.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
						rootViewController.PresentViewController (okayAlertController, true, null);

						// reset our badge
						UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
					}
				}
			}
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				var pushSettings = UIUserNotificationSettings.GetSettingsForTypes (
					   UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
					   new NSSet ());

				UIApplication.SharedApplication.RegisterUserNotificationSettings (pushSettings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications ();
			} else {
				UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (notificationTypes);
			}

			PushNotificationManager pushmanager = PushNotificationManager.PushManager;
			pushmanager.Delegate = this;

			if (launchOptions != null) {
				if (launchOptions.ContainsKey (UIApplication.LaunchOptionsRemoteNotificationKey)) {
					pushmanager.HandlePushReceived (launchOptions);
				}
			}
			pushmanager.StartLocationTracking ();
			pushmanager.RegisterForPushNotifications ();

		
						// Code to start the Xamarin Test Cloud Agent
			#if ENABLE_TEST_CLOUD
					Xamarin.Calabash.Start ();
			#endif

			return true;
		}
		void ProcessNotification (NSDictionary options, bool fromFinishedLaunching)
		{
			// Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
			if (null != options && options.ContainsKey (new NSString ("aps"))) {
				//Get the aps dictionary
				NSDictionary aps = options.ObjectForKey (new NSString ("aps")) as NSDictionary;

				string alert = string.Empty;

				//Extract the alert text
				// NOTE: If you're using the simple alert by just specifying
				// "  aps:{alert:"alert msg here"}  ", this will work fine.
				// But if you're using a complex alert with Localization keys, etc.,
				// your "alert" object from the aps dictionary will be another NSDictionary.
				// Basically the JSON gets dumped right into a NSDictionary,
				// so keep that in mind.
				if (aps.ContainsKey (new NSString ("alert")))
					alert = (aps [new NSString ("alert")] as NSString).ToString ();

				//If this came from the ReceivedRemoteNotification while the app was running,
				// we of course need to manually process things like the sound, badge, and alert.
				if (!fromFinishedLaunching) {
					//Manually show an alert
					if (!string.IsNullOrEmpty (alert)) {
						UIAlertView avAlert = new UIAlertView ("Notification", alert, null, "OK", null);
						avAlert.Show ();
					}
				}
			}
		}
		
		public override void OnResignActivation (UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground (UIApplication application)
		{
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background exection this method is called instead of WillTerminate when the user quits.
		}

		public override void WillEnterForeground (UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.



		}

		public override void OnActivated (UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.


		}

		public override void WillTerminate (UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
		}
	}

	/// <summary>
	/// This is very important it delegates the orientation so that it can be changed by each View Controller.
	/// View Controllers must override ShouldAutoRotate and SupportedInterfaceOrientations
	/// </summary>
	public class MainNavigationController : UINavigationController
	{
		// the sidebar controller for the app
		public SidebarController SidebarController { get; private set; }

		// the navigation controller
		public NavController NavController { get; private set; }



		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

		}
	}


}

