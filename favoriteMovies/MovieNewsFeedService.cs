using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using FavoriteMoviesPCL;
using MovieFriends;
using SQLite;
using UIKit;

namespace FavoriteMovies
{
	internal static class MovieNewsFeedService
	{
		static string url = "http://movieweb.com/rss/movie-news/";

		internal static List<FeedItem> GetFeedItems ()
		{
			List<FeedItem> feedItemsList = new List<FeedItem> ();

			try {
				WebRequest webRequest = WebRequest.Create (url);
				WebResponse webResponse = webRequest.GetResponse ();
				Stream stream = webResponse.GetResponseStream ();
				XmlDocument xmlDocument = new XmlDocument ();
				xmlDocument.Load (stream);
				XmlNamespaceManager nsmgr = new XmlNamespaceManager (xmlDocument.NameTable);
				nsmgr.AddNamespace ("dc", xmlDocument.DocumentElement.GetNamespaceOfPrefix ("dc"));
				nsmgr.AddNamespace ("content", xmlDocument.DocumentElement.GetNamespaceOfPrefix ("content"));
				XmlNodeList itemNodes = xmlDocument.SelectNodes ("rss/channel/item");

			
				DeleteAllFeedItems ();
				for (int i = 0; i < itemNodes.Count; i++) 
				{
					FeedItem feedItem = new FeedItem ();

					if (itemNodes [i].SelectSingleNode ("title") != null) {
						feedItem.Title = itemNodes [i].SelectSingleNode ("title").InnerText;
					}
					if (itemNodes [i].SelectSingleNode ("link") != null) {
						feedItem.Link = itemNodes [i].SelectSingleNode ("link").InnerText;
						//feedItem.Link = itemNodes [i].SelectSingleNode ("enclosure").Attributes ["url"].Value;
					}
					if (itemNodes [i].SelectSingleNode ("link") != null) {
						//feedItem.Link = itemNodes [i].SelectSingleNode ("link").InnerText;
						feedItem.ImageLink = itemNodes [i].SelectSingleNode ("enclosure").Attributes ["url"].Value;
					}
					if (itemNodes [i].SelectSingleNode ("pubDate") != null) {
						feedItem.PubDate =  itemNodes [i].SelectSingleNode ("pubDate").InnerText;
					}
					if (itemNodes [i].SelectSingleNode ("dc:creator", nsmgr) != null) {
						feedItem.Creator = itemNodes [i].SelectSingleNode ("dc:creator", nsmgr).InnerText;
					}
					if (itemNodes [i].SelectSingleNode ("category") != null) {
						feedItem.Category = itemNodes [i].SelectSingleNode ("category").InnerText;
					}
					if (itemNodes [i].SelectSingleNode ("description") != null) {
						feedItem.Description = itemNodes [i].SelectSingleNode ("description").InnerText;
					}
					if (itemNodes [i].SelectSingleNode ("content:encoded", nsmgr) != null) {
						feedItem.Content = itemNodes [i].SelectSingleNode ("content:encoded", nsmgr).InnerText;
					} else {
						feedItem.Content = feedItem.Description;
					}
					//feedItem.like = GetCloudLike (feedItem);

					//var id = InsertNewsFeed (feedItem);

					//feedItem.id = id;
					feedItemsList.Add (feedItem);

				}

			} catch (Exception) {
				throw;
			}

			return feedItemsList;
		}

