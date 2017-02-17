using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;

namespace FavoriteMoviesPCL
{
	public static class MovieService
	{

		const string _baseUrl = "https://api.themoviedb.org/3/";
		const string _pageString = "&page=";
		//static string _apiKey = "ab41356b33d100ec61e6c098ecc92140";
		public static string _apiKey = "63ac84758e2d96000620e55b773fc312";
		static string _sessionId = "";
		public static ObservableCollection<Movie> MovieList;


		public enum MovieType
		{
			TopRated = 0,
			NowPlaying = 1,
			Popular = 2,
			Similar = 3,
			TVLatest = 4,
			Upcoming = 5


		}
		public static string Database { get; set; }
		public static int TotalPagesTopRated { get; set; }
		public static int TotalPagesNowPlaying { get; set; }
		public static int TotalPagesPopular { get; set; }
		static FavoriteMovieStore store;
		public static FavoriteMovieStore Store {
			get {
				return store != null ? store : store = new FavoriteMovieStore (Database, "MovieEntries.db3");
			}
		}

		public static async Task<string> GetYouTubeMovieId (string url)
		{
			var client = new HttpClient ();
			try 
			{
				HttpResponseMessage result = await client.GetAsync (url, CancellationToken.None);
				if (result.IsSuccessStatusCode)
				{

					string content = await result.Content.ReadAsStringAsync ();
					string videoId = ParceVideoId (content).Id.ToString ();
					//return a ObservableCollection to fill a list of movies
					return videoId;

				}
				return "";
			} 
			catch (Exception ex) 
			{
				//Model Error
				Debug.WriteLine (ex);
				return "";
			}

		}


		static MoviePOCO ParceVideoId (string content)
		{
			JObject jresponse = JObject.Parse (content);
			var jarray = jresponse ["results"];
			var newMovie = new MoviePOCO ();
			try {
				foreach (var jObj in jarray) {

					newMovie.Id = jObj ["key"].ToString ();
					break;

				}
			} catch (Exception e) {

				Debug.WriteLine (e.Message);

			}
			return newMovie;

		}
		public static async Task<ObservableCollection<Movie>> GetMoviesAsyncString (MovieType type, int page = 1, string movieId = "")
		{
			var client = new HttpClient ();

			string Url = "";

			switch (type) {

			case MovieType.TopRated:
				Url = _baseUrl + "movie/top_rated?api_key=" + _apiKey + _pageString + page;
				break;
			case MovieType.NowPlaying:
				Url = _baseUrl + "movie/now_playing?api_key=" + _apiKey + _pageString + page;
				break;
			case MovieType.Popular:
				Url = _baseUrl + "movie/popular?api_key=" + _apiKey + _pageString + page;
				break;
			case MovieType.Similar:
				Url = _baseUrl + "movie/" + movieId + "/similar?api_key=" + _apiKey + _pageString + page;
				break;
			case MovieType.Upcoming:
				Url = _baseUrl + "movie/upcoming?api_key=" + _apiKey + "&language=en-US";
				break;
			case MovieType.TVLatest:
				Url = _baseUrl + "tv/airing_today?api_key=" + _apiKey + "&language=en-US";
				break;

			}
			try 
			{
				HttpResponseMessage result = await client.GetAsync (Url, CancellationToken.None);

				if (result.IsSuccessStatusCode) 
				{

					string content = await result.Content.ReadAsStringAsync ();
					JObject jresponse = JObject.Parse (content);
					var jarray = jresponse ["total_pages"];
					MovieList = GetJsonData (content);
					//return a ObservableCollection to fill a list of movies
					return MovieList;
				
				}
			} 
				catch (Exception ex) 
			{
				//Model Error
				Debug.WriteLine (ex);

			}
			//Server Error or no internet connection.
			return new ObservableCollection<Movie> ();
		}
		//GET movies from service
		public static async Task<ObservableCollection<Movie>> GetMoviesAsync (MovieType type, int page = 1, int movieId = 0)
		{
			var client = new HttpClient ();

			string Url = "";

			switch (type) 
			{

			case MovieType.TopRated:
				Url = _baseUrl + "movie/top_rated?api_key=" + _apiKey + _pageString + page;
				break;
			case MovieType.NowPlaying:
				Url = _baseUrl + "movie/now_playing?api_key=" + _apiKey + _pageString + page;
				break;
			case MovieType.Popular:
				Url = _baseUrl + "movie/popular?api_key=" + _apiKey + _pageString + page;
				break;
			case MovieType.Similar:
				Url = _baseUrl + "movie/" + movieId + "/similar?api_key=" + _apiKey + _pageString + page;
				break;
			case MovieType.Upcoming:
				Url = _baseUrl + "movie/upcoming?api_key=" + _apiKey + "&language=en-US";
				break;
			case MovieType.TVLatest:
				Url = _baseUrl + "tv/airing_today?api_key=" + _apiKey + "&language=en-US";
				break;

			}
			try {
				HttpResponseMessage result = await client.GetAsync (Url, CancellationToken.None);

				if (result.IsSuccessStatusCode) 
				{

					string content = await result.Content.ReadAsStringAsync ();
					JObject jresponse = JObject.Parse (content);
					var jarray = jresponse ["total_pages"];
					MovieList = GetJsonData (content);
					AddListToLocalDB (MovieList, type);
					//return a ObservableCollection to fill a list of movies
					return MovieList;


				}
			} catch (WebException we) 
			{
				//Server Error or no internet connection.
				Debug.WriteLine (we);
				return GetListFromLocalDB (GetCustomIdFromType(type.ToString()));

			} 
			catch (Exception ex) {
				//Model Error
				Debug.WriteLine (ex);
				return GetListFromLocalDB (GetCustomIdFromType (type.ToString ()));
			}

			return new ObservableCollection<Movie> ();;
		}

