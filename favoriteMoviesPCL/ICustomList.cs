namespace FavoriteMoviesPCL
{
	public interface ICustomList
	{
		int ?Id { get; set; }
		string Name { get; set; }
		int Order { get; set; }
	}
}