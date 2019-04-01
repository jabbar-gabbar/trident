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
        static IAmazonS3 s3;
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
            //if (!t.Result) {
            //    log.Error(string.Format("Could not find s3 bucket: {0}. The Access Key you are using might not have proper permission to read the bucket.", syncSetting.s3Bucket));
            //    return;
            //}

            // go to Inventory class and recursively iterate over the source folder and build file path list.
            // read inventory file and build file path list.
            // send both list to InventoryCore class to generate sync list. 
            Inventory inventory = new Inventory(syncSetting);
            inventory.build();

        }

        async Task<bool> checkIfBucketExists(string bucket)
        {
            return await s3.DoesS3BucketExistAsync(bucket);

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
