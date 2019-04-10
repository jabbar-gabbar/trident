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
        private static string currentDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static ILog log = LogManager.GetLogger(typeof(Inventory));

        public Inventory(Setting setting)
        {
            this.setting = setting;
        }

        public List<string> build()
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
                        inventoryFiles.Add(line); // TODO: Assumption is that the file is never modified manually by user.
                    }
                }
                // read content of file name list and return list.
            }
            //else return an empty(zero item count) list. 

            //setting.sourceFolderPath
            // iterate over the folder recursively and build relative paths list \iphone4\image.jpg in without order. 
            
            string fileExtensions = ConfigurationManager.AppSettings["FileExtensionExclusions"];

            var sourceFiles = Directory.EnumerateFiles(setting.sourceFolderPath, "*.*", SearchOption.AllDirectories).ToList();
            InventoryCore inventoryCore = new InventoryCore(sourceFiles, inventoryFiles, fileExtensions, setting);
            return inventoryCore.runInventory();
        }

        public void commit(List<string> inventoryList)
        {
            string files = Environment.NewLine;
            inventoryList.ForEach(x => files += x + Environment.NewLine);
            log.Info(string.Format("Source Folder: {0}, Committing count: {1}. Files: {2}", setting.sourceFolderPath, inventoryList.Count, files));

            string inventoryFilePath = this.getInventoryFilePath();

            using (StreamWriter inventoryStream = new StreamWriter(inventoryFilePath))
            {
                inventoryStream.Write(files);
                //string line;
                //while ((line = inventoryStream.ReadLine()) != null)
                //{
                //    //inventoryList.Add(line); // TODO: Assumption is that the file is never modified manually by user.
                //}
            }
            // read content of file name list and return list.

        }

        private string getInventoryFilePath()
        {
            return currentDirPath + "\\" + inventoryFolderName + "\\" + setting.inventoryFileName;
        }
    }
}
