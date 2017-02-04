using System;
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
			//this fixes problem when coming out of full screen after watching a trailer
			NavController.NavigationBar.Frame = new CGRect () { X = 0, Y = 20, Width = 320, Height = 44 };
			//loadPop.Hide ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// show the loading overlay on the UI thread using the correct orientation sizing
			//loadPop = new LoadingOverlay (View.Bounds);
			//View.Add (loadPop);

			// show the loading overlay on the UI thread using the correct orientation sizing
			//loadPop = new LoadingOverlay (View.Bounds);
			//View.Add (loadPop);

				NavigationItem.SetRightBarButtonItem (
					new UIBarButtonItem (UIImage.FromBundle ("threelines")
						, UIBarButtonItemStyle.Plain
						, (sender, args) => {
							SidebarController.ToggleMenu ();

						}), true);



		}
	}

}
