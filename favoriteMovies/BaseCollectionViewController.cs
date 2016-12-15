using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
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
		UIViewController viewController;

		protected BaseCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies, UIViewController vc) : base (layout)
		{
			_items = movies;
			this.viewController = vc;
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var longPress = new UILongPressGestureRecognizer(HandleAction);
			//longPress.MinimumPressDuration = .5f;
			longPress.DelaysTouchesBegan = true;
			CollectionView.AddGestureRecognizer (longPress);



		}
		public Task<bool> ShowAlert (string title, string message)
		{
			var tcs = new TaskCompletionSource<bool> ();

			UIApplication.SharedApplication.InvokeOnMainThread (new Action (() => {
				UIAlertView alert = new UIAlertView (title, message, null, NSBundle.MainBundle.LocalizedString ("Cancel", "Cancel"),
									NSBundle.MainBundle.LocalizedString ("OK", "OK"));
				alert.Clicked += (sender, buttonArgs) => tcs.SetResult (buttonArgs.ButtonIndex != alert.CancelButtonIndex);
				alert.Show ();
			}));

			return tcs.Task;
		}
		async void HandleAction (UILongPressGestureRecognizer lpgr)
		{
			if (lpgr.State != UIGestureRecognizerState.Ended)
				return;



				var p = lpgr.LocationInView (CollectionView);

				var indexPath = CollectionView.IndexPathForItemAtPoint (p);

				if (indexPath == null)
					Console.WriteLine ("Could not find index path");
				else {
					MovieListPickerViewController.DeleteAll (_items [indexPath.Row].CustomListID, _items [indexPath.Row].Id);
					var customListId = _items [indexPath.Row].CustomListID;

					bool accepted = await ShowAlert ("Confirm", "Are you sure you want to delete this movie?");
					Console.WriteLine ("Selected button {0}", accepted ? "Accepted" : "Canceled");
					if (accepted) 
					{
						_items.RemoveAt (indexPath.Row);

						CollectionView.DeleteItems (new NSIndexPath [] { indexPath });

						//cell.RemoveFromSuperview ();
						CollectionView.ReloadData ();
					}
					if (_items.Count == 0) 
					{
						MovieListPickerViewController.DeleteCustomList (customListId);
						MainViewController.NewCustomListToRefresh = 0;
						viewController.ViewWillAppear (true);
					}

				}

		}


		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			try {
				var row = _items [indexPath.Row];
				var bounds = UIScreen.MainScreen.Bounds;
				//// show the loading overlay on the UI thread using the correct orientation sizing
				//loadingOverlay = new LoadingOverlay (bounds);
				//View.Add (loadingOverlay);
				viewController.NavigationController.PushViewController (new MovieDetailViewController (row, false), true);
				//this.ParentViewController.NavigationController.PushViewController(new MovieDetailViewController (row, false), true);

				//loadingOverlay.Hide ();
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
		public PopularCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies, UIViewController vc) : base (layout, movies, vc)
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
			return (_items !=null?_items.Count:0);
		}

	}

	/// <summary>
	/// This is the view controller for the TopRated Movies scrollable list of movies
	/// </summary>
	public class TopRatedCollectionViewController : BaseCollectionViewController
	{
		ObservableCollection<Movie> _items { get; set; }
		public static NSString movieCellId = new NSString ("TopRatedMovieCell");
		public TopRatedCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies, UIViewController vc) : base (layout, movies, vc)
		{
			_items = movies;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return (_items!=null?_items.Count:0);
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
				Console.WriteLine (e.Message);
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
		UIViewController viewController;
		public FavoritesViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies, UIViewController vc) : base (layout, movies, vc)
		{
			_items = movies;
			this.viewController = vc;
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
				viewController.NavigationController.PushViewController (new MovieDetailViewController (row, true), true);

			} catch (Exception e) {
				Debug.WriteLine (e.Message);
			}
		}
	}
	public class MovieLatestViewController : BaseCollectionViewController
	{
		ObservableCollection<Movie> _items { get; set; }
		public static NSString movieCellId = new NSString ("LatestMovieCell");
		public MovieLatestViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies, UIViewController vc) : base (layout, movies, vc)
		{
			_items = movies;
		}
		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{

			return (_items != null?_items.Count:0);
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
		public NowPlayingCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies, UIViewController vc) : base (layout, movies, vc)
		{
			_items = movies;
		}
		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return (_items!=null?_items.Count:0);
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
	public class SimilarCollectionViewController : BaseCollectionViewController
	{
		ObservableCollection<Movie> _items { get; set; }
		public static NSString movieCellId = new NSString ("SimilarMovieCell");
		public SimilarCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies, UIViewController vc) : base (layout, movies, vc)
		{
			_items = movies;
		}
		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return (_items!=null?_items.Count:0);
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
