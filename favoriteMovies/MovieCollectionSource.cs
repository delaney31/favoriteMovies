using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using UIKit;

namespace FavoriteMovies
{
	public class MovieCollectionSource : UICollectionViewSource
	{
		

		public ObservableCollection<Movie> Items { get; set; }

		public Single FontSize { get; set; }

		public SizeF ImageViewSize { get; set; }
		protected const int SectionCount = 1;

		public MovieCollectionSource ()
		{
			Items = new ObservableCollection<Movie> ();
		}



		public override nint NumberOfSections (UICollectionView collectionView)
		{
			return SectionCount;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return Items.Count/ SectionCount;
		}


		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = (MovieCell)collectionView.DequeueReusableCell (SimpleCollectionViewController.movieCellId, indexPath);

			//Movie row = Items [indexPath.Section * (Items.Count/SectionCount)+indexPath.Row];
			var row = Items [indexPath.Row];

			cell.UpdateRow(row, UIColorExtensions.HEADER_FONT_SIZE);

			return cell;
		}



	}

}