		static string GetCustomIdFromType (string type)
		{
			var retId = "";
			try {
				
				using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
					var task = Task.Run (() => {
						// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);

						var query = db.Query<CustomList> ("SELECT [id] FROM [CustomList] WHERE [name] = '" + type + "'");
						retId = query.FirstOrDefault ().id.ToString ();
					});
					task.Wait ();
				}
			} catch (Exception ex) 
			{
				Debug.WriteLine (ex.Message);
			}

			return retId;

		}

		static ObservableCollection<Movie> GetListFromLocalDB (string id)
		{
			var returnList = new ObservableCollection<Movie> ();
			using (var db = new SQLite.SQLiteConnection (MovieService.Database)) {
				var task = Task.Run (() => 
				{
					// there is a sqllite bug here https://forums.xamarin.com/discussion/52822/sqlite-error-deleting-a-record-no-primary-keydb.Delete<Movie> (movieDetail);

						var query = db.Query<Movie> ("SELECT * FROM [Movie] WHERE [CustomListID] = " + id);

						foreach (var movie in query) 
						{
							var item = new Movie ();
							item.id = movie.id;
							item.name = movie.name;
							item.BackdropPath = movie.BackdropPath;
							item.CustomListID = movie.CustomListID;
							item.Favorite = movie.Favorite;
							item.HighResPosterPath = movie.HighResPosterPath;
							item.OriginalLanguage = movie.OriginalLanguage;
							item.Overview = movie.Overview;
							item.Popularity = movie.Popularity;
							item.PosterPath = movie.PosterPath;
							item.ReleaseDate = movie.ReleaseDate;
							item.VoteAverage = movie.VoteAverage;
							item.UserReview = movie.UserReview;
							item.UserRating = movie.UserRating;
							item.OriginalId = movie.OriginalId;
							item.cloudId = movie.cloudId;
							item.order = movie.order;
							returnList.Add (item);
						}


				});
				task.Wait ();

				return returnList;
			}
		
		}

		static void AddListToLocalDB (ObservableCollection<Movie> movieList, MovieType type)
		{

			var listItem = new CustomList ();
			listItem.shared = false;
			listItem.order = 0;
			listItem.custom = false;
			listItem.name = type.ToString ();

			using (var db = new SQLiteConnection (MovieService.Database)) {
				try {
					var list = db.Query<CustomList> ("SELECT id FROM [CustomList] WHERE name = '" + listItem.name + "'");
					if (list.Count > 0) {
						var id = list.FirstOrDefault ().id;
						db.Query<Movie> ("DELETE FROM [Movie] WHERE CustomListID = " + id);
						db.Query<CustomList> ("DELETE FROM [CustomList] WHERE name = '" + listItem.name + "'");
					}
					db.InsertOrReplace (listItem, typeof (CustomList));
					string sql = "select last_insert_rowid()";
					var scalarValue = db.ExecuteScalar<string> (sql);
					int value = scalarValue == null ? 0 : Convert.ToInt32 (scalarValue);
					foreach (var movie in movieList) {
						if (value > 0) {
							movie.CustomListID = value;
							db.InsertOrReplace (movie, typeof (Movie));

						}
					}
				} catch (Exception ex) {
					try {
						Debug.WriteLine (ex);
						using (var conn = new SQLite.SQLiteConnection (MovieService.Database)) 
						{
							conn.CreateTable<Movie> (CreateFlags.ImplicitPK | CreateFlags.AutoIncPK);
							conn.CreateTable<CustomList> (CreateFlags.ImplicitPK | CreateFlags.AutoIncPK);
						}
						db.InsertOrReplace (listItem, typeof (CustomList));
						string sql = "select last_insert_rowid()";
						var scalarValue = db.ExecuteScalar<string> (sql);
						int value = scalarValue == null ? 0 : Convert.ToInt32 (scalarValue);
						foreach (var movie in movieList) {
							if (value > 0) {
								movie.CustomListID = value;
								db.InsertOrReplace (movie, typeof (Movie));

							}
						}
					} catch (Exception e) {
						Debug.WriteLine (e);
					}
				}

			}
		}

