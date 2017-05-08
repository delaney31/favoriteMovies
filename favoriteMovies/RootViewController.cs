using System;
using UIKit;
using SidebarNavigation;
using System.Diagnostics;
using System.Collections.Generic;
using Foundation;
using LoginScreen;

namespace FavoriteMovies
{
	public class LogOutController : UIViewController
	{
		public LogOutController ()
		{

		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			LogOut ();
		} 

		public void LogOut ()
		{
			LoginScreenControl<CredentialsProvider, DefaultLoginScreenMessages>.Activate (this);
			this.NavigationController.PopToRootViewController (true);

		}
	}
	public partial class RootViewController : UIViewController
	{
		// the sidebar controller for the app
		public SidebarController SidebarController { get; private set; }

		//// the navigation controller
		public UINavigationController NavController { get; private set; }
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
			NavController = new UINavigationController ();
			NavController.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			NavController.NavigationBar.TintColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			//NavController.NavigationBar.Translucent = true;
			NavController.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f)
			};


			var notifications = new NotificationsViewController ();
			notifications.Title = "Notifications";
			notifications.TabBarItem.SetFinishedImages (UIImage.FromBundle ("ic_movie.png"), UIImage.FromBundle ("ic_movie.png"));


			var uinc6 = new UINavigationController (notifications);
			uinc6.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			uinc6.NavigationBar.TintColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			//uinc2.NavigationBar.Translucent = true;
			uinc6.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f)
			};

			var mainView = new MainViewController ();
			mainView.Title = "Movies";
			mainView.TabBarItem.SetFinishedImages (UIImage.FromBundle ("ic_movie.png"), UIImage.FromBundle ("ic_movie.png"));


			var uinc1 = new UINavigationController (mainView);
			uinc1.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			uinc1.NavigationBar.TintColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			//uinc2.NavigationBar.Translucent = true;
			uinc1.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f)
			};

			var tab2 = new NewsFeedViewController ();
			tab2.Title = "News";
			tab2.View.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			tab2.TabBarItem.SetFinishedImages (UIImage.FromBundle ("ic_whatshot.png"), UIImage.FromBundle ("ic_whatshot.png"));

			var uinc2 = new UINavigationController (tab2);
			uinc2.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			uinc2.NavigationBar.TintColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			//uinc2.NavigationBar.Translucent = true;
			uinc2.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f)
			};
			Console.WriteLine (uinc2.ViewControllers.Length);

			var tab3 = new UserCloudListViewController ();
			tab3.Title = "Invite";
			tab3.View.BackgroundColor = UIColor.White;
			tab3.TabBarItem.SetFinishedImages (UIImage.FromBundle ("ic_contact_mail.png"), UIImage.FromBundle ("ic_contact_mail.png"));


			var uinc3 = new UINavigationController (tab3);
			uinc3.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			uinc3.NavigationBar.TintColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			uinc3.NavigationBar.Translucent = true;
			uinc3.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f)
			};
			Console.WriteLine (uinc3.ViewControllers.Length);


			var tab4 = new MovieListPickerViewController (null, false);
			tab4.Title = "Lists";
			tab4.View.BackgroundColor = UIColor.White;
			tab4.TabBarItem.SetFinishedImages (UIImage.FromBundle ("ic_list.png"), UIImage.FromBundle ("ic_list.png"));

			var uinc4 = new UINavigationController (tab4);

			uinc4.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			uinc4.NavigationBar.TintColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			uinc4.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f)
			};

			var tab5 = new FindFriendsViewController ();
			tab5.Title = "Connect";
			tab5.View.BackgroundColor = UIColor.White;
			tab5.TabBarItem.SetFinishedImages (UIImage.FromBundle ("ic_person_add.png"), UIImage.FromBundle ("ic_person_add.png"));


			var uinc5 = new UINavigationController (tab5);
			uinc5.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			uinc5.NavigationBar.TintColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			//uinc4.NavigationBar.Translucent = true;
			uinc5.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f)
			};


			var tab7 = new SettingsViewController ();
			tab7.Title = "Profile";
			tab7.View.BackgroundColor = UIColor.White;

			var uinc7 = new UINavigationController (tab7);

			uinc7.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			uinc7.NavigationBar.TintColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			uinc7.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f)
			};

			var tab8 = new TipsViewController ();
			tab8.Title = "Tips";
			tab8.View.BackgroundColor = UIColor.White;

			var uinc8 = new UINavigationController (tab8);

			uinc8.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			uinc8.NavigationBar.TintColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			uinc8.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f)
			};

			var tab9 = new LogOutController ();
			tab9.Title = "Sign Out";
			tab9.View.BackgroundColor = UIColor.White;

			var uinc9 = new UINavigationController (tab9);

			uinc9.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			uinc9.NavigationBar.TintColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			uinc9.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f)
			};

			//LoginScreenControl<CredentialsProvider>.Activate (this);
			var tabs = new UIViewController []
			{
				uinc1,uinc4,uinc5,uinc3,uinc2,uinc6,uinc7,uinc8,uinc9
			};
			//var customizableControllers = new UIViewController [] 
			//{
			//	uinc1,uinc4,uinc5,uinc3,uinc2,uinc6
			//};


			TabController = new MovieTabBarController ();

			TabController.MoreNavigationController.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			TabController.MoreNavigationController.NavigationBar.TintColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			//uinc4.NavigationBar.Translucent = true;
			TabController.MoreNavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes () {
				ForegroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f)
			};


			//TabController.NavigationItem.SetLeftBarButtonItem (
			//	new UIBarButtonItem (UIImage.FromBundle ("threelines")
			//		, UIBarButtonItemStyle.Plain
			//		, (sender, args) => {
			//			SidebarController.ToggleMenu ();

			//		}), true);
			try {

				TabController.ViewControllers = tabs;
				TabController.SelectedViewController = uinc1;

			} catch (Exception ex) {
				Debug.Write (ex.Message);
			}

			UITableView moreList = (UIKit.UITableView)TabController.MoreNavigationController.TopViewController.View;


			if (moreList.Subviews.Length > 0) {
				foreach (UITableViewCell view in moreList.VisibleCells) {
					view.TextLabel.Font = UIFont.FromName (ColorExtensions.TITLE_FONT, ColorExtensions.HEADER_FONT_SIZE);
				}
			}


			// Tell the tab bar which controllers are allowed to customize.
			// If we don't set this, it assumes all controllers are customizable.
			TabController.CustomizableViewControllers = null;

			// If tab bar order has been edited, save to UserPrefs as a comma-seperated list of Tag ids
			// NOTE: assumes Tag id == order in the initial ViewControllers array
			TabController.FinishedCustomizingViewControllers += delegate (object sender, UITabBarCustomizeChangeEventArgs e) {
				Console.WriteLine ("FinishedCustomizingViewControllers - Changed=" + e.Changed);
				if (e.Changed) {
					var count = e.ViewControllers.Length;
					var tabOrderArray = new List<string> ();
					foreach (var viewController in e.ViewControllers) {
						var tag = viewController.TabBarItem.Tag;
						tabOrderArray.Add (tag.ToString ());
						Console.WriteLine ("Tag = " + tag);
					}
					NSArray arr = NSArray.FromStrings (tabOrderArray.ToArray ());
					NSUserDefaults.StandardUserDefaults ["tabBarOrder"] = arr;
				}
			};
			NavController.PushViewController (TabController, true);

			SidebarController = new SidebarController (this, NavController, new SideMenuController ());
			SidebarController.MenuWidth = 200;
			SidebarController.MenuLocation = SidebarController.MenuLocations.Left;
			SidebarController.ReopenOnRotate = true;
			SidebarController.HasShadowing = true;

		}
	}
}

