using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using SidebarNavigation;
using SQLite;
using UIKit;
using LoginScreen;
using MovieFriends;
using SDWebImage;
using BigTed;
using Facebook.AudienceNetwork;

namespace FavoriteMovies
{/// <summary>
 /// This is the Main View controller for the application. It serves as the Top Rated collection of movies and creates the Now Playing 
 /// Poplular scrollable collections.
 /// </summary>
	public class MainViewController : BaseController, IInterstitialAdDelegate
	{
		// Generate your own Placement ID on the Facebook app settings
		const string YourPlacementId = "696067800580326_696647140522392";
		InterstitialAd interstitialAd;
		static CGSize HeaderReferenceSize = new CGSize (50, 50);
		static int MinimumInteritemSpacing = 30;
		static int SpaceBetweenContainers = 30;
		static int SeparationBuffer = 20;
		static int SpaceBetweenListTypes = 50;
		static int MinimumLineSpacing = 10;
		static int DefaultYPositionTopRatedLabel = 10;
		static int DefaultYPositionTopRatedController = 30;
		static int DefaultYPositionNowPlayingLabel = 270;
		static int DefaultYPositionPopularLabel = 530;
		static int DefaultYPositionMovieLatestLabel = 790;
		public static int NewCustomListToRefresh = -1;
		//public static int OldCustomListToRefresh;
		//static CGSize ItemSize = new CGSize (153, 205);
		//static CGSize ItemSize = new CGSize (133, 185);
		static CGRect FavoriteLabelFrame = new CGRect (7, 10, 180, 20);
		static CGRect TopRatedLabelFrame = new CGRect (7, 205, 180, 20);
		static CGRect NowPlayingLabelFrame = new CGRect (7, 400, 180, 20);
		static CGRect PopularLabelFrame = new CGRect (7, 595, 180, 20);
		static CGRect MovieLatestLabelFrame = new CGRect (7, 790, 180, 20);
		static int SpaceBetweenLabelsAndFrames = 245;
		static CGRect FavoriteControllerFrame = new CGRect (-45, 30, 375, 205);
		static CGRect TopRatedControllerFrame = new CGRect (-45, 225, 375, 205);
		static CGRect NowPlayingControllerFrame = new CGRect (-45, 420, 375, 205);
		static CGRect PopularControllerFrame = new CGRect (-45, 615, 375, 205);
		static CGRect MovieLatestControllerFrame = new CGRect (-45, 810, 375, 205);
		static string TopRated = "Top Rated";
		static string NowPlaying = "Now Playing";
		static string Popular = "Popular";
		static string LatestMovies = "Latest Movies";
		const int LabelZPosition = 1;
		const int SectionCount = 1;
		ObservableCollection<Movie> topRated= new ObservableCollection<Movie>();
		ObservableCollection<Movie> nowPlaying= new ObservableCollection<Movie> ();
		ObservableCollection<Movie> popular= new ObservableCollection<Movie> ();
		ObservableCollection<CustomList> customLists= new ObservableCollection<CustomList> ();
		ObservableCollection<Movie> MovieLatest= new ObservableCollection<Movie> ();
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
		//static UIView horizontalLine = new UIView();
		static UIImageView velvetRopes = new UIImageView ();

		private bool needLogin = true;


