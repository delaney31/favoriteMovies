using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Accelerate;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using JSQMessagesViewController;
using Security;
using UIKit;

namespace FavoriteMovies
{
	public static class NetworkIndicator
	{
		static int _counter;

		public static void EnterActivity ()
		{
			Interlocked.Increment (ref _counter);
			RefreshIndicator ();
		}

		public static void LeaveActivity ()
		{
			Interlocked.Decrement (ref _counter);
			RefreshIndicator ();
		}

		public static void AttachToTask (Task task)
		{
			if (task.IsCanceled || task.IsCanceled || task.IsFaulted)
				return;

			EnterActivity ();
			task.ContinueWith (t => {
				LeaveActivity ();
			});
		}

		static void RefreshIndicator ()
		{
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible =
				(_counter > 0);
		}
	}
	public static class TaskExtensions
	{
		public static Task WithNetworkIndicator (this Task task)
		{
			NetworkIndicator.AttachToTask (task);
			return task;
		}

		public static Task<TResult> WithNetworkIndicator<TResult> (this Task<TResult> task)
		{
			NetworkIndicator.AttachToTask (task);
			return task;
		}
	}
	public static class ColorExtensions
	{
		public const string TITLE = "Movie Explorer";
		public const string containerName = "moviefriend";
		public static string NAV_BAR_COLOR= "#3B5998";//facebook blue
													  //public const string NAV_BAR_COLOR ="#323232";
		public const string AZURE_STORAGE_CONNECTION_STRING = "SharedAccessSignature=sv=2015-12-11&ss=bt&srt=sco&sp=rwdlacup&st=2017-01-13T01%3A11%3A00Z&se=2450-01-14T01%3A11%3A00Z&sig=f6ik2E%2BjiJX5eCBMMQnEffnXDGVhGTVt9oMVM9dqhPk%3D;BlobEndpoint=https://moviefriends.blob.core.windows.net/;TableEndpoint=https://moviefriends.table.core.windows.net";
		public const string TITLE_COLOR = "#DE9A2D";
		public const string TITLE_FONT = "HelveticaNeue-Bold";
		public const string CONTENT_FONT = "HelveticaNeue";
		public const string PROFILE_NAME = "HelveticaNeue";
		//public const string TAB_BACKGROUND_COLOR = "#000000";//black
		//public const string TAB_BACKGROUND_COLOR = "#555555"; //grey
		public static string TAB_BACKGROUND_COLOR= "#FFFFFF"; //white

		public const string PROFILE_BACKGROUND_COLOR = "#4B5F82"; //light grey
																  //public const string TAB_BACKGROUND_COLOR = "#3B3B3B"; //dark grey
		public const float HEADER_FONT_SIZE = 15f;
		public const float CAST_FONT_SIZE = 12f;
		public const string SQL_TABLE = "MovieEntries.db3";
		public static UIImage profileImage = UIImage.FromBundle ("1481507483_compose.png");
		public static User _user;
		public static nint _CurrentTileSize;
		public static CGSize SmallTiles = new CGSize (100, 150);
		public static CGSize MediumTiles = new CGSize (133, 185);
		public static CGSize LargeTiles = new CGSize (153, 205);
		public static CGSize CurrentSize;
		public static bool DarkTheme 
		{
			get {
				return TAB_BACKGROUND_COLOR == "#000000";
			}
			set {
				if (value) 
				{
					TAB_BACKGROUND_COLOR = "#000000";
					NAV_BAR_COLOR = "#3B5998";
					UITabBar.Appearance.BarTintColor = UIColor.Clear.FromHexString (NAV_BAR_COLOR, 1.0f);
					UITabBar.Appearance.TintColor = UIColor.Clear.FromHexString ("#FFFFFF", 1.0f);
				} else 
				{
					TAB_BACKGROUND_COLOR = "#FFFFFF";
					NAV_BAR_COLOR = "#3B5998";
					UITabBar.Appearance.BarTintColor = UIColor.Clear.FromHexString ("#FFFFFF", 1.0f);

				}
			}
		}

		public static string Method = "METHOD :";

		public static byte [] ConvertImageToByteArray (UIImage image)
		{
			byte [] byteArray;
			using (NSData imageData = image.AsPNG ()) {
				byteArray = new Byte [imageData.Length];
				System.Runtime.InteropServices.Marshal.Copy (imageData.Bytes, byteArray, 0, Convert.ToInt32 (imageData.Length));
			}
			return byteArray;
		}
		public static nint CurrentTileSize 
		{
			get { return _CurrentTileSize;}
			set
			{
				if (value == 0) 
				{
					
					CurrentSize = SmallTiles;

				} else if (value ==1) 
				{
					CurrentSize = MediumTiles;

				} else if (value ==2 ) 
				{
					CurrentSize = LargeTiles;

				}
				_CurrentTileSize = value;
				CurrentUser.tilesize = (int)value;
			}
		}

		public static PointF Rotate (this PointF pt)
		{
			return new PointF (pt.Y, pt.X);
		}
		public static User CurrentUser 
		{
			get { return _user; }
			set {_user = value;}
		}

