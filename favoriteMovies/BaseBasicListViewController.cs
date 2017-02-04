using System;
using Foundation;
using UIKit;

namespace FavoriteMovies
{
	public class BaseBasicListViewController: UIViewController
	{
		public UITableView tableView;
		protected LoadingOverlay loadPop;
		protected UITableViewSource tableSource;
		public SidebarNavigation.SidebarController SidebarController {
			get {
				//return (UIApplication.SharedApplication.Delegate as AppDelegate).NavController.SidebarController;
				return (UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.SidebarController;
			}
		}

		//// provide access to the navigation controller to all inheriting controllers
		public NavController NavController {
			get {
				//return (UIApplication.SharedApplication.Delegate as AppDelegate).NavController;
				return (UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.NavController;
			}
		}
		public MovieTabBarController TabController {
			get {
				//return (UIApplication.SharedApplication.Delegate as AppDelegate).NavController.SidebarController;
				return (UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.TabController;
			}
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			tableView = new UITableView (View.Bounds);
			tableView.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin;
			tableView.AllowsSelectionDuringEditing = true;
			tableView.Frame = View.Bounds;


		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);


		}

	}

}
