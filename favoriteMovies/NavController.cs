using System;
using System.Collections.Generic;
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

			//View.SendSubviewToBack (table);
			// Perform any additional setup after loading the view, typically from a nib.
		}


	}
	public class MenuTableSource : UITableViewSource
	{
		List<string> tableItems;
		string cellIdentifier = "TableCell";
		ContentController Owner;

		public MenuTableSource (List<string> items, ContentController owner)
		{
			tableItems = items;
			this.Owner = owner;

		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return tableItems.Count;
		}




		/// <summary>
		/// Called when a row is touched
		/// </summary>
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			//NavController.table.Hidden = true;
			//(UIApplication.SharedApplication.Delegate as AppDelegate).NavController.SidebarController.ToggleMenu ();
			var row = tableItems [indexPath.Row];
			(UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.NavController.PushViewController (new MovieListPickerViewController (null, false), true);


			tableView.DeselectRow (indexPath, true);
		}





		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			// request a recycled cell to save memory
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);


			var cellStyle = UITableViewCellStyle.Default;

			// if there are no cells to reuse, create a new one
			if (cell == null) {
				cell = new UITableViewCell (cellStyle, cellIdentifier);

			}
			cell.TextLabel.Text = tableItems [indexPath.Row];



			return cell;
		}
	}
}