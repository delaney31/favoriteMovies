using System;
using System.Collections.Generic;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using SidebarNavigation;
using UIKit;


namespace FavoriteMovies

{
	public partial class NavController : UINavigationController
	{
		// the sidebar controller for the app
		public SidebarController SidebarController { get; set; }
		public ContentController ContentController { get; set; }
		protected static SearchResultsViewController searchResultsController;
		protected UISearchController searchController;
		protected const float BackGroundColorAlpha = 1.0f;
		//public NavController () : base (typeof (CRNavigationBar), typeof (UIToolbar))

		//{
			
			
		//}

		public NavController () : base ((string)null, null)
		{
		}
		public override bool ShouldAutorotate ()
		{
			return base.ShouldAutorotate ();

		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.Portrait;
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();


			//var navBar = (UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.NavController;




		}


	}
	[Register ("CRNavigationBar")]
	public class CRNavigationBar : UINavigationBar
	{
		const float kDefaultColorOpacity = 0.5f;
		const float kSpaceToCover = 20.0f;
		const float BackGroundColorAlpha = 1.0f;

		public CALayer ColorLayer {
			get;
			set;
		}

		public CRNavigationBar (IntPtr handle) : base (handle)
		{
			Console.WriteLine ("Loading navbars");
			SetBarTintColor (UIColor.Clear.FromHexString (UIColorExtensions.NAV_BAR_COLOR, BackGroundColorAlpha));
		}
		public CRNavigationBar ()
		{
			Console.WriteLine ("Loading navbars");
		}

		public void SetBarTintColor (UIColor barTintColor)
		{
			base.BarTintColor = barTintColor;
			if (ColorLayer == null) {
				ColorLayer = new CALayer ();
				ColorLayer.Opacity = kDefaultColorOpacity;
				Layer.AddSublayer (ColorLayer);
			}
			ColorLayer.BackgroundColor = barTintColor.CGColor;
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			if (ColorLayer != null) {
				ColorLayer.Frame = new System.Drawing.RectangleF (0, 0 - kSpaceToCover, (float)Bounds.Width, (float)(Bounds.Height + kSpaceToCover));
				Layer.InsertSublayer (ColorLayer, 1);
			}
		}
	}
}