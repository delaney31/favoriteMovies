using System;
using System.Collections.Generic;
using Foundation;
using SidebarNavigation;
using UIKit;


namespace FavoriteMovies

{
	public partial class NavController : UINavigationController
	{
		// the sidebar controller for the app
		public SidebarController SidebarController { get; set; }
		UITableView table;
		List<string> tableItems = new List<string> ();

		MenuTableSource tableSource;
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
			table = new UITableView (View.Bounds);
			table.AutoresizingMask = UIViewAutoresizing.All;
			tableItems = new List<string> () { "Login", "Connections", "Flic Lists", "Settings" };
			tableSource = new MenuTableSource (tableItems, this);
			table.Source = tableSource;
			View.AddSubview(table);
			View.SendSubviewToBack (table);
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public void ShowMenu ()
		{
			Add (table);
		}
	}
	public class MenuTableSource : UITableViewSource
	{
		List<string> tableItems;
		string cellIdentifier = "TableCell";
		NavController Owner;

		public MenuTableSource (List<string> items, NavController owner)
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

			var row = tableItems [indexPath.Row];
			((UINavigationController)(UIApplication.SharedApplication.Delegate as AppDelegate).Window.RootViewController).PushViewController (new MovieListPickerViewController (null, true), true);


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