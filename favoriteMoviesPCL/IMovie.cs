using System;


namespace FavoriteMoviesPCL
{
	public interface IMovie
	{
		string Title { get; set; }
		string HighResPosterPath { get; set; }
		string PosterPath { get; set; }
		int Id { get; set; }
		string Overview { get; set; }
		double VoteCount { get; set; }
		DateTime ReleaseDate { get; set; }
		float VoteAverage { get; set; }

	}
}
