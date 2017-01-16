using System;
using System.Drawing;
using CoreGraphics;
using FavoriteMoviesPCL;
using JSQMessagesViewController;
using UIKit;

namespace FavoriteMovies
{
	public static class ColorExtensions
	{
		public const string TITLE = "Movie Explorer";
		public const string containerName = "moviefriend";
		public const string NAV_BAR_COLOR = "#3B5998";//facebook blue
													  //public const string NAV_BAR_COLOR ="#323232";
		public const string AZURE_STORAGE_CONNECTION_STRING = "SharedAccessSignature=sv=2015-12-11&ss=bt&srt=sco&sp=rwdlacup&st=2017-01-13T01%3A11%3A00Z&se=2450-01-14T01%3A11%3A00Z&sig=f6ik2E%2BjiJX5eCBMMQnEffnXDGVhGTVt9oMVM9dqhPk%3D;BlobEndpoint=https://moviefriends.blob.core.windows.net/;TableEndpoint=https://moviefriends.table.core.windows.net";
		public const string TITLE_COLOR = "#DE9A2D";
		public const string TITLE_FONT = "AppleSDGothicNeo-Bold";
		public const string CONTENT_FONT = "AppleSDGothicNeo-Regular";
		//public const string TAB_BACKGROUND_COLOR = "#000000";//black
		//public const string TAB_BACKGROUND_COLOR = "#555555"; //grey
		public const string TAB_BACKGROUND_COLOR = "#FFFFFF"; //white
		public const string PROFILE_BACKGROUND_COLOR = "#4B5F82"; //light grey
																  //public const string TAB_BACKGROUND_COLOR = "#3B3B3B"; //dark grey
		public const float HEADER_FONT_SIZE = 17f;
		public const float CAST_FONT_SIZE = 13f;
		public const string SQL_TABLE = "MovieEntries.db3";

		public static UIImage profileImage = UIImage.FromBundle ("profile.png");
		public static PointF Rotate (this PointF pt)
		{
			return new PointF (pt.Y, pt.X);
		}
		public static UserCloud CurrentUser {get;set;}
		// resize the image to be contained within a maximum width and height, keeping aspect ratio
		public static  UIImage MaxResizeImage (UIImage sourceImage, float maxWidth, float maxHeight)
		{
			var sourceSize = sourceImage.Size;
			var maxResizeFactor = Math.Max (maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
			if (maxResizeFactor > 1) return sourceImage;
			var width = maxResizeFactor * sourceSize.Width;
			var height = maxResizeFactor * sourceSize.Height;
			UIGraphics.BeginImageContext (new SizeF ((float)width, (float)height));
			sourceImage.Draw (new RectangleF (0, 0, (float)width, (float)height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return resultImage;
		}

		// resize the image (without trying to maintain aspect ratio)
		public static UIImage ResizeImage (UIImage sourceImage, float width, float height)
		{
			UIGraphics.BeginImageContext (new SizeF (width, height));
			sourceImage.Draw (new RectangleF (0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return resultImage;
		}

		// crop the image, without resizing
		private static UIImage CropImage (UIImage sourceImage, int crop_x, int crop_y, int width, int height)
		{
			var imgSize = sourceImage.Size;
			UIGraphics.BeginImageContext (new SizeF (width, height));
			var context = UIGraphics.GetCurrentContext ();
			var clippedRect = new RectangleF (0, 0, width, height);
			context.ClipToRect (clippedRect);
			var drawRect = new RectangleF (-crop_x, -crop_y, (float)imgSize.Width, (float)imgSize.Height);
			sourceImage.Draw (drawRect);
			var modifiedImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return modifiedImage;
		}
		public static UIImage WithColor (this UIImage image, UIColor color)
		{
			UIGraphics.BeginImageContextWithOptions (image.Size, false, image.CurrentScale);
			var context = UIGraphics.GetCurrentContext ();
			context.TranslateCTM (0, image.Size.Height);
			context.ScaleCTM (1.0f, -1.0f);
			context.SetBlendMode (CGBlendMode.Normal);
			var rect = new CGRect (0, 0, image.Size.Width, image.Size.Height);
			context.ClipToMask (rect, image.CGImage);
			color.SetFill ();
			context.FillRect (rect);

			var newImage = UIGraphics.GetImageFromCurrentImageContext ();
			return newImage;
		}
		public static PointF Center (this RectangleF  rect)
		{
			return new PointF (
				(rect.Right - rect.Left) / 2.0f,
				(rect.Bottom - rect.Top) / 2.0f
				);
		}
		public static PointF Center (this CGRect rect)
		{
			return new PointF (
			(float)((rect.Right - rect.Left) / 2.0f),
			(float)((rect.Bottom - rect.Top) / 2.0f)
				);
		}
		public static bool MovieIsFavorite (string id)
		{
			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					var favorite = db.Query<Movie> ("SELECT * FROM MOVIE WHERE ID = ?", id);
					return favorite.Count > 0;
				}
			} catch (SQLite.SQLiteException) { //db not created yet}
				return false;} 
		}

		public static void addDemoMessages (this MovieChatViewController controller)
		{
			for (var num = 0; num < 10; num++ ) 
			{
				var sender = num==0?"Server":controller.SenderId;
				var messageContent = "Message  nr" + num;
				var message = Message.Create (sender, sender, messageContent);
				//controller.messages
			}
		}
		public static UIColor FromHexString (this UIColor color, string hexValue, float alpha = 1.0f)
		{
			var colorString = hexValue.Replace ("#", "");
			if (alpha > 1.0f) {
				alpha = 1.0f;
			} else if (alpha < 0.0f) {
				alpha = 0.0f;
			}

			float red, green, blue;

			switch (colorString.Length) {
			case 3: // #RGB
			{
					red = Convert.ToInt32 (string.Format ("{0}{0}", colorString.Substring (0, 1)), 16) / 255f;
					green = Convert.ToInt32 (string.Format ("{0}{0}", colorString.Substring (1, 1)), 16) / 255f;
					blue = Convert.ToInt32 (string.Format ("{0}{0}", colorString.Substring (2, 1)), 16) / 255f;
					return UIColor.FromRGBA (red, green, blue, alpha);
				}
			case 6: // #RRGGBB
			{
					red = Convert.ToInt32 (colorString.Substring (0, 2), 16) / 255f;
					green = Convert.ToInt32 (colorString.Substring (2, 2), 16) / 255f;
					blue = Convert.ToInt32 (colorString.Substring (4, 2), 16) / 255f;
					return UIColor.FromRGBA (red, green, blue, alpha);
				}

			default:
				throw new ArgumentOutOfRangeException (string.Format ("Invalid color value {0} is invalid. It should be a hex value of the form #RBG, #RRGGBB", hexValue));

			}
		}
	}
}
