using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trident
{
    class Inventory
    {
        // local source folder path that are being synced to s3
        private readonly string sourceFolderPath;
        // folder where inventory is stored for the current source folder
        //private readonly string s3BucketName;
        // pipe separeted list of file extensions that are need to be excluded  
        //  from recursive file search
        private readonly string fileExclusionList;
        private bool fullSync = true;
        private string inventoryFilePath; 
        public Inventory(string sourceFolderPath, string inventoryFilePath, string excludedFileExtensions)
        {
            this.sourceFolderPath = sourceFolderPath;
            //this.s3BucketName = inventoryFilePath;
            this.fileExclusionList = excludedFileExtensions;
        }

        public void getInventory()
        {
            // find inventory file 
            if (string.IsNullOrEmpty(sourceFolderPath))
                return; // TODO: throw exception and log it.

            this.extractInventoryFolderPath();
            if (string.IsNullOrEmpty(inventoryFilePath))
                return;// TODO: throw indication that could not be able to build the inventory path.
            
            if (File.Exists(inventoryFilePath))
                fullSync = false;


        }

        public void commitInentory()
        {

        }

        private void extractInventoryFolderPath()
        {
            string[] folders = this.sourceFolderPath.Split('\\');
            if (folders == null || folders.Count() == 0)
                inventoryFilePath = string.Empty;
            string sourceFolderName = folders.Last();
            inventoryFilePath = Directory.GetCurrentDirectory() + "\\" + sourceFolderName;
        }
    }
}