		internal async static Task<List<MDCard>> GetMDCardItems ()
		{
			List<MDCard> feedItemsList = new List<MDCard> ();
			AzureTablesService postService = AzureTablesService.DefaultService;
			await postService.InitializeStoreAsync ();
			try 
			{
				WebRequest webRequest = WebRequest.Create (url);
				WebResponse webResponse = webRequest.GetResponse ();
				Stream stream = webResponse.GetResponseStream ();
				XmlDocument xmlDocument = new XmlDocument ();
				xmlDocument.Load (stream);
				XmlNamespaceManager nsmgr = new XmlNamespaceManager (xmlDocument.NameTable);
				nsmgr.AddNamespace ("dc", xmlDocument.DocumentElement.GetNamespaceOfPrefix ("dc"));
				nsmgr.AddNamespace ("content", xmlDocument.DocumentElement.GetNamespaceOfPrefix ("content"));
				XmlNodeList itemNodes = xmlDocument.SelectNodes ("rss/channel/item");

				DeleteAllFeedItems ();
				for (int i = 0; i < itemNodes.Count; i++) 
				{
					var feedItem = new MDCard (UITableViewCellStyle.Default, @"CardCell");
					var feed = new FeedItem ();

					if (itemNodes [i].SelectSingleNode ("title") != null) {
						feedItem.titleLabel.Text = itemNodes [i].SelectSingleNode ("title").InnerText;
						feed.Title = itemNodes [i].SelectSingleNode ("title").InnerText;
					}
					if (itemNodes [i].SelectSingleNode ("link") != null) {
						feedItem.Link = itemNodes [i].SelectSingleNode ("link").InnerText;
						feed.Link = itemNodes [i].SelectSingleNode ("link").InnerText;
						//feedItem.Link = itemNodes [i].SelectSingleNode ("enclosure").Attributes ["url"].Value;
					}
					if (itemNodes [i].SelectSingleNode ("link") != null) {
						//feedItem.Link = itemNodes [i].SelectSingleNode ("link").InnerText;
						feedItem.profileImage.Image = MovieCell.GetImageUrl (itemNodes [i].SelectSingleNode ("enclosure").Attributes ["url"].Value);
						feed.ImageLink = itemNodes [i].SelectSingleNode ("enclosure").Attributes ["url"].Value;
					}
					if (itemNodes [i].SelectSingleNode ("pubDate") != null) {
						feedItem.PubDate = itemNodes [i].SelectSingleNode ("pubDate").InnerText;
						feed.PubDate = itemNodes [i].SelectSingleNode ("pubDate").InnerText;
					}
					if (itemNodes [i].SelectSingleNode ("dc:creator", nsmgr) != null) {
						feedItem.Creator = itemNodes [i].SelectSingleNode ("dc:creator", nsmgr).InnerText;
						feed.Creator = itemNodes [i].SelectSingleNode ("dc:creator", nsmgr).InnerText;
					}
					if (itemNodes [i].SelectSingleNode ("category") != null) {
						feedItem.Category = itemNodes [i].SelectSingleNode ("category").InnerText;
						feed.Category = itemNodes [i].SelectSingleNode ("category").InnerText;
					}
					if (itemNodes [i].SelectSingleNode ("description") != null) {
						feedItem.descriptionLabel.Text = itemNodes [i].SelectSingleNode ("description").InnerText;
						feed.Description = itemNodes [i].SelectSingleNode ("description").InnerText;
					}
					if (itemNodes [i].SelectSingleNode ("content:encoded", nsmgr) != null) {
						feedItem.Content = itemNodes [i].SelectSingleNode ("content:encoded", nsmgr).InnerText;
						feed.Content = itemNodes [i].SelectSingleNode ("content:encoded", nsmgr).InnerText;
					} else {
						feedItem.Content = feedItem.Description;
						feed.Content = feedItem.Description;
					}


					//UIApplication.SharedApplication.InvokeOnMainThread (new Action (() => {
					var result = await postService.GetCloudLike (feed.Title);

					if (result.Count > 0) 
					{
						feedItem.likeLabel.Text = result [0].Like;
						feedItem.likes = result [0].Likes;
						feedItem.id = result [0].Id;
					} else
						feedItem.likeLabel.Text = "Like";

					feedItemsList.Add (feedItem);

				}

			} catch (Exception e) 
			{
				Console.WriteLine (@"Error{0}", e.Message + " No internet connection");
				 ShowAlert ("Limited Internet", "Your internet connection is down. Some items will not be available.", "Ok");
			}

			return feedItemsList;

		}


		static void  ShowAlert (string title, string message, params string [] buttons)
		{
			var tcs = new TaskCompletionSource<int> ();
			var alert = new UIAlertView {
				Title = title,
				Message = message
			};
			foreach (var button in buttons)
				alert.AddButton (button);
			alert.Clicked += (s, e) => tcs.TrySetResult ((int)e.ButtonIndex);
			alert.Show ();

		}

		public static void DeleteAllFeedItems ()
		{
			var task = Task.Run (async () => {
				try {
					using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
						// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);

						db.Query<Movie> ("DELETE FROM [FeedItem]");

					}
				} catch (SQLite.SQLiteException e) {
					//first time in no favorites yet.
					Debug.Write (e.Message);
				}
			});
			task.Wait ();
		}
		static int? InsertNewsFeed (FeedItem feedItem)
		{
			try {
				
				using (var db = new SQLiteConnection (MovieService.Database)) 
				{
					// there is a sqllite bug here https://forums.xamarin.com/discussion/
					//52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);
					//var query = db.Table<CustomList> ();

					//foreach (var list in feedItemsList) {

						if (feedItem.Title != null) 
						{
							db.InsertOrReplace (feedItem, typeof (FeedItem));
						}

					string sql = "select last_insert_rowid()";
					var scalarValue = db.ExecuteScalar<string> (sql);
					int value = scalarValue == null ? 0 : Convert.ToInt32 (scalarValue);
					return value;
					//}
				}

			} catch (SQLiteException e) {
				Debug.WriteLine (e.Message);

				using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) {
					conn.CreateTable<FeedItem> (CreateFlags.ImplicitPK | CreateFlags.AutoIncPK);

				}

				using (var db = new SQLiteConnection (MovieService.Database)) 
				{
				//	foreach (var list in feedItemsList) {
						if (feedItem.Title != null) {
							db.InsertOrReplace (feedItem, typeof (FeedItem));
						}
				//	}

					string sql = "select last_insert_rowid()";
					var scalarValue = db.ExecuteScalar<string> (sql);
					int value = scalarValue == null ? 0 : Convert.ToInt32 (scalarValue);
					return value;
				}
			}

		}

	}
}

