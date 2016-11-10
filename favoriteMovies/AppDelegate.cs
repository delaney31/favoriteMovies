using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using Mono.Data.Sqlite;
using SQLite;
using UIKit;

namespace FavoriteMovies
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register ("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		ObservableCollection<Movie> NowPlaying;
		ObservableCollection<Movie> TopRated;
		ObservableCollection<Movie> Similar;
		ObservableCollection<Movie> Popular;

		static int RandomNumber (int min, int max)
		{
			Random random = new Random (); return random.Next (min, max);

		}
		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method
			window = new UIWindow (UIScreen.MainScreen.Bounds);


			var task = Task.Run (async () => {
				NowPlaying = await MovieService.GetMoviesAsync (MovieService.MovieType.NowPaying,RandomNumber(1,50));
				TopRated = await MovieService.GetMoviesAsync (MovieService.MovieType.TopRated,RandomNumber (1, 50));
				Popular = await MovieService.GetMoviesAsync (MovieService.MovieType.Popular,RandomNumber (1, 50));
			});
			task.Wait ();

			var nav = new MainNavigationController ();
			//var nav = new UINavigationController ();
			//var scrollView = new UIScrollView ();
		//	scrollView.AddSubview (new TopRatedCollectionViewController (flowLayout, TopRated, NowPlaying, Popular).View);

			nav.AddChildViewController (new MainCollectionViewController (TopRated, NowPlaying, Popular));
			nav.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			nav.View.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			nav.NavigationBar.TintColor = UIColor.White;
			nav.NavigationBar.Translucent = false;
			nav.NavigationBar.TopItem.Title = UIColorExtensions.TITLE;
			nav.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 18),
				ForegroundColor =UIColor.White//= UIColor.Clear.FromHexString (UIColorExtensions.TITLE_COLOR, 1.0f)
			};

			MovieService.Database = Path.Combine (FileHelper.GetLocalStoragePath (), "MovieEntries.db3");


			window.RootViewController = nav;

			window.MakeKeyAndVisible ();

			// Code to start the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start ();
#endif

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
		public override bool ShouldAutorotate ()
		{
			return this.VisibleViewController.ShouldAutorotate();;

		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return this.VisibleViewController.GetSupportedInterfaceOrientations ();
		}
	}


}

