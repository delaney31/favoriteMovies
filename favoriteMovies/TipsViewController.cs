using System;
using System.Collections.Generic;
using System.Linq;
using Carousels;
using CoreGraphics;
using UIKit;

namespace FavoriteMovies
{
	public class TipsViewController : BaseController
	{
		iCarousel carousel;
		UIImageView background;


        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);
            NewsFeedTableSource.HideTabBar ((UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.TabController, View.BackgroundColor);
        }

        public override void ViewWillDisappear (bool animated)
        {
            base.ViewWillDisappear (animated);
			NewsFeedTableSource.ShowTabBar ((UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.TabController);
        }
		public override void ViewWillAppear (bool animated)
        	{
           	base.ViewWillAppear (animated);
		if (NavigationController != null) 
			{
				NavigationController.NavigationBar.Translucent = true;
			}
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			bool wrap = true;

			// create a nice background
			background = new UIImageView (View.Bounds);
			background.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
			background.ContentMode = UIViewContentMode.ScaleToFill;
			background.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			
			View.AddSubview (background);

			// create the carousel
			carousel = new iCarousel (new CGRect (10, 70, 300.0f, 500.0f));
			carousel.Type = iCarouselType.Cylinder;
			carousel.DataSource = new CarouselDataSource ();
			carousel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			//Bocarousel.ScrollSpeed = 0.5f;
			View.AddSubview (carousel);
			 //customize the appearance of the carousel
			carousel.GetValue = (sender, option, value) => 
			{
				// set a nice spacing between items
				if (option == iCarouselOption.Spacing) {
					return value * 1.1F;
				} else if (option == iCarouselOption.Wrap) {
					return wrap ? 1 : 0;
				}

				// use the defaults for everything else
				return value;
			};

			//// handle item selections
			//carousel.ItemSelected += (sender, args) => 
			//{
			//	using (var alert = new UIAlertView ("Item Selected", string.Format ("You selected item '{0}'.", args.Index), null, "OK"))
			//		alert.Show ();
			//};

			//NavigationItem.RightBarButtonItem = new UIBarButtonItem ("Wrap Off", UIBarButtonItemStyle.Plain, (sender, args) => {
			//	wrap = !wrap;
			//	carousel.ReloadData ();
			//	if (wrap)
			//		NavigationItem.RightBarButtonItem.Title = "Wrap On";
			//	else
			//		NavigationItem.RightBarButtonItem.Title = "Wrap Off";
			//});
			//NavigationItem.LeftBarButtonItem = new UIBarButtonItem (carousel.Type.ToString (), UIBarButtonItemStyle.Plain, (sender, args) => {
			//	// create the popup
			//	UIActionSheet sheet = new UIActionSheet ("Select Carousel Type");
			//	var names = Enum.GetNames (typeof (iCarouselType));
			//	foreach (var type in names.Where (n => n != "Custom"))
			//		sheet.AddButton (type);
			//	// change the type
			//	sheet.Dismissed += (_, e) => {
			//		if (e.ButtonIndex != -1) {
			//			// animate the change
			//			UIView.BeginAnimations (null);
			//			carousel.Type = (iCarouselType)Enum.Parse (typeof (iCarouselType), names [e.ButtonIndex]);
			//			UIView.CommitAnimations ();

			//			NavigationItem.LeftBarButtonItem.Title = carousel.Type.ToString ();
			//		}
			//	};
			//	// show the popup
			//	sheet.ShowInView (View);
			//});
		}
	}
	// a data source that displays 100 items
	class CarouselDataSource : iCarouselDataSource
	{
 		List<UIImage> items;

		public CarouselDataSource ()
		{
			// create our amazing data source
			//items = Enumerable.Range (0, 5).ToArray ();
			items = new List<UIImage> ();
			items.Add (UIImage.FromBundle ("1---search-movies.png"));
			items.Add (UIImage.FromBundle ("2 - list.png"));
			items.Add (UIImage.FromBundle ("3 - chat.png"));
			items.Add (UIImage.FromBundle ("4 -  news feed.png"));
			items.Add (UIImage.FromBundle ("5 - connect.png"));

		}

		// let the carousel know how many items to render
		public override nint GetNumberOfItems (iCarousel carousel)
		{
			// return the number of items in the data
			return items.Count;
		}

		// create the view each item in the carousel
		public override UIView GetViewForItem (iCarousel carousel, nint index, UIView view)
		{
			UILabel label = null;
			UIImageView imageView = null;

			if (view == null) 
			{
				// create new view if no view is available for recycling
				imageView = new UIImageView (new CGRect (0, 0, 320.0f, 320.0f));
				imageView.Image = items[(int)index];
				imageView.ContentMode = UIViewContentMode.ScaleAspectFill;

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
			//label.Text = items [index].ToString ();

			return imageView;
		}
	}

}
	

