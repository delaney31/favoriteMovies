using System;
using Newtonsoft.Json;
using SQLite;

namespace FavoriteMoviesPCL
{

	[Table ("Movies")]
	public class Movie :ICustomList
	{
		public string name { get; set; }
		public string HighResPosterPath { get; set; }
		public string PosterPath { get; set; }
		[PrimaryKey, AutoIncrement]
		public int ?id { get; set; }
		public int OriginalId { get; set;}
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
		public bool shared { get; set; }
		[AutoIncrement]
		public int order { get; set; }
		public bool deleted { get; set;}
		//public Version version { get; set;}
		public DateTime createdAt { get; set;}
		public DateTime updatedAt { get; set;}

	}
	public class MoviePOCO
	{
		public string Id { get; set; }
	}
	[Table ("CustomList")]
	public class CustomList:ICustomList
	{
		[PrimaryKey, AutoIncrement]
		public int? id { get; set; }
		[MaxLength (50)]
		public string name { get; set; }
		[AutoIncrement]
		public int order { get; set;}
		public bool shared { get; set;}
		//public Version version { get; set;}
		public DateTime createdAt { get; set;}
		public DateTime updatedAt { get; set;}
		public bool deleted { get; set;}
		public int userid { get; set;}
	}

	public class CastCrew
	{
		public string Character { get; set; }
		public string Actor { get; set; }
		public string ProfilePath { get; set; }
	}

	[Table ("FeedItem")]
	public class FeedItem
	{
		[PrimaryKey, AutoIncrement]
		public int? id { get; set; }
		public string Title { get; set; }
		public string Link { get; set; }
		public string PubDate { get; set; }
		public string Creator { get; set; }
		public string Category { get; set; }
		public string Description { get; set; }
		public string Content { get; set; }
		public string ImageLink { get; set; }
		public int likes { get; set;}
		public string like { get; set; }
		public int? CommentID { get; set; }
		public string Image { get; set; }
		//public Version version { get; set; }
		public DateTime createdAt { get; set; }
		public DateTime updatedAt { get; set; }
		public bool deleted { get; set; }


	}

	public class ToDoItem
	{
		public string Id { get; set; }

		[JsonProperty (PropertyName = "text")]
		public string Text { get; set; }

		[JsonProperty (PropertyName = "complete")]
		public bool Complete { get; set; }
	}

	public class Post
	{		
		public string Id { get; set; }
		[JsonProperty(PropertyName ="feedid")]
		public string FeedId { get; set; }
		[JsonProperty (PropertyName = "userid")]
		public string UserId { get; set; }
		[JsonProperty (PropertyName = "like")]
		public string Like { get; set; }

	}
	[Table ("Comments")]
	public class Comments
	{
		[PrimaryKey, AutoIncrement]
		public int? id { get; set; }
		[MaxLength (500)]
		public string Comment { get; set;}
		//public Version version { get; set; }
		public DateTime createdAt { get; set; }
		public DateTime updatedAt { get; set; }
		public bool deleted { get; set; }
	}

	[Table ("UserFriends")]
	public class UserFriends
	{
		[PrimaryKey, AutoIncrement]
		public int? id { get; set; }
		public int? CustomListID { get; set; }
		public int? friendid { get; set;}
		public int? userid { get; set;}
		//public Version version { get; set; }
		public DateTime createdAt { get; set; }
		public DateTime updatedAt { get; set; }
		public bool deleted { get; set; }
	}

	[Table ("User")]
	public class User

	{
		[PrimaryKey, AutoIncrement]
		public int? id { get; set; }
		public string lastname { get; set;}
		public string firstname { get; set;}
		public string email { get; set;}
		public string profilepic { get; set;}
		public string username { get; set;}
		public string city { get; set; }
		public string state { get; set;}
		public string country { get; set;}
		public string password { get; set;}
		//public Version version { get; set; }
		public DateTime createdAt { get; set; }
		public DateTime updatedAt { get; set; }
		public bool deleted { get; set; }

	}
}

