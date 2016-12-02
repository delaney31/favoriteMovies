using SidebarNavigation;
using UIKit;


namespace FavoriteMovies

{
	public partial class NavController : UINavigationController
	{
		// the sidebar controller for the app
		public SidebarController SidebarController { get; set; }

		public NavController () : base ((string)null, null)
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
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.
		}
	}
}