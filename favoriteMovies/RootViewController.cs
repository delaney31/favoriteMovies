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
		ObservableCollection<Movie> NowPlaying;
		ObservableCollection<Movie> TopRated;
		ObservableCollection<Movie> Popular;
		ObservableCollection<Movie> MovieLatest;
		int Page;

		public static int RandomNumber (int min, int max)
		{
			Random random = new Random (); return random.Next (min, max);

		}
		public RootViewController() : base(null, null)
		{
			try {
				Page = RandomNumber (1, 50);

				var task = Task.Run (async () => {
					NowPlaying = await MovieService.GetMoviesAsync (MovieService.MovieType.NowPlaying, 1);
					TopRated = await MovieService.GetMoviesAsync (MovieService.MovieType.TopRated, Page);
					Popular = await MovieService.GetMoviesAsync (MovieService.MovieType.Popular, Page);
					MovieLatest = await MovieService.GetMoviesAsync (MovieService.MovieType.Upcoming, Page);
					//TVNowAiring = await MovieService.GetMoviesAsync (MovieService.MovieType.TVLatest, Page);
				});
				task.Wait ();
			} catch(Exception e )
			{
				Debug.WriteLine (e.Message);

			}
		}
		public override bool ShouldAutorotate ()
		{
			return base.ShouldAutorotate ();
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// create a slideout navigation controller with the top navigation controller and the menu view controller
			NavController = new NavController ();
			NavController.NavigationBar.BarTintColor= UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			NavController.View.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			NavController.NavigationBar.TintColor = UIColor.White;
			NavController.NavigationBar.Translucent = true;
			NavController.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.White
			};
			NavController.PushViewController (new MainViewController (TopRated, NowPlaying, Popular, MovieLatest,Page), false);
			SidebarController = new SidebarController (this, NavController, new SideMenuController ());
			SidebarController.MenuWidth = 220;
			SidebarController.MenuLocation = SidebarController.MenuLocations.Right;
			SidebarController.ReopenOnRotate = false;
			SidebarController.HasShadowing = true;



		}
	}
}