		//	public MainViewController (ObservableCollection<Movie> topRated, ObservableCollection<Movie> nowPlaying, ObservableCollection<Movie> popular, ObservableCollection<Movie> movieLatest,int page)
		public MainViewController ()
		{

			flowLayout = new UICollectionViewFlowLayout () {
				HeaderReferenceSize = HeaderReferenceSize,
				ScrollDirection = UICollectionViewScrollDirection.Horizontal,
				MinimumInteritemSpacing = MinimumInteritemSpacing, // minimum spacing between cells
				MinimumLineSpacing = MinimumLineSpacing, // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
				ItemSize = ColorExtensions.CurrentSize
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


		public  override void  ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			needLogin = ColorExtensions.CurrentUser.username == null;

			if (needLogin) 
			{
				LoginScreenControl<CredentialsProvider>.Activate (this);
				needLogin = false;

				SideMenuController.ShowTipsController ();
			}
		

			//NewsFeedTableSource.ShowTabBar (TabController);
			TabController.NavigationController.NavigationBar.Hidden = false;
			UIApplication.SharedApplication.SetStatusBarHidden (false, true);

			//searchResultsController.TableView.ContentInset = new UIEdgeInsets (80, 0, 0, 0);
			//HACK until i find out why when you open a movie details and come back the view.height changes.
			if (View.Frame.Height == 504) {

				scrollView.ContentSize = new CGSize (View.Frame.Width, View.Frame.Height);

			}

			////*****this fixes a problem with the uitableview adding space at the top after each selection*****
			//Debug.Write (searchResultsController.TableView.ContentInset);
			// adding label views to View. 

		}

		public static void getUser ()
		{
			
			using (var db = new SQLiteConnection (MovieService.Database)) {
				var task = Task.Run (() => {
					try {
						// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
						var query = db.Query<User> ("SELECT * FROM [User]");
						var currentUser =  new User () { tilesize = 2, suggestmovies = true, darktheme = false};

						if (query.Count > 0) 
						{
							currentUser.username = query [0].username;
							currentUser.email = query [0].email;
							currentUser.Id = query [0].Id;
							currentUser.city = query [0].city;
							currentUser.state = query [0].state;
							currentUser.country = query [0].country;
							currentUser.zip = query [0].zip;
							currentUser.lastname = query [0].lastname;
							currentUser.firstname = query [0].firstname;
							currentUser.phone =  query [0].phone;
							currentUser.suggestmovies = query[0].suggestmovies;
							currentUser.darktheme = query[0].darktheme;
							currentUser.tilesize = query[0].tilesize;
							currentUser.password = query [0].password;
							currentUser.removeAds = query[0].removeAds;
						}
						ColorExtensions.CurrentUser = currentUser;
						//favoriteViewController.CollectionView.ReloadData ();
					} catch (SQLiteException e) 
					{
						Debug.WriteLine (e.Message);
						using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) {
							conn.CreateTable<User> ();
							var query = db.Query<User> ("SELECT * FROM [User]");
							var currentUser = new User () { tilesize = 2, suggestmovies = true, darktheme = false};
							if (query.Count > 0) 
							{
								currentUser.username = query [0].username;
								currentUser.email = query [0].email;
								currentUser.Id = query [0].Id;
								currentUser.city = query [0].city;
								currentUser.state = query [0].state;
								currentUser.country = query [0].country;
								currentUser.zip = query [0].zip;
								currentUser.lastname = query [0].lastname;
								currentUser.firstname = query [0].firstname;
								currentUser.phone =  query [0].phone;
								currentUser.suggestmovies = query[0].suggestmovies;
								currentUser.darktheme = query[0].darktheme;
								currentUser.tilesize = query[0].tilesize;
								currentUser.password = query [0].password;
								currentUser.removeAds = query[0].removeAds;
							}
							ColorExtensions.CurrentUser = currentUser;
						}

					}
					catch (Exception ex)
					{
						Debug.Write (ex.Message);
					}
				});
				task.Wait ();
			}

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (NewCustomListToRefresh == -1) 
			{
				//BTProgressHUD.Show ();
               
				if (ColorExtensions.CurrentUser.suggestmovies)
                      GetCollectionData ();
				LoadCollectionViewControllers ();
				UpdateCustomViews ();
				FavoritesDisplay ();
				NewCustomListToRefresh = 0;
				//BTProgressHUD.Dismiss ();
			}

			SidebarController.Disabled = false;
			if (NewCustomListToRefresh > 1) 
			{
				LoadCollectionViewControllers ();
			}
			if (NewCustomListToRefresh > 0)// 0 means nothing has changed and i don't have to refresh lists
				FavoritesDisplay ();
			
			LoadViews ();

		}

