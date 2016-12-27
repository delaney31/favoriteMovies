using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using UIKit;

namespace FavoriteMovies
{
	public class MovieDetailViewController : BaseController
	{
		
		UILabel dateOpenView = new UILabel ();
		UILabel descReview= new UILabel ();
		UILabel descriptView =new UILabel ();
		UILabel movieTitle =new UILabel ();
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
		string _embededMoveId;
		static CGSize HeaderReferenceSize = new CGSize (50, 50);
		static int MinimumInteritemSpacing = 30;
		static int MinimumLineSpacing = 5;
		static UILabel similarMoviesLabel;
		static CGSize ItemSize = new CGSize (100, 150);
		ObservableCollection<Movie> similarMovies;
		static ObservableCollection<CastCrew> castList;
		SimilarCollectionViewController similarMoviesController;
		Movie movieDetail;
		UIImageView moviePlay;
		UILabel userName;
		UIWebView webView;
		UIScrollView scrollView = new UIScrollView ();
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
		public MovieDetailViewController (Movie movie, bool canReview) 
		{
			movieDetail = movie;
			this.canReview = canReview;

			moviePlay = new UIImageView () { UserInteractionEnabled = true};
			Userstar1 = new UIImageView (){ UserInteractionEnabled = true };
			Userstar2 = new UIImageView (){ UserInteractionEnabled = true };
			Userstar3 = new UIImageView (){ UserInteractionEnabled = true };
			Userstar4 = new UIImageView (){ UserInteractionEnabled = true };
			Userstar5 = new UIImageView (){ UserInteractionEnabled = true };
			IMDB = new UIImageView ();
				
			moviePlay.Image = UIImage.FromBundle ("download.png");
			Userstar1.Image = UIImage.FromBundle ("star.png");
			Userstar2.Image = UIImage.FromBundle ("star.png");
			Userstar3.Image = UIImage.FromBundle ("star.png");
			Userstar4.Image = UIImage.FromBundle ("star.png");
			Userstar5.Image = UIImage.FromBundle ("star.png");
			IMDB.Image = UIImage.FromBundle ("imdb.png");
			backGroundColor= averageColor(MovieCell.GetImage (movieDetail.PosterPath));
			View.BackgroundColor = backGroundColor;
		}

		void Initialize ()
		{
			var imDbUrl = "http://api.themoviedb.org/3/movie/" + movieDetail.id + "/videos?api_key=" + MovieService._apiKey;


			var UTubeMovidId = Task.Run (async () => {
				youtubeMovieId = await MovieService.GetYouTubeMovieId (imDbUrl);

			});
			UTubeMovidId.Wait ();




		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			scrollView.Frame = new CGRect () { X = View.Frame.X, Y = View.Frame.Y, Width = View.Frame.Width, Height = View.Frame.Height };

			Initialize ();
			View.BackgroundColor = backGroundColor;
			posterImage.Layer.BorderWidth = 1.0f;
			posterImage.ContentMode = UIViewContentMode.ScaleToFill;
			posterImage.Frame = new CGRect () { X = 16, Y = 15, Width = 159, Height = 225 };
			addReviewButt.SetTitle ("Add/Edit Review", UIControlState.Normal);
			addReviewButt.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			addReviewButt.SetTitleColor (UIColor.White, UIControlState.Normal);
			addReviewButt.Frame = new CGRect (saveFavoriteButt.Frame.X, saveFavoriteButt.Frame.Y - 40, saveFavoriteButt.Frame.Width, saveFavoriteButt.Frame.Height);
			addReviewButt.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, UIColorExtensions.CAST_FONT_SIZE);
			addReviewButt.TouchDown += AddReviewButt_TouchDown;
			addReviewButt.Frame = new CGRect () { X = 183, Y = 180, Width = 130, Height = 25 };

			saveFavoriteButt.SetTitle ("Add To List", UIControlState.Normal);
			posterImage.Layer.BorderColor = UIColor.Clear.CGColor;
			saveFavoriteButt.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			saveFavoriteButt.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, UIColorExtensions.CAST_FONT_SIZE);
			saveFavoriteButt.SetTitleColor (UIColor.White, UIControlState.Normal);
			saveFavoriteButt.Frame = new CGRect () { X = 183, Y = 215, Width = 130, Height = 25 };

			moviePlay.Frame = new CGRect ((posterImage.Frame.Size.Width/2)- (moviePlay.Frame.Size.Width), (posterImage.Frame.Size.Height/2) - (moviePlay.Frame.Size.Height), 40, 40);
			posterImage.ClipsToBounds = true;
			posterImage.Image = MovieCell.GetImage (movieDetail.HighResPosterPath);

