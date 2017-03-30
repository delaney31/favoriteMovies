using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using BigTed;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using MovieFriends;
using SDWebImage;
using UIKit;


namespace FavoriteMovies
{
	public class MovieDetailViewController : BaseController
	{

		UILabel dateOpenView = new UILabel ();
		UILabel descReview = new UILabel ();
		UILabel descriptView = new UILabel ();
		UILabel movieTitle = new UILabel ();
		UIButton addReviewButt = new UIButton ();
		UIImageView posterImage = new UIImageView ();
		UIButton saveFavoriteButt = new UIButton ();
		UIImageView IMDB = new UIImageView ();
		UILabel voteResultText = new UILabel ();
		UILabel userResultText = new UILabel ();

		/// <summary>
		/// This is the view controller for the movie details page. In addition it allows you to save and clear favorite movies
		/// </summary>

		//static string _googleApiKey = "AIzaSyCu634TJuZR_0iUhJQ6D8E9xr2a3VbU3_M";
		static string _youTubeURl = "https://www.youtube.com/embed/";
		static string _embededMoveId;
		public static string URL;
		static CGSize HeaderReferenceSize = new CGSize (50, 50);
		static int MinimumInteritemSpacing = 30;
		static int MinimumLineSpacing = 5;
		static UILabel similarMoviesLabel;
		static CGSize ItemSize = new CGSize (100, 150);
		ObservableCollection<Movie> similarMovies = new ObservableCollection<Movie>();
		static ObservableCollection<CastCrew> castList;
		SimilarCollectionViewController similarMoviesController;
		Movie movieDetail;
		UIImageView moviePlay;
		UILabel userName;
		UIWebView webView;
		new UIScrollView scrollView = new UIScrollView ();
		static UILabel castHeader;
		//static UILabel cast;
		static UIImageView Userstar1;
		static UIImageView Userstar2;
		static UIImageView Userstar3;
		static UIImageView Userstar4;
		static UIImageView Userstar5;
		bool canReview;
		string youtubeMovieId = "";
		UIColor backGroundColor;
		string text;

		public MovieDetailViewController (Movie movie, bool canReview)
		{
			movieDetail = movie;
			this.canReview = canReview;

			moviePlay = new UIImageView () { UserInteractionEnabled = true };
			Userstar1 = new UIImageView () { UserInteractionEnabled = true };
			Userstar2 = new UIImageView () { UserInteractionEnabled = true };
			Userstar3 = new UIImageView () { UserInteractionEnabled = true };
			Userstar4 = new UIImageView () { UserInteractionEnabled = true };
			Userstar5 = new UIImageView () { UserInteractionEnabled = true };
			IMDB = new UIImageView ();

			moviePlay.Image = UIImage.FromBundle ("download.png");

			IMDB.Image = UIImage.FromBundle ("imdb.png");

		}

		public MovieDetailViewController (Movie movie, bool canReview, string text) : this (movie, canReview)
		{
			this.text = text;
		}

