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
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView dateOpenView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView descriptView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView detailsScrollView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView movieTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton playVideoButt { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView posterImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton saveFavoriteButt { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel similarView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView voteResultImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView voteResultText { get; set; }

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
        }
    }
}