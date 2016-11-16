// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace FavoriteMovies
{
    [Register ("MovieDetailsViewController")]
    partial class MovieDetailsViewController
    {
        [Outlet]
        UIKit.UILabel dateOpenView { get; set; }


        [Outlet]
        UIKit.UILabel descriptView { get; set; }


        [Outlet]
        UIKit.UIScrollView detailsScrollView { get; set; }


        [Outlet]
        UIKit.UILabel movieTitle { get; set; }


        [Outlet]
        UIKit.UIButton playVideoButt { get; set; }


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

            if (playVideoButt != null) {
                playVideoButt.Dispose ();
                playVideoButt = null;
            }

            if (posterImage != null) {
                posterImage.Dispose ();
                posterImage = null;
            }

            if (saveFavoriteButt != null) {
                saveFavoriteButt.Dispose ();
                saveFavoriteButt = null;
            }

            if (voteResultText != null) {
                voteResultText.Dispose ();
                voteResultText = null;
            }
        }
    }
}