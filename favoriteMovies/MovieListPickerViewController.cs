using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using SQLite;
using UIKit;

namespace FavoriteMovies
{
	public class MovieListPickerViewController : UIViewController,IUIPopoverPresentationControllerDelegate, IUIAdaptivePresentationControllerDelegate
	{
		UITableView table;
		List<CustomList> tableItems = new List<CustomList> ();
		public MovieListPickerViewController ()
		{
			ModalPresentationStyle = UIModalPresentationStyle.Popover;
			PopoverPresentationController.Delegate = this;
			PreferredContentSize = new CoreGraphics.CGSize (300, 300);
		}
		[Export ("adaptivePresentationStyleForPresentationController:traitCollection:")]
		public UIModalPresentationStyle GetAdaptivePresentationStyle (UIPresentationController controller, UITraitCollection traitCollection)
		{
			return UIModalPresentationStyle.None;
		}


		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			table = new UITableView (new CGRect () { X = 0, Y = 0, Width = 320, Height = 450 });
			//table = new UITableView (View.Bounds);
			table.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			tableItems = GetMovieList ();
			table.Source = new TableSource (tableItems, this);
			table.Alpha = .5f;
			var label = new UILabel () { Frame = new CGRect () { X = 0, Y = 36, Width = 320, Height = 30 } };
			label.Alpha = .5f;
			label.Text = "Select a list or create one!";
			label.TextAlignment = UITextAlignment.Center;
			label.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, 15f);
			table.TableHeaderView = label;
			table.TableHeaderView.SizeToFit ();

			var listName = new UITextView () { Frame = new CGRect () { X = 0, Y = 160, Width = 320, Height = 30 } };
			//listName.BackgroundColor = UIColor.Red;

			View.AddSubview (table);
			View.AddSubview(listName);

		}
		List<CustomList> GetMovieList ()
		{

			List<CustomList> result = new List<CustomList> ();

			try {

				using (var db = new SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/
					//52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					var query = db.Table<CustomList> ();
					foreach (var list in query) {
						var item = new CustomList ();
						item.Id = list.Id;
						item.Name = list.Name;
						result.Add (item);
					}
					var addItem = new CustomList ();
					addItem.Name = "Create new list";
					addItem.Id = result.Count + 1;
					result.Add (addItem);
				}

				//favoriteViewController.CollectionView.ReloadData ();
			} catch (SQLiteException e) {
				Debug.WriteLine (e.Message);

				using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) {
					conn.CreateTable<Movie> ();
					conn.CreateTable<CustomList> ();
				}
				using (var db = new SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/
					//52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					var query = db.Table<CustomList> ();
					foreach (var list in query) {
						var item = new CustomList ();
						item.Id = list.Id;
						item.Name = list.Name;
						result.Add (item);
					}
					var addItem = new CustomList ();
					addItem.Name = "Add new list";
					addItem.Id = result.Count + 1;
					result.Add (addItem);
				}

			}

			return result;
		}
	}
	public class ModalViewController : UIViewController
	{
		public SizeF OriginalViewSize { get; private set; }

		void Initialize ()
		{

		}

		public override void ViewDidLoad ()
		{
			OriginalViewSize = (System.Drawing.SizeF)View.Bounds.Size;
			base.ViewDidLoad ();
			ModalPresentationStyle = UIModalPresentationStyle.CurrentContext;
			View.Alpha = .2f;
		}

		public ModalViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		public ModalViewController (string nibName, NSBundle bundle) : base (nibName, bundle)
		{
			Initialize ();
		}

		public ModalViewController () : base ()
		{
			Initialize ();
		}
	}

	public static class ModalViewControllerExtensions
	{
		public static void PresentModalViewController (this UIViewController parent, ModalViewController target)
		{
			parent.PresentViewController (target, true, null);

			target.View.Superview.AutoresizingMask = UIViewAutoresizing.FlexibleMargins;
			target.View.Superview.Frame = new RectangleF (PointF.Empty, target.OriginalViewSize);
			target.View.Superview.Center = UIScreen.MainScreen.Bounds.Center ().Rotate ();
		}
	}

	public class InlineEditTableViewCell : UITableViewCell
	{

		public UITextField propertyTextField = new UITextField ();
		string cellIdentifier = "TableCell";

		[Export ("initWithStyle:reuseIdentifier:")]
    	public InlineEditTableViewCell (UITableViewCellStyle style, string cellIdentifier):base (UITableViewCellStyle.Default, cellIdentifier)
		{
			

		}
		public void UpdateCell (string name)
		{
			propertyTextField.Text = name;
		}


	}
	public class TableSource : UITableViewSource
	{
		List<CustomList> tableItems;
		string cellIdentifier = "TableCell";
		MovieListPickerViewController owner;

		public TableSource (List<CustomList> items, MovieListPickerViewController owner)
		{
			tableItems = items;
			this.owner = owner;

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
			//UIAlertController okAlertController = UIAlertController.Create ("Row Selected", tableItems [indexPath.Row].Name, UIAlertControllerStyle.Alert);
			//okAlertController.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
			//owner.PresentViewController (okAlertController, true, null);
			InlineEditTableViewCell cell = (InlineEditTableViewCell)tableView.CellAt (indexPath);
			//cell.propertyTextField.Text =tableItems [indexPath.Row].Name;
			cell.propertyTextField.BecomeFirstResponder ();
			//tableView.DeselectRow (indexPath, true);
		}

		/// <summary>
		/// Called when the DetailDisclosureButton is touched.
		/// Does nothing if DetailDisclosureButton isn't in the cell
		/// </summary>
		public override void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath)
		{
			UIAlertController okAlertController = UIAlertController.Create ("DetailDisclosureButton Touched", tableItems [indexPath.Row].Name, UIAlertControllerStyle.Alert);
			okAlertController.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
			owner.PresentViewController (okAlertController, true, null);

			tableView.DeselectRow (indexPath, true);
		}

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			// request a recycled cell to save memory
			InlineEditTableViewCell cell =(InlineEditTableViewCell) tableView.DequeueReusableCell (cellIdentifier);


			// UNCOMMENT one of these to use that style
			var cellStyle = UITableViewCellStyle.Default;

			// if there are no cells to reuse, create a new one
			if (cell == null) {
				cell = new InlineEditTableViewCell (cellStyle, cellIdentifier);
				cell.UpdateCell (tableItems [indexPath.Row].Name);
			}



			// UNCOMMENT one of these to see that accessory
			//			cell.Accessory = UITableViewCellAccessory.Checkmark;
			//			cell.Accessory = UITableViewCellAccessory.DetailButton;
			//			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			//			cell.Accessory = UITableViewCellAccessory.DetailDisclosureButton;  // implement AccessoryButtonTapped
			cell.Accessory = UITableViewCellAccessory.Checkmark; // to clear the accessory

			cell.TextLabel.Text = tableItems [indexPath.Row].Name;
			cell.propertyTextField.Text = tableItems [indexPath.Row].Name;
			// Default style doesn't support Subtitle
			//if (cellStyle == UITableViewCellStyle.Subtitle
			//   || cellStyle == UITableViewCellStyle.Value1
			//   || cellStyle == UITableViewCellStyle.Value2) {
			//	cell.DetailTextLabel.Text = tableItems [indexPath.Row].SubHeading;
			//}

			// Value2 style doesn't support an image
			//if (cellStyle != UITableViewCellStyle.Value2)
			//	cell.ImageView.Image = UIImage.FromFile ("Images/" + tableItems [indexPath.Row].ImageName);

			return cell;
		}
	}
}
