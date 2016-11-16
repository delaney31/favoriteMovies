using System;
using System.Drawing;
using UIKit;
using Foundation;
using CoreGraphics;
using SidebarNavigation;
using System.Collections.ObjectModel;
using FavoriteMoviesPCL;
using System.Threading.Tasks;

namespace FavoriteMovies
{
	public partial class RootViewController : UIViewController
	{
		private UIStoryboard _storyboard;

		ObservableCollection<Movie> NowPlaying;
		ObservableCollection<Movie> TopRated;
		ObservableCollection<Movie> Similar;
		ObservableCollection<Movie> Popular;
		ObservableCollection<Movie> MovieLatest;
		ObservableCollection<Movie> TVNowAiring;
		int Page;
		static int RandomNumber (int min, int max)
		{
			Random random = new Random (); return random.Next (min, max);

		}
		// the sidebar controller for the app
		public SidebarNavigation.SidebarController SidebarController { get; private set; }

		// the navigation controller
		public NavController NavController { get; private set; }

		// the storyboard
		public override UIStoryboard Storyboard {
			get {
				if (_storyboard == null)
					_storyboard = UIStoryboard.FromName("Phone", null);
				return _storyboard;
			}
		}

		public RootViewController() : base(null, null)
		{
			Page = RandomNumber (1, 50);

			var task = Task.Run (async () => {
				NowPlaying = await MovieService.GetMoviesAsync (MovieService.MovieType.NowPaying, Page);
				TopRated = await MovieService.GetMoviesAsync (MovieService.MovieType.TopRated, Page);
				Popular = await MovieService.GetMoviesAsync (MovieService.MovieType.Popular, Page);
				MovieLatest = await MovieService.GetMoviesAsync (MovieService.MovieType.Upcoming, Page);
				//TVNowAiring = await MovieService.GetMoviesAsync (MovieService.MovieType.TVLatest, Page);
			});
			task.Wait ();
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

			//var introControllero = (IntroController)Storyboard.InstantiateViewController("IntroController");
			var menuController = (MenuController)Storyboard.InstantiateViewController("MenuController");
			var introController = new MainViewController (TopRated, NowPlaying, Popular, MovieLatest, TVNowAiring, Page);

			NavController = new NavController();
			NavController.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			NavController.View.BackgroundColor = UIColor.White;//UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			NavController.NavigationBar.TintColor = UIColor.White;
			NavController.NavigationBar.Translucent = true;
			//nav.NavigationBar.TopItem.Title = UIColorExtensions.TITLE;
			//NavController.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
			//	Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 18),
			//	ForegroundColor =UIColor.White//= UIColor.Clear.FromHexString (UIColorExtensions.TITLE_COLOR, 1.0f)

			// create a slideout navigation controller with the top navigation controller and the menu view controller

			NavController.PushViewController(introController, false);

			SidebarController = new SidebarNavigation.SidebarController(this, NavController, menuController);
			SidebarController.MenuWidth = 220;
			SidebarController.ReopenOnRotate = false;
		}
	}
}

