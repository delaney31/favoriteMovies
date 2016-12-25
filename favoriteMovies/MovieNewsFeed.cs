using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using FavoriteMoviesPCL;

namespace FavoriteMovies
{
	internal static class MovieNewsFeed
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

			

				for (int i = 0; i < itemNodes.Count; i++) {
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
					feedItemsList.Add (feedItem);
				}
			} catch (Exception) {
				throw;
			}

			return feedItemsList;
		}

	}
}

