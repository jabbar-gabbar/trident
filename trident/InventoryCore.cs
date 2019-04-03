using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trident
{
    public class InventoryCore
    {
        private List<string> sourceFiles = new List<string>();
        private List<string> inventoryFiles = new List<string>();
        
        private string excludedExtension = string.Empty;
        private Setting syncSetting; 
        public InventoryCore(List<string> sourceFiles, List<string> inventoryFiles, string excludedExtensions, Setting setting)
        {
            this.sourceFiles = sourceFiles;
            this.inventoryFiles = inventoryFiles;
            this.excludedExtension = excludedExtensions;
            this.syncSetting = setting;
        }

        public List<string> runInventory()
        {
            if (this.sourceFiles == null)
                throw new ArgumentNullException("sourceFiles", string.Format("sourceFiles object is null for folder: {0}", syncSetting.sourceFolderPath));
            if (this.inventoryFiles == null)
                throw new ArgumentNullException("inventoryFiles", "inventoryFiles object is null.");
            if (this.excludedExtension == null)
                throw new ArgumentException("excludedExtension", "excludedExtension string is null.");
            // sort the lists. 
            this.sourceFiles.Sort();
            this.inventoryFiles.Sort();
            // remove the excluded extention files from source files.
            this.removeExcludedExtensionFiles();
            if (inventoryFiles.Count == 0) // no files in inventory, that means sync entire source folder.
                return sourceFiles;
            //List<string> finalList = sourceFiles; // save final list.
            return this.generateInventory();
            //return finalList;
        }

        private void removeExcludedExtensionFiles() {
            // remove files in sourceFiles that are excluded file extensions.
            string[] fileExtensions = this.excludedExtension.Split(';');
            
            foreach (var ext in fileExtensions)
            {
                sourceFiles.RemoveAll(x => x.EndsWith(ext));
            }
        }

        private List<string> generateInventory()
        {
            List<string> finalList = new List<string>();
            // 1. best case scenario.  compare first last elements and count are same. 
            // match first elements and last elements of both list.  if they are same, then most likely both list are same
            if (sourceFiles.First().Equals(inventoryFiles.First(), StringComparison.OrdinalIgnoreCase) && 
                sourceFiles.Last().Equals(inventoryFiles.Last(), StringComparison.OrdinalIgnoreCase))
            {
                if (sourceFiles.Count == inventoryFiles.Count)
                {
                    bool spotCheckSucceed = true;
                    int srcCount = sourceFiles.Count;
                    // spot check 10% indexes for similarities. 
                    double countToCheck = Math.Floor(srcCount * 0.1) + 1; // 10% of total + 1 count to check
                    int checkInterval = (int)Math.Floor(srcCount / countToCheck);
                    for (int i = checkInterval; i < srcCount; i = +checkInterval)
                    {
                        if (!sourceFiles[i].Equals(inventoryFiles[i], StringComparison.OrdinalIgnoreCase)) {
                            spotCheckSucceed = false;
                            break;
                        }
                    }
                    if (spotCheckSucceed)
                    {
                        /*finalList.Clear();*/ // reset final list to zero count since there is nothing to upload.
                        return finalList; // return the empty list.
                    }
                }
            }

            // 2. 
            HashSet<string> hashset = new HashSet<string>();
            foreach (var item in this.inventoryFiles)
            {
                hashset.Add(item);
            }
            
            foreach (var item in this.sourceFiles)
            {
                if (!hashset.Contains(item))
                {
                    finalList.Add(item);
                }
            }
            return finalList;
        }
    }
}