		public static bool NoInternet { get; internal set; }

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
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>
			(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			HashSet<TKey> seenKeys = new HashSet<TKey> ();
			foreach (TSource element in source) {
				if (seenKeys.Add (keySelector (element))) {
					yield return element;
				}
			}
		}
		// resize the image (without trying to maintain aspect ratio)
		public static UIImage ResizeImage (UIImage sourceImage, float width, float height)
		{
			if (sourceImage == null)
				return UIImage.FromBundle ("1481507483_compose.png");
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
		public static UIImage ApplyLightEffect (this UIImage self)
		{
			var tintColor = UIColor.FromWhiteAlpha (1.0f, 0.3f);

			return ApplyBlur (self, blurRadius: 30, tintColor: tintColor, saturationDeltaFactor: 1.8f, maskImage: null);
		}

		public static UIImage ApplyExtraLightEffect (this UIImage self)
		{
			var tintColor = UIColor.FromWhiteAlpha (0.97f, 0.82f);

			return ApplyBlur (self, blurRadius: 20, tintColor: tintColor, saturationDeltaFactor: 1.8f, maskImage: null);
		}

		public static UIImage ApplyDarkEffect (this UIImage self)
		{
			var tintColor = UIColor.FromWhiteAlpha (0.11f, 0.73f);

			return ApplyBlur (self, blurRadius: 20, tintColor: tintColor, saturationDeltaFactor: 1.8f, maskImage: null);
		}
		
        public static void DeleteAllSubviews (this UIView self, UIScrollView view)
		{
			foreach (UIView subview in view.Subviews) {
				subview.RemoveFromSuperview ();
			}

		}

		public static UIImage ApplyTintEffect (this UIImage self, UIColor tintColor)
		{
			const float EffectColorAlpha = 0.6f;
			var effectColor = tintColor;
			nfloat alpha;
			var componentCount = tintColor.CGColor.NumberOfComponents;
			if (componentCount == 2) {
				nfloat white;
				if (tintColor.GetWhite (out white, out alpha))
					effectColor = UIColor.FromWhiteAlpha (white, EffectColorAlpha);
			} else {
				try {
					nfloat r, g, b;
					tintColor.GetRGBA (out r, out g, out b, out alpha);
					effectColor = UIColor.FromRGBA (r, g, b, EffectColorAlpha);
				} catch 
				{
				}
			}
			return ApplyBlur (self, blurRadius: 10, tintColor: effectColor, saturationDeltaFactor: -1, maskImage: null);
		}
		public static string GetDeviceId()
		{
			/*
			* The value of CurrentDevice.IdentifierForVendor changes if the app is removed and reinstalled.
			* Therefore some ID (the value of IdentifierForVendor) is stored to the keychain and
			* reused every time (even after a reinstall). See http://stackoverflow.com/a/22937460
			*/

			String id;

			String ServiceId = "KeyChainAccountStore";

			//use the bundle identifier of the app to identifie the value in the keychain
			String appId = NSBundle.MainBundle.InfoDictionary ["CFBundleIdentifier"].ToString ();

			//Try to read the id from the keychain
			var rec = new SecRecord (SecKind.GenericPassword) {
				Service = ServiceId,
				Account = appId,
			};

			SecStatusCode res;
			var match = SecKeyChain.QueryAsRecord (rec, out res);
			 
			//Store a new ID to the keychain
			if (match?.Generic == null)
			{
			    //Get the vendor ID (does change after a reinstall of the app)
			    var vendorId = UIKit.UIDevice.CurrentDevice.IdentifierForVendor.AsString ().Replace ("-", "");

			SecRecord record = new SecRecord (SecKind.GenericPassword) {
				Service = ServiceId,
				Account = appId,
				Generic = NSData.FromString (vendorId),
				Accessible = SecAccessible.Always
			};

			var statusCode = SecKeyChain.Add (record);
			 
			    if(statusCode != SecStatusCode.Success)
			        Debug.WriteLine("Could not save key to KeyChain: " + statusCode);
			 
			    id = vendorId;
			}
			else
			{   //Use ID from keychain
			    id = match.Generic.ToString();
			}
			 
			return id;
		}
		public  static UIImage ApplyBlur (UIImage image, float blurRadius, UIColor tintColor, float saturationDeltaFactor, UIImage maskImage)
		{
			if (image.Size.Width < 1 || image.Size.Height < 1) {
				Debug.WriteLine (@"*** error: invalid size: ({0} x {1}). Both dimensions must be >= 1: {2}", image.Size.Width, image.Size.Height, image);
				return null;
			}
			if (image.CGImage == null) {
				Debug.WriteLine (@"*** error: image must be backed by a CGImage: {0}", image);
				return null;
			}
			if (maskImage != null && maskImage.CGImage == null) {
				Debug.WriteLine (@"*** error: maskImage must be backed by a CGImage: {0}", maskImage);
				return null;
			}

			var imageRect = new CGRect (CGPoint.Empty, image.Size);
			var effectImage = image;

			bool hasBlur = blurRadius > float.Epsilon;
			bool hasSaturationChange = Math.Abs (saturationDeltaFactor - 1) > float.Epsilon;

			if (hasBlur || hasSaturationChange) {
				UIGraphics.BeginImageContextWithOptions (image.Size, false, UIScreen.MainScreen.Scale);
				var contextIn = UIGraphics.GetCurrentContext ();
				contextIn.ScaleCTM (1.0f, -1.0f);
				contextIn.TranslateCTM (0, -image.Size.Height);
				contextIn.DrawImage (imageRect, image.CGImage);
				var effectInContext = contextIn.AsBitmapContext () as CGBitmapContext;

				var effectInBuffer = new vImageBuffer () {
					Data = effectInContext.Data,
					Width = (int)effectInContext.Width,
					Height = (int)effectInContext.Height,
					BytesPerRow = (int)effectInContext.BytesPerRow
				};

				UIGraphics.BeginImageContextWithOptions (image.Size, false, UIScreen.MainScreen.Scale);
				var effectOutContext = UIGraphics.GetCurrentContext ().AsBitmapContext () as CGBitmapContext;
				var effectOutBuffer = new vImageBuffer () {
					Data = effectOutContext.Data,
					Width = (int)effectOutContext.Width,
					Height = (int)effectOutContext.Height,
					BytesPerRow = (int)effectOutContext.BytesPerRow
				};

				if (hasBlur) {
					var inputRadius = blurRadius * UIScreen.MainScreen.Scale;
					uint radius = (uint)(Math.Floor (inputRadius * 3 * Math.Sqrt (2 * Math.PI) / 4 + 0.5));
					if ((radius % 2) != 1)
						radius += 1;
					vImage.BoxConvolveARGB8888 (ref effectInBuffer, ref effectOutBuffer, IntPtr.Zero, 0, 0, radius, radius, Pixel8888.Zero, vImageFlags.EdgeExtend);
					vImage.BoxConvolveARGB8888 (ref effectOutBuffer, ref effectInBuffer, IntPtr.Zero, 0, 0, radius, radius, Pixel8888.Zero, vImageFlags.EdgeExtend);
					vImage.BoxConvolveARGB8888 (ref effectInBuffer, ref effectOutBuffer, IntPtr.Zero, 0, 0, radius, radius, Pixel8888.Zero, vImageFlags.EdgeExtend);
				}
				bool effectImageBuffersAreSwapped = false;
				if (hasSaturationChange) {
					var s = saturationDeltaFactor;
					var floatingPointSaturationMatrix = new float [] {
						0.0722f + 0.9278f * s,  0.0722f - 0.0722f * s,  0.0722f - 0.0722f * s,  0,
						0.7152f - 0.7152f * s,  0.7152f + 0.2848f * s,  0.7152f - 0.7152f * s,  0,
						0.2126f - 0.2126f * s,  0.2126f - 0.2126f * s,  0.2126f + 0.7873f * s,  0,
						0,                    0,                    0,  1,
					};
					const int divisor = 256;
					var saturationMatrix = new short [floatingPointSaturationMatrix.Length];
					for (int i = 0; i < saturationMatrix.Length; i++)
						saturationMatrix [i] = (short)Math.Round (floatingPointSaturationMatrix [i] * divisor);
					if (hasBlur) {
						vImage.MatrixMultiplyARGB8888 (ref effectOutBuffer, ref effectInBuffer, saturationMatrix, divisor, null, null, vImageFlags.NoFlags);
						effectImageBuffersAreSwapped = true;
					} else
						vImage.MatrixMultiplyARGB8888 (ref effectInBuffer, ref effectOutBuffer, saturationMatrix, divisor, null, null, vImageFlags.NoFlags);
				}
				if (!effectImageBuffersAreSwapped)
					effectImage = UIGraphics.GetImageFromCurrentImageContext ();
				UIGraphics.EndImageContext ();
				if (effectImageBuffersAreSwapped)
					effectImage = UIGraphics.GetImageFromCurrentImageContext ();
				UIGraphics.EndImageContext ();
			}

			// Setup up output context
			UIGraphics.BeginImageContextWithOptions (image.Size, false, UIScreen.MainScreen.Scale);
			var outputContext = UIGraphics.GetCurrentContext ();
			outputContext.ScaleCTM (1, -1);
			outputContext.TranslateCTM (0, -image.Size.Height);

			// Draw base image
			if (hasBlur) {
				outputContext.SaveState ();
				if (maskImage != null)
					outputContext.ClipToMask (imageRect, maskImage.CGImage);
				outputContext.DrawImage (imageRect, effectImage.CGImage);
				outputContext.RestoreState ();
			}

			if (tintColor != null) {
				outputContext.SaveState ();
				outputContext.SetFillColor (tintColor.CGColor);
				outputContext.FillRect (imageRect);
				outputContext.RestoreState ();
			}
			var outputImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return outputImage;
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
