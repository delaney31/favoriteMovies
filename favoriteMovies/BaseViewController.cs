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
	public abstract class BaseViewController : UICollectionViewController
	{
		ObservableCollection<Movie> _items { get; set; }
		public float FontSize { get; set; }
		public SizeF ImageViewSize { get; set; }

		//UIWindow window;

		public BaseViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies):base(layout)
		{
			_items = movies;

		}

		public BaseViewController (UICollectionViewLayout layout): base (layout)
		{
			
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			try {
				var row = _items [indexPath.Row];
				((UINavigationController)(UIApplication.SharedApplication.Delegate as AppDelegate).Window.RootViewController).PushViewController (new MovieDetailsViewController (row), true);

			} catch (Exception e) {
				Debug.WriteLine (e.Message);
			}
		}



	}
	/// <summary>
	/// This is the view controller for the Popular Movies scrollable list of movies
	/// </summary>
	public class PopularCollectionViewController : BaseViewController
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
	public class TopRatedCollectionViewController : BaseViewController
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
	public class FavoritesViewController : BaseViewController
	{
		ObservableCollection<Movie> _items { get; set; }
		public static NSString movieCellId = new NSString ("FavoritesMovieCell");
		public FavoritesViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies) : base (layout, movies)
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
	public class MovieLatestViewController : BaseViewController
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
	public class NowPlayingCollectionViewController : BaseViewController
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
