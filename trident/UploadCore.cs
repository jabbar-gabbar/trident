using Amazon.S3;
using Amazon.S3.Transfer;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trident
{
    public class UploadCore
    {
        private IAmazonS3 s3Client;
        private static ILog log = LogManager.GetLogger(typeof(UploadCore));

        //private readonly string  sourceFilePath;
        private Setting setting;
        public UploadCore(Setting setting)
        {
            this.setting = setting;
            s3Client = new AmazonS3Client(); // s3 region is inferred from the app.config file. 
        }

        public bool upload(string sourceFilePath)
        {
            bool success = false;
            try
            {
                string keyName = getKeyName(sourceFilePath);
                // upload object here.
                Task<bool> result = uploadObjectToS3(keyName, sourceFilePath);
                success = result.Result;
            }
            catch (InvalidOperationException ex)
            {
                log.Error(ex);
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Error. Path:{0}, {1} ",sourceFilePath, setting.sourceFolderPath), ex);
            }
            return success;
        }

        public string getKeyName(string sourceFilePath)
        {
            // setting.sourceFolderPath = \\big\usr
            // sourceFilePath =            \\big\usr\folder\IMG_20190413_081415.jpg
            // Assumption is that since the sourceFilePath is retrieved using setting.sourceFolderPath in Inventory.cs, 
            // it should contain the beginning sequences.  just remove those chars to make object name. 
            int folderPathLength = setting.sourceFolderPath.Count();
            if (sourceFilePath.Count() <= folderPathLength)
            {
                throw new InvalidOperationException(string.Format("Source file path length ({0}) is equal or less then source folder path ({1}).", sourceFilePath, setting.sourceFolderPath));
            }
            string objectName = sourceFilePath.Substring(folderPathLength).Replace(@"\", "/");
            if (objectName[0] == '/') {
                objectName = objectName.Remove(0,1);
            }
            return objectName;
        }

        private async Task<bool> uploadObjectToS3(string keyName, string sourceFilePath) {
            bool uploaded = false;
            try
            {
                TransferUtilityUploadRequest req = new TransferUtilityUploadRequest();
                req.BucketName = setting.s3BucketName;
                req.Key = keyName;
                req.FilePath = sourceFilePath;

                var fileTransferUtility = new TransferUtility(s3Client);

                await fileTransferUtility.UploadAsync(req);// uploads an object to s3.  it will overwrite same key name. 
                uploaded = true;
            }
            catch (AmazonS3Exception ex)
            {
                log.Error(string.Format("S3 Error encountered on server when uploading key={0}, bucket={1}.",
                    keyName, setting.s3BucketName), ex);
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Unknown S3 error on server when uploading key={0}, bucket={1}",
                    keyName, setting.s3BucketName), ex);
            }
            return uploaded;
        }
    }   
}
