// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace FavoriteMovies
{
	[Register ("MovieDetailsViewController")]
	partial class MovieDetailsViewController
	{
		[Outlet]
		UIKit.UILabel dateOpenView { get; set; }

		[Outlet]
		UIKit.UILabel descReview { get; set; }

		[Outlet]
		UIKit.UILabel descriptView { get; set; }

		[Outlet]
		UIKit.UIScrollView detailsScrollView { get; set; }

		[Outlet]
		UIKit.UILabel movieTitle { get; set; }

		[Outlet]
		UIKit.UIButton AddReviewButt { get; set; }

		[Outlet]
		UIKit.UIImageView posterImage { get; set; }

		[Outlet]
		UIKit.UIButton saveFavoriteButt { get; set; }

		[Outlet]
		UIKit.UILabel similarView { get; set; }

		[Outlet]
		UIKit.UIImageView voteResultImage { get; set; }

		[Outlet]
		UIKit.UILabel voteResultText { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (dateOpenView != null) {
				dateOpenView.Dispose ();
				dateOpenView = null;
			}

			if (descriptView != null) {
				descriptView.Dispose ();
				descriptView = null;
			}

			if (detailsScrollView != null) {
				detailsScrollView.Dispose ();
				detailsScrollView = null;
			}

			if (movieTitle != null) {
				movieTitle.Dispose ();
				movieTitle = null;
			}

			if (AddReviewButt != null) {
				AddReviewButt.Dispose ();
				AddReviewButt = null;
			}

			if (posterImage != null) {
				posterImage.Dispose ();
				posterImage = null;
			}

			if (saveFavoriteButt != null) {
				saveFavoriteButt.Dispose ();
				saveFavoriteButt = null;
			}

			if (similarView != null) {
				similarView.Dispose ();
				similarView = null;
			}

			if (voteResultImage != null) {
				voteResultImage.Dispose ();
				voteResultImage = null;
			}

			if (voteResultText != null) {
				voteResultText.Dispose ();
				voteResultText = null;
			}

			if (descReview != null) {
				descReview.Dispose ();
				descReview = null;
			}
		}
	}
}
