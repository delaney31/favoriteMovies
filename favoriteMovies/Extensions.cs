using System;
using System.Drawing;
using CoreGraphics;
using FavoriteMoviesPCL;
using UIKit;

namespace FavoriteMovies
{
	public static class UIColorExtensions
	{	
		public const string TITLE = "Movie Explorer";
		public const string NAV_BAR_COLOR ="#3B5998";//facebook blue
		//public const string NAV_BAR_COLOR ="#323232";
		public const string TITLE_COLOR = "#DE9A2D";
		public const string TITLE_FONT = "AppleSDGothicNeo-Regular";
		public const string CONTENT_FONT = "AppleSDGothicNeo-Regular";
		//public const string TAB_BACKGROUND_COLOR = "#000000";//black
		//public const string TAB_BACKGROUND_COLOR = "#555555"; //grey
		public const string TAB_BACKGROUND_COLOR = "#FFFFFF"; //white
		//public const string TAB_BACKGROUND_COLOR = "#d3d3d3"; //light grey
		//public const string TAB_BACKGROUND_COLOR = "#3B3B3B"; //dark grey
		public const float HEADER_FONT_SIZE = 14f;
		public const float CAST_FONT_SIZE = 13f;
		public const string SQL_TABLE = "MovieEntries.db3";

		public static PointF Rotate (this PointF pt)
		{
			return new PointF (pt.Y, pt.X);
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