		void UpdateCustomListMovies (int cnt)
		{
			var watch = System.Diagnostics.Stopwatch.StartNew ();
			customLabels = new UILabel [customLists.Count];
			customControllers = new FavoritesViewController [customLists.Count];
			customLabels [cnt] = new UILabel () {
				TextColor =  ColorExtensions.DarkTheme ? UIColor.White: UIColor.Black,// UIColor.Clear.FromHexString (ColorExtensions.TITLE_COLOR);
				BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
				Font = UIFont.FromName (ColorExtensions.TITLE_FONT, ColorExtensions.HEADER_FONT_SIZE)
			};
			customControllers [cnt] = new FavoritesViewController (new UICollectionViewFlowLayout () {
				MinimumInteritemSpacing = MinimumInteritemSpacing, MinimumLineSpacing = MinimumLineSpacing,
				HeaderReferenceSize = HeaderReferenceSize, ItemSize = ColorExtensions.CurrentSize,
				ScrollDirection = UICollectionViewScrollDirection.Horizontal
			}, new ObservableCollection<Movie> ((GetMovieList (customLists [cnt])).Reverse ()), this);

			customControllers [cnt].CollectionView.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			customControllers [cnt].CollectionView.RegisterClassForCell (typeof (MovieCell), FavoritesViewController.movieCellId);
			customLabels [cnt].Text = customLists [cnt].name + " (" + customControllers [cnt].CollectionView.NumberOfItemsInSection (0) + ")";
			UpdateCustomListsPosition (cnt);
			watch.Stop ();
			Console.WriteLine ("UpdateCustomListMovies Method took " + watch.ElapsedMilliseconds / 1000.0 + "seconds");

		}

		void DeleteAllSubviews (UIScrollView view)
		{
			foreach (UIView subview in view.Subviews) {
				subview.RemoveFromSuperview ();
			}

		}

		void FavoritesDisplay ()
		{
			var watch = System.Diagnostics.Stopwatch.StartNew ();
			DeleteAllSubviews (scrollView);
			//horizontalLine.Frame = new CGRect ();
			velvetRopes.Frame = new CGRect ();
			customLists = GetCustomLists ();
			if (customLists.Count == 0 && ColorExtensions.CurrentUser.suggestmovies ) 
			{
				CreateSuggestedListNoCustom ();
			
			} 
			else 
			{
				/// <summary>
				/// A new list was added(0) or this is the first time through (-1)
				/// </summary>
				/// <param name="customList">Custom list.</param>
				if (NewCustomListToRefresh ==1 || NewCustomListToRefresh == -1) 
				{


					customLabels = new UILabel [customLists.Count];
					customControllers = new FavoritesViewController [customLists.Count];
					for (var cnt = 0; cnt < customLists.Count; cnt++) 
					{

						UpdateCustomListMovies (cnt);

					}
					if(ColorExtensions.CurrentUser.suggestmovies)
					  CreateSuggestedListAfterCustom ();

				} //else 
				//{
				//	 UpdateCustomListMovies (NewCustomListToRefresh);
					  
				//}

				velvetRopes.Image = UIImage.FromBundle ("Divider_large_red_large.png");
				velvetRopes.Frame =new CGRect () { X = 0, Y = TopRatedLabelFrame.Y + (SpaceBetweenLabelsAndFrames * (customLists.Count - 1) + SpaceBetweenContainers + SeparationBuffer + SpaceBetweenListTypes) - 40, Width =View.Bounds.Width, Height = 20};


			}
			NewCustomListToRefresh = 0;

			//For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
			if (ColorExtensions.CurrentUser.suggestmovies)
				scrollView.ContentSize = new CGSize (View.Frame.Width, MovieLatestController.CollectionView.Frame.Y + MovieLatestController.CollectionView.Frame.Height + SpaceBetweenContainers + SeparationBuffer);
			else 
			{
				if(customLists.Count>0)
				   scrollView.ContentSize = new CGSize (View.Frame.Width, customControllers [customLists.Count - 1].CollectionView.Frame.Y + customControllers [customLists.Count - 1].CollectionView.Frame.Height + SpaceBetweenContainers + SeparationBuffer);
			}
			scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);
			watch.Stop ();
			Console.WriteLine ("GetFavorites Method took " + watch.ElapsedMilliseconds / 1000.0 + "seconds");

		}

