using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading.Tasks;
using FavoriteMoviesPCL;
using Foundation;
using UIKit;

namespace FavoriteMovies
{
	public partial class MovieDetailsViewController : UIViewController
	{

		Movie movieDetail;
		ObservableCollection<Movie> similarMovies;

		public MovieDetailsViewController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}

		[Export ("initWithCoder:")]
		public MovieDetailsViewController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}
		public MovieDetailsViewController (Movie movie) : base("MovieDetailsViewController", null)
		{
			Initialize ();

			this.movieDetail = movie;
		}

		void Initialize ()
		{
			var task = Task.Run (async () => {
				similarMovies = MovieService.GetMoviesAsync (MovieService.MovieType.Similar).Result;

			});
			task.Wait ();



		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			// Perform any additional setup after loading the view, typically from a nib.

			var nav = NavigationController;


			nav.NavigationBar.BarTintColor = UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, 1.0f);
			nav.NavigationBar.TintColor = UIColor.White;
			nav.NavigationBar.Translucent = false;
			nav.NavigationBar.TopItem.Title = UIColorExtensions.TITLE;

			this.View.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);


			posterImage.ContentMode = UIViewContentMode.ScaleToFill;
			posterImage.Layer.BorderWidth = 1.0f;
			posterImage.Layer.BorderColor = UIColor.White.CGColor;
			posterImage.ClipsToBounds = true;
			posterImage.Image = MovieCell.GetImage (movieDetail.HighResPosterPath);

			movieTitle.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 15);
			movieTitle.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			movieTitle.TextColor = UIColor.White;
			movieTitle.Text = movieDetail.Title;
			movieTitle.ResignFirstResponder ();

			dateOpenView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			dateOpenView.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 10);
			dateOpenView.TextColor = UIColor.White;
			dateOpenView.Text = "Release Date: " + movieDetail.ReleaseDate.ToString ("MMMM dd, yyyy");

			voteResultText.Text = Convert.ToInt32(movieDetail.VoteAverage).ToString () + " of 10 Stars";
			voteResultText.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			voteResultText.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 10);
			voteResultText.TextColor = UIColor.White;

			descriptView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			descriptView.Text = movieDetail.Overview;

			descriptView.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 13);
			descriptView.TextColor = UIColor.White;
			descriptView.ResignFirstResponder ();

			//this..RegisterClassForCell (typeof (UserCell), UserCell.CellID);
			//collectionViewUser.ShowsHorizontalScrollIndicator = false;
			//collectionViewUser.Source = userSource;


		}

		public override void DidReceiveMemoryWarning ()
		{
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}
	}
	public class SimilarMoviesDataSource : UICollectionViewSource
	{
		public SimilarMoviesDataSource ()
		{
			Rows = new List<SimilarMovie> ();
		}

		public List<SimilarMovie> Rows { get; private set; }

		public Single FontSize { get; set; }

		public SizeF ImageViewSize { get; set; }

		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return Rows.Count;
		}

		public override Boolean ShouldHighlightItem (UICollectionView collectionView, NSIndexPath indexPath)
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

}

