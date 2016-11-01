using System;
using FavoriteMoviesPCL;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace FavoriteMovies
{
	public partial class MovieDetailViewController : UIViewController
	{
		Movie movieDetail;

		public MovieDetailViewController (Movie movie) : base ("MovieDetailViewController", null)
		{
			movieDetail = movie;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			var posterImageView = Runtime.GetNSObject (NSBundle.MainBundle.LoadNib 
			                     ("MovieDetailViewController",this, null).ValueAt (0)) as MovieDetailViewController;


		
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

