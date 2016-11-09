using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using UIKit;

namespace FavoriteMovies
{/// <summary>
 /// This is the Main View controller for the application. It serves as the Top Rated collection of movies and creates the Now Playing 
 /// Poplular scrollable collections.
 /// </summary>
	public class MainCollectionViewController : UIViewController
	{
		static NSString movieCellId = new NSString ("MovieCell");
		static CGSize HeaderReferenceSize = new CGSize (50, 50);
		static int MinimumInteritemSpacing = 30;
		static int MinimumLineSpacing = 5;
		static int DefaultYPositionTopRatedController = 30;
		static int DefaultYPositionTopRatedLabel = 10;
		static int DefaultYPositionNowPlayingController = 205;
		static int DefaultYPositionNowPlayingLabel = 185;
		static int DefaultYPositionPopularController = 380;
		static int DefaultYPositionPopularLabel = 360;
		static int DefaultHeaderHeight = 80;
		static CGSize ItemSize = new CGSize (100, 150);
		static CGRect FavoriteLabelFrame = new CGRect (7, 10, 90, 20);
		static CGRect TopRatedLabelFrame = new CGRect (7, 185, 90, 20);
		static CGRect NowPlayingLabelFrame = new CGRect (7, 360, 90, 20);
		static CGRect PopularLabelFrame = new CGRect (7, 535, 90, 20);

		static CGRect FavoriteControllerFrame = new CGRect (-45, 30, 400, 150);
		static CGRect TopRatedControllerFrame = new CGRect (-45, 205, 400, 150);
		static CGRect NowPlayingControllerFrame = new CGRect (-45, 380, 400, 150);
		static CGRect PopularControllerFrame = new CGRect (-45, 555, 400, 150);

		const float BackGroundColorAlpha = 1.0f;
		static string TopRated = "Top Rated";
		static string NowPlaying = "Now Playing";
		static string Popular = "Popular";
		static string Favorite = "Favorites";
		const int LabelZPosition = 1;
		const int SectionCount = 1;
		ObservableCollection<Movie> topRated;
		ObservableCollection<Movie> nowPlaying;
		ObservableCollection<Movie> popular;
		ObservableCollection<Movie> favorites;
		UICollectionViewFlowLayout flowLayout;
		NowPlayingCollectionViewController nowPlayingController;
		PopularCollectionViewController popularController;
		TopRatedCollectionViewController topRatedController;
		FavoritesViewController favoriteViewController;
		static UILabel TopRatedLabel;
		static UILabel PlayingNowLabel;
		static UILabel PopularLabel;
		static UILabel FavoriteLabel;
		UIScrollView scrollView = new UIScrollView ();



		public MainCollectionViewController (ObservableCollection<Movie> topRated, ObservableCollection<Movie> nowPlaying, ObservableCollection<Movie> popular) : base ()
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
				//SectionInset = new UIEdgeInsets (80, -40, 97, 127)
			};

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			topRatedController.CollectionView.ReloadData ();
			nowPlayingController.CollectionView.ReloadData ();
			popularController.CollectionView.ReloadData ();

			FavoritesDisplay ();

			if (favorites.Count > 0) {
				favoriteViewController.CollectionView.ReloadData ();
				favoriteViewController.CollectionView.CollectionViewLayout.InvalidateLayout ();
				favoriteViewController.CollectionView.CollectionViewLayout.PrepareLayout ();
				//favoriteViewController.CollectionView.CollectionViewLayout.CollectionViewContentSize = 
				scrollView.AddSubview (favoriteViewController.CollectionView);
			}
			;