			movieTitle.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 15);
			movieTitle.BackgroundColor = backGroundColor;
			movieTitle.TextColor =IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
			movieTitle.Text = movieDetail.name;
			movieTitle.Lines = 0;
			movieTitle.Frame = new CGRect (183, 30, 135, 40);

			dateOpenView.BackgroundColor = backGroundColor;
			dateOpenView.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, 11);
			dateOpenView.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
			dateOpenView.Text = "Release Date: " + movieDetail.ReleaseDate.Value.ToString ("MM/dd/yyyy",
				  CultureInfo.InvariantCulture);
			dateOpenView.Frame = new CGRect  (183, 70, 135, 20);

			Userstar1.Frame = new CGRect (183, 115, 20, 20);
			Userstar2.Frame = new CGRect (203, 115, 20, 20);
			Userstar3.Frame = new CGRect (223, 115, 20, 20);
			Userstar4.Frame = new CGRect (243, 115, 20, 20);
			Userstar5.Frame = new CGRect (263, 115, 20, 20);
			Userstar1.BackgroundColor = backGroundColor;
			Userstar2.BackgroundColor = backGroundColor;
			Userstar3.BackgroundColor = backGroundColor;
			Userstar4.BackgroundColor = backGroundColor;
			Userstar5.BackgroundColor = backGroundColor;
			Userstar1.Alpha = .2f;
			Userstar2.Alpha = .2f;
			Userstar3.Alpha = .2f;
			Userstar4.Alpha = .2f;
			Userstar5.Alpha = .2f;

			switch (movieDetail.UserRating) 
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
			var AddStar1 = new UITapGestureRecognizer();
			AddStar1.AddTarget (() => { AddStarAction (AddStar1); });
			Userstar1.AddGestureRecognizer (AddStar1);

			var AddStar2 = new UITapGestureRecognizer ();
			AddStar2.AddTarget (() => { AddStarAction (AddStar2); });
			Userstar2.AddGestureRecognizer (AddStar2);

			var AddStar3 = new UITapGestureRecognizer ();
			AddStar3.AddTarget (() => { AddStarAction (AddStar3); });
			Userstar3.AddGestureRecognizer(AddStar3);

			var AddStar4 = new UITapGestureRecognizer ();
			AddStar4.AddTarget (() => { AddStarAction (AddStar4); });
			Userstar4.AddGestureRecognizer (AddStar4);

			var AddStar5 = new UITapGestureRecognizer ();
			AddStar5.AddTarget (() => { AddStarAction (AddStar5); });
			Userstar5.AddGestureRecognizer(AddStar5);

			userResultText.Text = "Your Rating: " + Convert.ToInt32 (movieDetail.UserRating) + " of 5";
			userResultText.BackgroundColor = backGroundColor;
			userResultText.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, UIColorExtensions.CAST_FONT_SIZE);
			userResultText.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
			userResultText.Frame = new CGRect (183, 95, 140, 20);

			IMDB.Frame = new CGRect (183, 145, 30, 30);
			voteResultText.Text = Convert.ToInt32 (movieDetail.VoteAverage) + " of 10 Stars";
			voteResultText.BackgroundColor = backGroundColor;
			voteResultText.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, UIColorExtensions.CAST_FONT_SIZE);
			voteResultText.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
			voteResultText.Frame = new CGRect (215, 155, 110, 20);

			descriptView.BackgroundColor = backGroundColor;
			descriptView.Text = movieDetail.Overview;
			descriptView.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, UIColorExtensions.CAST_FONT_SIZE);
			descriptView.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
			descriptView.Frame = new CGRect  (16, 255, 290, 300); //(int)size.Height+10);
			descriptView.Lines = 0;
			descriptView.TextAlignment = UITextAlignment.Justified;

			descriptView.SizeToFit ();

			descReview.BackgroundColor = backGroundColor;
			descReview.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, UIColorExtensions.CAST_FONT_SIZE);
			descReview.TextColor = IsDarkColor(backGroundColor)? UIColor.White: UIColor.Black;
			descReview.Frame = new CGRect  (16, descriptView.Frame.Y + descriptView.Frame.Height + 30, 300, 100); //(int)size.Height+10);
			descReview.Lines = 0;
			descReview.TextAlignment = UITextAlignment.Natural;
			descReview.Text = movieDetail.UserReview;

			descReview.SizeToFit ();

			userName = new UILabel ();
			userName.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
			userName.Frame = new CGRect () { X = descReview.Frame.X, Y = descReview.Frame.Y - 15 };
			if (movieDetail.UserReview != null)
				userName.Text = GetUserName ();
			userName.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 12);
			userName.SizeToFit ();




			var playClip = new UITapGestureRecognizer (HandleAction);

			saveFavoriteButt.TouchDown += SaveFavoriteButt_TouchDown;
			posterImage.UserInteractionEnabled = true;
			if (_embededMoveId != "")
				moviePlay.AddGestureRecognizer (playClip);




		}

		void AddStarAction (UIGestureRecognizer gesture)
		{
			if (gesture.View.Alpha > .2f) {
				gesture.View.Alpha = .2f;
				movieDetail.UserRating--;
			} else 
			{
				gesture.View.Alpha = 1f;
				movieDetail.UserRating++;
			}
					
			userResultText.Text = "Your Rating: " + Convert.ToInt32 (movieDetail.UserRating) + " Stars";
			updateMovie (movieDetail);
		}

		string GetUserName ()
		{
			return "User Review by @tldelaney:";
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
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			if (similarMovies.Count > 0) 
			{


				//similar movies
				similarMoviesLabel = new UILabel () {
					TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black, Frame = new CGRect (16, descReview.Frame.Y + descReview.Frame.Height + userName.Frame.Height, 180, 20),
					BackgroundColor =backGroundColor,
					Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
					Text = "Similar Movies"
				};
				similarMoviesController = new SimilarCollectionViewController (new UICollectionViewFlowLayout () {
					MinimumInteritemSpacing = MinimumInteritemSpacing, MinimumLineSpacing = MinimumLineSpacing,
					HeaderReferenceSize = HeaderReferenceSize, ItemSize = ItemSize,
					ScrollDirection = UICollectionViewScrollDirection.Horizontal
				}, similarMovies, NavController);
				similarMoviesController.CollectionView.BackgroundColor = backGroundColor;
				similarMoviesController.CollectionView.RegisterClassForCell (typeof (MovieCell), SimilarCollectionViewController.movieCellId);
				//similar movies
				similarMoviesLabel.Frame = new CGRect (16, descReview.Frame.Y + descReview.Frame.Height + userName.Frame.Height, 180, 20);
				similarMoviesController.CollectionView.Frame = new CGRect (-35, descReview.Frame.Y + descReview.Frame.Height + userName.Frame.Height + 20, 345, 150);

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

			var getCast = Task.Run (async () => {
				castList = await MovieService.MovieCreditsAsync (movieDetail.id.ToString ());
			});
			getCast.Wait ();
			//DeleteAllSubviews (scrollView);
			if (castList.Count > 0) {
				castHeader.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE);
				castHeader.Text = "Cast";
				castHeader.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
				castHeader.SizeToFit ();
				scrollView.Add (castHeader);
			}

			int castCount = 0;
			foreach (var cast in castList) {
				var actorName = new UILabel ();
				actorName.Frame = new CGRect () { X = castHeader.Frame.X, Y = castHeader.Frame.Y + 20 + (30 * castCount), Width = 150 };
				actorName.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, UIColorExtensions.CAST_FONT_SIZE);
				actorName.Text = cast.Actor;
				actorName.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
				actorName.Lines = 2;
				actorName.SizeToFit ();

				var character = new UILabel ();
				character.Frame = new CGRect () { X = castHeader.Frame.X + 150, Y = castHeader.Frame.Y + 20 + (30 * castCount), Width = 150 };
				character.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, UIColorExtensions.CAST_FONT_SIZE);
				character.Text = cast.Character;
				character.TextColor = IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;
				character.TextAlignment = UITextAlignment.Left;
				character.Lines = 2;
				character.SizeToFit ();

				scrollView.Add (actorName);
				scrollView.Add (character);
				castCount++;
			}
			//For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
			scrollView.ContentSize = new CGSize (320, CalculateScrollContent ());
			//scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);
			scrollView.Bounces = true;

		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			_embededMoveId = youtubeMovieId;
			var getSimilarMovies = Task.Run (async () => {
				similarMovies = await MovieService.GetMoviesAsync (MovieService.MovieType.Similar, 1, (int)movieDetail.id);
			});
			getSimilarMovies.Wait ();


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

		nfloat CalculateScrollContent ()
		{
			nfloat scrollViewHeight = 0;

			foreach (var view in scrollView.Subviews) {
				//Debug.Write (view.Frame.X);
				//if (view.Frame.X == 16 ||view.Frame.X ==166)
					scrollViewHeight += view.Frame.Size.Height;
			}
			return scrollViewHeight ;
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
		internal void updateMovie (Movie item)
		{
			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					DeleteAll (item.CustomListID, (int)item.id);
					db.InsertOrReplace (item, typeof (Movie));

				}

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
			//NavigationController.NavigationBar.Frame = new CGRect () { X = 0, Y = 20, Width = 320, Height = 10 };
			//NavigationController.NavigationBarHidden = true;

			string videoCode = _embededMoveId;//.Substring (_embededMoveId.LastIndexOf ("/"));

			webView = new UIWebView () {
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
				BackgroundColor = UIColor.Black,
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

			webView.Frame = new CGRect (0, 0, (float)this.View.Frame.Width, (float)this.scrollView.Frame.Height);
			webView.LoadHtmlString (sb.ToString (), null);
			viewController.View.Add (webView);
			//this.View.AddSubview (webView);
			NavController.PushViewController (viewController, true);

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

			//UITextView textView= new UITextView ();
			//textView.Frame = new CGRect () { Width = textInputAlertController.View.Frame.Width, Height = textInputAlertController.View.Frame.Height / 2 };
			//textView.Text = movieDetail.UserReview;
			//textInputAlertController.Add (textView);

			//textInputAlertController.
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

				updateMovie(movieDetail);

				this.ViewWillAppear (true);
				this.ViewDidAppear (true);
			});

			textInputAlertController.AddAction (cancelAction);
			textInputAlertController.AddAction (okayAction);

			//Present Alert
			PresentViewController (textInputAlertController, true, null);

		}

		void DeleteAllSubviews (UIScrollView view)
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