		void Initialize ()
		{
			try {

				var imDbUrl = "http://api.themoviedb.org/3/movie/" + movieDetail.OriginalId + "/videos?api_key=" + MovieService._apiKey;


				var UTubeMovidId = Task.Run (async () => {
					youtubeMovieId = await MovieService.GetYouTubeMovieId (imDbUrl);

				});
				UTubeMovidId.Wait ();

			} catch (Exception ex) 
			{
				Debug.Write (ex.Message);
			}


		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			const string baseUrl = "https://image.tmdb.org/t/p/w300/";
			scrollView.Frame = new CGRect () { X = View.Frame.X, Y = View.Frame.Y, Width = View.Frame.Width, Height = View.Frame.Height };
			Initialize ();
			BTProgressHUD.Dismiss ();
			if (movieDetail.PosterPath != null) {
				var uri = new Uri (movieDetail.PosterPath);
				var imgUrl = new NSUrl (baseUrl + uri.AbsoluteUri.Substring (8));
				posterImage.SetImage (imgUrl, UIImage.FromBundle ("blank.png"));

			} else
				posterImage.Image = UIImage.FromBundle ("blank.png");
			posterImage.Layer.BorderWidth = 1.0f;
			posterImage.ContentMode = UIViewContentMode.ScaleToFill;
			posterImage.Frame = new CGRect () { X = 16, Y = 15, Width = 159, Height = 225 };
			posterImage.Layer.BorderColor = UIColor.Clear.CGColor;
			posterImage.ClipsToBounds = true;
			//posterImage.SetImage (MovieCell.GetImage (movieDetail.HighResPosterPath));

			addReviewButt.SetTitle ("Add/Edit Review", UIControlState.Normal);
			addReviewButt.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			addReviewButt.SetTitleColor (UIColor.White, UIControlState.Normal);
			addReviewButt.Frame = new CGRect (saveFavoriteButt.Frame.X, saveFavoriteButt.Frame.Y - 40, saveFavoriteButt.Frame.Width, saveFavoriteButt.Frame.Height);
			addReviewButt.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);
			addReviewButt.TouchDown += AddReviewButt_TouchDown;
			addReviewButt.Frame = new CGRect () { X = 183, Y = 180, Width = 130, Height = 25 };

			saveFavoriteButt.SetTitle ("Add To List", UIControlState.Normal);

