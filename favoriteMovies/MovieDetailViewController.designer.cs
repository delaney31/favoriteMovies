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
    [Register ("MovieDetailViewController")]
    partial class MovieDetailViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView MovieDescription { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        FavoriteMovies.MovieDetailViewController movieDetails { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView MovieTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView numberofVotes { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PlayVideo { get; set; }

        
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView rating { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView releaseDate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SaveToFavorites { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView scrollView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel similarMovies { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView similiarMoviesCollection { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (MovieDescription != null) {
                MovieDescription.Dispose ();
                MovieDescription = null;
            }

            if (movieDetails != null) {
                movieDetails.Dispose ();
                movieDetails = null;
            }

            if (MovieTitle != null) {
                MovieTitle.Dispose ();
                MovieTitle = null;
            }

            if (numberofVotes != null) {
                numberofVotes.Dispose ();
                numberofVotes = null;
            }

            if (PlayVideo != null) {
                PlayVideo.Dispose ();
                PlayVideo = null;
            }


            if (rating != null) {
                rating.Dispose ();
                rating = null;
            }

            if (releaseDate != null) {
                releaseDate.Dispose ();
                releaseDate = null;
            }

            if (SaveToFavorites != null) {
                SaveToFavorites.Dispose ();
                SaveToFavorites = null;
            }

            if (scrollView != null) {
                scrollView.Dispose ();
                scrollView = null;
            }

            if (similarMovies != null) {
                similarMovies.Dispose ();
                similarMovies = null;
            }

            if (similiarMoviesCollection != null) {
                similiarMoviesCollection.Dispose ();
                similiarMoviesCollection = null;
            }
        }
    }
}