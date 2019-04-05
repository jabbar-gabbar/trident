using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trident
{
    public class InventoryCore
    {
        private List<string> sourceFiles;
        private List<string> filteredSourceFiles;
        private List<string> inventoryFiles;
        
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
            if (sourceFiles == null)
                throw new ArgumentNullException("sourceFiles", string.Format("sourceFiles object is null for folder: {0}", syncSetting.sourceFolderPath));
            if (inventoryFiles == null)
                throw new ArgumentNullException("inventoryFiles", "inventoryFiles object is null.");
            if (excludedExtension == null)
                throw new ArgumentException("excludedExtension", "excludedExtension string is null.");
            
            // remove the excluded extention files from source files.
            removeExcludedExtensionFiles();
            // sort the lists. 
            //sourceFiles.Sort();
            filteredSourceFiles.Sort();
            inventoryFiles.Sort();
            if (inventoryFiles.Count == 0) // no files in inventory, that means sync entire source folder.
                return sourceFiles;
            //List<string> finalList = sourceFiles; // save final list.
            return generateInventory();
            //return finalList;
        }

        private void removeExcludedExtensionFiles() {
            // remove files in sourceFiles that are excluded file extensions.
            string[] fileExtensions = excludedExtension.Split(';');
            if (fileExtensions == null || fileExtensions.Count() == 0) {
                filteredSourceFiles = new List<string>();
                filteredSourceFiles.AddRange(sourceFiles);
                return;
            }

            Dictionary<string, List<string>> extensionIndexMap = new Dictionary<string, List<string>>();
            //HashSet<string> sourceFileHash = new HashSet<string>();
            foreach (var item in sourceFiles)
            {
                string extension = item.Substring(item.LastIndexOf('.'));
                //sourceFileHash.Add(item);
                List<string> localList; //= new List<string>();
                if (extensionIndexMap.TryGetValue(extension, out localList))
                {
                    localList.Add(item);
                }
                else
                {
                    localList = new List<string>();
                    localList.Add(item);
                    extensionIndexMap.Add(extension, localList);
                }
            }

            foreach (var ext in fileExtensions)
            {
                //sourceFiles.RemoveAll(x => x.EndsWith(ext));
                if (extensionIndexMap.ContainsKey(ext))
                {
                    extensionIndexMap.Remove(ext);
                }                
            }

            filteredSourceFiles = new List<string>();
            foreach (var item in extensionIndexMap)
            {
                filteredSourceFiles.AddRange(item.Value);
            }
        }

        private List<string> generateInventory()
        {
            List<string> finalList = new List<string>();
            // 1. best case scenario.  compare first last elements and count are same. 
            // match first elements and last elements of both list.  if they are same, then most likely both list are same
            if (filteredSourceFiles.First().Equals(inventoryFiles.First(), StringComparison.OrdinalIgnoreCase) && 
                filteredSourceFiles.Last().Equals(inventoryFiles.Last(), StringComparison.OrdinalIgnoreCase))
            {
                if (filteredSourceFiles.Count == inventoryFiles.Count)
                {
                    bool spotCheckSucceed = true;
                    int srcCount = filteredSourceFiles.Count;
                    // spot check 10% indexes for similarities. 
                    double countToCheck = Math.Floor(srcCount * 0.1) + 1; // 10% of total + 1 count to check
                    int checkInterval = (int)Math.Floor(srcCount / countToCheck);
                    for (int i = checkInterval; i < srcCount; i = +checkInterval)
                    {
                        if (!filteredSourceFiles[i].Equals(inventoryFiles[i], StringComparison.OrdinalIgnoreCase)) {
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
            foreach (var item in inventoryFiles)
            {
                hashset.Add(item);
            }
            
            foreach (var item in filteredSourceFiles)
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
