using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FavoriteMoviesPCL;
using Foundation;
using JSQMessagesViewController;
using Plugin.Messaging;
using UIKit;

namespace FavoriteMovies
{
	public class MovieChatViewController:MessagesViewController
	{
		MessagesBubbleImage outgoingBubbleImageData,incomingBubbleImageData;
		List<Message> messages = new List<Message> ();
		User sender = ColorExtensions.CurrentUser;
		UserFriendsCloud friend;

		MessageFactory messageFactory = new MessageFactory ();
		ContactCard row;

		public MovieChatViewController (ContactCard row, UserFriendsCloud userToChatWith)
		{
			this.row = row;
			this.friend = userToChatWith;
		}
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			NewsFeedTableSource.HideTabBar ((UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.TabController, View.BackgroundColor);
		}
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			//Title = "Xamarin Chat";

			// You must set your senderId and display name
			SenderId = sender.Id;
			SenderDisplayName = sender.username;

			// These MessagesBubbleImages will be used in the GetMessageBubbleImageData override
			var bubbleFactory = new MessagesBubbleImageFactory ();
			outgoingBubbleImageData = bubbleFactory.CreateOutgoingMessagesBubbleImage (UIColorExtensions.MessageBubbleLightGrayColor);
			incomingBubbleImageData = bubbleFactory.CreateIncomingMessagesBubbleImage (UIColorExtensions.MessageBubbleBlueColor);

			// Remove the AccessoryButton as we will not be sending pics
			InputToolbar.ContentView.LeftBarButtonItem = null;

			// Remove the Avatars
			CollectionView.CollectionViewLayout.IncomingAvatarViewSize = CoreGraphics.CGSize.Empty;
			CollectionView.CollectionViewLayout.OutgoingAvatarViewSize = CoreGraphics.CGSize.Empty;

			// Load some messagees to start
			messages.Add (new Message (sender.Id, sender.username, NSDate.DistantPast, "Hi There"));
			messages.Add (new Message (friend.Id, friend.friendusername, NSDate.DistantPast, "I'm sorry, my responses are limited. You must ask the right questions."));
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			//NewsFeedTableSource.HideTabBar ((UIApplication.SharedApplication.Delegate as AppDelegate).rootViewController.TabController);
		
				var cell = base.GetCell (collectionView, indexPath) as MessagesCollectionViewCell;
			try {
				if (cell == null) 
				{
					cell = new MessagesCollectionViewCell ();

				}
				// Override GetCell to make modifications to the cell
				// In this case darken the text for the sender
				var message = messages [indexPath.Row];
				if (message.SenderId == SenderId)
					cell.TextView.TextColor = UIColor.Black;
			} catch (Exception e) 
			{
				Console.WriteLine (e.Message);
			}
			return cell;
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return messages.Count;
		}

		public override IMessageData GetMessageData (MessagesCollectionView collectionView, NSIndexPath indexPath)
		{
			return messages [indexPath.Row];
		}

		public override IMessageBubbleImageDataSource GetMessageBubbleImageData (MessagesCollectionView collectionView, NSIndexPath indexPath)
		{
			var message = messages [indexPath.Row];
			if (message.SenderId == SenderId)
				return outgoingBubbleImageData;
			return incomingBubbleImageData;

		}

		public override IMessageAvatarImageDataSource GetAvatarImageData (MessagesCollectionView collectionView, NSIndexPath indexPath)
		{
			return null;
		}

		public override async void PressedSendButton (UIButton button, string text, string senderId, string senderDisplayName, NSDate date)
		{
			SystemSoundPlayer.PlayMessageSentSound ();

			var message = new Message (SenderId, SenderDisplayName, NSDate.Now, text);
			messages.Add (message);
			var smsMessenger = CrossMessaging.Current.SmsMessenger;
			if (smsMessenger.CanSendSms)
				//smsMessenger.SendSms ("+27213894839493", "Well hello there from Xam.Messaging.Plugin");
				smsMessenger.SendSms ("+18134937008", text);

			//var emailMessenger = CrossMessaging.Current.EmailMessenger;
			//if (emailMessenger.CanSendEmail) {
			//	// Send simple e-mail to single receiver without attachments, bcc, cc etc.
			//	//emailMessenger.SendEmail ("to.plugins@xamarin.com", "Xamarin Messaging Plugin", "Well hello there from Xam.Messaging.Plugin");
			//	emailMessenger.SendEmail ("tldelaney@gmail.com", "Xamarin Messaging Plugin", "Well hello there from Xam.Messaging.Plugin");
			//	// Alternatively use EmailBuilder fluent interface to construct more complex e-mail with multiple recipients, bcc, attachments etc. 
			//	var email = new EmailMessageBuilder ()
			//	  .To ("tldelaney@gmail.com")
			//	  .Cc ("tldelaney@gmail.com")
			//	  //.Bcc (new [] { "bcc1.plugins@xamarin.com", "bcc2.plugins@xamarin.com" })
			//	  .Subject ("Xamarin Messaging Plugin")
			//	  .Body ("Well hello there from Xam.Messaging.Plugin")
			//	  .Build ();

			//	emailMessenger.SendEmail (email);
			//}
			FinishSendingMessage (true);

			await Task.Delay (500);

			await SimulateDelayedMessageReceived ();
		}

		async Task SimulateDelayedMessageReceived ()
		{
			ShowTypingIndicator = true;

			ScrollToBottom (true);

			var delay = System.Threading.Tasks.Task.Delay (1500);
			var message = await messageFactory.CreateMessageAsync (friend);
			await delay;

			messages.Add (message);

			ScrollToBottom (true);

			SystemSoundPlayer.PlayMessageReceivedSound ();

			FinishReceivingMessage (true);
		}

		//public void ReloadMessagesView ()
		//{
		//	this.CollectionView.ReloadData ();
		//}

	}


}
