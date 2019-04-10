using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trident
{
    public class Upload
    {
        private List<string> finalList;
        private static int totalUploadCount = 0;
        private Setting setting;
        public Upload(List<string> finalList, Setting setting)
        {
            this.finalList = finalList;
            this.setting = setting;
        }

        public void start()
        {
            int batchCount=0;
            List<string> inventoryList = new List<string>();
            // iterate loop, call uploadCore to upload one file at a time, commit inventory/log progress at interval of 10 items
            foreach (var filePath in finalList)
            {
                // upload file using UploadCore.
                //UploadCore uploadCore = new UploadCore();
                //uploadCore.upload(filePath, setting);

                inventoryList.Add(filePath);
                totalUploadCount++;
                batchCount++;
                if (batchCount >= 10) // commit inventory in batch of ten files to not loose work in abrupt termination.
                {
                    commitInventory(inventoryList, out batchCount);
                }
            }
            if (batchCount > 0)// commit left over inventory in batch 
            {
                commitInventory(inventoryList, out batchCount);
            }
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
