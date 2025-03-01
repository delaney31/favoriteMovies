﻿using System;
using System.Threading.Tasks;
using CoreGraphics;
using FavoriteMoviesPCL;
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
		
		//public UIImageView likeButton;
		public UILabel numberLikes;



		public UIView cardView;
		public UIImageView profileImage;
	

		public bool liked { get; private set; }
		public int likes { get;set; }
		public bool reusable { get; private set; }
		public string nonreusableTitle { get; set; }
		public string Link { get; internal set; }
		public string PubDate { get; internal set; }
		public string Creator { get; internal set; }
		public string Category { get; internal set; }
		public string Content { get; internal set; }
		public string id { get; internal set; }







		[Export ("initWithStyle:reuseIdentifier:")]
		public MDCard (UITableViewCellStyle style, string cellId): base( style, cellId) 
		{

			reusable = true;
			profileImage = new UIImageView ();
			profileImage.Frame = new CGRect () { X = 5, Y = 140, Width = 289, Height = 140 };

			titleLabel = new UILabel ();
			titleLabel.Frame = new CoreGraphics.CGRect () { X = 15, Y = -5, Width = 250, Height = 100 };
			titleLabel.Font = UIFont.FromName (ColorExtensions.TITLE_FONT,14);
			titleLabel.Lines = 2;
			titleLabel.TextAlignment = UITextAlignment.Left;
			liked = true;

			nameLabel = new UILabel ();
			nameLabel.Frame = new CGRect () { X = 15, Y = 10, Width = 300, Height = 20 };
			nameLabel.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);

			descriptionLabel = new UILabel ();
			descriptionLabel.Frame = new CGRect () { X = 15, Y = 50, Width = 250, Height = 100 };
			descriptionLabel.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);
			descriptionLabel.Lines = 0;
			descriptionLabel.TextAlignment = UITextAlignment.Justified;


			numberLikes = new UILabel ();
			numberLikes.Frame = new CGRect () { X = 230, Y = 295, Width = 50, Height = 20 };
			numberLikes.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);

			likeLabel = new UILabel ();
			likeLabel.Frame = new CGRect () { X = 28, Y = 295, Width = 70, Height = 20 };
			likeLabel.Font = UIFont.FromName (ColorExtensions.CONTENT_FONT, ColorExtensions.CAST_FONT_SIZE);


			cardView = new UIView ();
			cardView.AddSubviews (profileImage, likeLabel, descriptionLabel, nameLabel, titleLabel, numberLikes);
			//cardView.BringSubviewToFront (likeButton);
			ContentView.AddSubviews (cardView);

		}


		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

		}

		public override void Draw (CGRect rect)
		{
			cardSetup ();
			imageSetup ();
		}
		void cardSetup ()
		{
			this.cardView.Alpha = 1;
			this.cardView.Frame = new CGRect () { X = 10, Y = 20, Width = 300, Height = 325 };
			//this.ContentView.Frame =  new CGRect(){new UIEdgeInsets (10, 10, 10, 10);

			this.cardView.Layer.MasksToBounds = false;
			this.cardView.Layer.CornerRadius = 10f;
			//this.cardView.Layer.CornerRadius = 1;
			this.cardView.Layer.ShadowOffset = new CoreGraphics.CGSize (-.2f, .2f);
			//this.cardView.Layer.ShadowRadius = 
			this.cardView.Layer.ShadowRadius = 10f;
			this.cardView.Layer.ShadowOpacity = 0.2f;

			UIBezierPath path = UIBezierPath.FromRect (cardView.Bounds);
			this.cardView.Layer.ShadowPath = path.CGPath;

		}

		void imageSetup ()
		{
			//profileImage.Layer.CornerRadius = profileImage.Frame.Size.Width / 2;
			profileImage.ClipsToBounds = true;
			profileImage.Layer.CornerRadius = 20f;
			profileImage.ContentMode = UIViewContentMode.ScaleAspectFit;
			//profileImage.BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f);
		}


		public override void SetSelected (bool selected, bool animated)
		{
			//base.SetSelected (selected, animated);
		}
	}
}
