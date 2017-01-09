using System;
namespace FavoriteMovies
{
	public class Constants
	{
		// Azure app-specific connection string and hub path
		public const string ConnectionString = "Endpoint=sb://moviefriendsnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=g73Rc6nUEr2QRIKW0M2OfI/p+tekd236g5x2ITGgvRA=";
		public const string NotificationHubPath = "moviefriends";
		//public const string NotificationHubPath = "Endpoint=sb://moviefriendsnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=g73Rc6nUEr2QRIKW0M2OfI/p+tekd236g5x2ITGgvRA=";
	}
}