		public static async Task<ObservableCollection<CastCrew>> MovieCreditsAsync (string query)
		{
			var client = new HttpClient ();
			string Url = "https://api.themoviedb.org/3/movie/" + query + "/credits?api_key=" + _apiKey;
			try 
			{
				HttpResponseMessage result = await client.GetAsync (Url, CancellationToken.None);
				if (result.IsSuccessStatusCode) 
				{
					
						string content = await result.Content.ReadAsStringAsync ();
						JObject jresponse = JObject.Parse (content);
						var jarray = jresponse ["cast"];
						var castCrew = GetCastJsonData (content);
						//return a ObservableCollection to fill a list of movies
						return castCrew;

					
				}
			}
			catch (Exception ex) 
			{
				//Model Error
				Debug.WriteLine (ex);

			}
			//Server Error or no internet connection.
			return new ObservableCollection<CastCrew>();

		}
		public static async Task<ObservableCollection<Movie>> MovieSearch (string query)
		{
			var client = new HttpClient ();
			string Url = "https://api.themoviedb.org/3/search/movie?api_key=" + _apiKey + "&language=en-US&query=" + query;
			try 
			{
				HttpResponseMessage result = await client.GetAsync (Url, CancellationToken.None);
				if (result.IsSuccessStatusCode) 
				{
				
					string content = await result.Content.ReadAsStringAsync ();
					JObject jresponse = JObject.Parse (content);
					var jarray = jresponse ["total_pages"];
					MovieList = GetJsonData (content);
					//return a ObservableCollection to fill a list of movies
					return MovieList;

				
				}
			} 
			catch (Exception ex) 
			{
				//Model Error
				Debug.WriteLine (ex);

			}
			//Server Error or no internet connection.
			return new ObservableCollection<Movie>();
		}

		static ObservableCollection<CastCrew> GetCastJsonData (string content)
		{

			JObject jresponse = JObject.Parse (content);
			var jarray = jresponse ["cast"];

			var castList = new ObservableCollection<CastCrew> ();
			try {
				foreach (var jObj in jarray) {
					var castCrew = new CastCrew ();
					castCrew.Character = (string)jObj ["character"];
					castCrew.ProfilePath = (jObj ["profile_path"] == null) ? "" : (string)jObj ["profile_path"];
					castCrew.Actor = (string)jObj ["name"];
				
					castList.Add (castCrew);
				}
			} catch (Exception e) {

				Debug.WriteLine (e.Message);

			}
			return castList;


		}

		static ObservableCollection<Movie> GetJsonData (string content)
		{

			JObject jresponse = JObject.Parse (content);
			var jarray = jresponse ["results"];

			var movieList = new ObservableCollection<Movie> ();
			try {
				foreach (var jObj in jarray) {
					var newMovie = new Movie ();
					newMovie.name = (string)jObj ["title"];
					newMovie.PosterPath =(string) jObj ["poster_path"];//(jObj ["poster_path"] == null) ? "" : (string)jObj ["poster_path"];
					newMovie.HighResPosterPath = (string)jObj ["poster_path"];// (jObj ["poster_path"] == null) ? "" : (string)jObj ["poster_path"];
					newMovie.OriginalId= (int)jObj ["id"];
					newMovie.id = null;
					newMovie.Overview = (string)jObj ["overview"];
					newMovie.VoteCount = (double)jObj ["vote_count"];
					newMovie.ReleaseDate = (string)jObj ["release_date"];
					newMovie.VoteAverage = (float)jObj ["vote_average"];
					newMovie.Adult = (bool)jObj ["adult"];//(jObj ["adult"] == null) ? false : (bool)jObj ["adult"];
					newMovie.BackdropPath = (string)jObj ["backdrop_path"];//(jObj ["backdrop_path"] == null) ? "" : (string)jObj ["backdrop_path"];
					//newMovie.genre_ids = jObj ["genre_ids"].ToObject<List<string>>();
					movieList.Add (newMovie);
				}
			} catch (Exception e) {

				Debug.WriteLine (e.Message);

			}
			return movieList;


		}

		//POST Example:
		public static async Task<bool> PostFavoriteMovieAsync (Movie movie, string sessionId)
		{
			try {
				var client = new HttpClient ();
				//_sessionId = string with the movie database session.
				string tokenUrl = _baseUrl + "account/id/favorite?" + _apiKey + sessionId;
				string postBody = JsonConvert.SerializeObject (new Favorite {
					favorite = !movie.Favorite,
					media_id = (int)movie.id
				});

				client.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
				HttpResponseMessage response = await client.PostAsync (tokenUrl,
					new StringContent (postBody, Encoding.UTF8, "application/json"));

				if (response.IsSuccessStatusCode)
					return true;
				else
					return false;
			} catch (Exception ex) {
				Debug.WriteLine (ex.Message);
				return false;
			}
		}
	}
}
