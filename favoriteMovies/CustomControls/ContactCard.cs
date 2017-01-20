using System;
using Foundation;
using UIKit;

namespace FavoriteMovies
{
	public class ContactCard : UITableViewCell
	{
		public UIImageView profileImage;
		public UILabel descriptionLabel;
		public UILabel nameLabel;
		public string id;
		public bool imageDataAvailable { get; set; }
		public bool connection { get; set;}
		[Export ("initWithStyle:reuseIdentifier:")]
		public ContactCard (UITableViewCellStyle style, string cellId) : base (style, cellId)
		{
			
			profileImage = new UIImageView ();
			descriptionLabel = new UILabel ();
			nameLabel = new UILabel ();


			profileImage.Image = ColorExtensions.profileImage;
			nameLabel.Font = UIFont.FromName (ColorExtensions.TITLE_FONT, 15);
			ContentView.AddSubviews (profileImage, descriptionLabel, nameLabel);

		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			profileImage.Frame = new CoreGraphics.CGRect () { X = 10, Y = 2, Width = 35, Height = 40 };
			profileImage.ClipsToBounds = true;
			profileImage.ContentMode = UIViewContentMode.ScaleAspectFill;
			profileImage.Layer.CornerRadius = profileImage.Frame.Size.Width / 2;
			profileImage.Layer.MasksToBounds = true;
			nameLabel.Frame = new CoreGraphics.CGRect () { X = 80, Y = 15, Width = ContentView.Bounds.Width-63, Height = 30 };

		}
	}

}
