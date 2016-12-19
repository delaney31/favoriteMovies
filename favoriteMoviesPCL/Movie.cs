using System;
using SQLite;

namespace FavoriteMoviesPCL
{

	[Table ("Movies")]
	public class Movie :ICustomList
	{
		public string Name { get; set; }
		public string HighResPosterPath { get; set; }
		public string PosterPath { get; set; }
		[PrimaryKey]
		public int ?Id { get; set; }
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
		public string UserReview { get; set; }
		public int UserRating{ get; set; }
		[AutoIncrement]
		public int Order { get; set; }

	}
	public class MoviePOCO
	{
		public string Id { get; set; }
	}
	[Table ("CustomList")]
	public class CustomList:ICustomList
	{
		[PrimaryKey, AutoIncrement]
		public int? Id { get; set; }
		[MaxLength (50)]
		public string Name { get; set; }
		[AutoIncrement]
		public int Order { get; set;}
	}

	public class CastCrew
	{
		public string Character { get; set; }
		public string Actor { get; set; }
		public string ProfilePath { get; set; }
	}
}

