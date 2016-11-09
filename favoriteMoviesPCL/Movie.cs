using System;
using SQLite;

namespace FavoriteMoviesPCL
{
	
	[Table ("DiaryEntry")]
	public class Movie : IMovie
	{
		
		public string Title { get; set; }
		public string HighResPosterPath { get; set; }
		public string PosterPath { get; set; }
		[PrimaryKey]
		public int Id { get; set; }
		public string Overview { get; set; }
		public double VoteCount { get; set; }
		public DateTime ReleaseDate { get; set;}
		public float VoteAverage { get; set; }
		public bool Favorite { get; set; }

	}
	public class MoviePOCO
	{
		public string Id { get; set; }
	}

}

