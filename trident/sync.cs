﻿using Amazon;
using Amazon.S3;
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
        // for each item, 
        // perform inventory from the local inventory file stored in the folder. 
        // if first time programs runs or if you add new folder to sync,  it will perform full sync of that setting.
        // in the full sync, use multipart s3 upload to upload the object. 

        // in delta sync, retrieve all key ids and arrange them in sequential order. 
        //      query source folder and arrange file names in sequential order. 
        //      match first element if matches, find last element of the key in sorted source collection. 
        //      retreive all elements that comes after that. sync all that files to s3 using multipart upload. 

        private static readonly string inventoryFolderName = ConfigurationManager.AppSettings["InventoryFolderName"];
        private IAmazonS3 s3Client;
        private static ILog log = LogManager.GetLogger(typeof(Sync));

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
                log.Info(string.Format("Starting sync of sourceFolderPath={0}, s3 bucket={1}", syncItem.sourceFolderPath, syncItem.s3BucketName));
                checkInitSync(syncItem);
            }
        }
        /// <summary>
        /// Check setting and Initialize sync of the source to destination.
        /// </summary>
        /// <param name="syncSetting"></param>
        private void checkInitSync(Setting syncSetting)
        {
            // TODO: wrap this method with try catch to prevent one fail all fail situation. 
            if (!Directory.Exists(syncSetting.sourceFolderPath))
            {
                log.Error(string.Format("Could not perform sync due to source Directory does not exist at {0}.", syncSetting.sourceFolderPath));
                return;
            }

            // perform few s3 operations for checking.  
            Task<bool> t = checkIfBucketExists(syncSetting.s3BucketName);
            if (!t.Result)
            {
                log.Error(string.Format("Could not find s3 bucket={0}. The Access Key you are using might not have proper permission to read the bucket.", syncSetting.s3BucketName));
                return;
            }
            abortS3MultipartUploadJob(syncSetting.s3BucketName).Wait();

            // go to Inventory class and recursively iterate over the source folder and build file path list.
            // read inventory file and build file path list. send both list to InventoryCore class to generate sync list. 
            Inventory inventory = new Inventory(syncSetting);
            List<string> finalList = inventory.build();
            Upload upload = new Upload(finalList, syncSetting);
            upload.start(); 
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
                log.Error("S3 Error encountered on server when aborting multipart upload.", ex);
            }
            catch (Exception ex)
            {
                log.Error("Unknown S3 encountered on server when aborting multipart upload.", ex);
            }
        }

        async Task<bool> checkIfBucketExists(string bucket)
        {
            try
            {
                s3Client = new AmazonS3Client();
                return await s3Client.DoesS3BucketExistAsync(bucket);
            }
            catch (AmazonS3Exception ex)
            {
                log.Error(string.Format("S3 Error encountered on server when checking bucket exists, bucket={0}", bucket), ex);
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Unknown S3 error on server when checking bucket exists. bucket={0}", bucket), ex);
            }
            return false;
        }
    }
}
