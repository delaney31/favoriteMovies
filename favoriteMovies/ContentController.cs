using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace FavoriteMovies
{
	partial class ContentController : BaseController
	{
		public UIButton ContentButton { get; set; }
		public ContentController ()
		{
			ContentButton = new UIButton ();
		}

		public ContentController (IntPtr handle) : base (handle)
		{
		}
	}
}

	