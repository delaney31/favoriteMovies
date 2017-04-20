using System;
using System.Text;
using AVFoundation;
using AVKit;
using Foundation;
using UIKit;


namespace FavoriteMovies
{
	public class MFVideoViewController:UIViewController
	{
		string Url= "";
		public MFVideoViewController ()
		{
		}

		public MFVideoViewController (string v)
		{
			this.Url = v;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			//var webView = new UIWebView () {
			//	AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
			//	BackgroundColor = UIColor.Clear.FromHexString (ColorExtensions.TAB_BACKGROUND_COLOR, 1.0f)
			//};
			//var viewController = new UIViewController ();

			//var sb = new StringBuilder ();
			//sb.Append ("<html><head>");
			//sb.Append ("<style>" +
			//           "body{font-family:Helvetica;font-size:10pt}" +
			//		  "</style>");
			//sb.Append ("</head>");
			//sb.Append ("<body>");
			//sb.Append ("<iframe width=\"300\" height=\"250\" src=\"" + this.Url+ "\" frameborder=\"0\" allowfullscreen></iframe>");
			//sb.Append ("<h4>" + movieDetail.name + "</h4>");
			//sb.Append ("<p>" + movieDetail.Overview + "</p>");
			//sb.Append ("</body></html>");

			//webView.Frame = new CGRect (0, 0, (float)this.View.Frame.Width, (float)scrollView.Frame.Height);
			//webView.LoadHtmlString (sb.ToString (), null);
			//viewController.View.Add (webView);
			////this.View.AddSubview (webView);
			//NavigationController.PushViewController (viewController, true);
			//AVPlayer avp;
			//AVPlayerViewController avpvc;

			//var url = NSUrl.FromString ("https://player.vimeo.com/external/200227894.m3u8?s=966859adf57990e378a4c67d89a61bfbc437fda5");
			//var url = NSUrl.FromString (this.Url);
			//avp = new AVPlayer (url);
			//avpvc = new AVPlayerViewController ();
			//avpvc.Player = avp;
			//AddChildViewController (avpvc);
			//View.AddSubview (avpvc.View);
			//avpvc.View.Frame = View.Frame;
			//avpvc.ShowsPlaybackControls = true;
			//avp.Play ();
		}
	}
}
