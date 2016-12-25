using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using MonoTouch.Dialog;
using SidebarNavigation;
using SQLite;
using UIKit;

namespace FavoriteMovies
{/// <summary>
 /// This is the Main View controller for the application. It serves as the Top Rated collection of movies and creates the Now Playing 
 /// Poplular scrollable collections.
 /// </summary>
	public class MainViewController : BaseController
	{
		static CGSize HeaderReferenceSize = new CGSize (50, 50);
		static int MinimumInteritemSpacing = 30;
		static int SpaceBetweenContainers = 30;
		static int SeparationBuffer = 20;
		static int SpaceBetweenListTypes = 50;
		static int MinimumLineSpacing = 5;
		static int DefaultYPositionTopRatedLabel = 10;
		static int DefaultYPositionTopRatedController = 30;
		static int DefaultYPositionNowPlayingLabel = 270;
		static int DefaultYPositionPopularLabel = 530;
		static int DefaultYPositionMovieLatestLabel = 790;
		public static int NewCustomListToRefresh =-1;
		//public static int OldCustomListToRefresh;
		static CGSize ItemSize = new CGSize (153,205);
		static CGRect FavoriteLabelFrame = new CGRect (7, 10, 180, 20);
		static CGRect TopRatedLabelFrame = new CGRect (7, 205, 180, 20);
		static CGRect NowPlayingLabelFrame = new CGRect (7, 400, 180, 20);
		static CGRect PopularLabelFrame = new CGRect (7, 595, 180, 20);
		static CGRect MovieLatestLabelFrame = new CGRect (7, 790, 180, 20);
		static int SpaceBetweenLabelsAndFrames = 245;
		static CGRect FavoriteControllerFrame = new CGRect (-45, 30, 385, 205);
		static CGRect TopRatedControllerFrame = new CGRect (-45, 225, 385, 205);
		static CGRect NowPlayingControllerFrame = new CGRect (-45, 420, 385, 205);
		static CGRect PopularControllerFrame = new CGRect (-45, 615, 385, 205);
		static CGRect MovieLatestControllerFrame = new CGRect (-45, 810, 385, 205);
		static string TopRated = "Top Rated";
		static string NowPlaying = "Now Playing";
		static string Popular = "Popular";
		static string LatestMovies = "Latest Movies";
		const int LabelZPosition = 1;
		const int SectionCount = 1;
		ObservableCollection<Movie> topRated;
		ObservableCollection<Movie> nowPlaying;
		ObservableCollection<Movie> popular;
		ObservableCollection<CustomList> customLists;
		ObservableCollection<Movie> MovieLatest;
		UICollectionViewFlowLayout flowLayout;
		BaseCollectionViewController nowPlayingController;
		static PopularCollectionViewController popularController;
		static TopRatedCollectionViewController topRatedController;
		static MovieLatestViewController MovieLatestController;
		static SearchResultsViewController searchResultsController;
		UISearchController searchController;
		FavoritesViewController [] customControllers;
		UILabel [] customLabels = null;
		static UILabel TopRatedLabel;
		static UILabel PlayingNowLabel;
		static UILabel PopularLabel;
		static UILabel MovieLatestLabel;
		static UIScrollView scrollView = new UIScrollView ();


		public MainViewController (ObservableCollection<Movie> topRated, ObservableCollection<Movie> nowPlaying, ObservableCollection<Movie> popular, ObservableCollection<Movie> movieLatest,int page)
		{
			this.topRated = topRated;
			this.nowPlaying = nowPlaying;
			this.popular = popular;
			this.MovieLatest = movieLatest;
			//this.TvNowAiring = TVNowAiring;
			flowLayout = new UICollectionViewFlowLayout () {
				HeaderReferenceSize = HeaderReferenceSize,
				ScrollDirection = UICollectionViewScrollDirection.Horizontal,
				MinimumInteritemSpacing = MinimumInteritemSpacing, // minimum spacing between cells
				MinimumLineSpacing = MinimumLineSpacing, // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
				ItemSize = ItemSize
				//SectionInset = new UIEdgeInsets (80, -40, 97, 127)
			};

		}

