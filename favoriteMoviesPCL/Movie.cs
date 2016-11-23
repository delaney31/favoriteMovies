using System;
using SQLite;

namespace FavoriteMoviesPCL
{

	[Table ("Movies")]
	public class Movie 
	{
		public string Title { get; set; }
		public string HighResPosterPath { get; set; }
		public string PosterPath { get; set; }
		[PrimaryKey]
		public int Id { get; set; }
		public int? CustomListID { get; set;}
		public string Overview { get; set; }
		public double VoteCount { get; set; }
		public DateTime? ReleaseDate { get; set; }
		public float VoteAverage { get; set; }
		public bool Favorite { get; set; }
		public bool Adult { get; set; }
		//public List<string> genre_ids { get; set; }
		public string OriginalTitle { get; set; }
		public string OriginalLanguage { get; set; }
		public string BackdropPath { get; set; }
		public string Popularity { get; set; }
		public bool Video { get; set; }

	}
	public class MoviePOCO
	{
		public string Id { get; set; }
	}
	[Table ("CustomList")]
	public class CustomList
	{
		[PrimaryKey, AutoIncrement]
		public int? Id { get; set; }
		[MaxLength (50)]
		public string Name { get; set; }
	}
}

