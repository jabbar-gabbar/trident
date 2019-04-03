using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace trident
{
    public class Inventory
    {
        // pipe separeted list of file extensions that are need to be excluded  
        //      from recursive file search
        private Setting setting;
        private static string inventoryFolderName =  ConfigurationManager.AppSettings["InventoryFolderName"];
        private static string currentDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        

        public Inventory(Setting setting)
        {
            this.setting = setting;
        }

        public void build()
        {
            //setting.inventoryFileName
            // see if the inventory file exists. 
            string inventoryFilePath = this.getInventoryFilePath();
            // instantiate this outside the if file exists so we know if any inventory found or not.
            List<string> inventoryFiles = new List<string>();
            if (File.Exists(inventoryFilePath))
            {
                using (StreamReader inventoryStream = new StreamReader(inventoryFilePath))
                {
                    string line;
                    while ((line = inventoryStream.ReadLine()) != null)
                    {
                        inventoryFiles.Add(line);
                    }
                }
                // read content of file name list and return list.
            }
            //else return an empty(zero item count) list. 

            //setting.sourceFolderPath
            // iterate over the folder recursively and build relative paths list \iphone4\image.jpg in without order. 
            //fileExclusionList
            string fileExtensions = ConfigurationManager.AppSettings["FileExtensionExclusions"];

            var sourceFiles = Directory.EnumerateFiles(setting.sourceFolderPath, "*.*", SearchOption.AllDirectories).ToList();
            InventoryCore inventoryCore = new InventoryCore(sourceFiles, inventoryFiles, fileExtensions, setting);
            inventoryCore.runInventory();
        }

        public void commitInventory()
        {
            throw new NotImplementedException();
        }

        private string getInventoryFilePath() {
            return currentDirPath + "\\" + inventoryFolderName + "\\" + setting.inventoryFileName;
        }
    }
}
