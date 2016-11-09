using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
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
		static string _movieImdbApiKey= "ab41356b33d100ec61e6c098ecc92140";
		string _embededMoveId;
		Movie movieDetail;
		UIImageView moviePlay;
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

		}

		void Initialize ()
		{
			var imDbUrl = "http://api.themoviedb.org/3/movie/" + movieDetail.Id + "/videos?api_key=" + _movieImdbApiKey;
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


			nav.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			nav.NavigationBar.TintColor = UIColor.White;
			nav.NavigationBar.Translucent = false;
			nav.NavigationBar.TopItem.Title = UIColorExtensions.TITLE;

			this.View.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			posterImage.Layer.BorderWidth = 1.0f;

			posterImage.ContentMode = UIViewContentMode.ScaleToFill;
			if (UIColorExtensions.MovieIsFavorite (movieDetail.Id.ToString ())) {

				posterImage.Layer.BorderColor = UIColor.Orange.CGColor;
			} else {

				posterImage.Layer.BorderColor = UIColor.White.CGColor;
			}

			moviePlay.Frame = new CGRect (posterImage.Frame.X+115, posterImage.Frame.Y+ 185, 30, 30);
			//moviePlay.Frame = posterImage.Frame;
			moviePlay.Alpha = .9f;
		//moviePlay.Image.Size = new CoreGraphics.CGSize () { Width = 10, Height = 10 };
			posterImage.ClipsToBounds = true;
			posterImage.Image = MovieCell.GetImage (movieDetail.HighResPosterPath);

			posterImage.AddSubview (moviePlay);
			movieTitle.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 15);
			movieTitle.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			movieTitle.TextColor = UIColor.White;
			movieTitle.Text = movieDetail.Title;
			movieTitle.Lines = 0;

			dateOpenView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			dateOpenView.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 10);
			dateOpenView.TextColor = UIColor.White;
			dateOpenView.Text = "Release Date: " + movieDetail.ReleaseDate.ToString ("MM/dd/yyyy",
				  CultureInfo.InvariantCulture);
			dateOpenView.Frame = new RectangleF (180, 70, 300, 10);
			voteResultText.Text = Convert.ToInt32 (movieDetail.VoteAverage) + " of 10 Stars";
			voteResultText.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			voteResultText.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 13);
			voteResultText.TextColor = UIColor.White;


			descriptView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			descriptView.Text = movieDetail.Overview;
			//descriptView.ScrollEnabled = true;
			descriptView.Lines = 0;
			descriptView.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 12);
			descriptView.TextColor = UIColor.White;

			descriptView.Frame = new RectangleF (10, 250, 300, ((movieDetail.Overview.Length / 50) * 25));

			descriptView.LineBreakMode = UILineBreakMode.WordWrap;
			//descriptView.TextAlignment = UITextAlignment.Left;


			var playClip  = new UITapGestureRecognizer(HandleAction);

			saveFavoriteButt.TouchDown += SaveFavoriteButt_TouchDown;
			saveFavoriteButt.BackgroundColor = UIColor.Orange;
			playVideoButt.SetTitle ("Delete Favorite", UIControlState.Normal);
			playVideoButt.TouchDown += PlayVideoButt_TouchDown;
			playVideoButt.BackgroundColor = UIColor.Green;
			posterImage.UserInteractionEnabled = true;
			posterImage.AddGestureRecognizer (playClip);



		}

		void HandleAction ()
		{
			var webView = new UIWebView (View.Bounds);
			if (_embededMoveId != "")
			   View.AddSubview (webView);

			//var url = "https://www.youtube.com/embed/watch?v=5794f3fdc3a3681ee9002e9f"; // NOTE: https secure request
			//var url = "https://gdata.youtube.com/feeds/api/videos/5794f3fdc3a3681ee9002e9f?v=2";

		//	var url = "https://www.googleapis.com/youtube/v3/videos?id=5794f3fdc3a3681ee9002e9f&key=AIzaSyCu634TJuZR_0iUhJQ6D8E9xr2a3VbU3_M&part=snippet,contentDetails,statistics,status";
			//var url = "https://www.youtube.com/embed/7lCDEYXw3mM";
			var url = "https://www.youtube.com/embed/" + _embededMoveId;
			webView.LoadRequest (new NSUrlRequest (new NSUrl (url)));
		}


			                                        /// <summary>
		 /// This is the button press delegate for clear favorites button. It was a last minute change and needs to be renamed
		 /// </summary>
		 /// <param name="sender">Sender.</param>
		 /// <param name="e">E.</param>
		void PlayVideoButt_TouchDown (object sender, EventArgs e)
		{
			posterImage.Layer.BorderWidth = 1.0f;
			posterImage.Layer.BorderColor = UIColor.White.CGColor;
			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					db.Query<Movie> ("DELETE FROM [Movie] WHERE [id] = " + movieDetail.Id);
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
			posterImage.Layer.BorderWidth = 1.0f;
			posterImage.Layer.BorderColor = UIColor.Orange.CGColor;
			movieDetail.Favorite = true;
			// Create the database and a table to hold Favorite information.
			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					db.Insert (movieDetail);
				}
			} catch (SQLite.SQLiteException) {

				using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) {
					conn.CreateTable<Movie> ();
					using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
						db.Insert (movieDetail);
					}
				}

			}


		}

		//dismiss keyboard
		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			descriptView.ResignFirstResponder ();
			movieTitle.ResignFirstResponder ();
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

