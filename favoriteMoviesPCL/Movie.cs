using System;


namespace FavoriteMoviesPCL
{
	public class Movie : IMovie
	{
		public Movie ()
		{
		}

		public string Title { get; set; }
		public string HighResPosterPath { get; set; }
		public string PosterPath { get; set; }
		public int Id { get; set; }
		public string Overview { get; set; }
		public double VoteCount { get; set; }
		public DateTime ReleaseDate { get; set;}
		public float VoteAverage { get; set; }
		public bool Favorite { get; set; }

	}
}

