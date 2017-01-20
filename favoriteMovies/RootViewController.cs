using System;
using System.Drawing;
using UIKit;
using Foundation;
using CoreGraphics;
using SidebarNavigation;
using System.Collections.ObjectModel;
using FavoriteMoviesPCL;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FavoriteMovies
{
	public partial class RootViewController : UIViewController
	{
		// the sidebar controller for the app
		public SidebarController SidebarController { get; private set; }

		//// the navigation controller
		public NavController NavController { get; private set; }
		//// the tab controller
		public MovieTabBarController TabController { get; private set; }
		//ObservableCollection<Movie> NowPlaying = new ObservableCollection<Movie>();
		//ObservableCollection<Movie> TopRated = new ObservableCollection<Movie>();
		//ObservableCollection<Movie> Popular = new ObservableCollection<Movie>();
		//ObservableCollection<Movie> MovieLatest = new ObservableCollection<Movie>();

		//int Page;

		//public static int RandomNumber (int min, int max)
		//{
		//	Random random = new Random (); return random.Next (min, max);

		//}

		public RootViewController ()
		{
			
			//try {
			//	Page = RandomNumber (1, 50);

			//	var task = Task.Run (async () => {
			//		NowPlaying = await MovieService.GetMoviesAsync (MovieService.MovieType.NowPlaying, 1);
			//		TopRated = await MovieService.GetMoviesAsync (MovieService.MovieType.TopRated, Page);
			//		Popular = await MovieService.GetMoviesAsync (MovieService.MovieType.Popular, Page);
			//		MovieLatest = await MovieService.GetMoviesAsync (MovieService.MovieType.Upcoming, Page);
			//		//TVNowAiring = await MovieService.GetMoviesAsync (MovieService.MovieType.TVLatest, Page);
			//	});
			//	TimeSpan ts = TimeSpan.FromMilliseconds (4000);
			//	task.Wait (ts);
			//	if (!task.Wait (ts))
			//		Console.WriteLine ("The timeout interval elapsed in RootViewController.");
			//} catch (Exception e) {
			//	Debug.WriteLine (e.Message);

			//}
		}
		public override bool ShouldAutorotate ()
		{
			return base.ShouldAutorotate ();
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			//// create a slideout navigation controller with the top navigation controller and the menu view controller
			NavController = new NavController ();
			NavController.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			NavController.NavigationBar.TintColor = UIColor.White;
			NavController.NavigationBar.Translucent = true;
			NavController.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.White
			};

			//var mainView = new MainViewController (TopRated, NowPlaying, Popular, MovieLatest, Page);
			var mainView = new MainViewController ();
			mainView.Title = "Movies";
			mainView.TabBarItem.SetFinishedImages(UIImage.FromBundle ("ic_movie.png"),UIImage.FromBundle ("ic_movie.png"));


			var tab2 = new NewsFeedViewController ();
			tab2.Title = "Hot";
			tab2.View.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			tab2.TabBarItem.SetFinishedImages (UIImage.FromBundle ("ic_whatshot.png"), UIImage.FromBundle ("ic_whatshot.png"));
			var uinc2 = new UINavigationController (tab2);
			uinc2.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			uinc2.NavigationBar.TintColor = UIColor.White;
			uinc2.NavigationBar.Translucent = true;
			uinc2.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.White
			};
			Console.WriteLine (uinc2.ViewControllers.Length);

			var tab3 = new UIViewController ();
			tab3.Title = "Memes";
			tab3.View.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			tab3.TabBarItem.SetFinishedImages (UIImage.FromBundle ("ic_photo.png"), UIImage.FromBundle ("ic_photo.png"));


			var uinc3 = new UINavigationController (tab3);
			uinc3.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			uinc3.NavigationBar.TintColor = UIColor.White;
			//uinc3.NavigationBar.Translucent = true;
			uinc3.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.White
			};
			Console.WriteLine (uinc3.ViewControllers.Length);

			var tab4 = new MovieFriendsViewController ();
			tab4.Title = "Chat";
			tab4.View.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			tab4.TabBarItem.SetFinishedImages (UIImage.FromBundle ("ic_chat.png"), UIImage.FromBundle ("ic_chat.png"));

			var uinc4 = new UINavigationController (tab4);
		
			uinc4.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			uinc4.NavigationBar.TintColor = UIColor.White;
			uinc4.NavigationBar.Translucent = true;
			uinc4.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.White
			};

			var tab5 = new ConnectViewController ();
			tab5.Title = "Connect";
			tab5.View.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			tab5.TabBarItem.SetFinishedImages (UIImage.FromBundle ("ic_person_add.png"),UIImage.FromBundle ("ic_person_add.png"));

			//tab4.TabBarItem = new UITabBarItem (UITabBarSystemItem.Contacts,0);
			//	tab4.TabBarItem.SetFinishedImages ();


			var uinc5 = new UINavigationController (tab5);
			uinc5.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			uinc5.NavigationBar.TintColor = UIColor.White;
			//uinc4.NavigationBar.Translucent = true;
			uinc5.NavigationBar.TitleTextAttributes = new UIStringAttributes () 
			{
				ForegroundColor = UIColor.White
			};

			var tabs = new UIViewController [] {
				mainView,uinc4, uinc2,uinc3,uinc5};
			TabController = new MovieTabBarController ();


			TabController.NavigationItem.SetRightBarButtonItem (
				new UIBarButtonItem (UIImage.FromBundle ("threelines")
					, UIBarButtonItemStyle.Plain
					, (sender, args) => {
						SidebarController.ToggleMenu ();

					}), true);
			try 
			{
				TabController.SetViewControllers (tabs, true);
			} catch (Exception ex) 
			{
				Debug.Write (ex.Message);
			}


			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			SidebarController = new SidebarController (this,NavController , new SideMenuController ());
			SidebarController.MenuWidth = 220;
			SidebarController.MenuLocation = SidebarController.MenuLocations.Right;
			SidebarController.ReopenOnRotate = false;
			SidebarController.HasShadowing = true;
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			NavController.PushViewController (TabController, true);


		}
	}
}

