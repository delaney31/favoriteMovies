using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
		static int MinimumLineSpacing = 5;
		static int DefaultYPositionTopRatedController = 30;
		static int DefaultYPositionTopRatedLabel = 10;
		static int DefaultYPositionNowPlayingLabel = 215;
		static int DefaultYPositionPopularLabel = 420;
		static int DefaultYPositionMovieLatestLabel = 625;
		public static int NewCustomListToRefresh =-1;
		//public static int OldCustomListToRefresh;
		static CGSize ItemSize = new CGSize (100,150);
		static CGRect FavoriteLabelFrame = new CGRect (7, 10, 180, 20);
		static CGRect TopRatedLabelFrame = new CGRect (7, 185, 180, 20);
		static CGRect NowPlayingLabelFrame = new CGRect (7, 360, 180, 20);
		static CGRect PopularLabelFrame = new CGRect (7, 535, 180, 20);
		static CGRect MovieLatestLabelFrame = new CGRect (7, 710, 180, 20);
		static int SpaceBetweenLabelsAndFrames = 200;
		static CGRect FavoriteControllerFrame = new CGRect (-45, 30, 385, 150);
		static CGRect TopRatedControllerFrame = new CGRect (-45, 205, 385, 150);
		static CGRect NowPlayingControllerFrame = new CGRect (-45, 380, 385, 150);
		static CGRect PopularControllerFrame = new CGRect (-45, 555, 385, 150);
		static CGRect MovieLatestControllerFrame = new CGRect (-45, 730, 385, 150);
		const float BackGroundColorAlpha = 1.0f;
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
		UISearchController searchController;
		FavoritesViewController [] customControllers;
		UILabel [] customLabels = null;
		static UILabel TopRatedLabel;
		static UILabel PlayingNowLabel;
		static UILabel PopularLabel;
		static UILabel MovieLatestLabel;
		static UIScrollView scrollView = new UIScrollView ();
		static SearchResultsViewController searchResultsController;

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
			searchResultsController.TableView.ContentInset = new UIEdgeInsets (80, 0, 0, 0);

		}


		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			SidebarController.Disabled = false;

			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).ResignFirstResponder ();

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
			},new ObservableCollection<Movie>(GetMovieList (customLists [cnt]).Reverse ()),this);

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
				topRatedController.CollectionView.Frame =new CGRect (TopRatedControllerFrame.X, DefaultYPositionTopRatedController, TopRatedControllerFrame.Width, TopRatedControllerFrame.Height);
				TopRatedLabel.Frame = new CGRect (TopRatedLabel.Frame.X, DefaultYPositionTopRatedLabel, TopRatedLabel.Frame.Width, TopRatedLabel.Frame.Height);

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
					TopRatedLabel.Frame = new CGRect () { X = TopRatedLabelFrame.X, Y = TopRatedLabelFrame.Y + (SpaceBetweenLabelsAndFrames * (customLists.Count - 1)+ SpaceBetweenContainers+ SeparationBuffer), Height = TopRatedLabelFrame.Height, Width = TopRatedLabelFrame.Width };
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
				Y = FavoriteLabelFrame.Y + (SpaceBetweenLabelsAndFrames * customList),
				Height = FavoriteLabelFrame.Height, Width = FavoriteLabelFrame.Width
			};
			customControllers [customList].CollectionView.Frame = new CGRect () {
				X = FavoriteControllerFrame.X,
				Y = FavoriteControllerFrame.Y + (SpaceBetweenLabelsAndFrames * customList), Height = FavoriteControllerFrame.Height,
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
						item.Title = movie.Title;
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

			#region   //Setup my movie collection controllers


			topRatedController = new TopRatedCollectionViewController (new UICollectionViewFlowLayout () {
				MinimumInteritemSpacing = MinimumInteritemSpacing, MinimumLineSpacing = MinimumLineSpacing,
				HeaderReferenceSize = HeaderReferenceSize, ItemSize = ItemSize,
				ScrollDirection = UICollectionViewScrollDirection.Horizontal
			}, topRated, this);
			topRatedController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			topRatedController.CollectionView.RegisterClassForCell (typeof (MovieCell), TopRatedCollectionViewController.movieCellId);
		

			View.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);

			nowPlayingController = new NowPlayingCollectionViewController (new UICollectionViewFlowLayout () {
				MinimumInteritemSpacing = MinimumInteritemSpacing, MinimumLineSpacing = MinimumLineSpacing,
				HeaderReferenceSize = HeaderReferenceSize, ItemSize = ItemSize,
				ScrollDirection = UICollectionViewScrollDirection.Horizontal
			}, nowPlaying, this);
			nowPlayingController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			nowPlayingController.CollectionView.RegisterClassForCell (typeof (MovieCell), NowPlayingCollectionViewController.movieCellId);



			popularController = new PopularCollectionViewController (new UICollectionViewFlowLayout () {
				MinimumInteritemSpacing = MinimumInteritemSpacing, MinimumLineSpacing = MinimumLineSpacing,
				HeaderReferenceSize = HeaderReferenceSize, ItemSize = ItemSize,
				ScrollDirection = UICollectionViewScrollDirection.Horizontal
			}, popular, this);
			popularController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			popularController.CollectionView.RegisterClassForCell (typeof (MovieCell), PopularCollectionViewController.movieCellId);


			MovieLatestController = new MovieLatestViewController (flowLayout, MovieLatest, this);
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
			searchResultsController = new SearchResultsViewController (NavigationController);

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
			searchController.SearchBar.SearchBarStyle = UISearchBarStyle.Minimal;
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
			NavigationItem.TitleView = searchController.SearchBar;


			#endregion

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
	public class SearchResultsUpdator : UISearchResultsUpdating
	{
		public event Action<string> UpdateSearchResults = delegate { };

		public override void UpdateSearchResultsForSearchController (UISearchController searchController)
		{
			this.UpdateSearchResults (searchController.SearchBar.Text);

		}
	}
	public class SearchResultsViewController : UITableViewController
	{
		static readonly string movieItemCellId = "movieItemCellId";
		public UISearchController searchController;
		UINavigationController navController;
		public ObservableCollection<Movie> MovieItems { get; set; }

		 [Export ("scrollViewWillEndDragging:withVelocity:targetContentOffset:")]
		public void WillEndDragging (UIScrollView scrollView, CGPoint velocity, ref CGPoint targetContentOffset)
		{
			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).ResignFirstResponder ();
		}

		[Export ("searchBarCancelButtonClicked:")]
		public void searchBarCancelButtonClicked (UISearchBar searchBar)
		{
			Console.WriteLine ("The default search bar cancel button was tapped.");
			//var size = ((UIScrollView)navigationController.TopViewController.View.Subviews [0]).ContentSize;

			searchBar.ResignFirstResponder ();

			MovieItems.Clear ();
			TableView.ReloadData ();
		}


		public SearchResultsViewController (UINavigationController navigationController)
		{
			MovieItems = new ObservableCollection<Movie> ();
			navController = navigationController;
	
		}


		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return MovieItems.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (movieItemCellId);

			if (cell == null) 
			{
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, movieItemCellId);
			}
			if (MovieItems.Count > indexPath.Row) 
			{
				cell.TextLabel.Text = MovieItems [indexPath.Row].Title;
				cell.TextLabel.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE);
				cell.DetailTextLabel.Text = MovieItems [indexPath.Row].Overview;
				cell.ImageView.Image = MovieCell.GetImage (MovieItems [indexPath.Row].PosterPath); // don't use for Value2
			}
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			try {
				
				var row = MovieItems [indexPath.Row];

				navController.PushViewController (new MovieDetailViewController (row, false), true);

				//*****this fixes a problem with the uitableview adding space at the top after each selection*****

				Debug.Write (this.TableView.ContentInset);
				this.TableView.ContentInset = new UIEdgeInsets (80, 0, 0, 0);


			} catch (Exception e) 
			{
				Debug.WriteLine (e.Message);
			}
		}

		public async void Search (string forSearchString)
		{
			try {
				// perform search
				if (forSearchString.Length > 0) {
					MovieItems = await MovieService.MovieSearch (forSearchString);
					TableView.ReloadData ();
				}
			} catch (Exception ex) 
			{
				Debug.WriteLine (ex.Message);
			}

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
				return UIImage.FromBundle ("blank.png");
			}

		}
	}



}










