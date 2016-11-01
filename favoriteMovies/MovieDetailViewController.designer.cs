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
        UIKit.UITextView Description { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView numberofVotes { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PlayVideo { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView posterImage { get; set; }

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
        UIKit.UILabel similarMovies { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView SimilarMovies { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView similiarMoviesCollection { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView Title { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Description != null) {
                Description.Dispose ();
                Description = null;
            }

            if (numberofVotes != null) {
                numberofVotes.Dispose ();
                numberofVotes = null;
            }

            if (PlayVideo != null) {
                PlayVideo.Dispose ();
                PlayVideo = null;
            }

            if (posterImage != null) {
                posterImage.Dispose ();
                posterImage = null;
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

            if (similarMovies != null) {
                similarMovies.Dispose ();
                similarMovies = null;
            }

            if (SimilarMovies != null) {
                SimilarMovies.Dispose ();
                SimilarMovies = null;
            }

            if (similiarMoviesCollection != null) {
                similiarMoviesCollection.Dispose ();
                similiarMoviesCollection = null;
            }

            if (Title != null) {
                Title.Dispose ();
                Title = null;
            }
        }
    }
}