using System;
using Foundation;
using UIKit;

namespace FavoriteMovies
{
	public class BaseBasicListViewController: MovieFriendsBaseViewController
	{
		//public UITableView tableView;



		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			//tableView = new UITableView (View.Bounds);
			//tableView.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin;
			//tableView.AllowsSelectionDuringEditing = true;
			//tableView.Frame = View.Bounds;


		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);


		}

	}

}