			saveFavoriteButt.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.NAV_BAR_COLOR, 1.0f);
			saveFavoriteButt.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);
			saveFavoriteButt.SetTitleColor (UIColor.White, UIControlState.Normal);
			saveFavoriteButt.Frame = new CGRect () { X = 183, Y = 215, Width = 130, Height = 25 };

			moviePlay.Frame = new CGRect ((posterImage.Frame.Size.Width / 2) - (moviePlay.Frame.Size.Width), (posterImage.Frame.Size.Height / 2) - (moviePlay.Frame.Size.Height), 40, 40);



			movieTitle.Font = UIFont.FromName (ColorExtensions.TITLE_FONT, 15);


			movieTitle.Text = movieDetail.name;
			movieTitle.Lines = 0;
			movieTitle.Frame = new CGRect (183, 30, 135, 40);


			dateOpenView.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 11);
			if (movieDetail is MovieCloud)
				dateOpenView.Text = ((MovieCloud)movieDetail).ReleaseDate;
			else 
			{
				if (movieDetail.ReleaseDate != "")
					dateOpenView.Text = "Release Date: " + DateTime.Parse (movieDetail.ReleaseDate).ToString ("MM/dd/yyyy", CultureInfo.InvariantCulture);
			}
			dateOpenView.Frame = new CGRect (183, 70, 135, 20);

			Userstar1.Frame = new CGRect (183, 115, 20, 20);
			Userstar2.Frame = new CGRect (203, 115, 20, 20);
			Userstar3.Frame = new CGRect (223, 115, 20, 20);
			Userstar4.Frame = new CGRect (243, 115, 20, 20);
			Userstar5.Frame = new CGRect (263, 115, 20, 20);
			Userstar1.Alpha = .2f;
			Userstar2.Alpha = .2f;
			Userstar3.Alpha = .2f;
			Userstar4.Alpha = .2f;
			Userstar5.Alpha = .2f;

			switch (movieDetail.UserRating) {
			case 1:
				Userstar1.Alpha = 1f;
				break;
			case 2:
				Userstar1.Alpha = 1f;
				Userstar2.Alpha = 1f;
				break;
			case 3:
				Userstar1.Alpha = 1f;
				Userstar2.Alpha = 1f;
				Userstar3.Alpha = 1f;
				break;
			case 4:
				Userstar1.Alpha = 1f;
				Userstar2.Alpha = 1f;
				Userstar3.Alpha = 1f;
				Userstar4.Alpha = 1f;
				break;
			case 5:
				Userstar1.Alpha = 1f;
				Userstar2.Alpha = 1f;
				Userstar3.Alpha = 1f;
				Userstar4.Alpha = 1f;
				Userstar5.Alpha = 1f;
				break;
			}
			var AddStar1 = new UITapGestureRecognizer ();
			AddStar1.AddTarget (() => { AddStarAction (AddStar1, 1); });
			Userstar1.AddGestureRecognizer (AddStar1);

			var AddStar2 = new UITapGestureRecognizer ();
			AddStar2.AddTarget (() => { AddStarAction (AddStar2, 2); });
			Userstar2.AddGestureRecognizer (AddStar2);

			var AddStar3 = new UITapGestureRecognizer ();
			AddStar3.AddTarget (() => { AddStarAction (AddStar3, 3); });
			Userstar3.AddGestureRecognizer (AddStar3);

			var AddStar4 = new UITapGestureRecognizer ();
			AddStar4.AddTarget (() => { AddStarAction (AddStar4, 4); });
			Userstar4.AddGestureRecognizer (AddStar4);

			var AddStar5 = new UITapGestureRecognizer ();
			AddStar5.AddTarget (() => { AddStarAction (AddStar5, 5); });
			Userstar5.AddGestureRecognizer (AddStar5);

			userResultText.Text = "Your Rating: " + Convert.ToInt32 (movieDetail.UserRating) + " of 5 Stars";
		
			userResultText.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);

			userResultText.Frame = new CGRect (183, 95, 140, 20);

			IMDB.Frame = new CGRect (183, 145, 30, 30);
			voteResultText.Text = Convert.ToInt32 (movieDetail.VoteAverage) + " of 10 Stars";

			voteResultText.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);

			voteResultText.Frame = new CGRect (215, 155, 110, 20);


			descriptView.Text = movieDetail.Overview;
			descriptView.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);

			descriptView.Frame = new CGRect (16, 255, 290, 300); //(int)size.Height+10);
			descriptView.Lines = 0;
			descriptView.TextAlignment = UITextAlignment.Justified;

			descriptView.SizeToFit ();


			descReview.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);

			descReview.Frame = new CGRect (16, descriptView.Frame.Y + descriptView.Frame.Height + 30, 300, 100); //(int)size.Height+10);
			descReview.Lines = 0;
			descReview.TextAlignment = UITextAlignment.Natural;
			descReview.Text = movieDetail.UserReview;

			descReview.SizeToFit ();

			userName = new UILabel ();

			userName.Frame = new CGRect () { X = descReview.Frame.X, Y = descReview.Frame.Y - 15 };
			if (movieDetail.UserReview != null)
				userName.Text = GetUserName ();
			userName.Font = UIFont.FromName (ColorExtensions.TITLE_FONT, 12);
			userName.SizeToFit ();




			var playClip = new UITapGestureRecognizer (HandleAction);

			saveFavoriteButt.TouchDown += SaveFavoriteButt_TouchDown;
			posterImage.UserInteractionEnabled = true;
			if (_embededMoveId != "")
				moviePlay.AddGestureRecognizer (playClip);

			updateBackGroundColors ();


		}

		void AddStarAction (UIGestureRecognizer gesture, int place)
		{
			if (place == 1 && Math.Round(Userstar2.Alpha,1) == .2 && Math.Round(Userstar3.Alpha,1) == .2 && Math.Round(Userstar4.Alpha,1) == .2 && Math.Round(Userstar5.Alpha,1) == .2 && Math.Round(Userstar1.Alpha,1) == 1) 
			{
				Userstar1.Alpha = .2f;
				movieDetail.UserRating = 0;
				userResultText.Text = "Not Rated.";
				return;
			}

			Userstar1.Alpha = .2f;
			Userstar2.Alpha = .2f;
			Userstar3.Alpha = .2f;
			Userstar4.Alpha = .2f;
			Userstar5.Alpha = .2f;


			switch (place) 
			{
				case 1:
				    
					Userstar1.Alpha = 1f;
					break;
				case 2:
					Userstar1.Alpha = 1f;
					Userstar2.Alpha = 1f;
					break;
				case 3:
					Userstar1.Alpha = 1f;
					Userstar2.Alpha = 1f;
					Userstar3.Alpha = 1f;
					break;
				case 4:
					Userstar1.Alpha = 1f;
					Userstar2.Alpha = 1f;
					Userstar3.Alpha = 1f;
					Userstar4.Alpha = 1f;
					break;
				case 5:
					Userstar1.Alpha = 1f;
					Userstar2.Alpha = 1f;
					Userstar3.Alpha = 1f;
					Userstar4.Alpha = 1f;
					Userstar5.Alpha = 1f;
					break;
			}
			movieDetail.UserRating = place;
			userResultText.Text = "Your Rating: " + Convert.ToInt32 (movieDetail.UserRating) + " of 5 Stars";
		}

		string GetUserName ()
		{
			return text!=null ?this.text:ColorExtensions.CurrentUser.username;
		}
		public static bool IsDarkColor (UIColor newColor)
		{
			 nfloat[] componentColors = newColor.CGColor.Components;

			nfloat colorBrightness = ((componentColors[0] * 299) + (componentColors[1] * 587) + 
				(componentColors[2] * 114))/ 1000;
			if (colorBrightness < 0.5)
				return true;
			else
				return false;
		}
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			updateMovie (movieDetail);
		}
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			NewsFeedTableSource.HideTabBar ((UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.TabController,View.BackgroundColor);
			nfloat lastViewPostion=0;
			if (similarMovies.Count > 0) 
			{


				//similar movies
				similarMoviesLabel = new UILabel () {
					TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black, Frame = new CGRect (16, descReview.Frame.Y + descReview.Frame.Height + userName.Frame.Height, 180, 20),
					BackgroundColor =backGroundColor,
					Font = UIFont.FromName (ColorExtensions.TITLE_FONT, ColorExtensions.HEADER_FONT_SIZE),
					Text = "Similar Movies"
				};
				similarMoviesController = new SimilarCollectionViewController (new UICollectionViewFlowLayout () {
					MinimumInteritemSpacing = MinimumInteritemSpacing, MinimumLineSpacing = MinimumLineSpacing,
					HeaderReferenceSize = HeaderReferenceSize, ItemSize = ItemSize,
					ScrollDirection = UICollectionViewScrollDirection.Horizontal
				}, similarMovies, this);
				similarMoviesController.CollectionView.BackgroundColor = backGroundColor;
				similarMoviesController.CollectionView.RegisterClassForCell (typeof (MovieCell), SimilarCollectionViewController.movieCellId);
				//similar movies
				similarMoviesController.CollectionView.Frame = new CGRect (-35, descReview.Frame.Y + descReview.Frame.Height + userName.Frame.Height + 20, 345, 150);
				lastViewPostion = similarMoviesController.CollectionView.Frame.Y + similarMoviesController.CollectionView.Frame.Height;
				castHeader = new UILabel ();
				castHeader.Frame = new CGRect () { X = 16, Y = similarMoviesController.CollectionView.Frame.Y + similarMoviesController.CollectionView.Frame.Height + 20 };
			} else {
				castHeader = new UILabel ();
				castHeader.Frame = new CGRect () { X = 16, Y = descReview.Frame.Y + descReview.Frame.Height + userName.Frame.Height };

			}
			if (similarMovies.Count > 0) {
				scrollView.Add (similarMoviesLabel);
				scrollView.Add (similarMoviesController.CollectionView);

			}

			var getCast = Task.Run (async () => 
			{
				if(movieDetail is MovieCloud)
				   castList = await MovieService.MovieCreditsAsync (((MovieCloud) movieDetail).OriginalId);
				else
				   castList = await MovieService.MovieCreditsAsync (movieDetail.OriginalId.ToString ());
			});
			getCast.Wait ();
			//DeleteAllSubviews (scrollView);
			if (castList.Count > 0) {
				castHeader.Font = UIFont.FromName (ColorExtensions.TITLE_FONT, ColorExtensions.HEADER_FONT_SIZE);
				castHeader.Text = "Cast";
				castHeader.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
				castHeader.SizeToFit ();
				scrollView.Add (castHeader);
			}

			int castCount = 0;
			foreach (var cast in castList) {
				var actorName = new UILabel ();
				actorName.Frame = new CGRect () { X = castHeader.Frame.X, Y = castHeader.Frame.Y + 20 + (30 * castCount), Width = 150 };
				actorName.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);
				actorName.Text = cast.Actor;
				actorName.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
				actorName.Lines = 2;
				actorName.SizeToFit ();

				var character = new UILabel ();
				character.Frame = new CGRect () { X = castHeader.Frame.X + 150, Y = castHeader.Frame.Y + 20 + (30 * castCount), Width = 150 };
				character.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);
				character.Text = cast.Character;
				character.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
				character.TextAlignment = UITextAlignment.Left;
				character.Lines = 2;
				character.SizeToFit ();

				lastViewPostion = character.Frame.Y + character.Frame.Height;
				scrollView.Add (actorName);
				scrollView.Add (character);
				castCount++;
			}
			//scrollView.SizeToFit ();

			//For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
			scrollView.ContentSize = new CGSize (320, lastViewPostion + 20);
			scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);
			scrollView.Bounces = true;

		}
		void updateBackGroundColors ()
		{
			backGroundColor = averageColor (posterImage.Image);
			movieTitle.BackgroundColor = backGroundColor;
			movieTitle.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
			dateOpenView.BackgroundColor = backGroundColor;
			dateOpenView.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
			Userstar1.Image = IsDarkColor (backGroundColor) ? UIImage.FromBundle ("star.png") : UIImage.FromBundle ("stardark.png");
			Userstar2.Image = IsDarkColor (backGroundColor) ? UIImage.FromBundle ("star.png") : UIImage.FromBundle ("stardark.png");
			Userstar3.Image = IsDarkColor (backGroundColor) ? UIImage.FromBundle ("star.png") : UIImage.FromBundle ("stardark.png");
			Userstar4.Image = IsDarkColor (backGroundColor) ? UIImage.FromBundle ("star.png") : UIImage.FromBundle ("stardark.png");
			Userstar5.Image = IsDarkColor (backGroundColor) ? UIImage.FromBundle ("star.png") : UIImage.FromBundle ("stardark.png");
			View.BackgroundColor = backGroundColor;
			userResultText.BackgroundColor = backGroundColor;
			userResultText.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
			voteResultText.BackgroundColor = backGroundColor;
			voteResultText.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
			descriptView.BackgroundColor = backGroundColor;
			descriptView.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
			descReview.BackgroundColor = backGroundColor;
			descReview.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
			userName.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
				try 
				{
				_embededMoveId = youtubeMovieId;
				var getSimilarMovies = Task.Run (async () => {
					if (movieDetail is MovieCloud)
						similarMovies = await MovieService.GetMoviesAsyncString (MovieService.MovieType.Similar, 1, ((MovieCloud)movieDetail).OriginalId);
					else
						similarMovies = await MovieService.GetMoviesAsync (MovieService.MovieType.Similar, 1, movieDetail.OriginalId);
				});
				getSimilarMovies.Wait ();

				} catch (Exception ex) 
				{
					Debug.WriteLine (ex.Message);
				}
				DeleteAllSubviews (scrollView);

				scrollView.AddSubview (dateOpenView);
				scrollView.AddSubview (descReview);
				scrollView.AddSubview (descriptView);
				scrollView.AddSubview (movieTitle);
				if (this.canReview)
					scrollView.AddSubview (addReviewButt);
				scrollView.AddSubview (posterImage);
				scrollView.AddSubview (saveFavoriteButt);
				scrollView.AddSubview (voteResultText);
				if (_embededMoveId != "")
					scrollView.AddSubview (moviePlay);
				if (this.canReview) {
					scrollView.Add (Userstar1);
					scrollView.Add (Userstar2);
					scrollView.Add (Userstar3);
					scrollView.Add (Userstar4);
					scrollView.Add (Userstar5);
					scrollView.Add (userResultText);
				}

				scrollView.Add (userName);

				scrollView.Add (IMDB);


				View.AddSubview (scrollView);

				//HACK until i find out why when you open a movie details and come back the view.height changes.
				if (View.Frame.Height == 504) {

					scrollView.ContentSize = new CGSize (View.Frame.Width, 723 + 50);
					scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);

				}
			
		}


		public override bool ShouldAutorotate ()
		{
			return base.ShouldAutorotate ();

		}
		void DeleteAll (int? CustomId, int id)
		{

			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					db.Query<Movie> ("DELETE FROM [Movie] WHERE [CustomListID] = " + CustomId + " AND [Id] = " + id);


				}
			} catch (SQLite.SQLiteException e) {
				//first time in no favorites yet.
				Debug.Write (e.Message);
			}


		}
		internal async void updateMovie (Movie item)
		{
			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					//DeleteAll (item.CustomListID, (int)item.OriginalId);

					db.InsertOrReplace (item, typeof (Movie));

				}
				AzureTablesService postService = AzureTablesService.DefaultService;
				await postService.UpdateMovieCloud (item);
				  

			} catch (SQLite.SQLiteException s) {
				Debug.Write (s.Message);
			}
		}
		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}

	
		void HandleAction ()
		{
			URL = _youTubeURl + _embededMoveId;
			string videoCode = _embededMoveId;//.Substring (_embededMoveId.LastIndexOf ("/"));

			//var bundle = NSBundle.MainBundle;
			//var resource = bundle.PathForResource (_youTubeURl + videoCode, "mp4");
			//var controller = new MFVideoViewController ();

			webView = new UIWebView () {
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
				BackgroundColor = UIColor.White
			};
			var viewController = new UIViewController ();

			var sb = new StringBuilder ();
			sb.Append ("<html><head>");
			sb.Append ("<style>" +
			           "body{font-family:Helvetica;font-size:10pt}" +
					  "</style>");
			sb.Append ("</head>");
			sb.Append ("<body>");
			sb.Append ("<iframe width=\"300\" height=\"250\" src=\"" + _youTubeURl + videoCode + "\" frameborder=\"0\" allowfullscreen></iframe>");
			sb.Append ("<h4>" + movieDetail.name + "</h4>");
			sb.Append ("<p>" + movieDetail.Overview + "</p>");
			sb.Append ("</body></html>");

			webView.Frame = new CGRect (0, 0, (float)this.View.Frame.Width, (float)scrollView.Frame.Height);
			webView.LoadHtmlString (sb.ToString (), null);
			viewController.View.Add (webView);
			//this.View.AddSubview (webView);
			NavigationController.PushViewController (viewController, true);

			//NavigationController.PushViewController (controller, true);

		}

		/// <summary>
		/// This is the button press delegate for clear favorites button. It was a last minute change and needs to be renamed
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void PlayVideoButt_TouchDown (object sender, EventArgs e)
		{
			posterImage.Layer.BorderWidth = 1.0f;
			posterImage.Layer.BorderColor = UIColor.Clear.CGColor;
			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					db.Query<Movie> ("DELETE FROM [Movie] WHERE [id] = " + movieDetail.id);

					saveFavoriteButt.TouchDown -= PlayVideoButt_TouchDown;
					saveFavoriteButt.TouchDown += SaveFavoriteButt_TouchDown;
					saveFavoriteButt.SetTitle ("Save Favorite", UIControlState.Normal);
					saveFavoriteButt.BackgroundColor = backGroundColor;
					saveFavoriteButt.SetTitleColor (UIColor.White, UIControlState.Normal);
				}
			} catch (SQLite.SQLiteException) {
				//first time in no favorites yet.
			}


		}
		public static UIColor averageColor (UIImage image)
		{
			if (image == null)
				return null;
			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB ();
			byte [] rgba = new byte [4];
			CGBitmapContext context = new CGBitmapContext (rgba, 1, 1, 8, 4, colorSpace, CGImageAlphaInfo.PremultipliedLast);
			context.DrawImage (new RectangleF (0, 0, 1, 1), image.CGImage);

			if (rgba [3] > 0) {
				var alpha = ((float)rgba [3]) / 255.0;
				var multiplier = alpha / 255.0;
				var color = new UIColor (
					(float)(rgba [0] * multiplier),
					(float)(rgba [1] * multiplier),
					(float)(rgba [2] * multiplier),
					(float)(alpha)
				);
				return color;
			} else {
				var color = new UIColor (
					(float)(rgba [0] / 255.0),
					(float)(rgba [1] / 255.0),
					(float)(rgba [2] / 255.0),
					(float)(rgba [3] / 255.0)
				);
				return color;
			}
		}

		void AddReviewButt_TouchDown (object sender, EventArgs e)
		{
			//Create Alert
			var textInputAlertController = UIAlertController.Create ("My Movie Review", "120 characters or less", UIAlertControllerStyle.Alert);

			//Add Text Input
			textInputAlertController.AddTextField (textField => { textField.Text = movieDetail.UserReview; });

			//Add Actions
			var cancelAction = UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, alertAction => {
				Console.WriteLine ("Cancel was Pressed");
			});
				var okayAction = UIAlertAction.Create ("Okay", UIAlertActionStyle.Default, alertAction => {
				Console.WriteLine ("The user entered '{0}'", textInputAlertController.TextFields [0].Text);
				movieDetail.UserReview = textInputAlertController.TextFields [0].Text;

				userName.Text = GetUserName();
				userName.SizeToFit ();
				descReview.Frame = new CGRect (16, descriptView.Frame.Y + descriptView.Frame.Height + 30, 300, 100); //(int)size.Height+10);
				descReview.Lines = 0;
				descReview.Layer.BorderWidth = .2f;
				descReview.TextAlignment = UITextAlignment.Natural;
				descReview.Text = movieDetail.UserReview.Substring(0,(movieDetail.UserReview.Length<120?movieDetail.UserReview.Length:120));
				descReview.SizeToFit ();

				this.ViewWillAppear (true);
				this.ViewDidAppear (true);
			});

			textInputAlertController.AddAction (cancelAction);
			textInputAlertController.AddAction (okayAction);

			//Present Alert
			PresentViewController (textInputAlertController, true, null);

		}

		public static void DeleteAllSubviews (UIScrollView view)
		{
			foreach (UIView subview in view.Subviews) {
				subview.RemoveFromSuperview ();
			}

		}
		/// <summary>
		/// This is the delegate for the save favorite button. It also has visual indicator for the current poster image
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void SaveFavoriteButt_TouchDown (object sender, EventArgs e)
		{
			var popoverController = new MovieListPickerViewController (movieDetail, true);

			ProvidesPresentationContextTransitionStyle = true;
			DefinesPresentationContext = true;
			popoverController.ModalPresentationStyle = UIModalPresentationStyle.CurrentContext;
			NavigationController.HidesBarsOnSwipe = false;
			NavigationController.PushViewController (popoverController, true);
		}


		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}



}	


