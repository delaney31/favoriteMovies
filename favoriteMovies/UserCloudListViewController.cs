﻿using System;
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
		public override async void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			tableItems = await GetUserContactsAsync ();


			tableSource = new UserCloudTableSource (tableItems, this);
			table.Source = tableSource;
			NavigationItem.Title = "Add Friends";
			Add (table);

		}

		async Task<List<ContactCard>> GetUserContactsAsync ()
		{
			// Define fields to be searched
			var fetchKeys = new NSString [] { CNContactKey.GivenName, CNContactKey.FamilyName, CNContactKey.EmailAddresses,CNContactKey.ImageDataAvailable, CNContactKey.ThumbnailImageData };
			List<ContactCard> result = new List<ContactCard> ();
			try {
				var store = new CNContactStore ();
				NSError error;
				CNContainer [] containers = store.GetContainers (null, out error);

				foreach (var container in containers) {
					var fetchPredicate = CNContact.GetPredicateForContactsInContainer (container.Identifier);

					var containerResults = store.GetUnifiedContacts (fetchPredicate, fetchKeys, out error);
					foreach (var contact in containerResults) {
						var conCard = new ContactCard (UITableViewCellStyle.Default, cellIdentifier);
						conCard.nameLabel.Text = contact.GivenName + " " + contact.FamilyName;

						if (contact.ImageDataAvailable)
							conCard.profileImage.Image = UIImage.LoadFromData (contact.ThumbnailImageData);
						else
							conCard.profileImage.Image = UIImage.FromBundle ("1481507483_compose.png"); //default image
						result.Add (conCard);
					}

				}
			} catch (Exception ex)
			{
				Console.WriteLine (ex.Message);
				return null;
			}
			return result.ToList ();

		}
}


	public class UserCloudTableSource : UITableViewSource
	{
		List<ContactCard> listItems;

		public UserCloudTableSource (List<ContactCard> items, UserCloudListViewController cont)
		{
			this.listItems = items;

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
