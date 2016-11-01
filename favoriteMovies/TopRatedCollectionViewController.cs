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
{
	public class TopRatedCollectionViewController : UICollectionViewController
	{
		static NSString movieTopRatedCellId = new NSString ("TopRatedMovieCell");
		static CGSize HeaderReferenceSize = new CGSize (50, 50);
		static int MinimumInteritemSpacing = 10;
		static int MinimumLineSpacing = -5;
		static CGSize ItemSize = new CGSize (110, 150);
		static CGRect TopRatedLabelFrame = new CGRect (7, 69, 90, 20);
		//static CGRect TopRatedControllerFrame = new CGRect (7, 170, 400, 150);
		static CGRect NowPlayingLabelFrame = new CGRect (7, 229, 90, 20);
		static CGRect NowPlayingControllerFrame = new CGRect (7, 245, 400, 150);
		static CGRect PopularLabelFrame = new CGRect (7, 390, 90, 20);
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
		//UICollectionViewLayout layout;
		UIWindow window;
		static UILabel TopRatedLabel;
		static UILabel PlayingNowLabel;
		static UILabel PopularLabel;

		//public CGRect FrameRec { get; set; }


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
				ItemSize = ItemSize
			};


		}


		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			ShowLabels ();
			window.AddSubview (nowPlayingController.CollectionView);
			window.AddSubview (popularController.CollectionView);
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

			nowPlayingController = new NowPlayingCollectionViewController (flowLayout,nowPlaying,window);
			nowPlayingController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			nowPlayingController.CollectionView.RegisterClassForCell (typeof (NowPlayingMovieCell), NowPlayingCollectionViewController.movieNowPlayingCellId);

			nowPlayingController.CollectionView.Frame = NowPlayingControllerFrame;
	


			popularController = new PopularCollectionViewController (flowLayout, popular,window);
			popularController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			popularController.CollectionView.RegisterClassForCell (typeof (PopularMovieCell), PopularCollectionViewController.popularCellId);

			popularController.CollectionView.Frame = PopularControllerFrame;
			popularController.CollectionView.ScrollEnabled = true;
			popularController.CollectionView.ContentMode = UIViewContentMode.ScaleAspectFit;
			popularController.CollectionView.SizeToFit ();


			window.AddSubview(TopRatedLabel);
			window.AddSubview(PlayingNowLabel);
			window.AddSubview (PopularLabel);
			//((UINavigationController)window.RootViewController).AddChildViewController (nowPlayingController);
			window.AddSubview (nowPlayingController.CollectionView);
			//((UINavigationController)window.RootViewController).AddChildViewController (popularController);
			//CollectionView.AddSubview (nowPlayingController.CollectionView);
			//CollectionView.AddSubview (popularController.CollectionView);
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
				((UINavigationController)window.RootViewController).PushViewController (new MovieDetailViewController (row), true);
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

			//IntPtr uikit = Dlfcn.dlopen (Constants.UIKitLibrary, 0);
			//NSString header = Dlfcn.GetStringConstant (uikit, "UICollectionElementKindSectionHeader");

			Moviecell.UpdateRow (row, UIColorExtensions.HEADER_FONT_SIZE);

			return Moviecell;
		}

	}

	public class MovieCell : UICollectionViewCell
	{
		public UIImageView ImageView { get; set; }
		CGRect topRatedRect = new CGRect (-42, 25, 97, 127);
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
			ImageView.ClipsToBounds = true;
			ContentView.AddSubview (ImageView);

		}

		//public UILabel LabelView { get; private set; }
		public void UpdateRow (Movie element, Single fontSize)
		{
			ImageView.Image = GetImage(element.PosterPath);
		}

		public UIImage GetImage (string posterPath)
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
		CGRect nowPlayingRect = new CGRect (-50, 5, 97, 133);
		[Export ("initWithFrame:")]
		public NowPlayingMovieCell (CGRect frame) : base (frame)
		{
			ImageView.Frame = nowPlayingRect;

		}
	}

	public class PopularMovieCell : MovieCell
	{

		static CGRect popularRect = new CGRect (-50, 3, 97, 133);
		[Export ("initWithFrame:")]
		public PopularMovieCell (CGRect frame) : base (frame)
		{
			ImageView.Frame = popularRect;

		}

	}

}   



