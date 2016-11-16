using System.IO;
using FavoriteMoviesPCL;
using Foundation;
using SidebarNavigation;
using UIKit;

namespace FavoriteMovies
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations


		public RootViewController RootViewController {
			get { return Window.RootViewController as RootViewController; }
		}
		public override UIWindow Window {
			get;
			set;
		}


		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method
			Window = new UIWindow (UIScreen.MainScreen.Bounds);

			MovieService.Database = Path.Combine (FileHelper.GetLocalStoragePath (), "MovieEntries.db3");

			// create a new window instance based on the screen size
			Window = new UIWindow (UIScreen.MainScreen.Bounds);

			// If you have defined a root view controller, set it here:
			Window.RootViewController = new RootViewController ();

			// make the window visible
			Window.MakeKeyAndVisible ();

			//			// Code to start the Xamarin Test Cloud Agent
			//#if ENABLE_TEST_CLOUD
			//			Xamarin.Calabash.Start ();
			//#endif

			return true;
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
		private UIStoryboard _storyboard;
		// the navigation controller
		public NavController NavController { get; private set; }

		// the storyboard
		public override UIStoryboard Storyboard {
			get {
				if (_storyboard == null)
					_storyboard = UIStoryboard.FromName ("Phone", null);
				return _storyboard;
			}
		}

		public override bool ShouldAutorotate ()
		{
			return this.VisibleViewController.ShouldAutorotate (); ;

		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return this.VisibleViewController.GetSupportedInterfaceOrientations ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var introController = (IntroController)Storyboard.InstantiateViewController ("IntroController");
			var menuController = (MenuController)Storyboard.InstantiateViewController ("MenuController");

			// create a slideout navigation controller with the top navigation controller and the menu view controller
			NavController = new NavController ();
			NavController.PushViewController (introController, false);
			SidebarController = new SidebarController (this, NavController, menuController);
			SidebarController.MenuWidth = 220;
			SidebarController.ReopenOnRotate = false;
		}
	}


}

