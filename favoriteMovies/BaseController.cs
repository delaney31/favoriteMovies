using System;
using Foundation;
using UIKit;

namespace FavoriteMovies
{
	public partial class BaseController : UIViewController
	{

		protected const float BackGroundColorAlpha = 1.0f;
		// provide access to the sidebar controller to all inheriting controllers

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
		//// provide access to the storyboard to all inheriting controllers
		//public override UIStoryboard Storyboard {
		//	get {
		//		return (UIApplication.SharedApplication.Delegate as AppDelegate).NavController.Storyboard;
		//	}
		//}

		public BaseController (IntPtr handle) : base (handle)
		{
		}
		public BaseController ()
		{
		}
		public BaseController (string nibName, NSBundle bundle) : base (nibName, bundle)
		{
		}
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);



		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			NavigationItem.SetRightBarButtonItem (
				new UIBarButtonItem (UIImage.FromBundle ("threelines")
					, UIBarButtonItemStyle.Plain
					, (sender, args) => {
						SidebarController.ToggleMenu ();

					}), true);
		}
	}
}
