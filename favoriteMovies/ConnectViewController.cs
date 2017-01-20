using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using MovieFriends;
using UIKit;

namespace FavoriteMovies
{
	public class ConnectViewController:BaseBasicListViewController
	{
		List<ContactCard> tableItems;
		const string cellIdentifier = "UserCloudCells";
		AzureTablesService postService = AzureTablesService.DefaultService;

		public ConnectViewController ()
		{

			InvokeOnMainThread (async () => {
				tableItems = await GetUserContactsAsync ();
				tableSource = new ConnectCloudTableSource (tableItems, this);
				tableView.Source = tableSource;
				tableView.TableHeaderView = new UIView () { Frame = new CoreGraphics.CGRect () { X = 0.0f, Y = 0.0f, Width = View.Layer.Frame.Width, Height = 20f } };
				Add (tableView);
			});

			//var task = Task.Run (async () => 
			//{
			//	await postService.InitializeStoreAsync ();
				

		//	});
		//	task.Wait ();
		}
		public override  void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			//await postService.InitializeStoreAsync ();
			//tableItems = await GetUserContactsAsync ();



		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewDidAppear (animated);


		}
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		async Task<List<ContactCard>> GetUserContactsAsync ()
		{
			var watch = System.Diagnostics.Stopwatch.StartNew ();


			const string cellId = "UserContacts";
			List<ContactCard> results = new List<ContactCard> ();
			var users = await postService.GetUserCloud ();

			foreach (var user in users) 
			{
				if (user.Id != ColorExtensions.CurrentUser.Id)
				{
					var result = new ContactCard (UITableViewCellStyle.Default, cellId);
					result.nameLabel.Text = user.username;
					result.connection = user.Friend;
					result.id = user.Id;
					results.Add (result);
				}

			}
			watch.Stop ();
			Console.WriteLine("ViewWillAppear Method took " + watch.ElapsedMilliseconds + " milli seconds");

			return results.ToList ();

		}
	}


	public class ConnectCloudTableSource : UITableViewSource
	{
		public List<ContactCard> listItems;
		ConnectViewController controller;

		//Your invitation to .... is on its way!!
		public ConnectCloudTableSource (List<ContactCard> items, ConnectViewController cont)
		{
			this.listItems = items;
			this.controller = cont;
			updateImages ();
		}
		public async void updateImages ()
		{
			InvokeOnMainThread (async () => {
				foreach (var user in listItems) {

					user.profileImage.Image = ColorExtensions.ResizeImage (await BlobUpload.getProfileImage (user.id), 50, 50);

				}
				 
			});
		}
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{

			var cell = listItems [indexPath.Row];

			var switchView = cell.AccessoryView;
			CGRect frame = new CGRect();
			if (switchView == null) 
			{
				switchView = new UIButton (UIButtonType.ContactAdd);
				frame = switchView.Frame;
				if (cell.connection) 
				{
					switchView = new UIImageView () { UserInteractionEnabled = true };
					switchView.Frame = frame;
					((UIImageView)switchView).Image = UIImage.FromBundle ("Q4ZWS.png");
				}
		
				if (switchView is UIButton) 
				{
					((UIButton)switchView).AddTarget ((sender, e) => 
					{

					}, UIControlEvent.ValueChanged);
				} else 
				{
					var tapGesture = new UITapGestureRecognizer ();
					tapGesture.AddTarget(() =>
					{
						
					});
					((UIImageView)switchView).AddGestureRecognizer (tapGesture);

				}
			}

			cell.AccessoryView = switchView;
			return cell;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return listItems.Count;
		}
	}

}