		void CreateSuggestedListNoCustom ()
		{
			TopRatedLabel.Frame = new CGRect (TopRatedLabel.Frame.X, DefaultYPositionTopRatedLabel, TopRatedLabel.Frame.Width, TopRatedLabel.Frame.Height);
			topRatedController.CollectionView.Frame = new CGRect (TopRatedControllerFrame.X, DefaultYPositionTopRatedController, TopRatedControllerFrame.Width, TopRatedControllerFrame.Height);

			PlayingNowLabel.Frame = new CGRect (PlayingNowLabel.Frame.X, DefaultYPositionNowPlayingLabel, PlayingNowLabel.Frame.Width, PlayingNowLabel.Frame.Height);
			nowPlayingController.CollectionView.Frame = new CGRect (NowPlayingControllerFrame.X, DefaultYPositionNowPlayingLabel + SeparationBuffer, NowPlayingControllerFrame.Width, NowPlayingControllerFrame.Height);

			PopularLabel.Frame = new CGRect (PopularLabel.Frame.X, DefaultYPositionPopularLabel, PopularLabel.Frame.Width, PopularLabel.Frame.Height);
			popularController.CollectionView.Frame = new CGRect (PopularControllerFrame.X, DefaultYPositionPopularLabel + SeparationBuffer, PopularControllerFrame.Width, PopularControllerFrame.Height);

			MovieLatestLabel.Frame = new CGRect (MovieLatestLabel.Frame.X, DefaultYPositionMovieLatestLabel, MovieLatestLabel.Frame.Width, MovieLatestLabel.Frame.Height);
			MovieLatestController.CollectionView.Frame = new CGRect (MovieLatestControllerFrame.X, DefaultYPositionMovieLatestLabel + SeparationBuffer, MovieLatestControllerFrame.Width, MovieLatestControllerFrame.Height);

		}

		void CreateSuggestedListAfterCustom ()
		{
			TopRatedLabel.Frame = new CGRect () { X = TopRatedLabelFrame.X, Y = TopRatedLabelFrame.Y + (SpaceBetweenLabelsAndFrames * (customLists.Count - 1) + SpaceBetweenContainers + SeparationBuffer + SpaceBetweenListTypes), Height = TopRatedLabelFrame.Height, Width = TopRatedLabelFrame.Width };
			topRatedController.CollectionView.Frame = new CGRect () { X = TopRatedControllerFrame.X, Y = TopRatedLabel.Frame.Y + SeparationBuffer, Height = TopRatedControllerFrame.Height, Width = TopRatedControllerFrame.Width };

			PlayingNowLabel.Frame = new CGRect () { X = NowPlayingLabelFrame.X, Y = topRatedController.CollectionView.Frame.Y + topRatedController.CollectionView.Frame.Height + SpaceBetweenContainers, Height = NowPlayingLabelFrame.Height, Width = NowPlayingLabelFrame.Width };
			nowPlayingController.CollectionView.Frame = new CGRect () { X = NowPlayingControllerFrame.X, Y = PlayingNowLabel.Frame.Y + SeparationBuffer, Height = NowPlayingControllerFrame.Height, Width = NowPlayingControllerFrame.Width }; ;

			PopularLabel.Frame = new CGRect () { X = PopularLabelFrame.X, Y = nowPlayingController.CollectionView.Frame.Y + nowPlayingController.CollectionView.Frame.Height + SpaceBetweenContainers, Height = PopularLabelFrame.Height, Width = PopularLabelFrame.Width };
			popularController.CollectionView.Frame = new CGRect () { X = PopularControllerFrame.X, Y = PopularLabel.Frame.Y + SeparationBuffer, Height = PopularControllerFrame.Height, Width = PopularControllerFrame.Width };

			MovieLatestLabel.Frame = new CGRect () { X = MovieLatestLabelFrame.X, Y = popularController.CollectionView.Frame.Y + popularController.CollectionView.Frame.Height + SpaceBetweenContainers, Height = MovieLatestLabelFrame.Height, Width = MovieLatestLabelFrame.Width };
			MovieLatestController.CollectionView.Frame = new CGRect () { X = MovieLatestControllerFrame.X, Y = MovieLatestLabel.Frame.Y + SeparationBuffer, Height = MovieLatestControllerFrame.Height, Width = MovieLatestControllerFrame.Width };



		}

