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
    class Sync
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

        private List<Settings> syncSettings;
        public Sync(List<Settings> syncSettings)
        {
            this.syncSettings = syncSettings;
        }

        /// <summary>
        /// starts the sync steps.
        /// </summary>
        public void start()
        {
            if (this.syncSettings == null)
            {
                log.Warn("syncSettings object is null or has zero sync settings.  Please check settings.json file content");
                return;
            }
            // iterate through each sync items and perform inventory and sync. 
            foreach (var syncItem in syncSettings)
            {
                setup(syncItem);
            }                       
        }

        void setup(Settings syncSetting) {
            if (!Directory.Exists(syncSetting.sourceFolder))
            {
                log.Warn(string.Format("Source Directory does not exist at {0}.", syncSetting.sourceFolder));
                return;
            }
            Console.WriteLine(AppContext.BaseDirectory);
        }

        //async Task listBuckets(string bucket)
        //{
        //    ListObjectsV2Request req = new ListObjectsV2Request() { BucketName = bucket, MaxKeys = 2 };
        //    ListObjectsV2Response res;
        //    do
        //    {
        //        res = await s3.ListObjectsV2Async(req);
        //        foreach (S3Object obj in res.S3Objects)
        //        {
        //            Console.WriteLine("key = {0}, size = {1}", obj.Key, obj.Size);
        //        }
        //        Console.WriteLine("Next cont. token {0} ", res.NextContinuationToken);
        //        req.ContinuationToken = res.NextContinuationToken;
        //    } while (res.IsTruncated);

        //}

    }
}
