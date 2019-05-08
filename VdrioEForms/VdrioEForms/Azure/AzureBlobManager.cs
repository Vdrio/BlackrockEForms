using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VdrioEForms.Azure
{
    public class AzureBlobManager
    {
        readonly static CloudStorageAccount _cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=bwltestingstorage;AccountKey=c/FpR400/p36lqlnjtpuykzlvmNKNCrW5pVkVa7giiZppRUQdGa3+UNmKdO0lH/5Bot1r2hDicqEmpV2oaR4eg==;EndpointSuffix=core.windows.net");
        readonly static CloudBlobClient _blobClient = _cloudStorageAccount.CreateCloudBlobClient();
        public static async Task<List<CloudBlockBlob>> GetBlobs(string containerName, string prefix = "", int? maxresultsPerQuery = null, BlobListingDetails blobListingDetails = BlobListingDetails.None)
        {
            var blobContainer = _blobClient.GetContainerReference(containerName);
            await blobContainer.CreateIfNotExistsAsync();
            var blobList = new List<CloudBlockBlob>();
            BlobContinuationToken continuationToken = null;

            try
            {
                do
                {
                    var response = await blobContainer.ListBlobsSegmentedAsync(prefix, true, blobListingDetails, maxresultsPerQuery, continuationToken, null, null);

                    continuationToken = response?.ContinuationToken;

                    foreach (var blob in response?.Results?.OfType<CloudBlockBlob>())
                    {
                        blobList.Add(blob);

                    }

                } while (continuationToken != null);
            }
            catch (Exception ex)
            {
                return null;
                //Handle Exception
            }

            return blobList;
        }

        public static async Task<CloudBlockBlob> GetBlob(string containerName, string blobName)
        {
            var blobContainer = _blobClient.GetContainerReference(containerName);
            await blobContainer.CreateIfNotExistsAsync();

            try
            {
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(blobName);// ListBlobsSegmentedAsync(prefix, true, blobListingDetails, maxresultsPerQuery, continuationToken, null, null);
                if (await blob.ExistsAsync())
                {
                    return blob;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception ex)
            {
                return null;
                //Handle Exception
            }

        }

        public static async Task<CloudBlobContainer> GetContainer(string containerName)
        {
            try
            {
                var blobContainer = _blobClient.GetContainerReference(containerName);
                await blobContainer.CreateIfNotExistsAsync();
                return blobContainer;
            }
            catch { return null; }


        }

        public static async Task<CloudBlobDirectory> GetDirectory(string containerName, string directoryName)
        {
            try
            {
                var blobContainer = _blobClient.GetContainerReference(containerName);
                await blobContainer.CreateIfNotExistsAsync();
                CloudBlobDirectory directory = blobContainer.GetDirectoryReference(directoryName);

                return directory;

            }
            catch { return null; }
        }

        public static async Task<CloudBlockBlob> SaveBlockBlob(string containerName, byte[] blob, string blobTitle)
        {
            try
            {
                var blobContainer = _blobClient.GetContainerReference(containerName);
                await blobContainer.CreateIfNotExistsAsync();

                if ((await GetBlobs(containerName)).Where(x => x.Name == blobTitle).ToList().Count > 0)
                {
                    Debug.WriteLine("this file already exists");
                    return null;
                }
                else
                {
                    var blockBlob = blobContainer.GetBlockBlobReference(blobTitle);
                    await blockBlob.UploadFromByteArrayAsync(blob, 0, blob.Length);

                    return blockBlob;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
