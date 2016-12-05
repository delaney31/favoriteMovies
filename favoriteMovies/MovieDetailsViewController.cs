using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	public partial class MovieDetailsViewController : UIViewController
	{
		



		/// <summary>
		/// This is the view controller for the movie details page. In addition it allows you to save and clear favorite movies
		/// </summary>


		static string _googleApiKey = "AIzaSyCu634TJuZR_0iUhJQ6D8E9xr2a3VbU3_M";
		static string _youTubeURl = "https://www.youtube.com/embed/";
		string _embededMoveId;

		Movie movieDetail;
		UIImageView moviePlay;
		UIWebView webView ;
		static UIScrollView scrollView = new UIScrollView ();

		public MovieDetailsViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		[Export ("initWithCoder:")]
		public MovieDetailsViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		public MovieDetailsViewController (Movie movie) : base ("MovieDetailsViewController", null)
		{
			movieDetail = movie;
			Initialize ();
			moviePlay = new UIImageView ();
			moviePlay.Image = UIImage.FromBundle ("download.png");


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
		
			AddReviewButt.SetTitle ("Add Review", UIControlState.Normal);
			AddReviewButt.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			AddReviewButt.SetTitleColor (UIColor.White, UIControlState.Normal);
			AddReviewButt.Frame = new CGRect (saveFavoriteButt.Frame.X, saveFavoriteButt.Frame.Y - 40,saveFavoriteButt.Frame.Width, saveFavoriteButt.Frame.Height);
			AddReviewButt.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 13f);
			AddReviewButt.TouchDown += AddReviewButt_TouchDown;
			saveFavoriteButt.SetTitle ("Add To List", UIControlState.Normal);
			posterImage.Layer.BorderColor = UIColor.Clear.CGColor;
			saveFavoriteButt.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			saveFavoriteButt.SetTitleColor (UIColor.White, UIControlState.Normal);
			//}

			moviePlay.Frame = new CGRect (posterImage.Frame.X+125, posterImage.Frame.Y+ 185, 20, 20);
			//moviePlay.Frame = posterImage.Frame;
			moviePlay.Alpha = 1f;
			//moviePlay.Image.Size = new CoreGraphics.CGSize () { Width = 10, Height = 10 };
			posterImage.ClipsToBounds = true;
			posterImage.Image = MovieCell.GetImage (movieDetail.HighResPosterPath);
			if (_embededMoveId != "")
			   posterImage.AddSubview (moviePlay);
			movieTitle.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 15);
			movieTitle.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			movieTitle.TextColor = UIColor.Black;
			movieTitle.Text = movieDetail.Title;
			movieTitle.Lines = 0;
			movieTitle.Frame = new CGRect  (180, 30, 140, 40);


			dateOpenView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			dateOpenView.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 10);
			dateOpenView.TextColor = UIColor.Black;
			dateOpenView.Text = "Release Date: " + movieDetail.ReleaseDate.Value.ToString ("MM/dd/yyyy",
				  CultureInfo.InvariantCulture);
			dateOpenView.Frame = new CGRect  (180, 70, 300, 20);

			voteResultText.Text = Convert.ToInt32 (movieDetail.VoteAverage) + " of 10 Stars";
			voteResultText.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			voteResultText.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 13);
			voteResultText.TextColor = UIColor.Black;


			descriptView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			descriptView.Text = movieDetail.Overview;
			descriptView.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 12);
			descriptView.TextColor = UIColor.Black;
			descriptView.Frame = new CGRect  (10, 250, 300, 300); //(int)size.Height+10);
			descriptView.Lines = 0;
			descriptView.TextAlignment = UITextAlignment.Natural;
			descriptView.SizeToFit ();

			descReview.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			//descReview.Text = movieDetail.Overview;
			//descriptView.ScrollEnabled = true;

			descReview.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 12);
			descReview.TextColor = UIColor.Black;
			descReview.Frame = new CGRect  (10, 600, 300, 300); //(int)size.Height+10);
			descReview.Lines = 0;
			descReview.TextAlignment = UITextAlignment.Natural;
			descReview.SizeToFit ();
			descReview.Text = movieDetail.UserReview;


			var playClip  = new UITapGestureRecognizer(HandleAction);

			saveFavoriteButt.TouchDown += SaveFavoriteButt_TouchDown;


			posterImage.UserInteractionEnabled = true;
			if (_embededMoveId != "")
			   posterImage.AddGestureRecognizer (playClip);
			//For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
			scrollView.ContentSize = new CGSize (320, View.Frame.Height + 155);
			scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);
			scrollView.Bounces = true;




		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			descReview.Text = movieDetail.UserReview;


		}
		public override bool ShouldAutorotate ()
		{
			return base.ShouldAutorotate ();

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
					  "body{font-family:Helvetica;font-size:10pt;}" +
					  "</style>");
			sb.Append ("</head>");
			sb.Append ("<body>");
			sb.Append ("<iframe width=\"300\" height=\"250\" src=\"" +_youTubeURl + videoCode + "\" frameborder=\"0\" allowfullscreen></iframe>");
			sb.Append ("<h4>" + movieDetail.Title + "</h4>");
			sb.Append ("<p>" + movieDetail.Overview + "</p>");
			sb.Append ("</body></html>");

			webView.Frame = new CGRect  (0, 0, (float)this.View.Frame.Width, (float)this.View.Frame.Height);
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
			textInputAlertController.AddTextField (textField => { });

			//Add Actions
			var cancelAction = UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, alertAction => {
				Console.WriteLine ("Cancel was Pressed");
			});
			var okayAction = UIAlertAction.Create ("Okay", UIAlertActionStyle.Default, alertAction => {
			Console.WriteLine ("The user entered '{0}'", textInputAlertController.TextFields [0].Text);
			movieDetail.UserReview = textInputAlertController.TextFields [0].Text;
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



	//#region Simlar Movies setup not used yet 
	///// <summary>
	///// This class ( and subsequent classes) is not currently being used. It is going to be the datassource for the similar movies collection
	///// </summary>
	//public class SimilarMoviesDataSource : UICollectionViewSource
	//{
	//	public SimilarMoviesDataSource ()
	//	{
	//		Rows = new List<SimilarMovie> ();
	//	}

	//	public List<SimilarMovie> Rows { get; private set; }
	//	public float FontSize { get; set; }
	//	public SizeF ImageViewSize { get; set; }

	//	public override nint NumberOfSections (UICollectionView collectionView)
	//	{
	//		return 1;
	//	}

	//	public override nint GetItemsCount (UICollectionView collectionView, nint section)
	//	{
	//		return Rows.Count;
	//	}

	//	public override bool ShouldHighlightItem (UICollectionView collectionView, NSIndexPath indexPath)
	//	{
	//		return true;
	//	}

	//	public override void ItemHighlighted (UICollectionView collectionView, NSIndexPath indexPath)
	//	{
	//		var cell = (UserCell)collectionView.CellForItem (indexPath);
	//		cell.ImageView.Alpha = 0.5f;
	//	}

	//	public override void ItemUnhighlighted (UICollectionView collectionView, NSIndexPath indexPath)
	//	{
	//		var cell = (UserCell)collectionView.CellForItem (indexPath);
	//		cell.ImageView.Alpha = 1;

	//		SimilarMovie row = Rows [indexPath.Row];
	//		row.Tapped.Invoke ();
	//	}

	//	public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
	//	{
	//		var cell = (UserCell)collectionView.DequeueReusableCell (UserCell.CellID, indexPath);

	//		SimilarMovie row = Rows [indexPath.Row];

	//		cell.UpdateRow (row, FontSize, ImageViewSize);

	//		return cell;
	//	}
	//}

	//public class SimilarMovie
	//{
	//	public SimilarMovie (UIImage image, Action tapped)
	//	{

	//		Image = image;
	//		Tapped = tapped;
	//	}


	//	public UIImage Image { get; set; }

	//	public Action Tapped { get; set; }
	//}

	//public class UserCell : UICollectionViewCell
	//{
	//	public static NSString CellID = new NSString ("SimilarMoviesDataSource");

	//	[Export ("initWithFrame:")]
	//	public UserCell (CGRect  frame)
	//		: base (frame)
	//	{
	//		ImageView = new UIImageView ();
	//		ImageView.Layer.BorderColor = UIColor.DarkGray.CGColor;
	//		ImageView.Layer.BorderWidth = 1f;
	//		ImageView.Layer.CornerRadius = 3f;
	//		ImageView.Layer.MasksToBounds = true;
	//		ImageView.ContentMode = UIViewContentMode.ScaleToFill;

	//		ContentView.AddSubview (ImageView);


	//	}

	//	public UIImageView ImageView { get; private set; }

	//	public UILabel LabelView { get; private set; }

	//	public void UpdateRow (SimilarMovie element, Single fontSize, SizeF imageViewSize)
	//	{

	//		ImageView.Image = element.Image;
	//		LabelView.Font = UIFont.FromName ("HelveticaNeue-Bold", fontSize);
	//		ImageView.Frame = new CGRect  (0, 0, imageViewSize.Width, imageViewSize.Height);

	//	}
	//}
	//#endregion
}