		void UpdateCustomListsPosition (int customList)
		{
			if (customList == 0) 
			{
				customLabels [customList].Frame = new CGRect () {
					X = FavoriteLabelFrame.X,
					Y = FavoriteLabelFrame.Y,
					Height = FavoriteLabelFrame.Height, Width = FavoriteLabelFrame.Width
				};
			}
			else
				
			{
				customLabels [customList].Frame = new CGRect () {
					X = FavoriteLabelFrame.X,
					Y = FavoriteLabelFrame.Y + (SpaceBetweenLabelsAndFrames * customList) + 10,
					Height = FavoriteLabelFrame.Height, Width = FavoriteLabelFrame.Width
				};
			}
			customControllers [customList].CollectionView.Frame = new CGRect () {
				X = FavoriteControllerFrame.X,
				Y = customLabels [customList].Frame.Y + SeparationBuffer,// FavoteControllerFrame.Y + (SpaceBetweenLabelsAndFrames * customList)
				Height = FavoriteControllerFrame.Height,
				Width = FavoriteControllerFrame.Width
			};
			scrollView.Add (customLabels [customList]);
			scrollView.Add (customControllers [customList].CollectionView);
		}

		 public static ObservableCollection<Movie> GetMovieList (CustomList customList)
		{
			var returnList = new ObservableCollection<Movie> ();
			try {
				var watch = System.Diagnostics.Stopwatch.StartNew ();
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) 
				{
					var task = Task.Run (() => {
						// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
						if (customList.id != null) {
							var query = db.Query<Movie> ("SELECT * FROM [Movie] WHERE [CustomListID] = " + customList.id);

							foreach (var movie in query) {
								var item = new Movie ();
								item.id = movie.id;
								item.name = movie.name;
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
								item.OriginalId = movie.OriginalId;
								item.cloudId = movie.cloudId;
								item.order = movie.order;
								returnList.Add (item);
							}
						}

					});
					task.Wait();

					watch.Stop ();
					Console.WriteLine ("GetMovieList Method took " + watch.ElapsedMilliseconds / 1000.0 + "seconds");

				}
			} catch (SQLite.SQLiteException e) {
				Debug.Write (e.Message);
				//throw;
			}
			return returnList;
		}
		public static int RandomNumber (int min, int max)
		{
			Random random = new Random (); return random.Next (min, max);

		}

		 void GetCollectionData ()
		{
			int Page;

			if (topRated.Count > 0)
				return;
			try 
			{

				var watch = System.Diagnostics.Stopwatch.StartNew ();
				Page = RandomNumber (1, 5);

				 Task.Run (async() => {
					 topRated = await  MovieService.GetMoviesAsync (MovieService.MovieType.TopRated, Page);
					 nowPlaying =  await MovieService.GetMoviesAsync (MovieService.MovieType.NowPlaying, Page);
					 popular =  await MovieService.GetMoviesAsync (MovieService.MovieType.Popular, Page);
					 MovieLatest =  await MovieService.GetMoviesAsync (MovieService.MovieType.Upcoming, Page);
					//TVNowAiring = await MovieService.GetMoviesAsync (MovieService.MovieType.TVLatest, Page);
				}).Wait();
				//TimeSpan ts = TimeSpan.FromMilliseconds (4000);
				watch.Stop ();
				Console.WriteLine ("GetCollectionData Method took " + watch.ElapsedMilliseconds / 1000.0 + "seconds");

				//	Console.WriteLine ("The timeout interval elapsed in GetCollectionData.");
			} catch (Exception e) {
				Debug.WriteLine (e.Message);
				//	throw;

			}
		}
		void LoadCollectionViewControllers ()
		{
			try {
				//scrollView.PagingEnabled = true;
				var watch = System.Diagnostics.Stopwatch.StartNew ();
				scrollView.Frame = new CGRect () { X = View.Frame.X, Y = View.Frame.Y, Width = View.Frame.Width, Height = View.Frame.Height };
				customLists = GetCustomLists ();

				customLabels = new UILabel [customLists.Count];
				customControllers = new FavoritesViewController [customLists.Count];

				TopRatedLabel = new UILabel () {
					TextColor = ColorExtensions.DarkTheme ? UIColor.White : UIColor.Black, Frame = TopRatedLabelFrame,
					BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
					Font = UIFont.FromName (ColorExtensions.TITLE_FONT, ColorExtensions.HEADER_FONT_SIZE),
					Text = TopRated
				};

				PlayingNowLabel = new UILabel () {
					TextColor = ColorExtensions.DarkTheme ? UIColor.White : UIColor.Black, Frame = NowPlayingLabelFrame,
					BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
					Font = UIFont.FromName (ColorExtensions.TITLE_FONT, ColorExtensions.HEADER_FONT_SIZE),
					Text = NowPlaying
				};
				PopularLabel = new UILabel () {
					TextColor = ColorExtensions.DarkTheme ? UIColor.White : UIColor.Black, Frame = PopularLabelFrame,
					BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
					Font = UIFont.FromName (ColorExtensions.TITLE_FONT, ColorExtensions.HEADER_FONT_SIZE),
					Text = Popular
				};

				MovieLatestLabel = new UILabel () {
					TextColor = ColorExtensions.DarkTheme ? UIColor.White : UIColor.Black, Frame = MovieLatestLabelFrame,
					BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
					Font = UIFont.FromName (ColorExtensions.TITLE_FONT, ColorExtensions.HEADER_FONT_SIZE),
					Text = LatestMovies
				};
				TopRatedLabel.Layer.ZPosition = LabelZPosition;
				PlayingNowLabel.Layer.ZPosition = LabelZPosition;
				PopularLabel.Layer.ZPosition = LabelZPosition;
				flowLayout = new UICollectionViewFlowLayout () {
					HeaderReferenceSize = HeaderReferenceSize,
					ScrollDirection = UICollectionViewScrollDirection.Horizontal,
					MinimumInteritemSpacing = MinimumInteritemSpacing, // minimum spacing between cells
					MinimumLineSpacing = MinimumLineSpacing, // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
					ItemSize = ColorExtensions.CurrentSize
				};
				//SectionInset = new UIEdgeInsets (80, -40, 97, 12			};


				topRatedController = new TopRatedCollectionViewController (flowLayout, topRated, this);
				topRatedController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
				topRatedController.CollectionView.RegisterClassForCell (typeof (MovieCell), TopRatedCollectionViewController.movieCellId);


				nowPlayingController = new NowPlayingCollectionViewController (flowLayout, nowPlaying, this);
				nowPlayingController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
				nowPlayingController.CollectionView.RegisterClassForCell (typeof (MovieCell), NowPlayingCollectionViewController.movieCellId);



				popularController = new PopularCollectionViewController (flowLayout, popular, this);
				popularController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
				popularController.CollectionView.RegisterClassForCell (typeof (MovieCell), PopularCollectionViewController.movieCellId);


				MovieLatestController = new MovieLatestViewController (flowLayout, MovieLatest, this);
				MovieLatestController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
				MovieLatestController.CollectionView.RegisterClassForCell (typeof (MovieCell), MovieLatestViewController.movieCellId);

				watch.Stop ();
				Console.WriteLine ("LoadCollectionViewControllers Method took " + watch.ElapsedMilliseconds / 1000.0 + "seconds");
			}
			    catch(Exception ex)
			{
				Debug.WriteLine (ex.Message);
			}

		}

		void UpdateCustomViews ()
		{
			customLists = GetCustomLists ();
			if (NewCustomListToRefresh == 1 || NewCustomListToRefresh == -1) {
				for (var cnt = 0; cnt < customLists.Count; cnt++) {
					UpdateCustomListMovies (cnt);
				}

			} else {
				UpdateCustomListMovies (NewCustomListToRefresh);
			}

			View.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);

		}

		void LoadViews ()
		{
			if (ColorExtensions.CurrentUser.suggestmovies) 
			{
				scrollView.AddSubview (topRatedController.CollectionView);
				scrollView.AddSubview (nowPlayingController.CollectionView);
				scrollView.AddSubview (popularController.CollectionView);
				scrollView.AddSubview (MovieLatestController.CollectionView);

				scrollView.AddSubview (PlayingNowLabel);
				scrollView.AddSubview (PopularLabel);
				scrollView.AddSubview (MovieLatestLabel);
				scrollView.AddSubview (TopRatedLabel);
				//scrollView.AddSubview (horizontalLine);
				scrollView.AddSubview (velvetRopes);
			}
			View.AddSubview (scrollView);
			if (!ColorExtensions.CurrentUser.removeAds && ColorExtensions.CurrentUser.username!=null) 
			{
				// Create a banner's ad view with a unique placement ID (generate your own on the Facebook app settings).
				// Use different ID for each ad placement in your app.
				interstitialAd = new InterstitialAd (YourPlacementId) {
					Delegate = this
				};

				// When testing on a device, add its hashed ID to force test ads.
				// The hash ID is printed to console when running on a device.
				AdSettings.AddTestDevice (AdSettings.TestDeviceHash);

				// Initiate a request to load an ad.
				interstitialAd.LoadAd ();

				// Verify if the ad is ready to be shown, if not, you will need to check later somehow (with a button, timer, delegate, etc.)
				if (interstitialAd.IsAdValid) {
					// Ad is ready, present it!
					interstitialAd.ShowAdFromRootViewController (this);
				}
			}
		}
		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

		}
		public override  void ViewDidLoad ()
		{
			base.ViewDidLoad ();
            
			// Creates an instance of a custom View Controller that holds the results
			searchResultsController = new SearchResultsViewController ();

			//Creates a search controller updater
			var searchUpdater = new SearchResultsUpdator ();
			searchUpdater.UpdateSearchResults += searchResultsController.Search;

			//add the search controller
			searchController = new UISearchController (searchResultsController) 
			{
				SearchResultsUpdater = searchUpdater,

				WeakDelegate = searchUpdater,
				WeakSearchResultsUpdater = searchUpdater,
			};

			searchResultsController.searchController = searchController;

			//format the search bar
			searchController.SearchBar.SizeToFit ();
			searchController.SearchBar.SearchBarStyle = UISearchBarStyle.Default;
			//searchController.SearchBar.Placeholder = "Add Your Movies";
           

			//searchResultsController.TableView.WeakDelegate = this;
			searchController.SearchBar.WeakDelegate = searchResultsController;

			//((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).TextColor = UIColor.White;
			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);
			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).BackgroundColor = UIColor.White;
			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).ResignFirstResponder ();
			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).AttributedPlaceholder=new NSAttributedString("Add Your Movies", null, UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, BackGroundColorAlpha));
			//the search bar is contained in the navigation bar, so it should be visible
			searchController.HidesNavigationBarDuringPresentation = false;

			//Ensure the searchResultsController is presented in the current View Controller 
			DefinesPresentationContext = true;

			//Set the search bar in the navigation bar
			TabController.NavigationItem.TitleView = searchController.SearchBar;
			NavigationItem.SetRightBarButtonItem (
					new UIBarButtonItem (UIImage.FromBundle ("threelines")
						, UIBarButtonItemStyle.Plain
						, (sender, args) => {
							SidebarController.ToggleMenu ();

						}), true);

		}



		public static ObservableCollection<CustomList> GetCustomLists ()
		{
			

			var result = new ObservableCollection<CustomList> ();
			try {
					using (var db = new SQLiteConnection (MovieService.Database)) 
					{
						//Task.Run (async () => 
						//{
							
						// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
						var query = db.Query<CustomList> ("SELECT * FROM [CustomList] WHERE [custom] = 1 ORDER BY [Order]");
						//var query = db.Table<CustomList> ();
						foreach (var list in query) 
						{
							var item = new CustomList ();
							item.id = list.id;
							item.name = list.name;
							item.cloudId = list.cloudId;
							item.custom = list.custom;
							result.Add (item);
						}
								

						//}).Wait();

					}
				} catch (SQLiteException e) 
				{
					Debug.WriteLine (e.Message);
					//throw;
				}
			return result;
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		#region IInterstitialAdDelegate

		[Export ("interstitialAdDidLoad:")]
		public void InterstitialAdDidLoad (InterstitialAd interstitialAd)
		{
			// Handle when the ad is loaded and ready to be shown
			if (interstitialAd.IsAdValid) {
				// Ad is ready, present it!
				interstitialAd.ShowAdFromRootViewController (this);
			}
		}

		[Export ("interstitialAd:didFailWithError:")]
		public void IntersitialDidFail (InterstitialAd interstitialAd, NSError error)
		{
			// Handle if the ad is not loaded correctly
		}

		[Export ("interstitialAdDidClick:")]
		public void InterstitialAdDidClick (InterstitialAd interstitialAd)
		{
			// Handle when the user tap the ad
		}

		[Export ("interstitialAdDidClose:")]
		public void InterstitialAdDidClose (InterstitialAd interstitialAd)
		{
			// Handle when the user close the ad
		}

		[Export ("interstitialAdWillClose:")]
		public void InterstitialAdWillClose (InterstitialAd interstitialAd)
		{
			// Handle before the ad is closed
		}

		#endregion
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
				ContentView.Layer.BorderWidth = -5.0f;
				//ContentView.Layer.BorderColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f).CGColor;
				ContentView.AddSubview (ImageView);
			} catch (Exception ex) { Debug.Write (ex.Message); throw; }

		}


		//public UILabel LabelView { get; private set; }
		public void UpdateRow (Movie element)
		{
		try 
			{
				if (element.HighResPosterPath != null) {
					var uri = new Uri (element.HighResPosterPath);
					var imgUrl = new NSUrl (baseUrl + uri.AbsoluteUri.Substring (8));
					ImageView.SetImage (imgUrl, UIImage.FromBundle ("blank.png"));

				} else 
				{
					var uri = new Uri (element.PosterPath);
					var imgUrl = new NSUrl (baseUrl + uri.AbsoluteUri.Substring (8));
					ImageView.SetImage (imgUrl, UIImage.FromBundle ("blank.png"));
				}
				if (ColorExtensions.MovieIsFavorite (element.id.ToString ())) 
				{
					//ContentView.Layer.BorderColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR).CGColor;
					//ContentView.Layer.BorderWidth = 2.0f;
				} else {
					//ContentView.Layer.BorderColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f).CGColor;
				}
			} catch (SQLite.SQLiteException ex) 
			{
				//no favorites yet
				Debug.Write (ex.Message);
				//throw;
			}
		}
		public static Uri GetImageUrl (string posterPath)
		{
			
			var returnImage = new Uri (posterPath);

			if (posterPath != null) {
				var uri = new Uri (posterPath);
				using (var imgUrl = new NSUrl (HttpUtility.UrlPathEncode (uri.AbsoluteUri))) 
				{
					returnImage = imgUrl;

				}
			}
			return returnImage;

		}

		public static Uri GetImage (string posterPath)
		{
			
			var returnImage = new Uri (posterPath);

			if (posterPath != null) {
				var uri = new Uri (posterPath);
				using (var imgUrl = new NSUrl (baseUrl + uri.AbsoluteUri.Substring (8))) {
					returnImage = imgUrl;

				}
			}

			return returnImage;


		}
	}



}










