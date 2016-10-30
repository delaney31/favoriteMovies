using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
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
		UICollectionViewFlowLayout flowLayout;

		public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method
			window = new UIWindow (UIScreen.MainScreen.Bounds);


			var task = Task.Run (async () => 
			{ 
				NowPlaying = MovieService.GetMoviesAsync(MovieService.MovieType.NowPaying).Result;
				TopRated = MovieService.GetMoviesAsync (MovieService.MovieType.TopRated).Result;
				Similar = MovieService.GetMoviesAsync (MovieService.MovieType.Similar).Result;
				Popular = MovieService.GetMoviesAsync (MovieService.MovieType.Popular).Result;
			});
			task.Wait ();

			flowLayout = new UICollectionViewFlowLayout () {
				HeaderReferenceSize = new CGSize (50, 50),
				SectionInset = new UIEdgeInsets (140, 23, 150, 100),
				//SectionInset = new UIEdgeInsets (400, 0, 400, 0),
				ScrollDirection = UICollectionViewScrollDirection.Horizontal,
				MinimumInteritemSpacing = 10, // minimum spacing between cells
				MinimumLineSpacing = -5, // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
				ItemSize = new CGSize(110,150)
			};



			var tab1 = new SimpleCollectionViewController (flowLayout, TopRated, NowPlaying, Similar, Popular);
			var tab2 = new SecondViewController (flowLayout);


			tab1.Title = "Movies";
			tab1.TabBarItem = new UITabBarItem (UITabBarSystemItem.TopRated, 0);
			//tab1.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);

			tab2.Title = "Favorites";
			tab2.TabBarItem = new UITabBarItem (UITabBarSystemItem.Favorites, 0);
			//tab2.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);


			var tab = new UITabBarController();
			tab.AddChildViewController (tab1);
			tab.AddChildViewController (tab2);

			var nav = new UINavigationController ();

			nav.AddChildViewController (tab);
			nav.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			nav.NavigationBar.TintColor = UIColor.White;
			nav.NavigationBar.Translucent = false;
			nav.NavigationBar.TopItem.Title = UIColorExtensions.TITLE;
			UINavigationBar.Appearance.SetTitleTextAttributes (new UITextAttributes () {
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 18),
				TextColor = UIColor.Clear.FromHexString (UIColorExtensions.TITLE_COLOR, 1.0f)
			});



			window.RootViewController = nav;

			window.MakeKeyAndVisible ();

			// Code to start the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
			Xamarin.Calabash.Start();
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


	
}

