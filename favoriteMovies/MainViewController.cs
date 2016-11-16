using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using FavoriteMoviesPCL;
using FlyoutNavigation;
using Foundation;
using MonoTouch.Dialog;
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
		static int DefaultYPositionMovieLatestLabel = 535;
		static int DefaultYPositionMovieLatestController = 555;
		static int DefaultHeaderHeight = 80;
		static CGSize ItemSize = new CGSize (100, 150);
		static CGRect FavoriteLabelFrame = new CGRect (7, 10, 90, 20);
		static CGRect TopRatedLabelFrame = new CGRect (7, 185, 90, 20);
		static CGRect NowPlayingLabelFrame = new CGRect (7, 360, 90, 20);
		static CGRect PopularLabelFrame = new CGRect (7, 535, 90, 20);
		static CGRect MovieLatestLabelFrame = new CGRect (7, 710, 90, 20);
		static int SpaceBetweenLabelsAndFrames = 175;
		static CGRect FavoriteControllerFrame = new CGRect (-45, 30, 400, 150);
		static CGRect TopRatedControllerFrame = new CGRect (-45, 205, 400, 150);
		static CGRect NowPlayingControllerFrame = new CGRect (-45, 380, 400, 150);
		static CGRect PopularControllerFrame = new CGRect (-45, 555, 400, 150);
		static CGRect MovieLatestControllerFrame = new CGRect (-45, 730, 400, 150);
		const float BackGroundColorAlpha = 1.0f;
		static string TopRated = "Top Rated";
		static string NowPlaying = "Now Playing";
		static string Popular = "Popular";
		static string Favorite = "Favorites";
		static string LatestMovies = "Latest Movies";
		const int LabelZPosition = 1;
		const int SectionCount = 1;
		ObservableCollection<Movie> topRated;
		ObservableCollection<Movie> nowPlaying;
		ObservableCollection<Movie> popular;
		ObservableCollection<Movie> favorites;
		ObservableCollection<Movie> MovieLatest;
		ObservableCollection<Movie> TvNowAiring;
		UICollectionViewFlowLayout flowLayout;
		NowPlayingCollectionViewController nowPlayingController;
		PopularCollectionViewController popularController;
		TopRatedCollectionViewController topRatedController;
		FavoritesViewController favoriteViewController;
		MovieLatestViewController MovieLatestController;
		UISearchController searchController;
		static UILabel TopRatedLabel;
		static UILabel PlayingNowLabel;
		static UILabel PopularLabel;
		static UILabel FavoriteLabel;
		static UILabel nowPlayingLabel;
		static UILabel MovieLatestLabel;
		static MovieCollectionViewController movieCollection;
		static UIScrollView scrollView = new UIScrollView ();
		static nfloat lastLabelY = PopularLabelFrame.Y;
		static nfloat lastFrameY = PopularControllerFrame.Y;



		public MainCollectionViewController (ObservableCollection<Movie> topRated, ObservableCollection<Movie> nowPlaying, ObservableCollection<Movie> popular, ObservableCollection<Movie> movieLatest, ObservableCollection<Movie> TVNowAiring,int page)
		{
			this.topRated = topRated;
			this.nowPlaying = nowPlaying;
			this.popular = popular;
			this.MovieLatest = movieLatest;
			this.TvNowAiring = TVNowAiring;
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

		void LoadMoreMovies ()
		{
			if (MovieService.TotalPagesNowPlaying > 0) {
				List<Task> Tasks = new List<Task> ();
				for (var x = 1; x < MovieService.TotalPagesNowPlaying; x++) {
					Tasks.Add (Task.Run (async () => nowPlaying = new ObservableCollection<Movie>
										 (nowPlaying.Concat (await MovieService.GetMoviesAsync
															 (MovieService.MovieType.NowPaying, x)))));
				}


				Task.WhenAll (Tasks);

			}
		}
		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}

		public async static Task<bool> AddCollectionView (ObservableCollection<Movie> movieList, string Label, MainCollectionViewController mainCollectionViewController)
		{
			lastLabelY = lastLabelY + SpaceBetweenLabelsAndFrames;
			lastFrameY = lastFrameY + SpaceBetweenLabelsAndFrames;
			//UILabel nowPlayingLabel;
			//FavoritesViewController movieCollection;


			nowPlayingLabel = new UILabel () {
				TextColor = UIColor.White, Frame = new CGRect () { X = FavoriteLabelFrame.X, Y = lastLabelY, Width = FavoriteLabelFrame.Width, Height = FavoriteLabelFrame.Height },
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
				Text = Label
			};

			movieCollection = new MovieCollectionViewController (new UICollectionViewFlowLayout () { MinimumInteritemSpacing = MinimumInteritemSpacing, MinimumLineSpacing = MinimumLineSpacing, HeaderReferenceSize = HeaderReferenceSize, ItemSize = ItemSize, ScrollDirection = UICollectionViewScrollDirection.Horizontal }, movieList, mainCollectionViewController);
			movieCollection.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			movieCollection.CollectionView.RegisterClassForCell (typeof (MovieCell), movieCellId);
			movieCollection.CollectionView.Frame = new CGRect () { X = FavoriteControllerFrame.X, Y = lastFrameY, Width = FavoriteControllerFrame.Width };

			//movieCollection.CollectionView.CollectionViewLayout.InvalidateLayout ();
			//movieCollection.CollectionView.CollectionViewLayout.PrepareLayout ();
			//movieCollection.CollectionView.SetNeedsLayout ();

			movieCollection._items = movieList;
			//For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
			scrollView.ContentSize = new CGSize (scrollView.Frame.Width, lastFrameY + SpaceBetweenLabelsAndFrames);
			//scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);
			scrollView.AddSubview (nowPlayingLabel);
			scrollView.AddSubview (movieCollection.CollectionView);
			//	mainCollectionViewController.View.AddSubview (scrollView);
			//View.AddSubview (scrollView);
			return true;
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			NavigationController.NavigationBar.Frame = new CGRect () { X = 0, Y = 20, Width = 320, Height = 44 };
			//topRatedController.CollectionView.ReloadData ();
			//nowPlayingController.CollectionView.ReloadData ();
			//popularController.CollectionView.ReloadData ();
			//MovieLatestController.CollectionView.ReloadData ();
			FavoritesDisplay ();

			if (favorites.Count > 0) {
				favoriteViewController.CollectionView.ReloadData ();
				favoriteViewController.CollectionView.CollectionViewLayout.InvalidateLayout ();
				favoriteViewController.CollectionView.CollectionViewLayout.PrepareLayout ();
				scrollView.AddSubview (favoriteViewController.CollectionView);
			}


			scrollView.AddSubview (topRatedController.CollectionView);
			scrollView.AddSubview (nowPlayingController.CollectionView);
			scrollView.AddSubview (popularController.CollectionView);
			scrollView.AddSubview (MovieLatestController.CollectionView);
			// adding label views to View. 
			if (favorites.Count > 0)
				scrollView.AddSubview (FavoriteLabel);
			scrollView.AddSubview (TopRatedLabel);
			scrollView.AddSubview (PlayingNowLabel);
			scrollView.AddSubview (PopularLabel);
			scrollView.AddSubview (MovieLatestLabel);
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

				MovieLatestController.CollectionView.Frame = new CGRect (MovieLatestController.CollectionView.Frame.X, DefaultYPositionMovieLatestController, MovieLatestController.CollectionView.Frame.Width, popularController.CollectionView.Frame.Height);
				MovieLatestLabel.Frame = new CGRect (MovieLatestLabel.Frame.X, DefaultYPositionMovieLatestLabel, MovieLatestLabel.Frame.Width, MovieLatestLabel.Frame.Height);

				//For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
				scrollView.ContentSize = new CGSize (View.Frame.Width, View.Frame.Height + DefaultHeaderHeight + MinimumInteritemSpacing+ 175);
				scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);

			} else {
				TopRatedLabel.Frame = TopRatedLabelFrame;
				topRatedController.CollectionView.Frame = TopRatedControllerFrame;

				nowPlayingController.CollectionView.Frame = NowPlayingControllerFrame;
				PlayingNowLabel.Frame = NowPlayingLabelFrame;

				popularController.CollectionView.Frame = PopularControllerFrame;
				PopularLabel.Frame = PopularLabelFrame;

				MovieLatestController.CollectionView.Frame = MovieLatestControllerFrame;
				MovieLatestLabel.Frame = MovieLatestLabelFrame;

				//For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
				scrollView.ContentSize = new CGSize (View.Frame.Width, View.Frame.Height + ItemSize.Height + DefaultHeaderHeight + MinimumInteritemSpacing + DefaultYPositionTopRatedLabel+ 175);
				scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);

			}


			topRatedController.CollectionView.SetNeedsLayout ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			scrollView.Frame = View.Frame;

			string [] Tasks = {
			"Get Xamarin",
			"Learn C#",
			"Write Killer App",
			"Add Platforms",
			"Profit",
			"Meet Obama",
				};

			var navigation = new FlyoutNavigationController ();
			navigation.Position = FlyOutNavigationPosition.Left;
			navigation.View.Frame = UIScreen.MainScreen.Bounds;
			View.AddSubview (navigation.View);
			this.AddChildViewController (navigation);

			// Create the menu:
			navigation.NavigationRoot = new RootElement ("Task List") {
				new Section ("Task List") {
					from page in Tasks
						select new StringElement (page) as Element
				}
			};

			// Create an array of UINavigationControllers that correspond to your
			// menu items:
			navigation.ViewControllers = Array.ConvertAll (Tasks, title =>
				   new UINavigationController (new TaskPageController (navigation, title))
			);

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

			MovieLatestLabel = new UILabel () {
				TextColor = UIColor.White, Frame = MovieLatestLabelFrame,
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
				Text = LatestMovies
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

			MovieLatestController = new MovieLatestViewController (flowLayout, MovieLatest, this);
			MovieLatestController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			MovieLatestController.CollectionView.RegisterClassForCell (typeof (MovieCell), movieCellId);
			MovieLatestController.CollectionView.Frame = MovieLatestControllerFrame;

			// Creates an instance of a custom View Controller that holds the results
 			var searchResultsController = new SearchResultsViewController (topRated[0]);

			//Creates a search controller updater
			var searchUpdater = new SearchResultsUpdator ();
			searchUpdater.UpdateSearchResults += searchResultsController.Search;

			//add the search controller
		    searchController = new UISearchController (searchResultsController) {
				SearchResultsUpdater = searchUpdater
			};

			//format the search bar
			searchController.SearchBar.SizeToFit ();
			searchController.SearchBar.SearchBarStyle = UISearchBarStyle.Minimal;
			searchController.SearchBar.Placeholder = "Enter a search query";

			// Main Part - Use KVO to access text field
			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).TextColor = UIColor.White;

			//the search bar is contained in the navigation bar, so it should be visible
			searchController.HidesNavigationBarDuringPresentation = false;

			//Ensure the searchResultsController is presented in the current View Controller 
			DefinesPresentationContext = true;

			//Set the search bar in the navigation bar
			NavigationItem.TitleView = searchController.SearchBar;

			//MovieLatestController.CollectionView.Frame = new CGRect (topRatedController.CollectionView.Frame.X, DefaultYPositionTopRatedController, topRatedController.CollectionView.Frame.Width, topRatedController.CollectionView.Frame.Height);
			//MovieLatestLabel.Frame = new CGRect (TopRatedLabel.Frame.X, DefaultYPositionTopRatedLabel, TopRatedLabel.Frame.Width, TopRatedLabel.Frame.Height);


			//scrollView.WillEndDragging += async (s, e) => {
			//	await
			//	//if (e.Velocity.X == 0) {


			//	//	// Animate the scroll to the right place
			//	//	if (page < 50) {
			//	//		UILabel nowPlayingLabel;
			//	//		FavoritesViewController movieCollection;
			//	Task.Run (async () => {
			//		var movieList = await MovieService.GetMoviesAsync (MovieService.MovieType.NowPaying, page + 1);
			//		InvokeOnMainThread (async () => {
			//			await AddCollectionView (movieList, "Now Playing", this);
			//		});
			//	});
			//	//task.Wait ();

			//	//	}
			//	//} else {
			//	//	//e.TargetContentOffset = CalculateContentOffset ();
			//	//}
			//};

			#endregion

		}
		public class SearchResultsUpdator : UISearchResultsUpdating
		{
			public event Action<string> UpdateSearchResults = delegate { };

			public override void UpdateSearchResultsForSearchController (UISearchController searchController)
			{
				this.UpdateSearchResults (searchController.SearchBar.Text);
			}
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
	public class SearchResultsViewController : UITableViewController
	{
		static readonly string movieItemCellId = "movieItemCellId";
		Movie movie;

		public List<Movie> MovieItems { get; set; }

		public SearchResultsViewController (Movie movie)
		{
			this.movie = movie;

			MovieItems = new List<Movie> ();
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return MovieItems.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (movieItemCellId);

			if (cell == null)
				cell = new UITableViewCell ();

			cell.TextLabel.Text = MovieItems [indexPath.Row].Title;
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			// add item to map
			//CLLocationCoordinate2D coord = MapItems [indexPath.Row].Placemark.Location.Coordinate;
			//map.AddAnnotations (new MKPointAnnotation () {
			//	Title = MapItems [indexPath.Row].Name,
			//	Coordinate = coord
			//});

			//map.SetCenterCoordinate (coord, true);

			//DismissViewController (false, null);
		}

		public void Search (string forSearchString)
		{
			// create search request
			//var searchRequest = new MKLocalSearchRequest ();
			//searchRequest.NaturalLanguageQuery = forSearchString;
			//searchRequest.Region = new MKCoordinateRegion (map.UserLocation.Coordinate, new MKCoordinateSpan (0.25, 0.25));

			//// perform search
			//var localSearch = new MKLocalSearch (searchRequest);

			//localSearch.Start (delegate (MKLocalSearchResponse response, NSError error) {
			//	if (response != null && error == null) {
			//		this.MapItems = response.MapItems.ToList ();
			//		this.TableView.ReloadData ();
			//	} else {
			//		Console.WriteLine ("local search error: {0}", error);
			//	}
			//});


		}
	}

	class TaskPageController : DialogViewController
	{
		public TaskPageController (FlyoutNavigationController navigation, string title) : base (null)
		{
			Root = new RootElement (title) {
					new Section {
						new CheckboxElement (title)
					}
				};
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem (UIBarButtonSystemItem.Action, delegate {
				navigation.ToggleMenu ();
			});
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
			ContentView.Layer.BorderColor = UIColor.Clear.CGColor;
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
					ContentView.Layer.BorderColor = UIColor.Clear.CGColor;
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



