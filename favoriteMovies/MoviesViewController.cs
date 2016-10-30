using System;
using CoreGraphics;
using UIKit;

namespace FavoriteMovies
{
	
	public class MoviesViewController : UIViewController
	{
		UIWindow window;
		UICollectionViewFlowLayout flowLayout;
		UICollectionViewController simpleCollectionViewController;
		public MoviesViewController () 
		{
			
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Flow Layout
			flowLayout = new UICollectionViewFlowLayout () {
				HeaderReferenceSize = new CGSize (50, 50),
				SectionInset = new UIEdgeInsets (20, 20, 20, 20),
				ScrollDirection = UICollectionViewScrollDirection.Vertical,
				MinimumInteritemSpacing = 50, // minimum spacing between cells
				MinimumLineSpacing = 50 // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
			};

			var user = new SimpleCollectionViewController (flowLayout);
			this.NavigationController.PushViewController (user, true);
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}
