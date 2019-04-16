using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using log4net;
using Amazon.S3.Transfer;

namespace trident
{
    /// <summary>
    /// sync class start the sync process. it accepts the array of sync settings
    /// and iterate over the items and sync each directory to corresponding s3 bucket
    /// </summary>
    public class Sync
    {
        // iterate over each item and start sync.
        // for each item, do this
        // 1. query somehow the total count of the object in the bucket
        // if count is zero,  start full sync. 
        // in the full sync, use multipart upload to upload the object. log error. 
        // if count is not zero, start daily sync.

        // in delta sync, retrieve all key ids and arrange them in sequential order. TBD more details
        //      query source folder and arrange file names in sequential order. 
        //      match first element if matches, find last element of the key in sorted source collection. 
        //      retreive all elements that comes after that. sync all that files to s3. 

        static readonly RegionEndpoint regionEndpoint = RegionEndpoint.USEast1;
        static readonly string inventoryFolderName = ConfigurationManager.AppSettings["InventoryFolderName"];
        static IAmazonS3 s3Client;
        static ILog log = LogManager.GetLogger(typeof(Sync));

        private List<Setting> syncSettings;
        public Sync(List<Setting> syncSettings)
        {
            this.syncSettings = syncSettings;
        }

        /// <summary>
        /// starts the sync steps.
        /// </summary>
        public void start()
        {
            // iterate through each sync items and perform inventory and sync in sequential operations. 
            foreach (var syncItem in syncSettings)
            {
                checkInitSync(syncItem);
            }                       
        }
        /// <summary>
        /// Check setting and Initialize sync of the source and destination.
        /// </summary>
        /// <param name="syncSetting"></param>
        private void checkInitSync(Setting syncSetting)
        {
            if (!Directory.Exists(syncSetting.sourceFolderPath))
            {
                log.Error(string.Format("Could not perform sync due to source Directory does not exist at {0}.", syncSetting.sourceFolderPath));
                return;
            }

            Task<bool> t = checkIfBucketExists(syncSetting.s3BucketName);
            if (!t.Result)
            {
                log.Error(string.Format("Could not find s3 bucket: {0}. The Access Key you are using might not have proper permission to read the bucket.", syncSetting.s3BucketName));
                return;
            }
            abortS3MultipartUploadJob(syncSetting.s3BucketName).Wait();
            // go to Inventory class and recursively iterate over the source folder and build file path list.
            // read inventory file and build file path list.
            // send both list to InventoryCore class to generate sync list. 
            Inventory inventory = new Inventory(syncSetting);// TODO: try catch to continue to next sync item.
            List<string> finalList = inventory.build();
            Upload upload = new Upload(finalList, syncSetting);
            upload.start();//implement inventory.commit(); inside start().
        }

        private async Task abortS3MultipartUploadJob(string bucketName)
        {
            try
            {
                var transferUtility = new TransferUtility(s3Client);

                // Abort all in-progress uploads initiated before today - 7 days.
                await transferUtility.AbortMultipartUploadsAsync(bucketName, DateTime.Now.AddDays(-7));
            }
            catch (AmazonS3Exception ex)
            {
                log.Error("Error encountered on server. Message:'{0}' when aborting multipart upload.", ex);           
            }
            catch (Exception ex)
            {
                log.Error("Unknown encountered on server. Message:'{0}' when writing an object", ex);
            }
        }

        async Task<bool> checkIfBucketExists(string bucket)
        {
            s3Client = new AmazonS3Client(); // aws region is set by app.config automatically. since we are using s3, it does not matter becuase it is a global.
            // abort multi part upload started on today - 7 days.
            var transferUtlity = new TransferUtility(s3Client);


            return await s3Client.DoesS3BucketExistAsync(bucket);

            //ListObjectsV2Request req = new ListObjectsV2Request() { BucketName = bucket, MaxKeys = 2 };
            //ListObjectsV2Response res;
            //do
            //{
            //    res = await s3.ListObjectsV2Async(req);
            //    foreach (S3Object obj in res.S3Objects)
            //    {
            //        Console.WriteLine("key = {0}, size = {1}", obj.Key, obj.Size);
            //    }
            //    Console.WriteLine("Next cont. token {0} ", res.NextContinuationToken);
            //    req.ContinuationToken = res.NextContinuationToken;
            //} while (res.IsTruncated);

        }

    }
}
