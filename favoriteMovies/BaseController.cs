using System;
using System.Drawing;

using Foundation;
using UIKit;

namespace FavoriteMovies
{
	public partial class BaseController : UIViewController
	{
		// provide access to the sidebar controller to all inheriting controllers
		protected SidebarNavigation.SidebarController SidebarController {
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.SidebarController;
			}
		}

		//// provide access to the navigation controller to all inheriting controllers
		protected NavController NavController {
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.NavController;
			}
		}

		// provide access to the storyboard to all inheriting controllers
		public override UIStoryboard Storyboard {
			get {
				return (UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.Storyboard;
			}
		}

		public BaseController (IntPtr handle) : base (handle)
		{
		}
		public BaseController ()
		{
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
