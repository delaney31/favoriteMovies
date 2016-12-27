using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FavoriteMovies;
using FavoriteMoviesPCL;
using Foundation;
using SQLite;
using UIKit;

namespace FavoriteMovies
{
	public class BaseListViewController : BaseController
	{
		protected List<ICustomList> tableItems = new List<ICustomList> ();
		Movie movieDetail;
		public bool fromAddList;
		protected UITableView table;
		protected UIBarButtonItem edit, done, add;
		protected TableSource<ICustomList> tableSource;
		public BaseListViewController (Movie movieDetail, bool fromAddList)
		{
			this.movieDetail = movieDetail;
			this.fromAddList = fromAddList;
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			tableItems = GetMovieList ();
			table = new UITableView (View.Bounds);
			table.AutoresizingMask = UIViewAutoresizing.All;
			tableSource = new TableSource<ICustomList>(tableItems, this);
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
			} else {
				add = new UIBarButtonItem (UIBarButtonSystemItem.Add, (s, e) => {
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
							listItem.order = 0;
							listItem.name = textInputAlertController.TextFields [0].Text;
							tableItems.Insert (0, listItem);
							table.EndUpdates (); // applies the changes
							table.ReloadData ();
							tableSource.ArrangeCustomList (true);
							UpdateCustomAndMovieList (tableItems [0].id, true, tableItems);
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


		public void UpdateCustomAndMovieList (int? Id, bool upDateMovieDetail, List<ICustomList> tableItems)
		{
			var task = Task.Run (async () => 
			{
				try {

					using (var db = new SQLiteConnection (MovieService.Database)) {
						// there is a sqllite bug here https://forums.xamarin.com/discussion/
						//52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
						//var query = db.Table<CustomList> ();

						if (movieDetail != null)
							DeleteMovie (movieDetail.id);

						//DeleteAll ();
						//if (Id != null)
						//	DeleteAll (Id);
						//else


						for (var list = 0; list < tableItems.Count; list++) {

							if (tableItems [list].name != "add new") {
								var item = tableItems [list] as CustomList;
								if (item != null)
									db.InsertOrReplace (tableItems [list], typeof (CustomList));
								else 
								{
									
									db.Insert (tableItems [list], typeof (Movie));
								}

							}

							if (movieDetail != null && (tableItems [list].id == Id)) {

								//get id of last inserted row
								string sql = "select last_insert_rowid()";
								var scalarValue = db.ExecuteScalar<string> (sql);
								int value = scalarValue == null ? 0 : Convert.ToInt32 (scalarValue);

								if (Id > 0)
									movieDetail.CustomListID = Id;
								else
									movieDetail.CustomListID = value;


								db.Insert (movieDetail, typeof (Movie));
							}
						}


					}

				} catch (SQLiteException e) {
					Debug.WriteLine (e.Message);

					using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) {
						conn.CreateTable<Movie> (CreateFlags.ImplicitPK | CreateFlags.AutoIncPK);
						conn.CreateTable<CustomList> (CreateFlags.ImplicitPK | CreateFlags.AutoIncPK);
					}
					;
					using (var db = new SQLiteConnection (MovieService.Database)) {
						if (movieDetail != null)
							DeleteMovie (movieDetail.id);

						//DeleteAll ();

						for (var list = 0; list < tableItems.Count; list++) {

							if (tableItems [list].name != "add new") {
								var item = tableItems [list] as CustomList;
								if (item != null)
									db.Insert (tableItems [list], typeof (CustomList));
								else 
								{
									
									db.Insert (tableItems [list], typeof (Movie));
								}
							}

							if (upDateMovieDetail && (tableItems [list].id == Id)) {

								//get id of last inserted row
								string sql = "select last_insert_rowid()";
								var scalarValue = db.ExecuteScalar<string> (sql);
								int value = scalarValue == null ? 0 : Convert.ToInt32 (scalarValue);

								if (Id > 0)
									movieDetail.CustomListID = Id;
								else
									movieDetail.CustomListID = value;


								db.Insert (movieDetail, typeof (Movie));
							}
						}


					}
				}
			});
			task.Wait ();                     
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
		protected void updateMovieOrder (CustomList item, int row)
		{
			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					item.order = row;
					db.InsertOrReplace (item, typeof (CustomList));

				}

			} catch (SQLite.SQLiteException s) {
				Debug.Write (s.Message);
			}
		}
		public static void DeleteMovie (int? id)
		{

			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					db.Query<Movie> ("DELETE FROM [Movie] WHERE [ID] = " + id);


				}
			} catch (SQLite.SQLiteException e) {
				//first time in no favorites yet.
				Debug.Write (e.Message);
			}

		}
		public static void DeleteMoviesInCustomList (int? CustomId)
		{

			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) 
				{
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					db.Query<Movie> ("DELETE FROM [Movie] WHERE [CustomListID] = " + CustomId);


				}
			} catch (SQLite.SQLiteException e) {
				//first time in no favorites yet.
				Debug.Write (e.Message);
			}

		}
		protected List<ICustomList> GetMovieList ()
		{

			List<ICustomList> result = new List<ICustomList> ();
			var task = Task.Run(async () => 
			{
				try {

					using (var db = new SQLiteConnection (MovieService.Database)) {
						// there is a sqllite bug here https://forums.xamarin.com/discussion/
						//52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
						var query = db.Query<CustomList> ("SELECT * FROM [CustomList] ORDER BY [Order]");


						foreach (var list in query) {
							var item = new CustomList ();
							item.id = list.id;
							item.name = list.name;
							item.order = list.order;
							item.shared = list.shared;
							result.Add (item);
						}
					}


				} catch (SQLiteException e) {
					Debug.WriteLine (e.Message);

					using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) {
						conn.CreateTable<Movie> (CreateFlags.ImplicitPK | CreateFlags.AutoIncPK);;
						conn.CreateTable<CustomList> (CreateFlags.ImplicitPK | CreateFlags.AutoIncPK);
					}
					using (var db = new SQLiteConnection (MovieService.Database)) {
						// there is a sqllite bug here https://forums.xamarin.com/discussion/
						//52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
						//	var query = db.Query<CustomList> ("SELECT * FROM CUSTOMLIST");
						var query = db.Query<CustomList> ("SELECT * FROM [CustomList] ORDER BY [Order]");
						foreach (var list in query) {
							var item = new CustomList ();
							item.id = list.id;
							item.name = list.name;
							item.order = list.order;
							item.shared = list.shared;
							result.Add (item);

						}

					}

				}
			});
			task.Wait ();
			return result;
		}
		public static void DeleteAll (int? Id)
		{
			try {
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);

					db.Query<Movie> ("DELETE FROM [CustomList] WHERE [ID] = " + Id);

				}
			} catch (SQLite.SQLiteException) {
				//first time in no favorites yet.
			}
		}
		public static void DeleteAll ()
		{
			var task = Task.Run (async () => 
			{
				try {
					using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
						// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);

						db.Query<Movie> ("DELETE FROM [CustomList]");

					}
				} catch (SQLite.SQLiteException e) {
					//first time in no favorites yet.
					Debug.Write (e.Message);
				}
			});
			task.Wait ();
		}

		protected void UpdateCustomList ()
		{
			try {

				using (var db = new SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/
					//52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					//var query = db.Table<CustomList> ();
					DeleteAll ();
					foreach (var list in tableItems) {

						if (list.name != null) {
							db.Insert (list, typeof (CustomList));
						}
					}
				}

			} catch (SQLiteException e) {
				Debug.WriteLine (e.Message);

				using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) {
					conn.CreateTable<Movie> (CreateFlags.ImplicitPK | CreateFlags.AutoIncPK);
					conn.CreateTable<CustomList> (CreateFlags.ImplicitPK | CreateFlags.AutoIncPK);
				}
				DeleteAll ();
				using (var db = new SQLiteConnection (MovieService.Database)) {
					foreach (var list in tableItems) {
						if (list.name != null) {
							db.Insert (list, typeof (CustomList));
						}
					}
				}
			}

		}
	}
	public class TableSource<T> : UITableViewSource
		{
		
			List<ICustomList> tableItems;
			string cellIdentifier = "TableCell";
			BaseListViewController Owner;
			public TableSource (List<ICustomList> items, BaseListViewController owner)
			{

			   tableItems = items;
			
			   this.Owner = owner;

			}
			public void WillBeginTableEditing (UITableView tableView)
			{
			try {
				tableView.BeginUpdates ();
				// insert the 'ADD NEW' row at the end of table display
				tableView.InsertRows (new NSIndexPath [] {
			NSIndexPath.FromRowSection (tableView.NumberOfRowsInSection (0), 0)
		}, UITableViewRowAnimation.Fade);
				// create a new item and add it to our underlying data (it is not intended to be permanent)
				//if(tableItems.Equals(typeof(List<CustomList>)))
					tableItems.Add (new CustomList () { name = "add new" });
				//else
				//	tableItems.Add (new Movie () { Name = "add new" });
				tableView.EndUpdates (); // applies the changes
			} catch (Exception ex) { Debug.Write (ex.Message);}
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
					var item = tableItems [(int)tableView.NumberOfRowsInSection (0) - 1] as CustomList;
					if (item is CustomList) 
					{
						Owner.UpdateCustomAndMovieList (((CustomList)tableItems [(int)tableView.NumberOfRowsInSection (0) - 1]).id, false, tableItems);
					} else 
					{
						Owner.UpdateCustomAndMovieList (((Movie)tableItems [(int)tableView.NumberOfRowsInSection (0) - 1]).CustomListID, true, tableItems);
			 		}
				} else
					BaseListViewController.DeleteAll ();
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
				return "Delete ";
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
				if ((sourceIndexPath.Section == destinationIndexPath.Section) && (destinationIndexPath.Row < sourceIndexPath.Row)) {
					//---- add one to where we delete, because we're increasing the index by inserting
					deleteAt += 1;
				} else {
					insertAt += 1;
				}

				//---- copy the item to the new location
				((ICustomList)item).order = destinationIndexPath.Row;
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
				try {
					if (tableView.Editing) {

						if (tableItems [indexPath.Row] as CustomList!=null ? ((CustomList)tableItems [indexPath.Row]).name == "add new" : ((Movie)tableItems [indexPath.Row]).name == "add new") 
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
									listItem.order = 0;
									listItem.name = textInputAlertController.TextFields [0].Text;
									tableItems.Insert (0, listItem);
									tableView.EndUpdates (); // applies the changes
									tableView.ReloadData ();
									ArrangeCustomList (true);
									var item = tableItems [(int)indexPath.Row] as CustomList;
									if (item is CustomList) {
										Owner.UpdateCustomAndMovieList (((CustomList)tableItems [(int)tableView.NumberOfRowsInSection (0) - 1]).id, false, tableItems);
									} else {
										Owner.UpdateCustomAndMovieList (((Movie)tableItems [(int)tableView.NumberOfRowsInSection (0) - 1]).id, true, tableItems);
									}
								} else {
									new UIAlertView ("Duplicate!"
									, "List already exist", null, "OK", null).Show ();
								}
							});

							textInputAlertController.AddAction (cancelAction);
							textInputAlertController.AddAction (okayAction);

							//Present Alert
							Owner.PresentViewController (textInputAlertController, true, null);
						}
					} else {

						ArrangeCustomList (true);
						//Debug.Write (tableItems [(int)tableView.NumberOfRowsInSection (0) - 1].GetType());
						var item = tableItems [(int)indexPath.Row] as CustomList;
						if (item is CustomList) {
							Owner.UpdateCustomAndMovieList (((CustomList)tableItems [indexPath.Row]).id, false, tableItems);
						} else {
							//Owner.UpdateCustomAndMovieList (((Movie)tableItems [(int)indexPath.Row]).Id, Owner.fromAddList, tableItems);
							Owner.NavigationController.PushViewController (new MovieDetailViewController (tableItems [indexPath.Row] as Movie, true), true);
						}
						MainViewController.NewCustomListToRefresh = 0;

						if (Owner.fromAddList)
							Owner.NavigationController.PopToRootViewController (true);
						else if (item is CustomList)
							Owner.NavigationController.PushViewController (new MovieListViewContoller (null, tableItems [indexPath.Row], Owner.fromAddList), true);

					}
					tableView.DeselectRow (indexPath, true);
				} catch (Exception ex) 
				{
					Debug.Write (ex.Message);
				}
			}

			public void ArrangeCustomList (bool StartAtZero)
			{

				for (var list = StartAtZero ? 0 : 1; list < tableItems.Count; list++) {
					int num = StartAtZero ? 0 : 1;
					if (tableItems [list - num].name != "add new")
						tableItems [list - num].order = list;
				}
			}

			public bool ValueUnique (string text)
			{
				foreach (var item in tableItems) {
					if (item.name == text) {
						return false;

					}
				}
				return true;
			}

		   public override UIView GetViewForHeader (UITableView tableView, nint section)
			{
			UILabel headerLabel = new UILabel () { Text = "List Name" }; // Set the frame size you need
				headerLabel.TextColor = UIColor.Red; // Set your color
				return headerLabel;
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
				if (tableItems [indexPath.Row] is CustomList) 
				{
					var switchView = (UISwitch)cell.AccessoryView;
					if (switchView == null) {
						switchView = new UISwitch ();
						switchView.AddTarget ((sender, e) => {
							if (((UISwitch)sender).On)
								tableItems [indexPath.Row].shared = true;
							else
								tableItems [indexPath.Row].shared = false;
							Owner.UpdateCustomAndMovieList (((CustomList)tableItems [indexPath.Row]).id, false, tableItems);

						}, UIControlEvent.ValueChanged);
					}

					if (tableItems [indexPath.Row].shared)
						switchView.On = true;
					else
						switchView.On = false;


					cell.AccessoryView = switchView;
				}
				cell.TextLabel.Text = tableItems [indexPath.Row].name;
				cell.TextLabel.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE);

				
				return cell;
			}
		}
	}



