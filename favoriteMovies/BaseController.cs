using System;
using System.Diagnostics;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace FavoriteMovies
{


	public partial class BaseController : UIViewController
	{
		
		protected const float BackGroundColorAlpha = 1.0f;
		// provide access to the sidebar controller to all inheriting controllers
		protected static UIScrollView scrollView = new UIScrollView ();
        protected static UIWindow Window = (UIApplication.SharedApplication.Delegate as AppDelegate).Window;
		public SidebarNavigation.SidebarController SidebarController {
			get {
				//return (UIApplication.SharedApplication.Delegate as AppDelegate).NavController.SidebarController;
				return (UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.SidebarController;
			}
		}

		//// provide access to the navigation controller to all inheriting controllers
		public UINavigationController NavController {
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
			
        }
	
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			
			////this fixes problem when coming out of full screen after watching a trailer
			//NavController.NavigationBar.Frame = new CGRect () { X = 0, Y = 20, Width = 320, Height = 44 };
			////loadPop.Hide ();
			//if ((UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.TabController != null) 
			//{
			//	NewsFeedTableSource.ShowTabBar ((UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.TabController ?? null);
			//	NavController.SetNavigationBarHidden (false, true);
			//}

		}

		
	}

}