		public override bool ShouldAutorotate ()
		{
			return base.ShouldAutorotate ();

		}
		protected override void Dispose (bool disposing)
		{
			Console.WriteLine ("Disposed MainViewController");
			base.Dispose (disposing);
		}

		~MainViewController ()
		{
			Console.WriteLine ("Finalized MainViewController");
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}


		void LoadMoreMovies ()
		{
			if (MovieService.TotalPagesNowPlaying > 0) {
				List<Task> Tasks = new List<Task> ();
				for (var x = 1; x < MovieService.TotalPagesNowPlaying; x++) {
					Tasks.Add (Task.Run (async () => nowPlaying = new ObservableCollection<Movie>
										 (nowPlaying.Concat (await MovieService.GetMoviesAsync
															 (MovieService.MovieType.NowPlaying, x)))));
				}
				Task.WhenAll (Tasks);
			}
		}
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			//searchResultsController.TableView.ContentInset = new UIEdgeInsets (80, 0, 0, 0);
			//HACK until i find out why when you open a movie details and come back the view.height changes.
			if (View.Frame.Height == 504) 
			{
		
				if (customLists.Count > 0) {
					scrollView.ContentSize = new CGSize (View.Frame.Width, 898 + 50);
					scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);
				} else {
					scrollView.ContentSize = new CGSize (View.Frame.Width, 723 + 50);
					scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);
				}
			}
			////*****this fixes a problem with the uitableview adding space at the top after each selection*****
			//Debug.Write (searchResultsController.TableView.ContentInset);

		}


		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			SidebarController.Disabled = false;

			//this fixes problem when coming out of full screen after watching a trailer
			NavigationController.NavigationBar.Frame = new CGRect () { X = 0, Y = 20, Width = 320, Height = 44 };
			//DeleteAllSubviews (scrollView);


			if(NewCustomListToRefresh != -1)
			   FavoritesDisplay ();

			// adding label views to View. 
			scrollView.AddSubview (topRatedController.CollectionView);
			scrollView.AddSubview (nowPlayingController.CollectionView);
			scrollView.AddSubview (popularController.CollectionView);
			scrollView.AddSubview (MovieLatestController.CollectionView);
			scrollView.AddSubview (PlayingNowLabel);
			scrollView.AddSubview (PopularLabel);
			scrollView.AddSubview (MovieLatestLabel);
			scrollView.AddSubview (TopRatedLabel);
			View.AddSubview (scrollView);
			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).ResignFirstResponder ();
		}

		void UpdateCustomListMovies (int cnt)
		{
			customLabels [cnt] = new UILabel () {
				TextColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR),
				//Frame = FavoriteLabelFrame,
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
				Text = customLists [cnt].Name
			};
			customControllers [cnt] = new FavoritesViewController (new UICollectionViewFlowLayout () {
				MinimumInteritemSpacing = MinimumInteritemSpacing, MinimumLineSpacing = MinimumLineSpacing,
				HeaderReferenceSize = HeaderReferenceSize, ItemSize = ItemSize,
				ScrollDirection = UICollectionViewScrollDirection.Horizontal
			},new ObservableCollection<Movie>(GetMovieList (customLists [cnt]).Reverse ()),NavController);

			customControllers [cnt].CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			customControllers [cnt].CollectionView.RegisterClassForCell (typeof (MovieCell), FavoritesViewController.movieCellId);

		}

		void DeleteAllSubviews (UIScrollView view)
		{
			foreach(UIView subview in view.Subviews)
			{
				subview.RemoveFromSuperview ();
			}

		}

		void FavoritesDisplay ()
		{
			DeleteAllSubviews (scrollView);
			customLists = GetCustomLists ();
			if (customLists.Count == 0) {
				TopRatedLabel.Frame = new CGRect (TopRatedLabel.Frame.X, DefaultYPositionTopRatedLabel, TopRatedLabel.Frame.Width, TopRatedLabel.Frame.Height);
				topRatedController.CollectionView.Frame = new CGRect (TopRatedControllerFrame.X, DefaultYPositionTopRatedController, TopRatedControllerFrame.Width, TopRatedControllerFrame.Height);

				PlayingNowLabel.Frame = new CGRect (PlayingNowLabel.Frame.X, DefaultYPositionNowPlayingLabel, PlayingNowLabel.Frame.Width, PlayingNowLabel.Frame.Height);
				nowPlayingController.CollectionView.Frame = new CGRect (NowPlayingControllerFrame.X, DefaultYPositionNowPlayingLabel+SeparationBuffer , NowPlayingControllerFrame.Width, NowPlayingControllerFrame.Height);

				PopularLabel.Frame = new CGRect (PopularLabel.Frame.X, DefaultYPositionPopularLabel, PopularLabel.Frame.Width, PopularLabel.Frame.Height);
				popularController.CollectionView.Frame = new CGRect (PopularControllerFrame.X, DefaultYPositionPopularLabel + SeparationBuffer, PopularControllerFrame.Width, PopularControllerFrame.Height);

				MovieLatestLabel.Frame = new CGRect (MovieLatestLabel.Frame.X, DefaultYPositionMovieLatestLabel, MovieLatestLabel.Frame.Width, MovieLatestLabel.Frame.Height);
				MovieLatestController.CollectionView.Frame = new CGRect (MovieLatestControllerFrame.X, DefaultYPositionMovieLatestLabel + SeparationBuffer , MovieLatestControllerFrame.Width, MovieLatestControllerFrame.Height);

				////For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
				//scrollView.ContentSize = new CGSize (320, View.Frame.Height + 155);
				//scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);

				} 
			else 
				{
				/// <summary>
				/// A new list was added(0) or this is the first time through (-1)
				/// </summary>
				/// <param name="customList">Custom list.</param>
				if (NewCustomListToRefresh == 0 || NewCustomListToRefresh==-1) 
				{


					customLabels = new UILabel [customLists.Count];
					customControllers = new FavoritesViewController [customLists.Count];
					for (var cnt = 0; cnt < customLists.Count; cnt++) 
					{
						
						UpdateCustomListMovies (cnt);
						UpdateCustomListsPosition (cnt);
						
					}
					TopRatedLabel.Frame = new CGRect () { X = TopRatedLabelFrame.X, Y = TopRatedLabelFrame.Y + (SpaceBetweenLabelsAndFrames * (customLists.Count - 1)+ SpaceBetweenContainers+ SeparationBuffer+ SpaceBetweenListTypes), Height = TopRatedLabelFrame.Height, Width = TopRatedLabelFrame.Width };
					topRatedController.CollectionView.Frame = new CGRect () { X = TopRatedControllerFrame.X, Y = TopRatedLabel.Frame.Y + SeparationBuffer, Height = TopRatedControllerFrame.Height, Width = TopRatedControllerFrame.Width };

					PlayingNowLabel.Frame = new CGRect () { X = NowPlayingLabelFrame.X, Y = topRatedController.CollectionView.Frame.Y +topRatedController.CollectionView.Frame.Height+ SpaceBetweenContainers, Height = NowPlayingLabelFrame.Height, Width = NowPlayingLabelFrame.Width };
					nowPlayingController.CollectionView.Frame = new CGRect () { X = NowPlayingControllerFrame.X, Y = PlayingNowLabel.Frame.Y + SeparationBuffer, Height = NowPlayingControllerFrame.Height, Width = NowPlayingControllerFrame.Width }; ;

					PopularLabel.Frame = new CGRect () { X = PopularLabelFrame.X, Y = nowPlayingController.CollectionView.Frame.Y + nowPlayingController.CollectionView.Frame.Height + SpaceBetweenContainers, Height = PopularLabelFrame.Height, Width = PopularLabelFrame.Width };
					popularController.CollectionView.Frame = new CGRect () { X = PopularControllerFrame.X, Y = PopularLabel.Frame.Y + SeparationBuffer, Height = PopularControllerFrame.Height, Width = PopularControllerFrame.Width };

					MovieLatestLabel.Frame = new CGRect () { X = MovieLatestLabelFrame.X, Y = popularController.CollectionView.Frame.Y + popularController.CollectionView.Frame.Height + SpaceBetweenContainers, Height = MovieLatestLabelFrame.Height, Width = MovieLatestLabelFrame.Width };
					MovieLatestController.CollectionView.Frame = new CGRect () { X = MovieLatestControllerFrame.X, Y = MovieLatestLabel.Frame.Y + SeparationBuffer, Height = MovieLatestControllerFrame.Height, Width = MovieLatestControllerFrame.Width };


				} 
				else 
				{
					UpdateCustomListMovies (NewCustomListToRefresh);
					UpdateCustomListsPosition (NewCustomListToRefresh);
				}

			}
			NewCustomListToRefresh = -1;

			//For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
			scrollView.ContentSize = new CGSize (View.Frame.Width, MovieLatestController.CollectionView.Frame.Y + MovieLatestController.CollectionView.Frame.Height + SeparationBuffer);
			scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);

		}

		void UpdateCustomListsPosition (int customList)
		{
			customLabels [customList].Frame = new CGRect () {
				X = FavoriteLabelFrame.X,
				Y = FavoriteLabelFrame.Y + (SpaceBetweenLabelsAndFrames * customList) + SpaceBetweenContainers   ,
				Height = FavoriteLabelFrame.Height, Width = FavoriteLabelFrame.Width
			};
			customControllers [customList].CollectionView.Frame = new CGRect () {
				X = FavoriteControllerFrame.X,
				Y = customLabels [customList].Frame.Y + 17,// FavoteControllerFrame.Y + (SpaceBetweenLabelsAndFrames * customList)
			    Height = FavoriteControllerFrame.Height,
				Width = FavoriteControllerFrame.Width
			};
			scrollView.Add(customLabels [customList]);
			scrollView.Add(customControllers [customList].CollectionView);
		}

		ObservableCollection<Movie> GetMovieList (CustomList customList)
		{
			var returnList = new ObservableCollection<Movie> ();
			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					var query = db.Query<Movie> ("SELECT * FROM [Movie] WHERE [CustomListID] = " + customList.Id);

					foreach (var movie in query) {
						var item = new Movie ();
						item.Id = movie.Id;
						item.Name = movie.Name;
						item.BackdropPath = movie.BackdropPath;
						item.CustomListID = movie.CustomListID;
						item.Favorite = movie.Favorite;
						item.HighResPosterPath = movie.HighResPosterPath;
						item.OriginalLanguage = movie.OriginalLanguage;
						item.Overview = movie.Overview;
						item.Popularity = movie.Popularity;
						item.PosterPath = movie.PosterPath;
						item.ReleaseDate = movie.ReleaseDate;
						item.VoteAverage = movie.VoteAverage;
						item.UserReview = movie.UserReview;
						item.UserRating = movie.UserRating;
						item.Order = movie.Order;
						returnList.Add (item);
					}
				}
			} catch (SQLite.SQLiteException e) {
				Debug.Write (e.Message);
			}
			return returnList;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			scrollView.Frame = new CGRect () { X = View.Frame.X, Y = View.Frame.Y, Width = View.Frame.Width, Height = View.Frame.Height};
			//scrollView.PagingEnabled = true;

			customLists = GetCustomLists ();

			customLabels = new UILabel [customLists.Count];
			customControllers = new FavoritesViewController [customLists.Count];



			TopRatedLabel = new UILabel () {
				TextColor = UIColor.Black, Frame = TopRatedLabelFrame,
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
				Text = TopRated
			};

			PlayingNowLabel = new UILabel () {
				TextColor = UIColor.Black, Frame = NowPlayingLabelFrame,
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
				Text = NowPlaying
			};
			PopularLabel = new UILabel () {
				TextColor = UIColor.Black, Frame = PopularLabelFrame,
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
				Text = Popular
			};

			MovieLatestLabel = new UILabel () {
				TextColor = UIColor.Black, Frame = MovieLatestLabelFrame,
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
				Text = LatestMovies
			};
			TopRatedLabel.Layer.ZPosition = LabelZPosition;
			PlayingNowLabel.Layer.ZPosition = LabelZPosition;
			PopularLabel.Layer.ZPosition = LabelZPosition;
			topRatedController = new TopRatedCollectionViewController (new UICollectionViewFlowLayout () {
				MinimumInteritemSpacing = MinimumInteritemSpacing, MinimumLineSpacing = MinimumLineSpacing,
				HeaderReferenceSize = HeaderReferenceSize, ItemSize = ItemSize,
				ScrollDirection = UICollectionViewScrollDirection.Horizontal
			}, topRated, NavController);
			topRatedController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			topRatedController.CollectionView.RegisterClassForCell (typeof (MovieCell), TopRatedCollectionViewController.movieCellId);


			View.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);

			nowPlayingController = new NowPlayingCollectionViewController (new UICollectionViewFlowLayout () {
				MinimumInteritemSpacing = MinimumInteritemSpacing, MinimumLineSpacing = MinimumLineSpacing,
				HeaderReferenceSize = HeaderReferenceSize, ItemSize = ItemSize,
				ScrollDirection = UICollectionViewScrollDirection.Horizontal
			}, nowPlaying, NavController);
			nowPlayingController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			nowPlayingController.CollectionView.RegisterClassForCell (typeof (MovieCell), NowPlayingCollectionViewController.movieCellId);



			popularController = new PopularCollectionViewController (new UICollectionViewFlowLayout () {
				MinimumInteritemSpacing = MinimumInteritemSpacing, MinimumLineSpacing = MinimumLineSpacing,
				HeaderReferenceSize = HeaderReferenceSize, ItemSize = ItemSize,
				ScrollDirection = UICollectionViewScrollDirection.Horizontal
			}, popular, NavController);
			popularController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			popularController.CollectionView.RegisterClassForCell (typeof (MovieCell), PopularCollectionViewController.movieCellId);


			MovieLatestController = new MovieLatestViewController (flowLayout, MovieLatest, NavController);
			MovieLatestController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			MovieLatestController.CollectionView.RegisterClassForCell (typeof (MovieCell), MovieLatestViewController.movieCellId);


			if (NewCustomListToRefresh == 0 || NewCustomListToRefresh == -1) {
				for (var cnt = 0; cnt < customLists.Count; cnt++) {
					UpdateCustomListMovies (cnt);
				}

			} else {
				UpdateCustomListMovies (NewCustomListToRefresh);
			}
			FavoritesDisplay ();

			// Creates an instance of a custom View Controller that holds the results
			searchResultsController = new SearchResultsViewController ();

			//Creates a search controller updater
			var searchUpdater = new SearchResultsUpdator ();
			searchUpdater.UpdateSearchResults += searchResultsController.Search;

			//add the search controller
			searchController = new UISearchController (searchResultsController) {
				SearchResultsUpdater = searchUpdater,

				WeakDelegate = searchUpdater,
				WeakSearchResultsUpdater = searchUpdater,
			};

			searchResultsController.searchController = searchController;

			//format the search bar
			searchController.SearchBar.SizeToFit ();
			searchController.SearchBar.SearchBarStyle = UISearchBarStyle.Prominent;
			searchController.SearchBar.Placeholder = "Find a movie";

			//searchResultsController.TableView.WeakDelegate = this;
			searchController.SearchBar.WeakDelegate = searchResultsController;

			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).TextColor = UIColor.White;
			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE);
			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, BackGroundColorAlpha);

			//the search bar is contained in the navigation bar, so it should be visible
			searchController.HidesNavigationBarDuringPresentation = false;

			//Ensure the searchResultsController is presented in the current View Controller 
			DefinesPresentationContext = true;

			//Set the search bar in the navigation bar
			TabController.NavigationItem.TitleView = searchController.SearchBar;

		}

	

		ObservableCollection<CustomList> GetCustomLists ()
		{

			ObservableCollection<CustomList> result = new ObservableCollection<CustomList> ();

			try {

				using (var db = new SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					var query = db.Query<CustomList> ("SELECT * FROM [CustomList] ORDER BY [Order]");
					//var query = db.Table<CustomList> ();
					foreach (var list in query) {
						var item = new CustomList ();
						item.Id = list.Id;
						item.Name = list.Name;
						result.Add (item);
					}
				}

				//favoriteViewController.CollectionView.ReloadData ();
			} catch (SQLiteException e) {
				Debug.WriteLine (e.Message);
			}

			return result;
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}
	}

	public class MovieCell : UICollectionViewCell
	{
		public UIImageView ImageView { get; set; }
		//protected CGRect topRatedRect = new CGRect (-2, 40, 97, 127);
		protected const string baseUrl = "https://image.tmdb.org/t/p/w300/";
		[Export ("initWithFrame:")]
		public MovieCell (CGRect frame) : base (frame)
		{
			try {
				ImageView = new UIImageView ();
				ImageView.Center = ContentView.Center;
				ImageView.Frame = ContentView.Frame;
				ImageView.ContentMode = UIViewContentMode.ScaleToFill;
				ContentView.Layer.BorderWidth = 1.0f;
				ContentView.Layer.BorderColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f).CGColor;
				ContentView.AddSubview (ImageView);
			} catch (Exception ex) {Debug.Write (ex.Message); }

		}


		//public UILabel LabelView { get; private set; }
		public void UpdateRow (Movie element)
		{
			
			try {
				ImageView.Image = GetImage (element.PosterPath);
				if (UIColorExtensions.MovieIsFavorite (element.Id.ToString ()))
				{
					ContentView.Layer.BorderColor = UIColor.Orange.CGColor;

				} else {
					ContentView.Layer.BorderColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f).CGColor;
				}
			} catch (SQLite.SQLiteException ex) {
				//no favorites yet
				Debug.Write (ex.Message);
			}
		}
		public static UIImage GetImageUrl (string posterPath)
		{
			if (posterPath != null) 
			{
				var uri = new Uri (posterPath);
				using (var imgUrl = new NSUrl (HttpUtility.UrlPathEncode (uri.AbsoluteUri)))
				{
					using (var data = NSData.FromUrl (imgUrl)) 
					{
						return (UIImage.LoadFromData (data));
					}
				}
			} else {
				return UIImage.FromBundle ("blank.png");
			}

		}
		public static UIImage GetImage (string posterPath)
		{
			var returnImage = new UIImage ();
			var task = Task.Run (async () => 
			{
				if (posterPath != null) {
					var uri = new Uri (posterPath);
					using (var imgUrl = new NSUrl (baseUrl + uri.AbsoluteUri.Substring (8))) {
						using (var data = NSData.FromUrl (imgUrl)) {
							returnImage= (UIImage.LoadFromData (data));
						}
					}
				} else {
					returnImage= UIImage.FromBundle ("blank.png");
				}
			});
			TimeSpan ts = TimeSpan.FromMilliseconds (1000);
			task.Wait (ts);

			if (!task.Wait (ts))
				Console.WriteLine ("The timeout interval elapsed.");
			return returnImage;


		}
	}



}










