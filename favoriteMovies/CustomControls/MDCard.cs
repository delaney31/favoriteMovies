using System;
using Foundation;
using UIKit;

namespace FavoriteMovies
{
	public class MDCard:UITableViewCell
	{
		public UILabel titleLabel;
		public UILabel nameLabel;
		public UILabel descriptionLabel;

		public UIButton commentButton ;
		public UIButton likeButton;


		public UIView cardView;
		public UIImageView profileImage;

	
		[Export ("initWithStyle:reuseIdentifier:")]
		public MDCard (UITableViewCellStyle style, string cellId): base( style, cellId) 
		{
			cardView = new UIView ();
			profileImage = new UIImageView ();
			likeButton = new UIButton ();
			descriptionLabel = new UILabel ();
			nameLabel = new UILabel ();
			titleLabel = new UILabel ();

			//likeButton.Frame = new CoreGraphics.CGRect () { X = 5, Y = 20, Width = 290, Height = 290 };

			titleLabel.Frame = new CoreGraphics.CGRect () { X = 15, Y = -5, Width = 250, Height = 100 };
			titleLabel.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE);
			titleLabel.Lines = 2;
			titleLabel.TextAlignment = UITextAlignment.Left;

			nameLabel.Frame = new CoreGraphics.CGRect () { X = 15, Y = 10, Width = 300, Height = 20 };
			nameLabel.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, UIColorExtensions.CAST_FONT_SIZE);

			descriptionLabel.Frame =new CoreGraphics.CGRect () { X = 15, Y = 50, Width = 250, Height = 100 };
			descriptionLabel.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, UIColorExtensions.CAST_FONT_SIZE);
			descriptionLabel.Lines = 0;
			descriptionLabel.TextAlignment = UITextAlignment.Justified;
			profileImage.Frame = new CoreGraphics.CGRect () { X = 5, Y = 140, Width = 289, Height = 140 };

			cardView.AddSubviews (profileImage, likeButton, descriptionLabel, nameLabel, titleLabel);
			ContentView.AddSubviews ( cardView);

		}


		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			cardSetup ();
			imageSetup ();
		}


		void cardSetup ()
		{
			this.cardView.Alpha = 1;

			this.cardView.Layer.MasksToBounds = false;
			this.cardView.Layer.CornerRadius = 1;
			this.cardView.Layer.ShadowOffset = new CoreGraphics.CGSize (-.2f, .2f);
			this.cardView.Layer.ShadowRadius = 1;
			this.cardView.Layer.ShadowOpacity = 0.2f;

			UIBezierPath path = UIBezierPath.FromRect (cardView.Bounds);
			this.cardView.Layer.ShadowPath = path.CGPath;

			this.BackgroundColor = UIColor.FromRGBA (.9f, .9f, .9f, 1);

		}

		void imageSetup ()
		{
			//profileImage.Layer.CornerRadius = profileImage.Frame.Size.Width / 2;
			profileImage.ClipsToBounds = true;
			profileImage.ContentMode = UIViewContentMode.ScaleAspectFit;
			//profileImage.BackgroundColor = UIColor.White;
		}


		public override void SetSelected (bool selected, bool animated)
		{
			base.SetSelected (selected, animated);
		}
	}
}