			scrollView.AddSubview (topRatedController.CollectionView);
			scrollView.AddSubview (nowPlayingController.CollectionView);
			scrollView.AddSubview (popularController.CollectionView);
			// adding label views to View. 
			if (favorites.Count > 0)
				scrollView.AddSubview (FavoriteLabel);
			scrollView.AddSubview (TopRatedLabel);
			scrollView.AddSubview (PlayingNowLabel);
			scrollView.AddSubview (PopularLabel);
			View.AddSubview (scrollView);



		}

		void FavoritesDisplay ()
		{


			favorites = GetFavorites ();
			favoriteViewController._items = new ObservableCollection<Movie> (favorites.Reverse ());
			if (favorites.Count == 0) {
				topRatedController.CollectionView.Frame = new CGRect (topRatedController.CollectionView.Frame.X, DefaultYPositionTopRatedController, topRatedController.CollectionView.Frame.Width, topRatedController.CollectionView.Frame.Height);
				TopRatedLabel.Frame = new CGRect (TopRatedLabel.Frame.X, DefaultYPositionTopRatedLabel, TopRatedLabel.Frame.Width, TopRatedLabel.Frame.Height);

				nowPlayingController.CollectionView.Frame = new CGRect (nowPlayingController.CollectionView.Frame.X, DefaultYPositionNowPlayingController, nowPlayingController.CollectionView.Frame.Width, nowPlayingController.CollectionView.Frame.Height);
				PlayingNowLabel.Frame = new CGRect (PlayingNowLabel.Frame.X, DefaultYPositionNowPlayingLabel, PlayingNowLabel.Frame.Width, PlayingNowLabel.Frame.Height);

				popularController.CollectionView.Frame = new CGRect (popularController.CollectionView.Frame.X, DefaultYPositionPopularController, popularController.CollectionView.Frame.Width, popularController.CollectionView.Frame.Height);
				PopularLabel.Frame = new CGRect (PopularLabel.Frame.X, DefaultYPositionPopularLabel, PopularLabel.Frame.Width, PopularLabel.Frame.Height);

				//For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
				scrollView.ContentSize = new CGSize (View.Frame.Width, View.Frame.Height + DefaultHeaderHeight + MinimumInteritemSpacing );
				scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);

			} else {
				TopRatedLabel.Frame = TopRatedLabelFrame;
				topRatedController.CollectionView.Frame = TopRatedControllerFrame;

				nowPlayingController.CollectionView.Frame = NowPlayingControllerFrame;
				PlayingNowLabel.Frame = NowPlayingLabelFrame;

				popularController.CollectionView.Frame = PopularControllerFrame;
				PopularLabel.Frame = PopularLabelFrame;

				//For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
				scrollView.ContentSize = new CGSize (View.Frame.Width, View.Frame.Height + ItemSize.Height + DefaultHeaderHeight + MinimumInteritemSpacing + DefaultYPositionTopRatedLabel);
				scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);

			}


			topRatedController.CollectionView.SetNeedsLayout ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			scrollView.Frame = View.Frame;
			favorites = GetFavorites ();



			//Create Labels for Movie Collections
			FavoriteLabel = new UILabel () {
				TextColor = UIColor.White, Frame = FavoriteLabelFrame,
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
				Text = Favorite
			};


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



			//this collectionview can be differenct sizes so i have to new up a collectionviewflowlayout.
			favoriteViewController = new FavoritesViewController (new UICollectionViewFlowLayout () { MinimumInteritemSpacing = MinimumInteritemSpacing, MinimumLineSpacing = MinimumLineSpacing, HeaderReferenceSize = HeaderReferenceSize, ItemSize = ItemSize, ScrollDirection = UICollectionViewScrollDirection.Horizontal }, favorites, this);
			favoriteViewController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			favoriteViewController.CollectionView.RegisterClassForCell (typeof (MovieCell), movieCellId);
			favoriteViewController.CollectionView.Frame = FavoriteControllerFrame;



			topRatedController = new TopRatedCollectionViewController (flowLayout, topRated, this);
			topRatedController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			topRatedController.CollectionView.RegisterClassForCell (typeof (MovieCell), movieCellId);
			topRatedController.CollectionView.Frame = TopRatedControllerFrame;

			View.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			nowPlayingController = new NowPlayingCollectionViewController (flowLayout, nowPlaying, this);
			nowPlayingController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			nowPlayingController.CollectionView.RegisterClassForCell (typeof (MovieCell), movieCellId);
			nowPlayingController.CollectionView.Frame = NowPlayingControllerFrame;


			popularController = new PopularCollectionViewController (flowLayout, popular, this);
			popularController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			popularController.CollectionView.RegisterClassForCell (typeof (MovieCell), movieCellId);
			popularController.CollectionView.Frame = PopularControllerFrame;



			#endregion

		}
		ObservableCollection<Movie> GetFavorites ()
		{

			ObservableCollection<Movie> result = new ObservableCollection<Movie> ();

			try {


				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					var query = db.Table<Movie> ();
					foreach (var movie in query) {
						var item = new Movie ();
						item.Id = movie.Id;
						item.Favorite = true;
						item.PosterPath = movie.PosterPath;
						item.HighResPosterPath = movie.HighResPosterPath;
						item.Overview = movie.Overview;
						item.ReleaseDate = movie.ReleaseDate;
						item.Title = movie.Title;
						item.VoteAverage = movie.VoteAverage;
						item.VoteCount = movie.VoteCount;
						result.Add (item);
					}
				}

				//favoriteViewController.CollectionView.ReloadData ();
			} catch (SQLite.SQLiteException e) {
				Debug.WriteLine (e.Message);
			}

			return result;
		}
	}

	public class MovieCell : UICollectionViewCell
	{
		public UIImageView ImageView { get; set; }
		protected CGRect topRatedRect = new CGRect (-2, 40, 97, 127);
		protected const string baseUrl = "https://image.tmdb.org/t/p/w300/";
		[Export ("initWithFrame:")]
		public MovieCell (CGRect frame) : base (frame)
		{
			ImageView = new UIImageView ();
			ImageView.Center = ContentView.Center;
			ImageView.Frame = ContentView.Frame;
			ImageView.ContentMode = UIViewContentMode.ScaleToFill;
			ContentView.Layer.BorderWidth = 1.0f;
			ContentView.Layer.BorderColor = UIColor.White.CGColor;
			ContentView.AddSubview (ImageView);

		}


		//public UILabel LabelView { get; private set; }
		public void UpdateRow (Movie element)
		{
			ImageView.Image = GetImage (element.PosterPath);
			try {
				if (UIColorExtensions.MovieIsFavorite (element.Id.ToString ())) {

					ContentView.Layer.BorderColor = UIColor.Orange.CGColor;

				} else {
					ContentView.Layer.BorderColor = UIColor.White.CGColor;
				}
			} catch (SQLite.SQLiteException ex) {
				//no favorites yet
				Debug.Write (ex.Message);
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



}



