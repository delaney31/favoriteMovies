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
		

		void SearchResultsController_TouchesCancelled (NSSet arg1, UIEvent arg2)
		{

		}

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
		static CGRect FavoriteLabelFrame = new CGRect (7, 10, 180, 20);
		static CGRect TopRatedLabelFrame = new CGRect (7, 185, 180, 20);
		static CGRect NowPlayingLabelFrame = new CGRect (7, 360, 180, 20);
		static CGRect PopularLabelFrame = new CGRect (7, 535, 180, 20);
		static CGRect MovieLatestLabelFrame = new CGRect (7, 710, 180, 20);
		static int SpaceBetweenLabelsAndFrames = 175;
		static CGRect FavoriteControllerFrame = new CGRect (-45, 30, 385, 150);
		static CGRect TopRatedControllerFrame = new CGRect (-45, 205, 385, 150);
		static CGRect NowPlayingControllerFrame = new CGRect (-45, 380, 385, 150);
		static CGRect PopularControllerFrame = new CGRect (-45, 555, 385, 150);
		static CGRect MovieLatestControllerFrame = new CGRect (-45, 730, 385, 150);
		const float BackGroundColorAlpha = 1.0f;
		static string TopRated = "Top Rated";
		static string NowPlaying = "Now Playing";
		static string Popular = "Popular";
		static string Favorite = "My Favorites";
		static string LatestMovies = "Latest Movies";
		const int LabelZPosition = 1;
		const int SectionCount = 1;
		ObservableCollection<Movie> topRated;
		ObservableCollection<Movie> nowPlaying;
		ObservableCollection<Movie> popular;
		ObservableCollection<CustomList> customLists;
		static int customListCount;
		ObservableCollection<Movie> MovieLatest;
		//ObservableCollection<Movie> TvNowAiring;
		UICollectionViewFlowLayout flowLayout;
		BaseViewController nowPlayingController;
		PopularCollectionViewController popularController;
		TopRatedCollectionViewController topRatedController;
		//FavoritesViewController favoriteViewController;
		MovieLatestViewController MovieLatestController;
		UISearchController searchController;
		FavoritesViewController [] customControllers;
		UILabel [] customLabels = null;
		static UILabel TopRatedLabel;
		static UILabel PlayingNowLabel;
		static UILabel PopularLabel;
		static UILabel FavoriteLabel;
		//static UILabel nowPlayingLabel;
		static UILabel MovieLatestLabel;
		static UIScrollView scrollView = new UIScrollView ();
		static SearchResultsViewController searchResultsController;
		static nfloat lastLabelY = PopularLabelFrame.Y;
		static nfloat lastFrameY = PopularControllerFrame.Y;
	

		public MainViewController (ObservableCollection<Movie> topRated, ObservableCollection<Movie> nowPlaying, ObservableCollection<Movie> popular, ObservableCollection<Movie> movieLatest, ObservableCollection<Movie> TVNowAiring,int page)
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
			customLists = GetCustomLists ();
			customLabels = new UILabel [customLists.Count];
			customControllers = new FavoritesViewController [customLists.Count];

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
															 (MovieService.MovieType.NowPlaying, x)))));
				}


				Task.WhenAll (Tasks);

			}
		}
		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
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
			//searchResultsController.TableView.ContentInset = new UIEdgeInsets (80, 0, 0, 0);


		}


		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);


			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).ResignFirstResponder ();

			//this fixes problem when coming out of full screen after watching a trailer
			//NavigationController.NavigationBar.Frame = new CGRect () { X = 0, Y = 20, Width = 320, Height = 44 };
			//DeleteAllSubviews (scrollView);

			customLists = GetCustomLists ();

			customLabels = new UILabel [customLists.Count];
			customControllers = new FavoritesViewController [customLists.Count];
			for (var cnt = 0; cnt < customLists.Count; cnt++) {
				//var movies = GetMovieList (customLists [cnt]);
				//if (movies.Count > 0) 
				//{
				customLabels [cnt] = new UILabel () {
					TextColor = UIColor.Black, 
					//Frame = FavoriteLabelFrame,
					BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
					Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
					Text = customLists [cnt].Name
				};

				customControllers [cnt] = new FavoritesViewController (new UICollectionViewFlowLayout () {
					MinimumInteritemSpacing = MinimumInteritemSpacing, MinimumLineSpacing = MinimumLineSpacing,
					HeaderReferenceSize = HeaderReferenceSize, ItemSize = ItemSize,
					ScrollDirection = UICollectionViewScrollDirection.Horizontal
				}, GetMovieList (customLists [cnt]));
				customControllers [cnt].CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
				customControllers [cnt].CollectionView.RegisterClassForCell (typeof (MovieCell), FavoritesViewController.movieCellId);
				//customControllers [cnt].CollectionView.Frame = FavoriteControllerFrame;
				//}
			}

				
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

		void DeleteAllSubviews (UIScrollView view)
		{
			foreach(UIView subview in view.Subviews)
			{
				subview.RemoveFromSuperview ();
			}

		}

		void FavoritesDisplay ()
		{
			
				if (customLists.Count == 0) {
					topRatedController.CollectionView.Frame =new CGRect (TopRatedControllerFrame.X, DefaultYPositionTopRatedController, TopRatedControllerFrame.Width, TopRatedControllerFrame.Height);
					TopRatedLabel.Frame = new CGRect (TopRatedLabel.Frame.X, DefaultYPositionTopRatedLabel, TopRatedLabel.Frame.Width, TopRatedLabel.Frame.Height);

					nowPlayingController.CollectionView.Frame = new CGRect (NowPlayingControllerFrame.X, DefaultYPositionNowPlayingController, NowPlayingControllerFrame.Width, NowPlayingControllerFrame.Height);
					PlayingNowLabel.Frame = new CGRect (PlayingNowLabel.Frame.X, DefaultYPositionNowPlayingLabel, PlayingNowLabel.Frame.Width, PlayingNowLabel.Frame.Height);

					popularController.CollectionView.Frame = new CGRect (PopularControllerFrame.X, DefaultYPositionPopularController, PopularControllerFrame.Width, PopularControllerFrame.Height);
					PopularLabel.Frame = new CGRect (PopularLabel.Frame.X, DefaultYPositionPopularLabel, PopularLabel.Frame.Width, PopularLabel.Frame.Height);

					MovieLatestController.CollectionView.Frame = new CGRect (MovieLatestControllerFrame.X, DefaultYPositionMovieLatestController, MovieLatestControllerFrame.Width, MovieLatestControllerFrame.Height);
					MovieLatestLabel.Frame = new CGRect (MovieLatestLabel.Frame.X, DefaultYPositionMovieLatestLabel, MovieLatestLabel.Frame.Width, MovieLatestLabel.Frame.Height);

					//For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
					scrollView.ContentSize = new CGSize (320, View.Frame.Height + 155);
					scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);

				} else 
				{
					
					for (var cnt = 0; cnt < customLists.Count; cnt++)
					{
						
						customLabels[cnt].Frame = new CGRect () { X =FavoriteLabelFrame.X, 
							Y = FavoriteLabelFrame.Y + (175 * cnt), 
							Height = FavoriteLabelFrame.Height, Width = FavoriteLabelFrame.Width };
 						customControllers[cnt].CollectionView.Frame  = new CGRect () { X = FavoriteControllerFrame.X, 
							Y = FavoriteControllerFrame.Y + (175 *cnt), Height = FavoriteControllerFrame.Height, 
							Width = FavoriteControllerFrame.Width };
						scrollView.AddSubview (customLabels [cnt]);
						scrollView.AddSubview (customControllers [cnt].CollectionView);

					}
				TopRatedLabel.Frame = new CGRect () { X = TopRatedLabelFrame.X, Y = TopRatedLabelFrame.Y + (175 * (customLists.Count-1)), Height = TopRatedLabelFrame.Height, Width = TopRatedLabelFrame.Width };
				topRatedController.CollectionView.Frame = new CGRect () { X = TopRatedControllerFrame.X, Y = TopRatedControllerFrame.Y + (175 * (customLists.Count-1)), Height = TopRatedControllerFrame.Height, Width = TopRatedControllerFrame.Width };

				nowPlayingController.CollectionView.Frame = new CGRect () { X = NowPlayingControllerFrame.X, Y = NowPlayingControllerFrame.Y + (175 * (customLists.Count-1)), Height = NowPlayingControllerFrame.Height, Width = NowPlayingControllerFrame.Width };;
				PlayingNowLabel.Frame = new CGRect () { X = NowPlayingLabelFrame.X, Y = NowPlayingLabelFrame.Y + (175 * (customLists.Count-1)), Height = NowPlayingLabelFrame.Height, Width = NowPlayingLabelFrame.Width };

				popularController.CollectionView.Frame = new CGRect () { X = PopularControllerFrame.X, Y = PopularControllerFrame.Y + (175 * (customLists.Count-1)), Height = PopularControllerFrame.Height, Width = PopularControllerFrame.Width };
				PopularLabel.Frame = new CGRect () { X = PopularLabelFrame.X, Y = PopularLabelFrame.Y + (175 * (customLists.Count-1)), Height = PopularLabelFrame.Height, Width = PopularLabelFrame.Width };

				MovieLatestController.CollectionView.Frame = new CGRect () { X = MovieLatestControllerFrame.X, Y = MovieLatestControllerFrame.Y + (175 * (customLists.Count-1)), Height = MovieLatestControllerFrame.Height, Width = MovieLatestControllerFrame.Width };
				MovieLatestLabel.Frame = new CGRect () { X = MovieLatestLabelFrame.X, Y = MovieLatestLabelFrame.Y + (175 * (customLists.Count-1)), Height = MovieLatestLabelFrame.Height, Width = MovieLatestLabelFrame.Width };

				//For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
				scrollView.ContentSize = new CGSize (View.Frame.Width, 898 + (175 * (customLists.Count-1)));
					scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);

				}
			
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
						item.Adult = movie.Adult;
						item.BackdropPath = movie.BackdropPath;
						item.CustomListID = movie.CustomListID;
						item.Favorite = movie.Favorite;
						item.HighResPosterPath = movie.HighResPosterPath;
						item.OriginalLanguage = movie.OriginalLanguage;
						item.Overview = movie.Overview;
						item.Popularity = movie.Popularity;
						item.PosterPath = movie.PosterPath;
						item.ReleaseDate = movie.ReleaseDate;

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

			FavoriteLabel = new UILabel () {
				TextColor = UIColor.Black, Frame = FavoriteLabelFrame,
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
				Text = Favorite
			};


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
			}, topRated);
			topRatedController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			topRatedController.CollectionView.RegisterClassForCell (typeof (MovieCell), TopRatedCollectionViewController.movieCellId);
			//topRatedController.CollectionView.Frame = TopRatedControllerFrame;

			View.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);

			nowPlayingController = new NowPlayingCollectionViewController (new UICollectionViewFlowLayout () {
				MinimumInteritemSpacing = MinimumInteritemSpacing, MinimumLineSpacing = MinimumLineSpacing,
				HeaderReferenceSize = HeaderReferenceSize, ItemSize = ItemSize,
				ScrollDirection = UICollectionViewScrollDirection.Horizontal
			}, nowPlaying);
			nowPlayingController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			nowPlayingController.CollectionView.RegisterClassForCell (typeof (MovieCell), NowPlayingCollectionViewController.movieCellId);
			//nowPlayingController.CollectionView.Frame = NowPlayingControllerFrame;


			popularController = new PopularCollectionViewController (new UICollectionViewFlowLayout () {
				MinimumInteritemSpacing = MinimumInteritemSpacing, MinimumLineSpacing = MinimumLineSpacing,
				HeaderReferenceSize = HeaderReferenceSize, ItemSize = ItemSize,
				ScrollDirection = UICollectionViewScrollDirection.Horizontal
			}, popular);
			popularController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			popularController.CollectionView.RegisterClassForCell (typeof (MovieCell), PopularCollectionViewController.movieCellId);
			//popularController.CollectionView.Frame = PopularControllerFrame;

			MovieLatestController = new MovieLatestViewController (flowLayout, MovieLatest);
			MovieLatestController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			MovieLatestController.CollectionView.RegisterClassForCell (typeof (MovieCell), MovieLatestViewController.movieCellId);
			//MovieLatestController.CollectionView.Frame = MovieLatestControllerFrame;

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
			searchController.SearchBar.Placeholder = "Enter a search query";

			//searchResultsController.TableView.WeakDelegate = this;
			searchController.SearchBar.WeakDelegate = searchResultsController;

			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).TextColor = UIColor.White;

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
		UINavigationController navigationController;
		public UISearchController searchController;

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
			this.navigationController = navigationController;
	
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
				cell.DetailTextLabel.Text = MovieItems [indexPath.Row].Overview;
				cell.ImageView.Image = MovieCell.GetImage (MovieItems [indexPath.Row].PosterPath); // don't use for Value2
			}
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			try {
				
				var row = MovieItems [indexPath.Row];

				((UINavigationController)(UIApplication.SharedApplication.Delegate as AppDelegate).Window.RootViewController).PushViewController (new MovieDetailsViewController (row), true);

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
			// perform search
			if (forSearchString.Length > 0) {
				MovieItems = await MovieService.MovieSearch (forSearchString);
				TableView.ReloadData ();
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



