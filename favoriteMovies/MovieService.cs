using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FavoriteMovies
{
	public class MovieService
	{
		public MovieService ()
		{
		}
		const string _baseUrl = "http://api.themoviedb.org/3/";
		const string _pageString = "&page=";

		//GET Example
		public static async Task<ObservableCollection<Movie>> GetTopRatedMoviesAsync (int page = 1)
		{
			HttpClient client = new HttpClient ();
			//_apiKey = themoviedb api key, page = page size 1 = first 20 movies;
			string topRatedUrl = _baseUrl + "movie/top_rated?" + _apiKey + _pageString + page;
			HttpResponseMessage result = await client.GetAsync (topRatedUrl, CancellationToken.None);

			if (result.IsSuccessStatusCode) {
				try {
					string content = await result.Content.ReadAsStringAsync ();
					ObservableCollection<Movie> MovieList = GetJsonData (content);
					//return a ObservableCollection to fill a list of top rated movies
					return MovieList;

				} catch (Exception ex) {
					//Model Error
					Console.WriteLine (ex);
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
				newMovie.PosterPath = _baseImgUrl + (string)jObj ["poster_path"];
				newMovie.HighResPosterPath = _baseImgUrl + (string)jObj ["poster_path"];
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
				Console.WriteLine (ex.Message);
				return false;
			}
		}
	}
}
