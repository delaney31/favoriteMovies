using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using FavoriteMoviesPCL;
using System.Collections.Generic;

namespace FavoriteMovies
{
	public class MenuController : BaseController
	{
		public MenuController ()
		{
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var contentController = new ContentController ();
			contentController.ContentButton.TouchUpInside += (o, e) => {
				if (NavController.NavigationController.TopViewController as ContentController == null)
					NavController.NavigationController.PushViewController (contentController, false);
				SidebarController.CloseMenu ();
			};


		}
	}

}
