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

		public MovieDetailViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		[Export ("initWithCoder:")]
		public MovieDetailViewController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public MovieDetailViewController () : base("MovieDetailViewController", null)
		{
			Initialize ();
		}

		public MovieDetailViewController (Movie movie)
		{
			movieDetail = movie;
		}



		void Initialize ()
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			posterImage = new UIImageView ();
			posterImage.ContentMode = UIViewContentMode.ScaleToFill;
			posterImage.Layer.BorderWidth = 1.0f;
			posterImage.Layer.BorderColor = UIColor.White.CGColor;
			posterImage.ClipsToBounds = true;
			posterImage.Image = MovieCell.GetImage (movieDetail.PosterPath);

			MovieDescription.Text = movieDetail.Overview;
			MovieTitle.Text = movieDetail.Title;
			releaseDate.Text = movieDetail.ReleaseDate.ToString ("ddd d MMM");
		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

