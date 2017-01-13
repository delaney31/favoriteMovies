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
		public BlobUpload ()
		{
		}
		public static async Task createContainerAndUpload (byte[] image)
		{
			// Retrieve storage account from connection string.
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse ("SharedAccessSignature=sv=2015-12-11&ss=bt&srt=sco&sp=rwdlacup&st=2017-01-13T01%3A11%3A00Z&se=2450-01-14T01%3A11%3A00Z&sig=f6ik2E%2BjiJX5eCBMMQnEffnXDGVhGTVt9oMVM9dqhPk%3D;BlobEndpoint=https://moviefriends.blob.core.windows.net/;TableEndpoint=https://moviefriends.table.core.windows.net");

			// Create the blob client.
			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient ();

			// Retrieve reference to a previously created container.
			CloudBlobContainer container = blobClient.GetContainerReference ("moviefriend");

			// Create the container if it doesn't already exist.
			await container.CreateIfNotExistsAsync ();

			// Retrieve reference to a blob named "myblob".
			CloudBlockBlob blockBlob = container.GetBlockBlobReference (ColorExtensions.CurrentUser.Id);

			// Create the "myblob" blob with the text "Hello, world!"
			await blockBlob.UploadFromByteArrayAsync (image, 0, image.Length);
		}

		public static async Task<UIImage> getProfileImage ()
		{
			try {
				// Retrieve storage account from connection string.
				CloudStorageAccount storageAccount = CloudStorageAccount.Parse ("SharedAccessSignature=sv=2015-12-11&ss=bt&srt=sco&sp=rwdlacup&st=2017-01-13T01%3A11%3A00Z&se=2450-01-14T01%3A11%3A00Z&sig=f6ik2E%2BjiJX5eCBMMQnEffnXDGVhGTVt9oMVM9dqhPk%3D;BlobEndpoint=https://moviefriends.blob.core.windows.net/;TableEndpoint=https://moviefriends.table.core.windows.net");

				// Create the blob client.
				CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient ();

				// Retrieve reference to a previously created container.
				CloudBlobContainer container = blobClient.GetContainerReference ("moviefriend");

				// Create the container if it doesn't already exist.
				await container.CreateIfNotExistsAsync ();

				// Retrieve reference to a blob named "myblob".
				CloudBlockBlob blockBlob = container.GetBlockBlobReference (ColorExtensions.CurrentUser.Id);

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
