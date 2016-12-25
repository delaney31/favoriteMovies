using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using UIKit;

namespace FavoriteMovies
{

	public class NewsFeedViewController : UIViewController
	{
		List<FeedItem> tableItems = new List<FeedItem> ();
		UITableView table;
		NewsFeedTableSource tableSource;
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			var task = Task.Run (async () => 
			{
				tableItems = MovieNewsFeed.GetFeedItems ();
			});
			TimeSpan ts = TimeSpan.FromMilliseconds (1000);
			task.Wait (ts);
			if (!task.Wait (ts))
				Console.WriteLine ("The timeout interval elapsed.");
			
			table = new UITableView (View.Bounds);
			table.AutoresizingMask = UIViewAutoresizing.All;
			tableSource = new NewsFeedTableSource (tableItems, this);
			table.Source = tableSource;
			table.AllowsSelectionDuringEditing = true;
			Add (table);
		}
	}


	public class NewsFeedTableSource:UITableViewSource
	{
		List<FeedItem> tableItems;
		NewsFeedViewController newsFeedViewController;

		public NewsFeedTableSource (List<FeedItem> tableItems, NewsFeedViewController newsFeedViewController)
		{
			this.tableItems = tableItems;
			this.newsFeedViewController = newsFeedViewController;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return tableItems.Count;
		}
		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}
		public override nfloat GetHeightForRow (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			//return base.GetHeightForRow (tableView, indexPath);
			return 300;
		}
		public override void RowSelected (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			var webView = new UIWebView () {
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth,
				BackgroundColor = UIColor.Black,
			};
			var viewController = new UIViewController ();

			webView.Frame = new CGRect (0, 0, (float)newsFeedViewController.View.Frame.Width, (float)newsFeedViewController.View.Frame.Height);
			//webView.LoadHtmlString (sb.ToString (), null);
			webView.LoadRequest (new NSUrlRequest (new NSUrl(tableItems [indexPath.Row].Link)));
			viewController.View.Add (webView);
			//this.View.AddSubview (webView);
			newsFeedViewController.NavigationController.PushViewController (viewController, true);
			Console.WriteLine (newsFeedViewController.NavigationController.ViewControllers.Length);
		}
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			const string CellIdentifier = @"CardCell";
			var cell = (MDCard)tableView.DequeueReusableCell (CellIdentifier);
			if (cell == null) {
				cell = new MDCard (UITableViewCellStyle.Default, CellIdentifier);
			}
			cell.profileImage.Image = MovieCell.GetImageUrl(tableItems [indexPath.Row].ImageLink);
			var backGroundColor = MovieDetailViewController.averageColor (cell.profileImage.Image);
			cell.titleLabel.Text = tableItems [indexPath.Row].Title;
			cell.titleLabel.TextColor = MovieDetailViewController.IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;

			cell.nameLabel.Text = tableItems [indexPath.Row].Creator;
			cell.nameLabel.TextColor = MovieDetailViewController.IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;

			cell.descriptionLabel.Text = tableItems [indexPath.Row].Description;
			cell.descriptionLabel.TextColor = MovieDetailViewController.IsDarkColor (backGroundColor) ? UIColor.White : UIColor.Black;

			cell.cardView.Frame = new CGRect () { X = 10, Y = 10, Width = 300, Height=290};
			cell.cardView.BackgroundColor = backGroundColor;
			return cell;
		}
	}
}
