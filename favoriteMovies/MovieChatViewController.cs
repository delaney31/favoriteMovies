using System;
using System.Collections.Generic;
using FavoriteMoviesPCL;
using Foundation;
using JSQMessagesViewController;
using UIKit;

namespace FavoriteMovies
{
	public class MovieChatViewController:MessagesViewController
	{
		MessagesBubbleImage outgoingBubbleImageData;
		MessagesBubbleImage incomingBubbleImageData;
		List<Message> messages = new List<Message> ();

		User sender = new User { id = 1234, username = "Your Name Here" };
		User friend = new User { id = 12345, username = "Tom Anderson" };

		MessageFactory messageFactory = new MessageFactory ();
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// You must set your senderId and display name
			SenderId = "123";
			SenderDisplayName = sender.username;

			// These MessagesBubbleImages will be used in the GetMessageBubbleImageData override
			var bubbleFactory = new MessagesBubbleImageFactory ();
			outgoingBubbleImageData = bubbleFactory.CreateOutgoingMessagesBubbleImage (UIColorExtensions.MessageBubbleLightGrayColor);
			incomingBubbleImageData = bubbleFactory.CreateIncomingMessagesBubbleImage (UIColorExtensions.MessageBubbleBlueColor);

			// Remove the Avatars
			CollectionView.CollectionViewLayout.IncomingAvatarViewSize = CoreGraphics.CGSize.Empty;
			CollectionView.CollectionViewLayout.OutgoingAvatarViewSize = CoreGraphics.CGSize.Empty;
		}

		//To specify the number of total cells to display override IUICollectionViewDataSource.GetItemsCount`. 

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return 2;
		}

		//To populate the view with messages, the message data that corresponds to the specified item at indexPath must be returned from IMessagesCollectionViewDataSource.GetMessageData.

		public override IMessageData GetMessageData (MessagesCollectionView collectionView, NSIndexPath indexPath)
		{
			if (indexPath.Row == 0)
				return Message.Create ("123", "Me", "Ping");

			return Message.Create ("456", "You", "Pong");
		}

		//The IMessagesCollectionViewDataSource.GetMessageBubbleImageData method asks the data source for the message bubble image data that corresponds to the specified message data item at indexPath in the view. 

		public override IMessageBubbleImageDataSource GetMessageBubbleImageData (MessagesCollectionView collectionView, NSIndexPath indexPath)
		{
			if (indexPath.Row == 0)
				return outgoingBubbleImageData;

			return incomingBubbleImageData;
		}

		//Although the example does not display avatars, it is required to implement IMessagesCollectionViewDataSource.GetAvatarImageData.Simply return null. 

		public override IMessageAvatarImageDataSource GetAvatarImageData (MessagesCollectionView collectionView, NSIndexPath indexPath)
		{
			return null;
		}

		//At this point the sample will run displaying the two messages, however the text for the sender is white on a light background.To change text color override IUICollectionViewDataSource.GetCell and call the base class to get the MessagesCollectionViewCell.Then change the TextView.TextColor to Black. 

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = base.GetCell (collectionView, indexPath) as MessagesCollectionViewCell;

			// Override GetCell to make modifications to the cell
			// In this case darken the text for the sender
			if (indexPath.Row == 0)
				cell.TextView.TextColor = UIColor.Black;

			return cell;
		}

		public void ReloadMessagesView ()
		{
			this.CollectionView.ReloadData ();
		}

	}


}
