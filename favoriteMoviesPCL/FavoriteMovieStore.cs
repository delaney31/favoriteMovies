using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using SQLitePCL;

namespace FavoriteMoviesPCL
{
	public class FavoriteMovieStore
	{

		string path;
		readonly SQLiteAsyncConnection connection;

		public FavoriteMovieStore (string folder, string filename)
		{
			Batteries.Init ();

			path = System.IO.Path.Combine (folder, filename);

			connection = new SQLiteAsyncConnection (path);

			connection.CreateTableAsync<Movie> ().Wait ();
		}
		public Task SaveEntryAsync (Movie entry)
		{
			//if (entry.Id == -1)
				return connection.InsertAsync (entry);
		//	else
			//	return connection.InsertOrReplaceAsync (entry);
		}

		public Task DeleteEntryAsync (Movie entry)
		{
			return connection.DeleteAsync (entry);
		}

		public Task<List<Movie>> GetEntriesAsync (string title)
		{
			return connection.Table<Movie> ().Where (d => d.name == title).ToListAsync ();
		}

		public Task<Movie> GetEntry (int id)
		{
			return connection.Table<Movie> ().Where (d => d.id == id).FirstOrDefaultAsync ();
		}

	}
}
