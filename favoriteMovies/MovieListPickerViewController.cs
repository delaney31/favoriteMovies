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
	public class MovieListPickerViewController : UIViewController//,IUIPopoverPresentationControllerDelegate, IUIAdaptivePresentationControllerDelegate
	{
		Movie movieDetail;
		UITableView table;
		List<CustomList> tableItems = new List<CustomList> ();
		UIBarButtonItem edit, done;
		TableSource tableSource;
		public MovieListPickerViewController (Movie movieDetail)
		{
			this.movieDetail = movieDetail;
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			//UpdateCustomList ();
		}

		public void UpdateCustomList ()
		{
			try {

				using (var db = new SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/
					//52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					//var query = db.Table<CustomList> ();
					DeleteAll();
					foreach (var list in tableItems) 
					{

						if (list.Name != null) {
							db.Insert (list, typeof (CustomList));
						}
					}
				}

			} catch (SQLiteException e) {
				Debug.WriteLine (e.Message);

				using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) {
					conn.CreateTable<Movie> ();
					conn.CreateTable<CustomList> (CreateFlags.ImplicitPK | CreateFlags.AutoIncPK);
				}
				DeleteAll ();
				using (var db = new SQLiteConnection (MovieService.Database)) {
					foreach (var list in tableItems) {
						if (list.Name != null) 
						{
							db.Insert (list, typeof (CustomList));
						}
					}
				}
			}

		}

		void DeleteAll ()
		{
			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);

					db.Query<Movie> ("DELETE FROM [CustomList]");

				}
			} catch (SQLite.SQLiteException) {
				//first time in no favorites yet.
			}
		}

		void DeleteAll (int Id)
		{
			
			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					db.Query<Movie> ("DELETE FROM [Movie] WHERE [id] = " + Id);
					db.Query<Movie> ("DELETE FROM [CustomList]");

				}
			} catch (SQLite.SQLiteException) {
				//first time in no favorites yet.
			}


		}
		public void UpdateCustomAndMovieList (CustomList listItem, bool upDateMovieDetail)
		{

			try {

				using (var db = new SQLiteConnection (MovieService.Database)) 
				{
					// there is a sqllite bug here https://forums.xamarin.com/discussion/
					//52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					//var query = db.Table<CustomList> ();
					DeleteAll (movieDetail.Id);

					for (var list=0; list < tableItems.Count; list++) 
					{

						if (tableItems[list].Name!="add new")
						{
							 db.Insert (tableItems [list]);
						}

						if (upDateMovieDetail && (tableItems [list].Name == listItem.Name)) 
						{
							movieDetail.CustomListID = listItem.Id;
							db.Insert (movieDetail, typeof (Movie));
						}
					 }

				}

			} catch (SQLiteException e) {
				Debug.WriteLine (e.Message);

				using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) 
				{
					conn.CreateTable<Movie> ();
					conn.CreateTable<CustomList>(CreateFlags.ImplicitPK | CreateFlags.AutoIncPK);
				}
				;
				using (var db = new SQLiteConnection (MovieService.Database)) 
				{
					DeleteAll ();
					for (var list = 0; list < tableItems.Count; list++) {

						if (tableItems [list].Name != "add new") 
						{
							db.Insert (tableItems [list]);
						}

						if (upDateMovieDetail && (tableItems [list].Name == listItem.Name)) {
							movieDetail.CustomListID = listItem.Id;
							db.Insert (movieDetail, typeof (Movie));
						}
					}
				}
			}


		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			//table = new UITableView (new CGRect () { X = 0, Y = 50, Width = 320, Height = 350 });
			table = new UITableView (View.Bounds);
			//table.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			tableItems = GetMovieList ();
			tableSource = new TableSource (tableItems, this);
			table.Source = tableSource;
			table.AllowsSelectionDuringEditing = true;

			done = new UIBarButtonItem (UIBarButtonSystemItem.Done, (s, e) => {
				table.SetEditing (false, true);
				NavigationItem.RightBarButtonItem = edit;
				tableSource.DidFinishTableEditing (table);
			});
			edit = new UIBarButtonItem (UIBarButtonSystemItem.Edit, (s, e) => {
				if (table.Editing)
					table.SetEditing (false, true); // if we've half-swiped a row
				tableSource.WillBeginTableEditing (table);
				table.SetEditing (true, true);
				NavigationItem.LeftBarButtonItem = null;
				NavigationItem.RightBarButtonItem = done;
			});

			NavigationItem.RightBarButtonItem = edit;

			View.AddSubview (table);

		}
		List<CustomList> GetMovieList ()
		{

			List<CustomList> result = new List<CustomList> ();

			try {

				using (var db = new SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/
					//52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					//var query = db.Query<CustomList> ("SELECT * FROM CUSTOMLIST");
					var query = db.Table<CustomList> ();
					//var addItem = new CustomList ();
					//result.Add (addItem);
				

					foreach (var list in query) {
						var item = new CustomList ();
						item.Id = list.Id;
						item.Name = list.Name;
						result.Add (item);
					}
				}

				//favoriteViewController.CollectionView.ReloadData ();
			} catch (SQLiteException e) {
				Debug.WriteLine (e.Message);

				using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) {
					conn.CreateTable<Movie> ();
					conn.CreateTable<CustomList>(CreateFlags.ImplicitPK | CreateFlags.AutoIncPK);
				}
				using (var db = new SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/
					//52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
				//	var query = db.Query<CustomList> ("SELECT * FROM CUSTOMLIST");
					var query = db.Table<CustomList> ();
					foreach (var list in query) {
						var item = new CustomList ();
						item.Id = list.Id;
						item.Name = list.Name;
						result.Add (item);
					}

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
		public void WillBeginTableEditing (UITableView tableView)
		{
			tableView.BeginUpdates ();
			// insert the 'ADD NEW' row at the end of table display
			tableView.InsertRows (new NSIndexPath [] {
			NSIndexPath.FromRowSection (tableView.NumberOfRowsInSection (0), 0)
		}, UITableViewRowAnimation.Fade);
			// create a new item and add it to our underlying data (it is not intended to be permanent)
			tableItems.Add (new CustomList () { Name = "add new"});
			tableView.EndUpdates (); // applies the changes
		}
		public void DidFinishTableEditing (UITableView tableView)
		{
			tableView.BeginUpdates ();
			// remove our 'ADD NEW' row from the underlying data
			tableItems.RemoveAt ((int)tableView.NumberOfRowsInSection (0) - 1); // zero based :)
																				// remove the row from the table display
			tableView.DeleteRows (new NSIndexPath [] { NSIndexPath.FromRowSection (tableView.NumberOfRowsInSection (0) - 1, 0) }, UITableViewRowAnimation.Fade);
			tableView.EndUpdates (); // applies the changes
			if(tableView.NumberOfRowsInSection (0) - 1 >0)
			   owner.UpdateCustomAndMovieList (tableItems[(int)tableView.NumberOfRowsInSection (0) - 1], false);
			else
				owner.UpdateCustomAndMovieList (new CustomList () { Name = "add new" }, false);
		}
		//public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
		//{
		//	if (tableView.Editing) {
		//		if (indexPath.Row == tableView.NumberOfRowsInSection (0) - 1)
		//			return UITableViewCellEditingStyle.Insert;
		//		else
		//			return UITableViewCellEditingStyle.Delete;
		//	} else // not in editing mode, enable swipe-to-delete for all rows
		//		return UITableViewCellEditingStyle.Delete;
		//}
		public override NSIndexPath CustomizeMoveTarget (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath proposedIndexPath)
		{
			var numRows = tableView.NumberOfRowsInSection (0) - 1; // less the (add new) one
			if (proposedIndexPath.Row >= numRows)
				return NSIndexPath.FromRowSection (numRows - 1, 0);
			else
				return proposedIndexPath;
		}
		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			return indexPath.Row < tableView.NumberOfRowsInSection (0) - 1;
		}
		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
		{
			switch (editingStyle) {
			case UITableViewCellEditingStyle.Delete:
				// remove the item from the underlying data source
				tableItems.RemoveAt (indexPath.Row);
				// delete the row from the table
				tableView.DeleteRows (new NSIndexPath [] { indexPath }, UITableViewRowAnimation.Fade);
				break;
			case UITableViewCellEditingStyle.None:
				Console.WriteLine ("CommitEditingStyle:None called");
				break;
			}
		}
		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true; // return false if you wish to disable editing for a specific indexPath or for all rows
		}
		public override string TitleForDeleteConfirmation (UITableView tableView, NSIndexPath indexPath)
		{   // Optional - default text is 'Delete'
			return "Delete (" + tableItems [indexPath.Row].Name + ")";
		}
		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return tableItems.Count;
		}
		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			var item = tableItems [sourceIndexPath.Row];
			var deleteAt = sourceIndexPath.Row;
			var insertAt = destinationIndexPath.Row;

			// are we inserting 
			if (destinationIndexPath.Row < sourceIndexPath.Row) {
				// add one to where we delete, because we're increasing the index by inserting
				deleteAt += 1;
			} else {
				// add one to where we insert, because we haven't deleted the original yet
				insertAt += 1;
			}
			tableItems.Insert (insertAt, item);
			tableItems.RemoveAt (deleteAt);
		}
		public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
		{
			if (tableView.Editing) {
				if (indexPath.Row == tableView.NumberOfRowsInSection (0) - 1)
					return UITableViewCellEditingStyle.Insert;
				else
					return UITableViewCellEditingStyle.Delete;
			} else // not in editing mode, enable swipe-to-delete for all rows
				return UITableViewCellEditingStyle.Delete;
		}


		/// <summary>
		/// Called when a row is touched
		/// </summary>
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (tableView.Editing) { 
				//Create Alert
				var textInputAlertController = UIAlertController.Create ("Create Movie List", "List Name", UIAlertControllerStyle.Alert);

				//Add Text Input
				textInputAlertController.AddTextField (textField => {});

				//Add Actions
				var cancelAction = UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, alertAction => {
					Console.WriteLine ("Cancel was Pressed");
				});
				var okayAction = UIAlertAction.Create ("Okay", UIAlertActionStyle.Default, alertAction => {
					Console.WriteLine ("The user entered '{0}'", textInputAlertController.TextFields [0].Text);
					var listItem = new CustomList ();
					listItem.Name = textInputAlertController.TextFields [0].Text;
					tableItems.Insert (0,listItem);
					tableView.EndUpdates (); // applies the changes
					tableView.ReloadData ();
					owner.UpdateCustomAndMovieList (tableItems [indexPath.Row], true);
				});

				textInputAlertController.AddAction (cancelAction);
				textInputAlertController.AddAction (okayAction);

				//Present Alert
				owner.PresentViewController (textInputAlertController, true, null);
			} else 
			{
				owner.UpdateCustomAndMovieList (tableItems [indexPath.Row], true);
				owner.NavigationController.PopViewController (true);
			}
			tableView.DeselectRow (indexPath, true);
		}

		/// <summary>
		/// Called when the DetailDisclosureButton is touched.
		/// Does nothing if DetailDisclosureButton isn't in the cell
		/// </summary>
		//public override void AccessoryButtonTapped (UITableView tableView, NSIndexPath indexPath)
		//{
		////	UIAlertController okAlertController = UIAlertController.Create ("DetailDisclosureButton Touched", tableItems [indexPath.Row].Name, UIAlertControllerStyle.Alert);
		////	okAlertController.AddAction (UIAlertAction.Create ("OK", UIAlertActionStyle.Default, null));
		////	owner.PresentViewController (okAlertController, true, null);

		//	tableView.DeselectRow (indexPath, true);


		//}

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			// request a recycled cell to save memory
			UITableViewCell cell =tableView.DequeueReusableCell (cellIdentifier);


			// UNCOMMENT one of these to use that style
			var cellStyle = UITableViewCellStyle.Default;

			// if there are no cells to reuse, create a new one
			if (cell == null) {
				cell = new UITableViewCell (cellStyle, cellIdentifier);

			}

			if (indexPath.Row == 0) 
			{
				var accessButt = new UIButton (UIButtonType.ContactAdd);
				accessButt.UserInteractionEnabled = false;
				//cell.AccessoryView = accessButt;
			}
			cell.TextLabel.Text = tableItems [indexPath.Row].Name;

			return cell;
		}
	}
}
