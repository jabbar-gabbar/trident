using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace trident
{
    public class Upload
    {
        private List<string> finalList;
        private int totalUploadCount;
        private Setting setting;
        private static ILog log = LogManager.GetLogger(typeof(Upload));

        public Upload(List<string> finalList, Setting setting)
        {
            this.finalList = finalList;
            this.setting = setting;
        }

        public void start()
        {
            int batchCount=0;
            string lastfile = string.Empty;
            List<string> inventoryList = new List<string>();
            // iterate loop, call uploadCore to upload one file at a time, commit inventory/log progress at interval of 10 items
            foreach (var filePath in finalList)
            {
                // upload file using UploadCore.
                UploadCore uploadCore = new UploadCore(filePath, setting);
                uploadCore.upload();
                // var result = uploadCore.upload(filePath, setting);
                // TODO: if successful upload, do next four lines. 
                inventoryList.Add(filePath);
                totalUploadCount++;
                batchCount++;
                lastfile = filePath;
                if (batchCount >= 10) // commit inventory in batch of 10 files to not loose work in abrupt termination.
                {
                    commitInventory(inventoryList, out batchCount);
                }                
            }
            if (batchCount > 0)// commit left over inventory in batch 
            {
                commitInventory(inventoryList, out batchCount);
            }

            log.Info(string.Format("Upload finish >>>>> SOURCE FOLDER: {0}, COUNT: {1}. LAST FILE: {2}.<<<<<<<<<<<<", setting.sourceFolderPath, totalUploadCount, lastfile));
        }

        private void commitInventory(List<string> inventoryList, out int batchCount)
        {
            // call to update inventory and log message
            Inventory inventory = new Inventory(setting);
            inventory.commit(inventoryList);
            // resets inventoryList after update.
            inventoryList.Clear();
            batchCount = 0; // reset
        }
    }
}
