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

		public ContentController ()
		{
			ContentButton = new UIButton ();


		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			View.AddSubview (table);
		}


	}





}

	