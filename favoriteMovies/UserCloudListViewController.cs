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
		List<ContactCard> tableItems;
		const string cellIdentifier = "UserCloudCells";
		public override  void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			tableItems = GetUserContactsAsync ();


			tableSource = new UserCloudTableSource (tableItems, this);
			tableView.Source = tableSource;
			NavigationItem.Title = "Add Friends";
			Add (tableView);

		}

		List<ContactCard> GetUserContactsAsync ()
		{
			// Create predicate to locate requested contact
			//var predicate = CNContact.GetPredicateForContacts(null);

			// Define fields to be searched
			var fetchKeys = new NSString [] { CNContactKey.GivenName, CNContactKey.FamilyName, CNContactKey.EmailAddresses,CNContactKey.ImageDataAvailable, CNContactKey.ThumbnailImageData };


			var store = new CNContactStore ();
			NSError error;
			CNContainer [] containers= store.GetContainers (null, out error) ;
			List<ContactCard> result = new List<ContactCard> ();
			foreach (var container in containers) 
			{
				var fetchPredicate = CNContact.GetPredicateForContactsInContainer (container.Identifier);

				var containerResults = store.GetUnifiedContacts (fetchPredicate, fetchKeys, out error);
				foreach (var contact in containerResults) 
				{
					var conCard = new ContactCard (UITableViewCellStyle.Default,cellIdentifier);
					conCard.nameLabel.Text = contact.GivenName + " " + contact.FamilyName;
					if(contact.ImageDataAvailable)
					   conCard.profileImage.Image = UIImage.LoadFromData(contact.ThumbnailImageData);
					else
					   conCard.profileImage.Image = UIImage.FromBundle ("1481507483_compose.png"); //default image
					result.Add (conCard);
				}

			}

			return result.ToList ();

		}
}


	public class UserCloudTableSource : UITableViewSource
	{
		List<ContactCard> listItems;
		UserCloudListViewController controller;


		public UserCloudTableSource (List<ContactCard> items, UserCloudListViewController cont)
		{
			this.listItems = items;
			this.controller = cont;

		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			
			var cell = listItems [indexPath.Row];
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
			return cell;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return  listItems.Count;
		}
	}
}
