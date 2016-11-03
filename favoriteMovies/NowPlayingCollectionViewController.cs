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
		public static NSString movieNowPlayingCellId = new NSString ("NowPlayingMovieCell");
		public ObservableCollection<Movie> _items { get; internal set; }
		public float FontSize { get; set; }
		public SizeF ImageViewSize { get; set; }
		UINavigationController rootNav;
		UIWindow window;


		public NowPlayingCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies, UIWindow rootnav):base(layout)
		{
			_items = movies;
			rootNav =(UINavigationController)rootnav.RootViewController;
			window = rootnav;

		}

		public NowPlayingCollectionViewController (UICollectionViewLayout layout,UIWindow rootnav): base (layout)
		{
			rootNav = (UINavigationController)rootnav.RootViewController;
			window = rootnav;
		}
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			TopRatedCollectionViewController.ShowLabels ();
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

		}
		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
			try {
				var row = _items [indexPath.Row];
				TopRatedCollectionViewController.HideLabels ();
				var views = window.Subviews;
				foreach (var view in views) {
					if(view is UICollectionView)
					   view.RemoveFromSuperview ();
				}
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
			var cell = (NowPlayingMovieCell)collectionView.DequeueReusableCell (movieNowPlayingCellId, indexPath);
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
		
		public static NSString popularCellId = new NSString ("PopularMovieCell");
		public PopularCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> movies, UIWindow rootnav) : base (layout,rootnav)
		{
			_items = movies;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			
			var cell = (PopularMovieCell)collectionView.DequeueReusableCell (popularCellId, indexPath);
			var row = _items [indexPath.Row];
			cell.UpdateRow (row, UIColorExtensions.HEADER_FONT_SIZE);
			return cell;
		}



	}

}
