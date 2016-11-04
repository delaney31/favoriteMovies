using System;
using System.Collections.Generic;
using UIKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using FavoriteMoviesPCL;
using System.Drawing;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;

namespace FavoriteMovies
{/// <summary>
/// This is the Main View controller for the application. It serves as the Top Rated collection of movies and creates the Now Playing 
	/// Poplular scrollable collections.
/// </summary>
	public class MainCollectionViewController : UICollectionViewController
	{
		static NSString movieCellId = new NSString ("MovieCell");
		static CGSize HeaderReferenceSize = new CGSize (50, 50);
		static int MinimumInteritemSpacing = 30;
		static int MinimumLineSpacing = 5;
		static CGSize ItemSize = new CGSize (110, 150);
		static CGRect TopRatedLabelFrame = new CGRect (7, 10, 90, 20);
		static CGRect NowPlayingLabelFrame = new CGRect (7, 185, 90, 20);
		static CGRect PopularLabelFrame = new CGRect (7, 360, 90, 20);
		static CGRect TopRatedControllerFrame = new CGRect (-45, 30, 400, 150);
		static CGRect NowPlayingControllerFrame = new CGRect (-45, 205, 400, 150);
		static CGRect PopularControllerFrame = new CGRect (-45, 380, 400, 150);

		const float BackGroundColorAlpha = 1.0f;
		static string TopRated = "Top Rated";
		static string NowPlaying = "Now Playing";
		static string Popular = "Popular";
		const int LabelZPosition = 1;
		const int SectionCount = 1;
		ObservableCollection<Movie> topRated;
		ObservableCollection<Movie> nowPlaying;
		ObservableCollection<Movie> popular;
		UICollectionViewFlowLayout flowLayout;
		NowPlayingCollectionViewController nowPlayingController;
		PopularCollectionViewController popularController;
		TopRatedCollectionViewController topRatedController;
		UIWindow window;
		static UILabel TopRatedLabel;
		static UILabel PlayingNowLabel;
		static UILabel PopularLabel;



		public MainCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> topRated, ObservableCollection<Movie> nowPlaying, ObservableCollection<Movie> popular ): base (layout)
		{
			this.topRated = topRated;
			this.nowPlaying = nowPlaying;
			this.popular = popular;
			flowLayout = new UICollectionViewFlowLayout () {
				HeaderReferenceSize = HeaderReferenceSize,
				ScrollDirection = UICollectionViewScrollDirection.Horizontal,
				MinimumInteritemSpacing = MinimumInteritemSpacing, // minimum spacing between cells
				MinimumLineSpacing = MinimumLineSpacing, // minimum spacing between rows if ScrollDirection is Vertical or between columns if Horizontal
				ItemSize = ItemSize
				//SectionInset = new UIEdgeInsets (80, -40, 97, 127)
			};

		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			//ShowLabels ();
			CollectionView.AddSubview (topRatedController.CollectionView);
			CollectionView.AddSubview (nowPlayingController.CollectionView);
			CollectionView.AddSubview (popularController.CollectionView);
			// adding label views to View. 
			CollectionView.AddSubview (TopRatedLabel);
			CollectionView.AddSubview (PlayingNowLabel);
			CollectionView.AddSubview (PopularLabel);
			topRatedController.CollectionView.ReloadData ();
			nowPlayingController.CollectionView.ReloadData ();
			popularController.CollectionView.ReloadData ();

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			window = UIApplication.SharedApplication.KeyWindow;


			//Create Labels for Movie Collections
			TopRatedLabel = new UILabel () {
				TextColor = UIColor.White, Frame = TopRatedLabelFrame,
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
				Text = TopRated
			};

			PlayingNowLabel = new UILabel () {
				TextColor = UIColor.White, Frame = NowPlayingLabelFrame,
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
				Text = NowPlaying
			};
			PopularLabel = new UILabel () {
				TextColor = UIColor.White, Frame = PopularLabelFrame,
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha),
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE),
				Text = Popular
			};
		

			TopRatedLabel.Layer.ZPosition = LabelZPosition;
			PlayingNowLabel.Layer.ZPosition = LabelZPosition;
			PopularLabel.Layer.ZPosition = LabelZPosition;

			#region   //Setup my movie collection controllers

			topRatedController = new TopRatedCollectionViewController( flowLayout, topRated, window);
			topRatedController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			topRatedController.CollectionView.RegisterClassForCell (typeof (MovieCell), movieCellId);
			topRatedController.CollectionView.Frame = TopRatedControllerFrame;

			CollectionView.BackgroundColor =UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			CollectionView.Bounces = true;
			CollectionView.ScrollEnabled = true;


			nowPlayingController = new NowPlayingCollectionViewController (flowLayout,nowPlaying,window);
			nowPlayingController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			nowPlayingController.CollectionView.RegisterClassForCell (typeof (MovieCell), movieCellId);
			nowPlayingController.CollectionView.Frame = NowPlayingControllerFrame;


			popularController = new PopularCollectionViewController (flowLayout, popular,window);
			popularController.CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			popularController.CollectionView.RegisterClassForCell (typeof (MovieCell), movieCellId);
			popularController.CollectionView.Frame = PopularControllerFrame;

	
			#endregion

		}

	}
	
	public class MovieCell : UICollectionViewCell
	{
		public UIImageView ImageView { get; set; }
		protected CGRect topRatedRect = new CGRect (-2, 40, 97, 127);
		protected const string baseUrl = "https://image.tmdb.org/t/p/w300/";
		[Export ("initWithFrame:")]
		public MovieCell (CGRect frame) : base (frame)
		{
			
			ImageView = new UIImageView ();
			ImageView.Center = ContentView.Center;
			ImageView.Frame = ContentView.Frame;
			ImageView.ContentMode = UIViewContentMode.ScaleToFill;
			ContentView.Layer.BorderWidth = 2.0f;
			ContentView.Layer.BorderColor = UIColor.White.CGColor;
			ContentView.AddSubview (ImageView);

		}


		//public UILabel LabelView { get; private set; }
		public void UpdateRow (Movie element, Single fontSize)
		{
			ImageView.Image = GetImage (element.PosterPath);
			try {
				if (UIColorExtensions.MovieIsFavorite (element.Id.ToString ())) {

					ContentView.Layer.BorderColor = UIColor.Orange.CGColor;

				} else 
				{
					ContentView.Layer.BorderColor = UIColor.White.CGColor;
				}
			} catch (SQLite.SQLiteException ex) 
			{
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
				return UIImage.FromBundle ("placeholder.png");
			}

		}
	}



}   



