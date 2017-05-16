using System;
namespace FavoriteMovies
{
	public class Constants
	{
		// Azure app-specific connection string and hub path
		public const string ConnectionString = "Endpoint=sb://moviefriendsnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=g73Rc6nUEr2QRIKW0M2OfI/p+tekd236g5x2ITGgvRA=";
		public const string NotificationHubPath = "moviefriends";
		public const string ModifyFollowerNotification = "FollowerModifiedNotification";
		public const string ModifyFollowerNotificationReceived = "FollowerModifiedNotificationReceived:";
		public const string ChangedOrientationToLandscape = "ChangedOrientationToLandscape";
		public const string ChangedOrientationToLandscapeReceived = "ChangedOrientationToLandscapeReceived";
		public const string CurrentUserSetNotification = "CurrentUserSetNotification";
		public const string CurrentUserSetNotificationReceived = "CurrentUserSetNotificationReceived:";	
		public const string CustomListChange = "CustomListChange";
		public const string CustomListChangeReceived = "CustomListChangeReceived:";
		//public const string NotificationHubPath = "Endpoint=sb://moviefriendsnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=g73Rc6nUEr2QRIKW0M2OfI/p+tekd236g5x2ITGgvRA=";
	}
}
