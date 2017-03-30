using System;
using Foundation;
using UIKit;

namespace FavoriteMovies
{
	public class ContactCard : UITableViewCell
	{
		public UIImageView profileImage;
		public UILabel descriptionLabel;
		public UIImageView addRemove;
		public UILabel nameLabel;
		public int moviesInCommon { get; set; }
		public string location { get; set; }
		public UILabel locationLabel { get; set; }
		public string id;
		public bool imageDataAvailable { get; set; }
		public bool? connection { get; set; }
		[Export ("initWithStyle:reuseIdentifier:")]
		public ContactCard (UITableViewCellStyle style, string cellId) : base (style, cellId)
		{

			profileImage = new UIImageView ();
			descriptionLabel = new UILabel ();
			nameLabel = new UILabel ();
			addRemove = new UIImageView ();
			locationLabel = new UILabel ();

			//profileImage.Image = ColorExtensions.profileImage;
			nameLabel.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 13);
			descriptionLabel.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 10);
			locationLabel.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, 10);
			ContentView.AddSubviews (profileImage, descriptionLabel, nameLabel, addRemove, locationLabel);

		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			//profileImage.Frame = new CoreGraphics.CGRect () { X = ContentView.Bounds.X - 80, Y = ContentView.Bounds.Y + 43, Width = ContentView.Bounds.Width - 103, Height = ContentView.Bounds.Height - 90 };
			profileImage.Frame = new CoreGraphics.CGRect (10, 5, 40, 40);
			profileImage.ClipsToBounds = true;
			profileImage.ContentMode = UIViewContentMode.ScaleAspectFit;
			profileImage.Layer.CornerRadius =profileImage.Frame.Size.Width / 2 ;
			profileImage.Layer.MasksToBounds = true;
			nameLabel.Frame = new CoreGraphics.CGRect () { X = 60, Y = 0, Width = ContentView.Bounds.Width - 63, Height = 30 };
			descriptionLabel.Frame = new CoreGraphics.CGRect () { X = 60, Y = 17, Width = ContentView.Bounds.Width - 50, Height = 30 };
			locationLabel.Frame = new CoreGraphics.CGRect () { X = 60, Y = 32, Width = ContentView.Bounds.Width - 50, Height = 30 };
			//if(moviesInCommon>0)
			//   descriptionLabel.Text = "You have " + moviesInCommon + " movie" + (moviesInCommon==0||moviesInCommon > 1 ?"s":"") + " in common";
			addRemove.Frame = new CoreGraphics.CGRect () { X = 215, Y = 12, Width = 100, Height = 20 };
			addRemove.ContentMode = UIViewContentMode.ScaleAspectFit	;
			addRemove.Layer.CornerRadius = profileImage.Frame.Size.Width / 2;
			addRemove.Layer.MasksToBounds = true;

		}
	}

}
