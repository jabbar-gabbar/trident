using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace trident
{
    public class Inventory
    {
        private Setting setting;
        private static string inventoryFolderName =  ConfigurationManager.AppSettings["InventoryFolderName"];
        private static string fileExtensions = ConfigurationManager.AppSettings["FileExtensionExclusions"];

        private static string currentDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public Inventory(Setting setting)
        {
            this.setting = setting;
        }

        public List<string> build()
        {
            // read data from inventory file setting.inventoryFileName, e.g. C:\program files\trident\inventory\my-inventory.csv
            // see if the inventory file exists. 
            string inventoryFilePath = this.getInventoryFilePath();
            // instantiate this outside the if file exists so we know if any inventory found or not.
            List<string> inventoryFiles = new List<string>();
            if (File.Exists(inventoryFilePath))
            {
                // read content of file names into inventoryFiles list.
                using (StreamReader inventoryStream = new StreamReader(inventoryFilePath))
                {
                    string line;
                    while ((line = inventoryStream.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if(!string.IsNullOrEmpty(line))
                            inventoryFiles.Add(line); // TODO: Assumption is that the file is never hand modified manually by user.
                    }
                }
            }
            //else an empty(zero item count) inventoryFiles list. 

            // build a list of file absolute path from the source folder in setting.sourceFolderPath. e.g. \\my_photo_server\iphone  OR C:\users\me\photos
            // iterate over the source folder recursively and build absolute paths list \\my_photo_server\iphone\IMG0001.jpg or C:\users\me\photos\IMG0100.jpg. 
            var sourceFiles = Directory.EnumerateFiles(setting.sourceFolderPath, "*.*", SearchOption.AllDirectories).ToList();
            // call inventorycore to build the inventory of files that need to be uploaded to s3. 
            InventoryCore inventoryCore = new InventoryCore(sourceFiles, inventoryFiles, fileExtensions, setting);
            // returns final list of files to be uploaded. 
            return inventoryCore.runInventory();
        }

        public void commit(List<string> inventoryList, out int batchCount)
        {
            string files = string.Empty;
            inventoryList.ForEach(x => files += x + "\r\n");
            //log.Info(string.Format("Source Folder: {0}, Committing count: {1}. Files: {2}", setting.sourceFolderPath, inventoryList.Count, files));

            string inventoryFilePath = this.getInventoryFilePath();

            using (StreamWriter inventoryStream = File.AppendText(inventoryFilePath))
            {
                inventoryStream.Write(files); // TODO: Assumption is that the file is never modified manually by user.
            }
            // resets inventoryList after update.
            inventoryList.Clear();
            batchCount = 0; // reset
        }

        private string getInventoryFilePath()
        {
            return currentDirPath + "\\" + inventoryFolderName + "\\" + setting.inventoryFileName;
        }
    }
}
