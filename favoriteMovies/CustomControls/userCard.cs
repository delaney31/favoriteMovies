using System;
using Foundation;
using UIKit;

namespace FavoriteMovies
{
	public class userCard : MDCard
	{
		UIImageView userImage;
		[Export ("initWithStyle:reuseIdentifier:")]
		public userCard (UITableViewCellStyle style, string cellId) : base (style, cellId)
		{
			cardView = new UIView ();
			profileImage = new UIImageView ();
			userImage = new UIImageView ();
			likeButton = new UIImageView ();
			descriptionLabel = new UILabel ();
			nameLabel = new UILabel ();
			titleLabel = new UILabel ();

			profileImage.Image = UIColorExtensions.profileImage;
			//likeButton.Frame = new CoreGraphics.CGRect () { X = 5, Y = 20, Width = 290, Height = 290 };
			profileImage.Frame = new CoreGraphics.CGRect () { X = 5, Y = 140, Width = 289, Height = 140 };
			titleLabel.Frame = new CoreGraphics.CGRect () { X = 15, Y = 0, Width = 250, Height = 100 };
			titleLabel.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE);
			titleLabel.Lines = 0;
			titleLabel.TextAlignment = UITextAlignment.Left;

			nameLabel.Frame = new CoreGraphics.CGRect () { X = 15, Y = 10, Width = 300, Height = 20 };
			nameLabel.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, UIColorExtensions.CAST_FONT_SIZE);

			descriptionLabel.Frame = new CoreGraphics.CGRect () { X = 15, Y = 50, Width = 250, Height = 100 };
			descriptionLabel.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, UIColorExtensions.CAST_FONT_SIZE);
			descriptionLabel.Lines = 0;
			descriptionLabel.TextAlignment = UITextAlignment.Justified;

			cardView.AddSubviews (profileImage, likeButton, descriptionLabel, nameLabel, titleLabel);
			ContentView.AddSubviews (cardView);

		}
	}

}
