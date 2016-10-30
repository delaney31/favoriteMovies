using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FavoriteMoviesPCL
{
	public static class MovieService
	{
		
		const string _baseUrl = "https://api.themoviedb.org/3/";
		const string _pageString = "&page=";
		static string _baseImgUrl;
		static string _apiKey = "ab41356b33d100ec61e6c098ecc92140";
		static string _sessionId = "";
		public static ObservableCollection<Movie> MovieList;

		public enum MovieType
		{
			TopRated =0,
			NowPaying =1,
			Popular =2,
			Similar =3
		}

		//GET Example
		public static async Task<ObservableCollection<Movie>> GetMoviesAsync (MovieType type,int page = 1)
		{
			HttpClient client = new HttpClient ();
			string Url = "";

			switch (type) {

				case MovieType.TopRated:
					Url = _baseUrl + "movie/top_rated?api_key=" + _apiKey + _pageString + page;              
					break;
				case MovieType.NowPaying:
					Url = _baseUrl + "movie/now_playing?api_key=" + _apiKey + _pageString + page;
					break;
				case MovieType.Popular:
					Url = _baseUrl + "movie/popular?api_key=" + _apiKey + _pageString + page;
					break;
				case MovieType.Similar:
					Url = _baseUrl + "movie/simular?api_key=" + _apiKey + _pageString + page;
				break;
			}

			HttpResponseMessage result = await client.GetAsync (Url, CancellationToken.None);

			if (result.IsSuccessStatusCode) {
				try {
					string content = await result.Content.ReadAsStringAsync ();
					MovieList = GetJsonData (content);
					//return a ObservableCollection to fill a list of movies
					return MovieList;

				} catch (Exception ex) {
					//Model Error
					Debug.WriteLine (ex);
					return null;
				}
			}
			//Server Error or no internet connection.
			return null;
		}


		static ObservableCollection<Movie> GetJsonData (string content)
		{

			JObject jresponse = JObject.Parse (content);
			var jarray = jresponse ["results"];
			ObservableCollection<Movie> movieList = new ObservableCollection<Movie> ();

			foreach (var jObj in jarray) {
				Movie newMovie = new Movie ();
				newMovie.Title = (string)jObj ["title"];
				newMovie.PosterPath = new Uri(_baseImgUrl + (string)jObj ["poster_path"]);
				newMovie.HighResPosterPath = new Uri(_baseImgUrl + (string)jObj ["poster_path"]);
				newMovie.Id = (int)jObj ["id"];
				newMovie.Overview = (string)jObj ["overview"];
				newMovie.VoteCount = (double)jObj ["vote_count"];
				newMovie.ReleaseDate = (DateTime)jObj ["release_date"];
				newMovie.VoteAverage = (float)jObj ["vote_average"];
				movieList.Add (newMovie);
			}
			return movieList;
		}

		//POST Example:
		public static async Task<bool> PostFavoriteMovieAsync (Movie movie)
		{
			try {
				HttpClient client = new HttpClient ();
				//_sessionId = string with the movie database session.
				string tokenUrl = _baseUrl + "account/id/favorite?" + _apiKey + _sessionId;
				string postBody = JsonConvert.SerializeObject (new Favorite {
					favorite = !movie.Favorite,
					media_id = movie.Id,
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
