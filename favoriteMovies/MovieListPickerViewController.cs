using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using SQLite;
using UIKit;

namespace FavoriteMovies
{
	public class MovieListPickerViewController : BaseListViewController
	{

		public MovieListPickerViewController (Movie movieDetail, bool fromAddList) : base (movieDetail, fromAddList)
		{
		}
        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);
          
        }
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			//NavigationItem.Title = "Share Your Movie Lists";
		}
	}

}
