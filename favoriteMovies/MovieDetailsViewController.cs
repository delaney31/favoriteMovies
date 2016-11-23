using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		CustomList listType;
		static UIScrollView scrollView = new UIScrollView ();
		ObservableCollection<Movie> similarMovies;

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
			//listType = new CustomList ();
			//listType.Id = movieDetail.Id;

		}

		void Initialize ()
		{
			var imDbUrl = "http://api.themoviedb.org/3/movie/" + movieDetail.Id + "/videos?api_key=" + MovieService._apiKey;
			var youtubeMovieId = "";

			//var task = Task.Run (async () => {
			//	similarMovies = MovieService.GetMoviesAsync (MovieService.MovieType.Similar).Result;

			//});
			//task.Wait ();

			var UTubeMovidId = Task.Run (async () => {
				youtubeMovieId = await MovieService.GetYouTubeMovieId(imDbUrl);

			});
			UTubeMovidId.Wait ();

			_embededMoveId = youtubeMovieId;

		}
		/// <summary>
		/// /Setup the UI Elements based on the passed in movieDetails
		/// </summary>
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var nav = NavigationController;



			//nav.NavigationBar.TopItem.Title = UIColorExtensions.TITLE;

			//UIImageView backgroundImageView = new UIImageView () { Frame = View.Frame };
			//backgroundImageView.Image = MovieCell.GetImage (movieDetail.BackdropPath);
			//backgroundImageView.Center = View.Center;
			//backgroundImageView.ContentMode = UIViewContentMode.ScaleAspectFill;

			View.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			//View.InsertSubview (backgroundImageView, 0);

			posterImage.Layer.BorderWidth = 1.0f;

			posterImage.ContentMode = UIViewContentMode.ScaleToFill;
			//if (UIColorExtensions.MovieIsFavorite (movieDetail.Id.ToString ())) {
			//	saveFavoriteButt.SetTitle ("Delete Favorite", UIControlState.Normal);
			//	posterImage.Layer.BorderColor = UIColor.Orange.CGColor;
			//	saveFavoriteButt.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			//	saveFavoriteButt.SetTitleColor (UIColor.White, UIControlState.Normal);
			//} else {

			playVideoButt.SetTitle ("Add Review", UIControlState.Normal);
			playVideoButt.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			playVideoButt.SetTitleColor (UIColor.White, UIControlState.Normal);
			playVideoButt.Frame = new CGRect (saveFavoriteButt.Frame.X, saveFavoriteButt.Frame.Y - 40,saveFavoriteButt.Frame.Width, saveFavoriteButt.Frame.Height);
			playVideoButt.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 13f);
			saveFavoriteButt.SetTitle ("Add To List", UIControlState.Normal);
			posterImage.Layer.BorderColor = UIColor.Clear.CGColor;
			saveFavoriteButt.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			saveFavoriteButt.SetTitleColor (UIColor.White, UIControlState.Normal);
			//}

			moviePlay.Frame = new CGRect (posterImage.Frame.X+115, posterImage.Frame.Y+ 185, 30, 30);
			//moviePlay.Frame = posterImage.Frame;
			moviePlay.Alpha = .9f;
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

			dateOpenView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			dateOpenView.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 10);
			dateOpenView.TextColor = UIColor.Black;
			dateOpenView.Text = "Release Date: " + movieDetail.ReleaseDate.Value.ToString ("MM/dd/yyyy",
				  CultureInfo.InvariantCulture);
			dateOpenView.Frame = new RectangleF (180, 70, 300, 10);

			voteResultText.Text = Convert.ToInt32 (movieDetail.VoteAverage) + " of 10 Stars";
			voteResultText.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			voteResultText.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 13);
			voteResultText.TextColor = UIColor.Black;


			descriptView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			descriptView.Text = movieDetail.Overview;
			//descriptView.ScrollEnabled = true;

			descriptView.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 12);
			descriptView.TextColor = UIColor.Black;
			//descriptView.Layer.BorderWidth = 10f;
			//descriptView.Frame = new RectangleF (10, 250, 300, ((movieDetail.Overview.Length / 50) * 25));
			//descriptView.Frame = new RectangleF (10, 260, 300, 220);
			//var size = descriptView.Text.StringSize(descriptView.Font, descriptView.Frame.Size, UILineBreakMode.CharacterWrap);
			//descriptView.LineBreakMode = UILineBreakMode.TailTruncation;
			descriptView.Frame = new RectangleF (10, 250, 300, 300); //(int)size.Height+10);
			descriptView.Lines = 0;
			descriptView.TextAlignment = UITextAlignment.Natural;
			descriptView.SizeToFit ();

			var playClip  = new UITapGestureRecognizer(HandleAction);

			saveFavoriteButt.TouchDown += SaveFavoriteButt_TouchDown;

			//playVideoButt.SetTitle ("Delete Favorite", UIControlState.Normal);
			//playVideoButt.TouchDown += PlayVideoButt_TouchDown;
			//playVideoButt.BackgroundColor = UIColor.Green;
			//playVideoButt.Hidden = true;
			posterImage.UserInteractionEnabled = true;
			if (_embededMoveId != "")
			   posterImage.AddGestureRecognizer (playClip);
			//For scrolling to work the scrollview Content size has to be bigger than the View.Frame.Height
			scrollView.ContentSize = new CGSize (View.Frame.Width, 898);
			scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);
			scrollView.Bounces = true;
			//View.AddSubview (scrollView);	

			scrollView.AddSubview (saveFavoriteButt);
			scrollView.AddSubview (posterImage);

			scrollView.AddSubview (movieTitle);
			scrollView.AddSubview (dateOpenView);
			scrollView.AddSubview (voteResultText);
			scrollView.AddSubview (descriptView);
			scrollView.AddSubview (playVideoButt);
			//scrollView.AddSubview (createListButt);
			View.AddSubview (scrollView);

		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			////scrollView.Frame = View.Frame;
			//scrollView.Frame = new CGRect () {
			//	X = descriptView.Frame.X, Y = descriptView.Frame.Y,
			//	Width = descriptView.Frame.Width, Height = descriptView.Frame.Height + 50};
			//scrollView.Add (descriptView);
			//scrollView.ContentSize = new CGSize (descriptView.Frame.Width, descriptView.Frame.Height+ 50);
			//scrollView.ContentOffset = new CGPoint (0, -scrollView.ContentInset.Top);



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

			webView.Frame = new RectangleF (0, 0, (float)this.View.Frame.Width, (float)this.View.Frame.Height);
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
		/// <summary>
		/// This is the delegate for the save favorite button. It also has visual indicator for the current poster image
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void SaveFavoriteButt_TouchDown (object sender, EventArgs e)
		{
			//posterImage.Layer.BorderWidth = 1.0f;
			//posterImage.Layer.BorderColor = UIColor.Orange.CGColor;
			//movieDetail.Favorite = true;
			//listType.Name = "Favorites";
			//// Create the database and a table to hold Favorite information.
			//try {
			//	using (var db = new SQLite.SQLiteConnection (MovieService.Database)) 
			//	{
			//		db.Insert(movieDetail, typeof(Movie));
			//		db.Insert (listType, typeof (CustomList));
			//	}

			//} catch (SQLite.SQLiteException s) {

			//	using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) 
			//	{
			//		conn.CreateTable<Movie> ();
			//		conn.CreateTable<CustomList> ();
			//	}

			//}
			//saveFavoriteButt.TouchDown -= SaveFavoriteButt_TouchDown;
			//saveFavoriteButt.TouchDown += PlayVideoButt_TouchDown;
			//saveFavoriteButt.SetTitle ("Delete Favorite", UIControlState.Normal);
			//saveFavoriteButt.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			//saveFavoriteButt.SetTitleColor (UIColor.White, UIControlState.Normal);
			//var movieList = new MovieListPickerViewController ()
			//{
			//	ModalPresentationStyle = UIModalPresentationStyle.FormSheet,
			//	ModalTransitionStyle = UIModalTransitionStyle.CoverVertical
			//};
			//NavigationController.PushViewController(movieList, false);
			//this.PresentViewController (movieList, true, null);

			var popoverController = new MovieListPickerViewController (movieDetail);

			ProvidesPresentationContextTransitionStyle = true;
			DefinesPresentationContext = true;
			popoverController.ModalPresentationStyle = UIModalPresentationStyle.CurrentContext;
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
		public UserCell (RectangleF frame)
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
			ImageView.Frame = new RectangleF (0, 0, imageViewSize.Width, imageViewSize.Height);

		}
	}
	#endregion
}

