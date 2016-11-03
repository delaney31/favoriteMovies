using System;
using System.Collections.Generic;
using UIKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using FavoriteMoviesPCL;
using System.Drawing;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FavoriteMovies
{/// <summary>
/// This is the Main View controller for the application. It serves as the Top Rated collection of movies and creates the Now Playing 
	/// Poplular scrollable collections.
/// </summary>
	public class TopRatedCollectionViewController : UICollectionViewController
	{
		static NSString movieTopRatedCellId = new NSString ("TopRatedMovieCell");
		static CGSize HeaderReferenceSize = new CGSize (50, 50);
		static int MinimumInteritemSpacing = 30;
		static int MinimumLineSpacing = -5;
		static CGSize ItemSize = new CGSize (110, 150);
		static CGRect TopRatedLabelFrame = new CGRect (7, 74, 90, 20);
		static CGRect NowPlayingLabelFrame = new CGRect (7, 231, 90, 20);
		static CGRect NowPlayingControllerFrame = new CGRect (7, 235, 400, 150);
		static CGRect PopularLabelFrame = new CGRect (8, 400, 90, 20);
		static CGRect PopularControllerFrame = new CGRect (7, 409, 400, 150);
		const float BackGroundColorAlpha = 1.0f;
		static string TopRated = "Top Rated";
		static string NowPlaying = "Now Playing";
		static string Popular = "Popular";
		const int LabelZPosition = 1;
		const int SectionCount = 1;
		ObservableCollection<Movie> topRated;
		ObservableCollection<Movie> nowPlaying;
		ObservableCollection<Movie> popular;
		UICollectionViewFlowLayout flowLayout;
		NowPlayingCollectionViewController nowPlayingController;
		PopularCollectionViewController popularController;

		UIWindow window;
		static UILabel TopRatedLabel;
		static UILabel PlayingNowLabel;
		static UILabel PopularLabel;



		public TopRatedCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> topRated, ObservableCollection<Movie> nowPlaying, ObservableCollection<Movie> popular ): base (layout)
		{
			this.topRated = topRated;
			this.nowPlaying = nowPlaying;
			this.popular = popular;
			flowLayout = new UICollectionViewFlowLayout () {
				HeaderReferenceSize = HeaderReferenceSize,
				ScrollDirection = UICollectionViewScrollDirection.Horizontal,
				MinimumInteritemSpacing = MinimumInteritemSpacing, // minimum spacing between cells
				MinimumLineSpacing = MinimumLineSpacing, // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
				ItemSize = ItemSize,
				SectionInset = new UIEdgeInsets (80, -40, 97, 127)
			};


		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			ShowLabels ();
			window.AddSubview (nowPlayingController.CollectionView);
			window.AddSubview (popularController.CollectionView);
			nowPlayingController.CollectionView.ReloadData ();
			popularController.CollectionView.ReloadData ();
			CollectionView.ReloadData ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			window = UIApplication.SharedApplication.KeyWindow;


			//Create Labels for Movie Collections
			TopRatedLabel = new UILabel () {
				TextColor = UIColor.White, Frame = TopRatedLabelFrame,
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
				Text = TopRated
			};

			PlayingNowLabel = new UILabel () {
				TextColor = UIColor.White, Frame = NowPlayingLabelFrame,
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
				Text = NowPlaying
			};
			PopularLabel = new UILabel () {
				TextColor = UIColor.White, Frame = PopularLabelFrame,
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
				Text = Popular
			};
		
			TopRatedLabel.Layer.ZPosition = LabelZPosition;
			PlayingNowLabel.Layer.ZPosition = LabelZPosition;
			PopularLabel.Layer.ZPosition = LabelZPosition;


			#region   //Setup my movie collection controllers


			CollectionView.BackgroundColor =UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			CollectionView.RegisterClassForCell (typeof (MovieCell), movieTopRatedCellId);

			CollectionView.SetCollectionViewLayout (flowLayout, true);
			CollectionView.CollectionViewLayout.InvalidateLayout ();
			CollectionView.ContentMode = UIViewContentMode.ScaleAspectFit;


			nowPlayingController = new NowPlayingCollectionViewController (flowLayout,nowPlaying,window);
			nowPlayingController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			nowPlayingController.CollectionView.RegisterClassForCell (typeof (NowPlayingMovieCell), NowPlayingCollectionViewController.movieNowPlayingCellId);
			nowPlayingController.CollectionView.Frame = NowPlayingControllerFrame;



			popularController = new PopularCollectionViewController (flowLayout, popular,window);
			popularController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			popularController.CollectionView.RegisterClassForCell (typeof (PopularMovieCell), PopularCollectionViewController.popularCellId);
			popularController.CollectionView.Frame = PopularControllerFrame;
			popularController.CollectionView.ScrollEnabled = true;
		


			// adding views to keywindow. 
			window.AddSubview(TopRatedLabel);
			window.AddSubview(PlayingNowLabel);
			window.AddSubview (PopularLabel);
			window.AddSubview (nowPlayingController.CollectionView);
			window.AddSubview (popularController.CollectionView);


			#endregion

		}

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return SectionCount;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return topRated.Count/SectionCount ;
		}

		public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{

			try {
				var row = topRated [indexPath.Row];
				HideLabels ();
				var views = window.Subviews;
				foreach (var view in views) {
					if (view is UICollectionView)
						view.RemoveFromSuperview ();
				}
				((UINavigationController)window.RootViewController).PushViewController (new MovieDetailsViewController (row), true);
			} catch (Exception e) {
				Debug.WriteLine (e.Message);
			}


		}

		public static void HideLabels ()
		{
			TopRatedLabel.Hidden = true;
			PlayingNowLabel.Hidden = true;
			PopularLabel.Hidden = true;
		}
		public static  void ShowLabels ()
		{
			TopRatedLabel.Hidden = false;
			PlayingNowLabel.Hidden = false;
			PopularLabel.Hidden = false;
		}
		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var Moviecell= (MovieCell)collectionView.DequeueReusableCell (movieTopRatedCellId, indexPath);

			var row = topRated[indexPath.Row];
			// left this comment because this a really cool way to pass a objective c enum to Xamarin
			//IntPtr uikit = Dlfcn.dlopen (Constants.UIKitLibrary, 0);
			//NSString header = Dlfcn.GetStringConstant (uikit, "UICollectionElementKindSectionHeader");

			Moviecell.UpdateRow (row, UIColorExtensions.HEADER_FONT_SIZE);

			return Moviecell;
		}

	}
	
	public class MovieCell : UICollectionViewCell
	{
		public UIImageView ImageView { get; set; }
		CGRect topRatedRect = new CGRect (-2, 40, 97, 127);
		protected const string baseUrl = "https://image.tmdb.org/t/p/w300/";
		[Export ("initWithFrame:")]
		public MovieCell (CGRect frame) : base (frame)
		{
			
			ImageView = new UIImageView ();
			ImageView.Center = ContentView.Center;
			ImageView.Frame = topRatedRect;
			ImageView.ContentMode = UIViewContentMode.ScaleToFill;
			ImageView.Layer.BorderWidth = 1.0f;
			ImageView.Layer.BorderColor = UIColor.White.CGColor;
			ContentView.AddSubview (ImageView);

		}


		//public UILabel LabelView { get; private set; }
		public void UpdateRow (Movie element, Single fontSize)
		{
			ImageView.Image = GetImage (element.PosterPath);
			try {
				if (UIColorExtensions.MovieIsFavorite (element.Id.ToString ())) {

					ImageView.Layer.BorderWidth = 2.0f;
					ImageView.Layer.BorderColor = UIColor.Orange.CGColor;

				} else 
				{
					ImageView.Layer.BorderWidth = 1.0f;
					ImageView.Layer.BorderColor = UIColor.White.CGColor;
				}
			} catch (SQLite.SQLiteException) 
			{
				//so favorites yet
			}
		}



		public static UIImage GetImage (string posterPath)
		{
			if (posterPath != null) {
				var uri = new Uri (posterPath);
				using (var imgUrl = new NSUrl (baseUrl + uri.AbsoluteUri.Substring (8))) {
					using (var data = NSData.FromUrl (imgUrl)) {
						return (UIImage.LoadFromData (data));

					}
				}
			} else {
				return UIImage.FromBundle ("placeholder.png");
			}

		}
	}

	public class NowPlayingMovieCell : MovieCell
	{
		CGRect nowPlayingRect = new CGRect (-9, 26, 97, 133);
		[Export ("initWithFrame:")]
		public NowPlayingMovieCell (CGRect frame) : base (frame)
		{
			ImageView.Frame = nowPlayingRect;

		}
	}

	public class PopularMovieCell : MovieCell
	{

		static CGRect popularRect = new CGRect (-9, 22, 97, 133);
		[Export ("initWithFrame:")]
		public PopularMovieCell (CGRect frame) : base (frame)
		{
			ImageView.Frame = popularRect;

		}

	}

}   



