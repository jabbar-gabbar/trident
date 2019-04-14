using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trident
{
    public class UploadCore
    {
        private readonly IAmazonS3 s3Client;
        private string sourceFilePath;
        private Setting setting;
        public UploadCore(string sourceFilePath, Setting setting)
        {
            this.sourceFilePath = sourceFilePath;
            this.setting = setting;
        }

        public bool upload()
        {
            bool success = false;
            try
            {

                getObjectName();
                success = true;
            }
            catch (Exception ex)
            {
                //todo:log
            }
            finally
            {
            }
            return success;
        }

        public string getObjectName() {
            if (string.IsNullOrEmpty(sourceFilePath))
            {
                // log error.
                throw new InvalidOperationException("Source file path is null or empty.");
            }
            int folderPathLength =  this.setting.sourceFolderPath.Count();
            if (sourceFilePath.Count() <= folderPathLength)
            {
                throw new InvalidOperationException(string.Format("Source file path length ({0}) is equal or less then source folder path ({1}).", sourceFilePath, setting.sourceFolderPath));
            }
            // setting.sourceFolderPath = \\big\usr
            // sourceFilePath =            \\big\usr\folder\IMG_20190413_081415.jpg
            // Assumption is that since the sourceFilePath is retrieved using setting.sourceFolderPath in Inventory.cs, 
            // it should contain the beginning sequences.  just remove those chars to make object name. 
            string objectName = sourceFilePath.Substring(folderPathLength).Replace(@"\", "/");
            if (objectName[0] == '/') {
                objectName = objectName.Remove(0,1);
            }
            return objectName;
        }
    }   
}
