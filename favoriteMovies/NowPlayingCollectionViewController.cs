using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using UIKit;

namespace FavoriteMovies
{/// <summary>
/// This is the View Controller for the scrollable movies under the Now Playing header. It also serves as a base class for the 
	/// Popular movies header
/// </summary>
	public class NowPlayingCollectionViewController : UICollectionViewController
	{
		public static NSString movieNowPlayingCellId = new NSString ("MovieCell");
		public ObservableCollection<Movie> _items { get; internal set; }
		public float FontSize { get; set; }
		public SizeF ImageViewSize { get; set; }
		UINavigationController rootNav;
		//UIWindow window;


		public NowPlayingCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies, UIWindow rootnav):base(layout)
		{
			_items = movies;
			rootNav =(UINavigationController)rootnav.RootViewController;


		}

		public NowPlayingCollectionViewController (UICollectionViewLayout layout,UIWindow rootnav): base (layout)
		{
			rootNav = (UINavigationController)rootnav.RootViewController;

		}
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			CollectionView.AddGestureRecognizer (new UILongPressGestureRecognizer (handleLongPress));
		}
		void handleLongPress (UILongPressGestureRecognizer gestureRecognizer)
		{
			if (gestureRecognizer.State != UIGestureRecognizerState.Ended)
				return;

			CGPoint pointPressed = gestureRecognizer.LocationInView (CollectionView);
			NSIndexPath indexPath = CollectionView.IndexPathForItemAtPoint (pointPressed);
			if (indexPath != null) {
				var row = _items [indexPath.Row];
				UICollectionViewCell cell = CollectionView.CellForItem (indexPath);
				if (row.Favorite == false) {
					//cell.ContentView.Subviews [0].Layer.BorderWidth = 3.0f;
					cell.ContentView.Subviews [0].Layer.BorderColor = UIColor.Orange.CGColor;
					row.Favorite = true;
					// Create the database and a table to hold Favorite information.
					try {
						using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
							db.Insert (row);
						}

					} catch (SQLite.SQLiteException) {

						//create db if not created.
						using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) {
							conn.CreateTable<Movie> ();
							using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
								db.Insert (row);
							}
						}

					} finally 
					{ 
						CollectionView.ReloadData ();
					}


				} else {
					try {
						row.Favorite = false;
						//cell.ContentView.Subviews [0].Layer.BorderWidth = 1.0f;
						cell.ContentView.Subviews [0].Layer.BorderColor = UIColor.White.CGColor;

						using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
							// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
							db.Query<Movie> ("DELETE FROM [Movie] WHERE [id] = " + row.Id);
						}
						CollectionView.ReloadData ();
					} catch (SQLite.SQLiteException) {
						//first time in no favorites yet.
					} finally {
						CollectionView.ReloadData ();
					}
				}


			}

		}
		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			try {
				var row = _items [indexPath.Row];

				rootNav.PushViewController (new MovieDetailsViewController (row), true);
			} catch (Exception e) 
			{
				Debug.WriteLine (e.Message);
			}
		}


		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return _items.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (MovieCell)collectionView.DequeueReusableCell (movieNowPlayingCellId, indexPath);
			var row = _items [indexPath.Row];
			cell.UpdateRow (row, UIColorExtensions.HEADER_FONT_SIZE);
			return cell;
		}

	}
	/// <summary>
	/// This is the view controller for the Popular Movies scrollable list of movies
	/// </summary>
	public class PopularCollectionViewController : NowPlayingCollectionViewController
	{
		
		public static NSString popularCellId = new NSString ("MovieCell");
		public PopularCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies, UIWindow rootnav) : base (layout,rootnav)
		{
			_items = movies;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			
			var cell = (MovieCell)collectionView.DequeueReusableCell (popularCellId, indexPath);
			var row = _items [indexPath.Row];
			cell.UpdateRow (row, UIColorExtensions.HEADER_FONT_SIZE);
			return cell;
		}



	}

	/// <summary>
	/// This is the view controller for the Popular Movies scrollable list of movies
	/// </summary>
	public class TopRatedCollectionViewController : NowPlayingCollectionViewController
	{

		public static NSString topRatedCellId = new NSString ("MovieCell");
		public TopRatedCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies, UIWindow rootnav) : base (layout, rootnav)
		{
			_items = movies;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{

			var cell = (MovieCell)collectionView.DequeueReusableCell (topRatedCellId, indexPath);
			var row = _items [indexPath.Row];
			cell.UpdateRow (row, UIColorExtensions.HEADER_FONT_SIZE);
			return cell;
		}



	}


}
