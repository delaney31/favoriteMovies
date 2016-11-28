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
		// the sidebar controller for the app
		public SidebarController SidebarController { get; private set; }

		//// the navigation controller
		public NavController NavController { get; private set; }

		public RootViewController() : base(null, null)
		{
			
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


		}
	}
}

