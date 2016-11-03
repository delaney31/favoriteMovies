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
{
	public class MovieCollectionView : UICollectionView
	{
		const float BackGroundColorAlpha = 1.0f;
		UICollectionView View;
		static NSString movieTopRatedCellId = new NSString ("TopRatedMovieCell");
		public MovieCollectionView (UICollectionView view,UICollectionViewLayout layout) : base (view.Frame, layout)
		{
			this.DataSource = view.DataSource;
			this.BackgroundColor = UIColor.Clear.FromHexString (UIColorExtensions.TAB_BACKGROUND_COLOR, BackGroundColorAlpha);
			this.RegisterClassForCell (typeof (MovieCell), movieTopRatedCellId);

			this.SetCollectionViewLayout (layout, true);
			this.CollectionViewLayout.InvalidateLayout ();
		}


		public override UICollectionViewCell CellForItem (NSIndexPath indexPath)
		{
			//return base.CellForItem (indexPath);
			var cell = base.CellForItem (indexPath);
			cell.ContentView.Frame = cell.Bounds;
			cell.ContentView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			//var cgReg = View.Frame;
			//cell.Frame = new CGRect (cgReg.X-90, cgReg.Y, cgReg.Width, cgReg.Height);
			return cell;
		}
	}

}   
