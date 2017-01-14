using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contacts;
using FavoriteMoviesPCL;
using Foundation;
using MovieFriends;
using UIKit;

namespace FavoriteMovies
{
	public class UserCloudListViewController:BaseBasicListViewController
	{
		List<CNContact> tableItems;
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			var contacts = Task.Run (async () => {
				
				tableItems = GetUserContactsAsync ();
			});
			contacts.Wait ();

			tableSource = new UserCloudTableSource (tableItems, this);
			tableView.Source = tableSource;
			NavigationItem.Title = "Add Friends";
			Add (tableView);

		}

		List<CNContact> GetUserContactsAsync ()
		{
			// Create predicate to locate requested contact
			//var predicate = CNContact.GetPredicateForContacts(null);

			// Define fields to be searched
			var fetchKeys = new NSString [] { CNContactKey.GivenName, CNContactKey.FamilyName, CNContactKey.EmailAddresses,CNContactKey.ImageDataAvailable, CNContactKey.ThumbnailImageData };


			var store = new CNContactStore ();
			NSError error;
			CNContainer [] containers= store.GetContainers (null, out error) ;
			List<CNContact> result = new List<CNContact> ();
			foreach (var container in containers) 
			{
				var fetchPredicate = CNContact.GetPredicateForContactsInContainer (container.Identifier);

				var containerResults = store.GetUnifiedContacts (fetchPredicate, fetchKeys, out error);
				foreach (var contact in containerResults) 
				{
					result.Add (contact);
				}

			}

			return result.ToList ();

		}
}


	public class UserCloudTableSource : UITableViewSource
	{
		List<CNContact> listItems;
		UserCloudListViewController controller;
		const string cellIdentifier = "UserCloudCells";

		public UserCloudTableSource (List<CNContact> items, UserCloudListViewController cont)
		{
			this.listItems = items;
			this.controller = cont;

		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cellStyle = UITableViewCellStyle.Value1;
			UITableViewCell cell = new UITableViewCell (cellStyle, cellIdentifier);

			var switchView = (UISwitch)cell.AccessoryView;
			if (switchView == null) {
				switchView = new UISwitch ();
				switchView.AddTarget ((sender, e) => {
					if (((UISwitch)sender).On)
						//tableItems [indexPath.Row].shared = true;
						switchView.On = true;
					else
						switchView.On = false;
						//tableItems [indexPath.Row].shared = false;
					//Owner.UpdateCustomAndMovieList (((CustomList)tableItems [indexPath.Row]).id, false, tableItems);

				}, UIControlEvent.ValueChanged);
			}

			cell.AccessoryView = switchView;
			var name = listItems [indexPath.Row].GivenName + " " +listItems [indexPath.Row].FamilyName ;
			cell.TextLabel.Text = name;
			cell.TextLabel.Font = UIFont.FromName (ColorExtensions.TITLE_FONT, ColorExtensions.HEADER_FONT_SIZE);
			var profileImage = UIImage.FromBundle ("1481507483_compose.png"); //default image
			cell.ImageView.Image = profileImage;
			if (listItems [indexPath.Row].ImageDataAvailable)
				
				cell.ImageView.Image = UIImage.LoadFromData (listItems [indexPath.Row].ThumbnailImageData);

			return cell;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return  listItems.Count;
		}
	}
}
