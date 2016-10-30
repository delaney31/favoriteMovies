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

namespace FavoriteMovies
{
	public class SimpleCollectionViewController : UICollectionViewController,IUICollectionViewDelegateFlowLayout
	{
		public static NSString movieCellId = new NSString ("MovieCell");
		static NSString headerId = new NSString ("Header");
		const int SectionCount = 1;
		ObservableCollection<Movie> topRated;
		ObservableCollection<Movie> nowPlaying;
		ObservableCollection<Movie> similar;
		ObservableCollection<Movie> popular;



		public SimpleCollectionViewController (UICollectionViewLayout layout, ObservableCollection<Movie> topRated, ObservableCollection<Movie> nowPlaying, ObservableCollection<Movie> similar, ObservableCollection<Movie> popular) : this (layout, topRated)
		{
			this.topRated = topRated;
			this.nowPlaying = nowPlaying;
			this.similar = similar;
			this.popular = popular;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			//var screenSize = UIScreen.MainScreen.Bounds;
			//var screenWidth = screenSize.Width ;
			//var screenHight = screenSize.Height;
			//var layout = new UICollectionViewFlowLayout();

			var label = new UILabel () {
				TextColor = UIColor.White, Frame = new CGRect (5, 70, 90, 20),
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f),
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE)
			};
			label.Text = "Top Rated";
			label.Layer.ZPosition = 1;

			var window = UIApplication.SharedApplication.KeyWindow;
			//layout.SectionInset = new UIEdgeInsets () { Top = 10, Left = 0, Bottom = 10, Right = 0 };
			//layout.ItemSize = new CGSize () { Width = screenWidth / 4, Height = screenWidth / 4};
			//CollectionView.PagingEnabled = false;
			//CollectionView.Frame = new CGRect (-45, 140, 306, 560);
			//CollectionView.Frame = new CGRect(-45,-140, 110, 150);
			//CollectionView.Transform = CGAffineTransform.MakeScale (.8f, .8f);
			CollectionView.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			//CollectionView.RegisterClassForSupplementaryView (typeof (Header), UICollectionElementKindSection.Header, headerId);
			CollectionView.RegisterClassForCell (typeof (MovieCell), movieCellId);
			//_dataSource = new MovieCollectionSource ();
			//_dataSource.Items = await MovieService.GetTopRatedMoviesAsync ();
			//CollectionView.Source = _dataSource;

			//CollectionView.Frame = this.View.Frame;
			//layout.MinimumInteritemSpacing = 3;
			//layout.MinimumLineSpacing = 3;
			//CollectionView.CollectionViewLayout = layout;
			//CollectionView.AlwaysBounceVertical = true;
			CollectionView.ScrollEnabled = true;
			CollectionView.ContentMode = UIViewContentMode.ScaleAspectFit;
			CollectionView.SizeToFit ();

			window.AddSubview (label);
			//CollectionView.ContentSize = new CGSize (CollectionView.Frame.Size.Width, CollectionView.Frame.Size.Height);
			//UIMenuController.SharedMenuController.MenuItems = new UIMenuItem [] {
			//	new UIMenuItem ("Custom", new Selector ("custom"))
			//};
		}
		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return SectionCount;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return topRated.Count / SectionCount;
		}


		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var Moviecell = (MovieCell)collectionView.DequeueReusableCell (SimpleCollectionViewController.movieCellId, indexPath);

			//IntPtr uikit = Dlfcn.dlopen (Constants.UIKitLibrary, 0);
			//NSString header = Dlfcn.GetStringConstant (uikit, "UICollectionElementKindSectionHeader");

			//if (startup) {
				//NSString header = (NSString)UICollectionElementKindSection.Header.ToString ();
			//	GetViewForSupplementaryElement (collectionView, header, indexPath);
			//}

			var row = topRated [indexPath.Row];

			Moviecell.UpdateRow (row,UIColorExtensions.HEADER_FONT_SIZE);

			return Moviecell;
		}



		//public override UICollectionReusableView GetViewForSupplementaryElement (UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
		//{

		//	var headerView = (Header)collectionView.DequeueReusableSupplementaryView (elementKind, headerId, indexPath);
		//	headerView.Text = "Top Rated";
		//	startup = true;
		//	return headerView;


		//}


	}

	public class MovieCell : UICollectionViewCell
	{
		public UIImageView ImageView { get; private set; }
		public UIImage Image { get; internal set; }

		const string baseUrl = "https://image.tmdb.org/t/p/w300/";
		[Export ("initWithFrame:")]
		public MovieCell (CGRect frame) : base (frame)
		{
			//BackgroundView = new UIView { BackgroundColor = UIColor.White };
			//BackgroundView.Frame = new CGRect (5, 6.5, 54, 25);
			//SelectedBackgroundView = new UIView { BackgroundColor = UIColor.White };
			//SelectedBackgroundView.Frame=new CGRect (-19, -48, 87, 122);

			//ContentView.Frame = new CGRect (5, 5, 100000, 64);
			//ContentView.Frame = new CGRect (-45, -140, 110, 150);
			//ContentView.Layer.BorderColor = UIColor.White.CGColor;
			//ContentView.Layer.BorderWidth = 2.0f;
			//ContentView.BackgroundColor = UIColor.Clear;
			//ContentView.Transform = CGAffineTransform.MakeScale (.8f, .8f);

			ImageView = new UIImageView ();
			ImageView.Center = ContentView.Center;
			//ImageView.Transform = CGAffineTransform.MakeScale (.8f, .8f);
			ImageView.Frame = new CGRect (-65,-145, 97, 127);
			ImageView.ContentMode = UIViewContentMode.ScaleToFill;
			ImageView.Layer.BorderWidth = 1.0f;
			ImageView.Layer.BorderColor = UIColor.White.CGColor;
			ImageView.ClipsToBounds = true;
			ContentView.AddSubview (ImageView);

			//LabelView = new UILabel ();
			//LabelView.BackgroundColor = UIColor.Clear;
			//LabelView.TextColor = UIColor.White;
			//LabelView.TextAlignment = UITextAlignment.Left;

			//ContentView.AddSubview (LabelView);

		}
		public UILabel LabelView { get; private set; }
		public void UpdateRow (Movie element, Single fontSize)
		{
			
			ImageView.Image = GetImage(element.PosterPath);

		}

		public UIImage GetImage (Uri posterPath)
		{
			

			using (var imgUrl = new NSUrl (baseUrl + posterPath.AbsoluteUri.Substring(8))) {
				using (var data = NSData.FromUrl (imgUrl)) {
					return (UIImage.LoadFromData (data));

				}
			}
		}


	}

	public class Header : UICollectionReusableView
	{
		UILabel label;

		public string Text {
			get {
				return label.Text;
			}
			set {
				label.Text = value;
				SetNeedsDisplay ();
			}
		}

		[Export ("initWithFrame:")]
		public Header (CGRect frame) : base (frame)
		{
			label = new UILabel () { TextColor= UIColor.White,Frame = new CGRect (5, 5, frame.Width+40, 20), 
				BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, 1.0f), 
				Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE) };
			label.Layer.ZPosition = 1;
			AddSubview (label);
		}
	}
}   



