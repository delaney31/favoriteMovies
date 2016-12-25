namespace FavoriteMoviesPCL
{
	public interface ICustomList
	{
		int ?id { get; set; }
		string name { get; set; }
		int order { get; set; }
		bool shared { get; set; }
	}
}