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
	public class MovieDetailViewController : UIViewController
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
		Movie movieDetail;
		UIImageView moviePlay;
		UILabel userName;
		UIWebView webView;
		UIScrollView scrollView = new UIScrollView ();
		UIImageView Userstar1;
		UIImageView Userstar2;
		UIImageView Userstar3;
		UIImageView Userstar4;
		UIImageView Userstar5;
		bool canReview;

		//ObservableCollection<Movie> similarMovies;

		public MovieDetailViewController (Movie movie, bool canReview) 
		{
			movieDetail = movie;
			this.canReview = canReview;
			Initialize ();
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
		}

		void Initialize ()
		{
			var imDbUrl = "http://api.themoviedb.org/3/movie/" + movieDetail.Id + "/videos?api_key=" + MovieService._apiKey;
			var youtubeMovieId = "";

			var UTubeMovidId = Task.Run (async () => {
				youtubeMovieId = await MovieService.GetYouTubeMovieId (imDbUrl);

			});
			UTubeMovidId.Wait ();

			_embededMoveId = youtubeMovieId;

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			scrollView.Frame = new CGRect () { X = View.Frame.X, Y = View.Frame.Y, Width = View.Frame.Width, Height = View.Frame.Height };

			View.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			posterImage.Layer.BorderWidth = 1.0f;
			posterImage.ContentMode = UIViewContentMode.ScaleToFill;
			posterImage.Frame = new CGRect () { X = 16, Y = 15, Width = 159, Height = 225 };
			addReviewButt.SetTitle ("Add/Edit Review", UIControlState.Normal);
			addReviewButt.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			addReviewButt.SetTitleColor (UIColor.White, UIControlState.Normal);
			addReviewButt.Frame = new CGRect (saveFavoriteButt.Frame.X, saveFavoriteButt.Frame.Y - 40, saveFavoriteButt.Frame.Width, saveFavoriteButt.Frame.Height);
			addReviewButt.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 13f);
			addReviewButt.TouchDown += AddReviewButt_TouchDown;
			addReviewButt.Frame = new CGRect () { X = 183, Y = 180, Width = 130, Height = 25 };

			saveFavoriteButt.SetTitle ("Add To List", UIControlState.Normal);
			posterImage.Layer.BorderColor = UIColor.Clear.CGColor;
			saveFavoriteButt.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			saveFavoriteButt.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 13f);
			saveFavoriteButt.SetTitleColor (UIColor.White, UIControlState.Normal);
			saveFavoriteButt.Frame = new CGRect () { X = 183, Y = 215, Width = 130, Height = 25 };

			moviePlay.Frame = new CGRect ((posterImage.Frame.Size.Width/2)- (moviePlay.Frame.Size.Width), (posterImage.Frame.Size.Height/2) - (moviePlay.Frame.Size.Height), 40, 40);
			posterImage.ClipsToBounds = true;
			posterImage.Image = MovieCell.GetImage (movieDetail.HighResPosterPath);

			movieTitle.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 15);
			movieTitle.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			movieTitle.TextColor = UIColor.Black;
			movieTitle.Text = movieDetail.Title;
			movieTitle.Lines = 0;
			movieTitle.Frame = new CGRect (183, 30, 135, 40);

			dateOpenView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			dateOpenView.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 10);
			dateOpenView.TextColor = UIColor.Black;
			dateOpenView.Text = "Release Date: " + movieDetail.ReleaseDate.Value.ToString ("MM/dd/yyyy",
				  CultureInfo.InvariantCulture);
			dateOpenView.Frame = new CGRect  (183, 70, 140, 20);

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
			Userstar3.AddGestureRecognizer (AddStar3);

			var AddStar4 = new UITapGestureRecognizer ();
			AddStar4.AddTarget (() => { AddStarAction (AddStar4); });
			Userstar4.AddGestureRecognizer (AddStar4);

			var AddStar5 = new UITapGestureRecognizer ();
			AddStar5.AddTarget (() => { AddStarAction (AddStar5); });
			Userstar5.AddGestureRecognizer (AddStar5);

			userResultText.Text = "Your Rating: " + Convert.ToInt32 (movieDetail.UserRating) + " of 5";
			userResultText.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			userResultText.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 13);
			userResultText.TextColor = UIColor.Black;
			userResultText.Frame = new CGRect (183, 95, 140, 20);

			IMDB.Frame = new CGRect (183, 145, 30, 30);
			voteResultText.Text = Convert.ToInt32 (movieDetail.VoteAverage) + " of 10 Stars";
			voteResultText.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			voteResultText.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 13);
			voteResultText.TextColor = UIColor.Black;
			voteResultText.Frame = new CGRect (215, 155, 110, 20);

			descriptView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			descriptView.Text = movieDetail.Overview;
			descriptView.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 12);
			descriptView.TextColor = UIColor.Black;
			descriptView.Frame = new CGRect  (16, 250, 300, 300); //(int)size.Height+10);
			descriptView.Lines = 0;
			descriptView.TextAlignment = UITextAlignment.Natural;
			descriptView.SizeToFit ();

			descReview.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			descReview.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 12);
			descReview.TextColor = UIColor.Black;
			descReview.Frame = new CGRect  (16, descriptView.Frame.Y + descriptView.Frame.Height + 20, 300, 100); //(int)size.Height+10);
			descReview.Lines = 0;
			descReview.TextAlignment = UITextAlignment.Natural;
			descReview.Text = movieDetail.UserReview;
			descReview.SizeToFit ();

			userName = new UILabel ();
			userName.TextColor = UIColor.Blue;
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
			return "Review:@tldelaney";
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

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
			if (this.canReview) 
			{
				scrollView.Add (Userstar1);
				scrollView.Add (Userstar2);
				scrollView.Add (Userstar3);
				scrollView.Add (Userstar4);
				scrollView.Add (Userstar5);
				scrollView.Add (userResultText);
			}
			scrollView.Add (userName);
			scrollView.Add (IMDB);
			//For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
			scrollView.ContentSize = new CGSize (320, View.Frame.Height + 10);
			scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);
			scrollView.Bounces = true;
			//HACK until i find out why when you open a movie details and come back the view.height changes.
			if (View.Frame.Height == 504) {
				
				scrollView.ContentSize = new CGSize (View.Frame.Width, 723 + 50);
				scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);

			}
			View.AddSubview (scrollView);

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
					DeleteAll (item.CustomListID, item.Id);
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
			string videoCode = _embededMoveId;//.Substring (_embededMoveId.LastIndexOf ("/"));

			webView = new UIWebView () {
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
				BackgroundColor = UIColor.Black,
			};

			var sb = new StringBuilder ();
			sb.Append ("<html><head>");
			sb.Append ("<style>" +
			           "body{font-family:Helvetica;font-size:10pt;margin-top:5.67em;}" +
					  "</style>");
			sb.Append ("</head>");
			sb.Append ("<body>");
			sb.Append ("<iframe width=\"300\" height=\"250\" src=\"" + _youTubeURl + videoCode + "\" frameborder=\"40\" allowfullscreen></iframe>");
			sb.Append ("<h4>" + movieDetail.Title + "</h4>");
			sb.Append ("<p>" + movieDetail.Overview + "</p>");
			sb.Append ("</body></html>");

			webView.Frame = new CGRect (0, 0, (float)this.View.Frame.Width, (float)this.scrollView.Frame.Height);
			webView.LoadHtmlString (sb.ToString (), null);
			this.View.AddSubview (webView);

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
					db.Query<Movie> ("DELETE FROM [Movie] WHERE [id] = " + movieDetail.Id);

					saveFavoriteButt.TouchDown -= PlayVideoButt_TouchDown;
					saveFavoriteButt.TouchDown += SaveFavoriteButt_TouchDown;
					saveFavoriteButt.SetTitle ("Save Favorite", UIControlState.Normal);
					saveFavoriteButt.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
					saveFavoriteButt.SetTitleColor (UIColor.White, UIControlState.Normal);
				}
			} catch (SQLite.SQLiteException) {
				//first time in no favorites yet.
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
				userName.Text = "Review:@tldelaney";
				userName.SizeToFit ();
				descReview.Frame = new CGRect (16, descriptView.Frame.Y + descriptView.Frame.Height + 20, 300, 100); //(int)size.Height+10);
				descReview.Lines = 0;
				descReview.TextAlignment = UITextAlignment.Natural;
				descReview.Text = movieDetail.UserReview.Substring(0,(movieDetail.UserReview.Length<125?movieDetail.UserReview.Length:125));
				descReview.SizeToFit ();
				updateMovie(movieDetail);
			});

			textInputAlertController.AddAction (cancelAction);
			textInputAlertController.AddAction (okayAction);

			//Present Alert
			PresentViewController (textInputAlertController, true, null);

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



	#region Simlar Movies setup not used yet 
	/// <summary>
	/// This class ( and subsequent classes) is not currently being used. It is going to be the datassource for the similar movies collection
	/// </summary>
	public class SimilarMoviesDataSource : UICollectionViewSource
	{
		public SimilarMoviesDataSource ()
		{
			Rows = new List<SimilarMovie> ();
		}

		public List<SimilarMovie> Rows { get; private set; }
		public float FontSize { get; set; }
		public SizeF ImageViewSize { get; set; }

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return Rows.Count;
		}

		public override bool ShouldHighlightItem (UICollectionView collectionView, NSIndexPath indexPath)
		{
			return true;
		}

		public override void ItemHighlighted (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (UserCell)collectionView.CellForItem (indexPath);
			cell.ImageView.Alpha = 0.5f;
		}

		public override void ItemUnhighlighted (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (UserCell)collectionView.CellForItem (indexPath);
			cell.ImageView.Alpha = 1;

			SimilarMovie row = Rows [indexPath.Row];
			row.Tapped.Invoke ();
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (UserCell)collectionView.DequeueReusableCell (UserCell.CellID, indexPath);

			SimilarMovie row = Rows [indexPath.Row];

			cell.UpdateRow (row, FontSize, ImageViewSize);

			return cell;
		}
	}

	public class SimilarMovie
	{
		public SimilarMovie (UIImage image, Action tapped)
		{

			Image = image;
			Tapped = tapped;
		}


		public UIImage Image { get; set; }

		public Action Tapped { get; set; }
	}

	public class UserCell : UICollectionViewCell
	{
		public static NSString CellID = new NSString ("SimilarMoviesDataSource");

		[Export ("initWithFrame:")]
		public UserCell (CGRect  frame)
			: base (frame)
		{
			ImageView = new UIImageView ();
			ImageView.Layer.BorderColor = UIColor.DarkGray.CGColor;
			ImageView.Layer.BorderWidth = 1f;
			ImageView.Layer.CornerRadius = 3f;
			ImageView.Layer.MasksToBounds = true;
			ImageView.ContentMode = UIViewContentMode.ScaleToFill;

			ContentView.AddSubview (ImageView);


		}

		public UIImageView ImageView { get; private set; }

		public UILabel LabelView { get; private set; }

		public void UpdateRow (SimilarMovie element, Single fontSize, SizeF imageViewSize)
		{

			ImageView.Image = element.Image;
			LabelView.Font = UIFont.FromName ("HelveticaNeue-Bold", fontSize);
			ImageView.Frame = new CGRect  (0, 0, imageViewSize.Width, imageViewSize.Height);

		}
	}
	#endregion
}

