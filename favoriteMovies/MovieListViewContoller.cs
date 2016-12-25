using System;
using System.Collections.Generic;
using System.Diagnostics;
using FavoriteMoviesPCL;
using SQLite;
using UIKit;

namespace FavoriteMovies
{
	class MovieListViewContoller : BaseListViewController
	{

		ICustomList customList;
		public MovieListViewContoller (Movie movieDetail,ICustomList customList, bool fromAddList):base(movieDetail,fromAddList)
		{
			this.customList = customList;
		}

		public override void ViewDidLoad ()
		{
			//base.ViewDidLoad ();

			var movieItems = GetMovieListInList (customList);

			table = new UITableView (View.Bounds);
			table.AutoresizingMask = UIViewAutoresizing.All;
			tableSource = new TableSource<ICustomList> (movieItems, this);
			table.Source = tableSource;
			table.AllowsSelectionDuringEditing = true;
			NavigationItem.Title = "Movies";
			Add (table);
		}

		List<ICustomList> GetMovieListInList (ICustomList movie)
		{
			List<ICustomList> result = new List<ICustomList> ();

			try {

				using (var db = new SQLiteConnection (MovieService.Database)) {
					// there is a sqllite bug here https://forums.xamarin.com/discussion/
					//52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					var query = db.Query<Movie> ("SELECT * FROM [Movie] WHERE CustomListID = "+ movie.id + " ORDER BY [Order]");


					foreach (var mov in query) {
						var item = new Movie ();
						item.id = mov.id;
						item.name = mov.name;
						item.BackdropPath = mov.BackdropPath;
						item.CustomListID = mov.CustomListID;
						item.Favorite = mov.Favorite;
						item.HighResPosterPath = mov.HighResPosterPath;
						item.OriginalLanguage = mov.OriginalLanguage;
						item.Overview = mov.Overview;
						item.Popularity = mov.Popularity;
						item.PosterPath = mov.PosterPath;
						item.ReleaseDate = mov.ReleaseDate;
						item.VoteAverage = mov.VoteAverage;
						item.UserReview = mov.UserReview;
						item.UserRating = mov.UserRating;
						item.order = mov.order;
						result.Add (item);
					}
				}


			} catch (SQLiteException e) {
				Debug.WriteLine (e.Message);

				using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) {
					conn.CreateTable<Movie> ();
					conn.CreateTable<CustomList> (CreateFlags.ImplicitPK | CreateFlags.AutoIncPK);
				}


			}

			return result;
		}
}
}