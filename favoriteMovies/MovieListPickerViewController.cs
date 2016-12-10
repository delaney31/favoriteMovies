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
	public class MovieListPickerViewController : BaseController
	{
		Movie movieDetail;
		UITableView table;
		List<CustomList> tableItems = new List<CustomList> ();
		UIBarButtonItem edit, done, add;
		TableSource tableSource;
		public bool fromAddList;
		public MovieListPickerViewController (Movie movieDetail, bool fromAddList)
		{
			this.movieDetail = movieDetail;
			this.fromAddList = fromAddList;

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
		internal void updateMovieOrder (CustomList item, int row)
		{
			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					item.Order = row;
					db.InsertOrReplace (item, typeof (CustomList));

				}

			} catch (SQLite.SQLiteException s) {
				Debug.Write (s.Message);
			}
		}
		public static  void DeleteAll ()
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

		public static void DeleteAll (int? CustomId, int id)
		{
			
			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					db.Query<Movie> ("DELETE FROM [Movie] WHERE [CustomListID] = " + CustomId + " AND [Id] = " + id);


				}
			} catch (SQLite.SQLiteException e) {
				//first time in no favorites yet.
				Debug.Write (e.Message);
			}

		}

		public static void DeleteCustomList (int? CustomId)
		{

			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					db.Query<Movie> ("DELETE FROM [CustomList] WHERE [ID] = " + CustomId);


				}
			} catch (SQLite.SQLiteException e) {
				//first time in no favorites yet.
				Debug.Write (e.Message);
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

					if (upDateMovieDetail) {
						DeleteAll (listItem.Id, movieDetail.Id);
						DeleteAll ();
					} else
						DeleteAll ();

					for (var list=0; list < tableItems.Count; list++) 
					{
						
						if (tableItems[list].Name!="add new")
						{
							db.Insert (tableItems [list],typeof(CustomList));
						}

						if (upDateMovieDetail && (tableItems [list].Name == listItem.Name)) {

							//get id of last inserted row
							string sql = "select last_insert_rowid()";
							var scalarValue = db.ExecuteScalar<string> (sql);
							int value = scalarValue == null ? 0 : Convert.ToInt32 (scalarValue);

							if (listItem.Id != null)
								movieDetail.CustomListID = listItem.Id;
							else
								movieDetail.CustomListID = value;
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
					if (upDateMovieDetail) 
					{
						DeleteAll (listItem.Id, movieDetail.Id);
						DeleteAll ();
					} else
						DeleteAll ();

					for (var list = 0; list < tableItems.Count; list++) {
						
						if (tableItems [list].Name != "add new") 
						{
							 db.Insert (tableItems [list], typeof (CustomList));
						}

						if (upDateMovieDetail && (tableItems [list].Name == listItem.Name)) {

							string sql = "select last_insert_rowid()";
							var scalarValue = db.ExecuteScalar<string> (sql);
							int value = scalarValue == null ? 0 : Convert.ToInt32 (scalarValue);

							if (listItem.Id != null)
								movieDetail.CustomListID = listItem.Id;
							else
								movieDetail.CustomListID = value;
							db.Insert (movieDetail, typeof (Movie));
						}
					}

				}
			}


		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			table = new UITableView (View.Bounds);
			table.AutoresizingMask = UIViewAutoresizing.All;
			tableItems = GetMovieList ();
			tableSource = new TableSource (tableItems, this);
			table.Source = tableSource;
			table.AllowsSelectionDuringEditing = true;
			NavigationItem.Title = "Movie List";

			if (!fromAddList) {
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
					SidebarController.Disabled = true;
					MainViewController.NewCustomListToRefresh = 0;
				});
				NavigationItem.RightBarButtonItem = edit;
			} else 
			{
				add = new UIBarButtonItem (UIBarButtonSystemItem.Add, (s, e) => 
				{
					//Create Alert
					var textInputAlertController = UIAlertController.Create ("Create Movie List", "List Name", UIAlertControllerStyle.Alert);

					//Add Text Input
					textInputAlertController.AddTextField (textField => { });

					//Add Actions
					var cancelAction = UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, alertAction => {
						Console.WriteLine ("Cancel was Pressed");
					});
					var okayAction = UIAlertAction.Create ("Okay", UIAlertActionStyle.Default, alertAction => {
						Console.WriteLine ("The user entered '{0}'", textInputAlertController.TextFields [0].Text);
						if (tableSource.ValueUnique (textInputAlertController.TextFields [0].Text)) {
							tableSource.ArrangeCustomList (false);
							var listItem = new CustomList ();
							listItem.Order = 0;
							listItem.Name = textInputAlertController.TextFields [0].Text;
							tableItems.Insert (0, listItem);
							table.EndUpdates (); // applies the changes
							table.ReloadData ();
							tableSource.ArrangeCustomList (true);
							UpdateCustomAndMovieList (tableItems [0], true);
							MainViewController.NewCustomListToRefresh = 0;
							NavigationController.PopToRootViewController (true);
						} else {
							new UIAlertView ("Duplicate!"
							, "List already exist", null, "OK", null).Show ();
						}
					});

					textInputAlertController.AddAction (cancelAction);
					textInputAlertController.AddAction (okayAction);

					//Present Alert
					PresentViewController (textInputAlertController, true, null);



				});

				NavigationItem.RightBarButtonItem = add;
			}

			Add (table);

		}
		List<CustomList> GetMovieList ()
		{

			List<CustomList> result = new List<CustomList> ();

			try {

				using (var db = new SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/
					//52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					var query = db.Query<CustomList> ("SELECT * FROM [CustomList] ORDER BY [Order]");
				

					foreach (var list in query) {
						var item = new CustomList ();
						item.Id = list.Id;
						item.Name = list.Name;
						result.Add (item);
					}
				}


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
					var query = db.Query<CustomList> ("SELECT * FROM [CustomList] ORDER BY [Order]");
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


	public class TableSource : UITableViewSource
	{
		List<CustomList> tableItems;
		string cellIdentifier = "TableCell";
		MovieListPickerViewController Owner;

		public TableSource (List<CustomList> items, MovieListPickerViewController owner)
		{
			tableItems = items;
			this.Owner = owner;

		}
		public void WillBeginTableEditing (UITableView tableView)
		{
			tableView.BeginUpdates ();
			// insert the 'ADD NEW' row at the end of table display
			tableView.InsertRows (new NSIndexPath [] {
			NSIndexPath.FromRowSection (tableView.NumberOfRowsInSection (0), 0)
		}, UITableViewRowAnimation.Fade);
			// create a new item and add it to our underlying data (it is not intended to be permanent)
			tableItems.Add (new CustomList () { Name = "add new" });
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
			if (tableView.NumberOfRowsInSection (0) > 0) 
			{
				Owner.UpdateCustomAndMovieList (tableItems [(int)tableView.NumberOfRowsInSection (0)-1], false);
			} else
				MovieListPickerViewController.DeleteAll ();
		}

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
			//---- get a reference to the item
			var item = tableItems [sourceIndexPath.Row];
			var deleteAt = sourceIndexPath.Row;
			var insertAt = destinationIndexPath.Row;

			//---- if we're moving within the same section, and we're inserting it before
			if ((sourceIndexPath.Section == destinationIndexPath.Section) && (destinationIndexPath.Row < sourceIndexPath.Row)) 
			{
				//---- add one to where we delete, because we're increasing the index by inserting
				deleteAt += 1;
			} else {
				insertAt += 1;
			}

			//---- copy the item to the new location
			item.Order = destinationIndexPath.Row;
			tableItems.Insert (destinationIndexPath.Row, item);

			//---- remove from the old
			tableItems.RemoveAt (deleteAt);

			//Owner.updateMovieOrder (item,destinationIndexPath.Row);
			ArrangeCustomList (true);
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
			if (tableView.Editing )
			{
				if (tableItems [indexPath.Row].Name == "add new") 
				{
					//Create Alert
					var textInputAlertController = UIAlertController.Create ("Create Movie List", "List Name", UIAlertControllerStyle.Alert);

					//Add Text Input
					textInputAlertController.AddTextField (textField => { });

					//Add Actions
					var cancelAction = UIAlertAction.Create ("Cancel", UIAlertActionStyle.Cancel, alertAction => {
						Console.WriteLine ("Cancel was Pressed");
					});
					var okayAction = UIAlertAction.Create ("Okay", UIAlertActionStyle.Default, alertAction => {
					Console.WriteLine ("The user entered '{0}'", textInputAlertController.TextFields [0].Text);
					if (ValueUnique (textInputAlertController.TextFields [0].Text)) {
						ArrangeCustomList (false);
						var listItem = new CustomList ();
						listItem.Order = 0;
						listItem.Name = textInputAlertController.TextFields [0].Text;
						tableItems.Insert (0, listItem);
						tableView.EndUpdates (); // applies the changes
						tableView.ReloadData ();
						ArrangeCustomList (true);

						Owner.UpdateCustomAndMovieList (tableItems [indexPath.Row], Owner.fromAddList);
						} else 
						{
							new UIAlertView ("Duplicate!"
							, "List already exist", null, "OK", null).Show ();
						}
					});

					textInputAlertController.AddAction (cancelAction);
					textInputAlertController.AddAction (okayAction);

					//Present Alert
					Owner.PresentViewController (textInputAlertController, true, null);
				}
			} else 
			{
				ArrangeCustomList (true);
				Owner.UpdateCustomAndMovieList (tableItems [indexPath.Row], Owner.fromAddList);
				Owner.NavigationController.PopToRootViewController (true);
				MainViewController.NewCustomListToRefresh = 0;

			}
			tableView.DeselectRow (indexPath, true);
		}

		public void ArrangeCustomList (bool StartAtZero)
		{

			for (var list = StartAtZero ? 0 : 1; list < tableItems.Count; list++) 
			{
				int num = StartAtZero ? 0 : 1;
				if (tableItems [list - num].Name != "add new" && tableItems [list - num].Order == 0)
					tableItems [list - num].Order = list;
			}
		}

		public bool ValueUnique (string text)
		{
			foreach (var item in tableItems) 
			{
				if (item.Name == text) 
				{
					return false;

				}
			}
			return true;
		}



		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			// request a recycled cell to save memory
			UITableViewCell cell =tableView.DequeueReusableCell (cellIdentifier);


			var cellStyle = UITableViewCellStyle.Default;

			// if there are no cells to reuse, create a new one
			if (cell == null) {
				cell = new UITableViewCell (cellStyle, cellIdentifier);

			}
			cell.TextLabel.Text = tableItems [indexPath.Row].Name;
			cell.TextLabel.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE);


			return cell;
		}
	}
}
