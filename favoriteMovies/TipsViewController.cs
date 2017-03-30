using System;
using System.Linq;
using Carousels;
using CoreGraphics;
using UIKit;

namespace FavoriteMovies
{
	public class TipsViewController : UIViewController
	{
		iCarousel carousel;
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// create and add the Carousel to the view
			carousel = new iCarousel ();
			carousel.Type = iCarouselType.CoverFlow2;
			carousel.DataSource = new CarouselDataSource ();
			carousel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			View.AddSubview (carousel);

			// handle item selections / taps
			carousel.ItemSelected += (sender, args) => {
				var indexSelected = args.Index;
				// do something with a selection
			};
		}
	}
	// a data source that displays 100 items
	class CarouselDataSource : iCarouselDataSource
	{
		int [] items;

		public CarouselDataSource ()
		{
			// create our amazing data source
			items = Enumerable.Range (0, 5).ToArray ();
		}

		// let the carousel know how many items to render
		public override nint GetNumberOfItems (iCarousel carousel)
		{
			// return the number of items in the data
			return items.Length;
		}

		// create the view each item in the carousel
		public override UIView GetViewForItem (iCarousel carousel, nint index, UIView view)
		{
			UILabel label = null;
			UIImageView imageView = null;

			if (view == null) 
			{
				// create new view if no view is available for recycling
				imageView = new UIImageView (new CGRect (0, 0, 200.0f, 200.0f));
				imageView.Image = UIImage.FromBundle ("page.png");
				imageView.ContentMode = UIViewContentMode.Center;

				label = new UILabel (imageView.Bounds);
				label.BackgroundColor = UIColor.Clear;
				label.TextAlignment = UITextAlignment.Center;
				label.Font = label.Font.WithSize (50);
				label.Tag = 1;
				imageView.AddSubview (label);
			} else 
			{
				// get a reference to the label in the recycled view
				imageView = (UIImageView)view;
				label = (UILabel)view.ViewWithTag (1);
			}

			// set the values of the view
			label.Text = items [index].ToString ();

			return imageView;
		}
	}

}
	

