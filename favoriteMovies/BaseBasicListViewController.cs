using System;
using Foundation;
using UIKit;

namespace FavoriteMovies
{
	public class BaseBasicListViewController: BaseController
	{
		protected UITableView tableView;

		protected UITableViewSource tableSource;

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			tableView = new UITableView (View.Bounds);
			tableView.AutoresizingMask = UIViewAutoresizing.All;
			tableView.AllowsSelectionDuringEditing = true;


		}


	}

}
