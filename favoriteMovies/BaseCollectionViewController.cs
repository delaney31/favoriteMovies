using System;
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
	public abstract class BaseCollectionViewController : UICollectionViewController
	{
		ObservableCollection<Movie> _items { get; set; }
		public float FontSize { get; set; }
		public SizeF ImageViewSize { get; set; }

		protected BaseCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies) : base (layout)
		{
			_items = movies;

		}



		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			try {
				var row = _items [indexPath.Row];
				((UINavigationController)(UIApplication.SharedApplication.Delegate as AppDelegate).Window.RootViewController).PushViewController (new MovieDetailViewController (row, false), true);

			} catch (Exception e) {
				Debug.WriteLine (e.Message);
			}
		}



	}
	/// <summary>
	/// This is the view controller for the Popular Movies scrollable list of movies
	/// </summary>
	public class PopularCollectionViewController : BaseCollectionViewController
	{
		ObservableCollection<Movie> _items { get; set; }
		public static NSString movieCellId = new NSString ("PopularPlayingMovieCell");
		public PopularCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies) : base (layout, movies)
		{
			_items = movies;
		}
		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (MovieCell)collectionView.DequeueReusableCell (movieCellId, indexPath);
			try {

				var row = _items [indexPath.Row];
				cell.UpdateRow (row);
				return cell;
			} catch (Exception e) {
				Debug.WriteLine (e.Message);

			}
			return cell;
		}
		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return _items.Count;
		}

	}

	/// <summary>
	/// This is the view controller for the Popular Movies scrollable list of movies
	/// </summary>
	public class TopRatedCollectionViewController : BaseCollectionViewController
	{
		ObservableCollection<Movie> _items { get; set; }
		public static NSString movieCellId = new NSString ("TopRatedMovieCell");
		public TopRatedCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies) : base (layout, movies)
		{
			_items = movies;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return _items.Count;
		}
		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (MovieCell)collectionView.DequeueReusableCell (movieCellId, indexPath);
			try {

				var row = _items [indexPath.Row];
				cell.UpdateRow (row);
				return cell;
			} catch (Exception e) {
				Debug.WriteLine (e.Message);

			}
			return cell;
		}

	}

	/// <summary>
	/// This is the view controller for the Favorite Movies scrollable list
	/// </summary>
	public class FavoritesViewController : BaseCollectionViewController
	{
		ObservableCollection<Movie> _items { get; set; }
		public static NSString movieCellId = new NSString ("FavoritesMovieCell");

		public FavoritesViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies) : base (layout, movies)
		{
			_items = movies;
		}
		protected override void Dispose (bool disposing)
		{
			Console.WriteLine ("Disposed FavoritesViewController");
			base.Dispose (disposing);
		}

		~FavoritesViewController ()
		{
			Console.WriteLine ("Finalized FavoritesViewController");
		}
		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return _items.Count;
		}
		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (MovieCell)collectionView.DequeueReusableCell (movieCellId, indexPath);
			try {

				var row = _items [indexPath.Row];
				cell.UpdateRow (row);
				return cell;
			} catch (Exception e) {
				Debug.WriteLine (e.Message);
			}
			return cell;
		}
		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			try {
				var row = _items [indexPath.Row];
				//MainViewController.OldCustomListToRefresh = this.Row;
				((UINavigationController)(UIApplication.SharedApplication.Delegate as AppDelegate).Window.RootViewController).PushViewController (new MovieDetailViewController (row, true), true);

			} catch (Exception e) {
				Debug.WriteLine (e.Message);
			}
		}
	}
	public class MovieLatestViewController : BaseCollectionViewController
	{
		ObservableCollection<Movie> _items { get; set; }
		public static NSString movieCellId = new NSString ("LatestMovieCell");
		public MovieLatestViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies) : base (layout, movies)
		{
			_items = movies;
		}
		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return _items.Count;
		}
		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (MovieCell)collectionView.DequeueReusableCell (movieCellId, indexPath);
			try {

				var row = _items [indexPath.Row];
				cell.UpdateRow (row);
				return cell;
			} catch (Exception e) {
				Debug.WriteLine (e.Message);

			}
			return cell;
		}


	}
	public class NowPlayingCollectionViewController : BaseCollectionViewController
	{
		ObservableCollection<Movie> _items { get; set; }
		public static NSString movieCellId = new NSString ("NowPlayingMovieCell");
		public NowPlayingCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies) : base (layout, movies)
		{
			_items = movies;
		}
		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return _items.Count;
		}
		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (MovieCell)collectionView.DequeueReusableCell (movieCellId, indexPath);
			try {

				var row = _items [indexPath.Row];
				cell.UpdateRow (row);
				return cell;
			} catch (Exception e) {
				Debug.WriteLine (e.Message);

			}
			return cell;
		}


	}


}
