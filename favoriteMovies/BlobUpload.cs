using System;
using System.Threading.Tasks;
using Foundation;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using UIKit;

namespace FavoriteMovies
{
	public class BlobUpload
	{
		
		public static async Task createContainerAndUpload (byte[] image)
		{
			// Retrieve storage account from connection string.
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse (ColorExtensions.AZURE_STORAGE_CONNECTION_STRING);

			// Create the blob client.
			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient ();

			// Retrieve reference to a previously created container.
			CloudBlobContainer container = blobClient.GetContainerReference (ColorExtensions.containerName);

			// Create the container if it doesn't already exist.
			await container.CreateIfNotExistsAsync ();

			// Retrieve reference to a blob.
			CloudBlockBlob blockBlob = container.GetBlockBlobReference (ColorExtensions.CurrentUser.Id);

			// Create the blob with the image"
			await blockBlob.UploadFromByteArrayAsync (image, 0, image.Length);
		}
		public static async Task DeleteBlob ()
		{
			// Retrieve storage account from connection string.
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse (ColorExtensions.AZURE_STORAGE_CONNECTION_STRING);

			// Create the blob client.
			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient ();

			// Retrieve reference to a previously created container.
			CloudBlobContainer container = blobClient.GetContainerReference (ColorExtensions.containerName);


			// Retrieve reference to a blob.
			CloudBlockBlob blockBlob = container.GetBlockBlobReference (ColorExtensions.CurrentUser.Id);

			// Delete
			await blockBlob.DeleteIfExistsAsync ();
		}
		public static async Task<UIImage> getProfileImage (string userid, float width, float height)
		{
			try {
				// Retrieve storage account from connection string.
				CloudStorageAccount storageAccount = CloudStorageAccount.Parse (ColorExtensions.AZURE_STORAGE_CONNECTION_STRING);
				// Create the blob client.
				CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient ();

				// Retrieve reference to a previously created container.
				CloudBlobContainer container = blobClient.GetContainerReference (ColorExtensions.containerName);

				// Create the container if it doesn't already exist.
				await container.CreateIfNotExistsAsync ();

				// Retrieve reference to a blob named "myblob".
				CloudBlockBlob blockBlob = container.GetBlockBlobReference (userid);

				await blockBlob.FetchAttributesAsync ();

				byte [] target = new byte [blockBlob.Properties.Length];

				await blockBlob.DownloadToByteArrayAsync (target, 0);

				var data = NSData.FromArray (target);

				return ColorExtensions.ResizeImage (UIImage.LoadFromData (data), width, height);
			} catch (Exception ex) {
				Console.Write (ex.Message);
				return ColorExtensions.ResizeImage (UIImage.FromBundle ("1481507483_compose.png"), width, height);
			}

		}
		public static async Task<NSUrl> getProfileNSUrl (string userid, float width, float height)
		{
			try {
				// Retrieve storage account from connection string.
				CloudStorageAccount storageAccount = CloudStorageAccount.Parse (ColorExtensions.AZURE_STORAGE_CONNECTION_STRING);
				// Create the blob client.
				CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient ();

				// Retrieve reference to a previously created container.
				CloudBlobContainer container = blobClient.GetContainerReference (ColorExtensions.containerName);

				// Create the container if it doesn't already exist.
				await container.CreateIfNotExistsAsync ();

				// Retrieve reference to a blob named "myblob".
				CloudBlockBlob blockBlob = container.GetBlockBlobReference (userid);

				await blockBlob.FetchAttributesAsync ();

				byte [] target = new byte [blockBlob.Properties.Length];

				await blockBlob.DownloadToByteArrayAsync (target, 0);

				var data = NSData.FromArray (target);

		
				var nsUrlString = new NSString(UIImage.LoadFromData (data).AsJPEG(), NSStringEncoding.ASCIIStringEncoding);

				return new NSUrl (nsUrlString);
			} catch (Exception ex) {
				Console.Write (ex.Message);
				return new NSUrl ("");
			}

		}
		public static async Task<UIImage> getProfileImage (string userid)
		{
			try {
				// Retrieve storage account from connection string.
				CloudStorageAccount storageAccount = CloudStorageAccount.Parse (ColorExtensions.AZURE_STORAGE_CONNECTION_STRING);
				// Create the blob client.
				CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient ();

				// Retrieve reference to a previously created container.
				CloudBlobContainer container = blobClient.GetContainerReference (ColorExtensions.containerName);

				// Create the container if it doesn't already exist.
				await container.CreateIfNotExistsAsync ();

				// Retrieve reference to a blob named "myblob".
				CloudBlockBlob blockBlob = container.GetBlockBlobReference (userid);

				await blockBlob.FetchAttributesAsync ();

				byte [] target = new byte [blockBlob.Properties.Length];

				await blockBlob.DownloadToByteArrayAsync (target, 0);

				var data = NSData.FromArray (target);

				return UIImage.LoadFromData (data);
			} catch (Exception ex)
			{
				Console.Write (ex.Message);
				return null;
			}

		}
	}
}
