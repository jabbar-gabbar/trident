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
                throw new ArgumentNullException("excludedExtension", "excludedExtension string is null.");
            if (sourceFiles.Count == 0)
                return sourceFiles;
            // filter out the excluded extention files from source files.
            filterExcludedExtensionFiles();
            // sort the lists. 
            filteredSourceFiles.Sort();
            inventoryFiles.Sort();
            if (inventoryFiles.Count == 0) // no files in inventory, that means sync entire source folder.
                return filteredSourceFiles;
            return generateInventory();
        }

        private void filterExcludedExtensionFiles()
        {
            // remove files in sourceFiles that are excluded file extensions.
            string[] fileExtensions = excludedExtension.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (fileExtensions == null || fileExtensions.Count() == 0)
            {
                filteredSourceFiles = sourceFiles;//new List<string>();
                return;
            }
            
            // build a case insensetive keys distionary map of extension in source file list
            Dictionary<string, List<string>> extensionMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var file in sourceFiles)
            {
                string extension = string.Empty;
                int lastIdx = file.LastIndexOf('.');
                if(lastIdx > 0)// handles no period in file name
                    extension = file.Substring(lastIdx);
                //string extension = item.Substring(item.LastIndexOf('.'));
                List<string> localList; //= new List<string>();
                if (extensionMap.TryGetValue(extension, out localList))
                {
                    localList.Add(file);
                }
                else
                {
                    localList = new List<string>();
                    localList.Add(file);
                    extensionMap.Add(extension, localList);
                }
            }

            foreach (var ext in fileExtensions)
            {
                string extKey = ext.LastIndexOf('.') < 0 ? "." + ext : ext; // prefix with period if user did not put period in string
                //sourceFiles.RemoveAll(x => x.EndsWith(ext)); // inefficient solution
                if (extensionMap.ContainsKey(ext))
                {
                    extensionMap.Remove(ext);
                }                
            }

            filteredSourceFiles = new List<string>();
            foreach (var item in extensionMap)
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
                    for (int i = checkInterval; i < srcCount; i += checkInterval)
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
            HashSet<string> inventoryHash = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in inventoryFiles)
            {
                inventoryHash.Add(item);
            }
            
            foreach (var item in filteredSourceFiles)
            {
                if (!inventoryHash.Contains(item))
                {
                    finalList.Add(item);
                }
            }            
            return finalList;
        }
    }
}
