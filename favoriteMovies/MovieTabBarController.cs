using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CoreGraphics;
using FavoriteMoviesPCL;
using Foundation;
using UIKit;

namespace FavoriteMovies
{
	public class MovieTabBarController : UITabBarController
	{
		
		protected static SearchResultsViewController searchResultsController;
		protected UISearchController searchController;
		protected const float BackGroundColorAlpha = 1.0f;
		public MovieTabBarController ():base ((string)null, null)
		{
			
		}
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		//	searchResultsController.TableView.ContentInset = new UIEdgeInsets (80, 0, 0, 0);
		}
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

		//	((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).ResignFirstResponder ();

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();


			this.ViewControllerSelected += (sender, e) => 
			{
				
				// Take action based on the tab being selected
				if (TabBar.SelectedItem.Title == "Home")
					NavigationController.NavigationBar.Hidden = false;
				else
					NavigationController.NavigationBar.Hidden  = true;
					

			};
		}
	}
	public class SearchResultsUpdator : UISearchResultsUpdating
	{
		public event Action<string> UpdateSearchResults = delegate { };

		public override void UpdateSearchResultsForSearchController (UISearchController searchController)
		{
			this.UpdateSearchResults (searchController.SearchBar.Text);

		}
	}
	public class SearchResultsViewController : UITableViewController
	{
		static readonly string movieItemCellId = "movieItemCellId";
		public UISearchController searchController;
		public ObservableCollection<Movie> MovieItems { get; set; }
	


		// provide access to the navigation controller to all inheriting controllers
		public NavController NavController {
			get {
				//return (UIApplication.SharedApplication.Delegate as AppDelegate).NavController;
				return (UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.NavController;
			}
		}
		[Export ("scrollViewWillEndDragging:withVelocity:targetContentOffset:")]
		public void WillEndDragging (UIScrollView scrollView, CGPoint velocity, ref CGPoint targetContentOffset)
		{
			((UITextField)searchController.SearchBar.ValueForKey (new NSString ("_searchField"))).ResignFirstResponder ();
		}

		[Export ("searchBarCancelButtonClicked:")]
		public void searchBarCancelButtonClicked (UISearchBar searchBar)
		{
			Console.WriteLine ("The default search bar cancel button was tapped.");
			//var size = ((UIScrollView)navigationController.TopViewController.View.Subviews [0]).ContentSize;

			searchBar.ResignFirstResponder ();

			MovieItems.Clear ();
			TableView.ReloadData ();
		}
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			//View.Frame = new CGRect () { X = 0, Y = 60, Width = 320, Height = 568 };
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

		}
		public SearchResultsViewController ()
		{
			MovieItems = new ObservableCollection<Movie> ();

		}


		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return MovieItems.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (movieItemCellId);

			if (cell == null) {
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, movieItemCellId);
			}
			if (MovieItems.Count > indexPath.Row) {
				cell.TextLabel.Text = MovieItems [indexPath.Row].name;
				cell.TextLabel.Font = UIFont.FromName (UIColorExtensions.TITLE_FONT, UIColorExtensions.HEADER_FONT_SIZE);
				cell.DetailTextLabel.Text = MovieItems [indexPath.Row].Overview;
				cell.ImageView.Image = MovieCell.GetImage (MovieItems [indexPath.Row].PosterPath); // don't use for Value2
			}
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			try {
				//this.searchController.Active = false;
				var row = MovieItems [indexPath.Row];


				NavController.PushViewController (new MovieDetailViewController (row, false), true);

				//*****this fixes a problem with the uitableview adding space at the top after each selection*****

				Debug.Write (this.TableView.ContentInset);
				this.TableView.ContentInset = new UIEdgeInsets (64, 0, 49, 0);

				//this.searchController.Active = true;
			} catch (Exception e) {
				Debug.WriteLine (e.Message);
			}
		}

		public async void Search (string forSearchString)
		{
			//var test = NavController.NavigationController.PopViewController (true);
			try 
			{
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
				// perform search
				if (forSearchString.Length > 0) 
				{
					MovieItems = await MovieService.MovieSearch (forSearchString);
					TableView.ReloadData ();
				}
				UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
			} catch (Exception ex) {
				Debug.WriteLine (ex.Message);
			}

		}
	}


}
