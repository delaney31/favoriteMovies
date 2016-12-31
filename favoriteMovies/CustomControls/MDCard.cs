using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace FavoriteMovies
{
	public class MDCard:UITableViewCell
	{
		public UILabel titleLabel;
		public UILabel nameLabel;
		public UILabel descriptionLabel;
		public UILabel likeLabel;
		public UIButton commentButton ;
		public UIImageView likeButton;


		public UIView cardView;
		public UIImageView profileImage;
		public bool liked { get; private set; }
		public int likes { get;set; }
	
		[Export ("initWithStyle:reuseIdentifier:")]
		public MDCard (UITableViewCellStyle style, string cellId): base( style, cellId) 
		{
			cardView = new UIView ();
			profileImage = new UIImageView ();
			likeButton = new UIImageView ();
			descriptionLabel = new UILabel ();
			nameLabel = new UILabel ();
			titleLabel = new UILabel ();
			likeLabel = new UILabel ();
			titleLabel.Frame = new CoreGraphics.CGRect () { X = 15, Y = -5, Width = 250, Height = 100 };
			titleLabel.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE);
			titleLabel.Lines = 2;
			titleLabel.TextAlignment = UITextAlignment.Left;
			liked = true;
			nameLabel.Frame = new CoreGraphics.CGRect () { X = 15, Y = 10, Width = 300, Height = 20 };
			nameLabel.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, UIColorExtensions.CAST_FONT_SIZE);

			descriptionLabel.Frame =new CoreGraphics.CGRect () { X = 15, Y = 50, Width = 250, Height = 100 };
			descriptionLabel.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, UIColorExtensions.CAST_FONT_SIZE);
			descriptionLabel.Lines = 0;
			descriptionLabel.TextAlignment = UITextAlignment.Justified;
			profileImage.Frame = new CGRect () { X = 5, Y = 140, Width = 289, Height = 140 };


			likeLabel = new UILabel ();
			likeButton.Frame = new CGRect () { X = 15, Y = 300, Width = 20, Height = 20 };
			likeLabel.Frame = new CGRect () { X = 40, Y = 302, Width = 50, Height = 20 };
			likeLabel.Font = UIFont.FromName (UIColorExtensions.CONTENT_FONT, UIColorExtensions.CAST_FONT_SIZE);

			likeButton.Image = UIImage.FromBundle ("like.png");

			var likepress = new UITapGestureRecognizer (HandleAction);
			likeButton.AddGestureRecognizer (likepress);
			likeButton.UserInteractionEnabled = true;

			cardView.AddSubviews (profileImage, likeButton, likeLabel,descriptionLabel, nameLabel, titleLabel);
			//cardView.BringSubviewToFront (likeButton);
			ContentView.AddSubviews ( cardView);
		}

		void HandleAction ()
		{

			if (liked)
				likes++;
			else if(likes >0)
				likes--;

			liked = !liked;

			if (likes > 0)
				likeLabel.Text = likes == 1 ? likes + " Like" : likes + " Likes";
			else
				likeLabel.Text = "";
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
			this.cardView.Frame = new CGRect () { X = 10, Y = 20, Width = 300, Height = 320 };
			//this.ContentView.Frame =  new CGRect(){new UIEdgeInsets (10, 10, 10, 10);

			this.cardView.Layer.MasksToBounds = false;
			this.cardView.Layer.CornerRadius = 20f;
			//this.cardView.Layer.CornerRadius = 1;
			this.cardView.Layer.ShadowOffset = new CoreGraphics.CGSize (-.2f, .2f);
			//this.cardView.Layer.ShadowRadius = 
			this.cardView.Layer.ShadowRadius = 20f;
			this.cardView.Layer.ShadowOpacity = 0.2f;

			UIBezierPath path = UIBezierPath.FromRect (cardView.Bounds);
			this.cardView.Layer.ShadowPath = path.CGPath;

			//	this.BackgroundColor = UIColor.FromRGBA (.9f, .9f, .9f, 1);
			this.BackgroundColor = UIColor.White;
		}

		void imageSetup ()
		{
			//profileImage.Layer.CornerRadius = profileImage.Frame.Size.Width / 2;
			profileImage.ClipsToBounds = true;
			profileImage.Layer.CornerRadius = 20f;
			profileImage.ContentMode = UIViewContentMode.ScaleAspectFit;
			//profileImage.BackgroundColor = UIColor.White;
		}


		public override void SetSelected (bool selected, bool animated)
		{
			//base.SetSelected (selected, animated);
		}
	}
}
