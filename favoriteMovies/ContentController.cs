using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;

namespace FavoriteMovies
{
	public class ContentController : BaseController
	{
		public UIButton ContentButton { get; set; }
		public static UITableView table;
		List<string> tableItems = new List<string> ();

		MenuTableSource tableSource;

		public ContentController ()
		{
			ContentButton = new UIButton ();


		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			table = new UITableView (View.Bounds);
			table.AutoresizingMask = UIViewAutoresizing.All;
			tableItems = new List<string> () { "Login", "Connections", "Movie Lists", "Settings" };
			tableSource = new MenuTableSource (tableItems, this);
			table.Source = tableSource;
			table.Frame = new CGRect () { X = 80, Y = 70, Width = 280, Height = 180 };

			View.AddSubview (table);
		}


	}





}

	