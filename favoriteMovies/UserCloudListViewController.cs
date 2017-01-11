using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FavoriteMoviesPCL;
using Foundation;
using MovieFriends;
using UIKit;

namespace FavoriteMovies
{
	public class UserCloudListViewController:BaseBasicListViewController
	{
		List<UserCloud> tableItems;
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var cloudItems = Task.Run (async () => {
				
				tableItems = await GetUserCloudListAsync ();
			});
			cloudItems.Wait ();

			tableSource = new UserCloudTableSource (tableItems, this);
			tableView.Source = tableSource;
			NavigationItem.Title = "Add Friends";
			Add (tableView);

		}

		async Task<List<UserCloud>> GetUserCloudListAsync ()
		{
			AzureTablesService userFriendsService = AzureTablesService.DefaultService;
			await userFriendsService.InitializeStoreAsync ();


			var retList = await userFriendsService.GetUserCloud();
			return retList;
		}
}


	public class UserCloudTableSource : UITableViewSource
	{
		List<UserCloud> listItems;
		UserCloudListViewController controller;
		const string cellIdentifier = "UserCloudCells";

		public UserCloudTableSource (List<UserCloud> items, UserCloudListViewController cont)
		{
			this.listItems = items;
			this.controller = cont;

		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			// request a recycled cell to save memory
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);


			var cellStyle = UITableViewCellStyle.Default;

			// if there are no cells to reuse, create a new one
			if (cell == null) {
				cell = new UITableViewCell (cellStyle, cellIdentifier);

			}
			cell.TextLabel.Text = listItems [indexPath.Row].username;
			cell.TextLabel.Font = UIFont.FromName (ColorExtensions.TITLE_FONT, ColorExtensions.HEADER_FONT_SIZE);

			return cell;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return  listItems.Count;
		}
	}
}
