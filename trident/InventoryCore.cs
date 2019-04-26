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
            
            if (inventoryFiles.Count == 0) // no files in inventory, that means sync entire source folder.
                return filteredSourceFiles;
            // sort the lists. 
            filteredSourceFiles.Sort();
            inventoryFiles.Sort();

            return generateInventory();
        }

        private void filterExcludedExtensionFiles()
        {
            // remove files in sourceFiles that are excluded file extensions.
            string[] fileExtensions = excludedExtension.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (fileExtensions == null || fileExtensions.Count() == 0) 
            {
                // no filter specified in the config setting. so potentially sync all source files to s3. 
                filteredSourceFiles = sourceFiles;  // set and returns filtersourcefile ref which points to sourceFiles object.   
                return;
            }

            // build a case insensetive keys distionary map of extensions from source file list
            Dictionary<string, List<string>> extensionMap = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var file in sourceFiles)
            {
                string extension = string.Empty;
                int lastIdx = file.LastIndexOf('.');

                // Below handles no period in file name, lastIdx = -1, e.g C:\users\me\photos\IMG_file_withoug_extension. 
                //   all files with no extensions are added to empty key in the map. 
                if (lastIdx > 0)
                    extension = file.Substring(lastIdx);
                List<string> localList; 
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
                // prefix with period if user did not put period in string
                string extKey = ext.LastIndexOf('.') < 0 ? "." + ext : ext; 
                if (extensionMap.ContainsKey(extKey))
                {
                    extensionMap.Remove(extKey);// removes the whole key with its values based on filter keys.
                }
                //sourceFiles.RemoveAll(x => x.EndsWith(ext)); // inefficient solution for large collections. 
            }

            filteredSourceFiles = new List<string>(); //store filtered list here. 
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

            // 2. check through hashset
            //  creat hash of the inventory file names and iterate over each items in filteredSourceFiles to see 
            //  if the key exits, if it does not, add the file to final list which needs to be uploaded to s3. 
            HashSet<string> inventoryHash = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in inventoryFiles)
            {
                inventoryHash.Add(item);
            }
            
            foreach (var item in filteredSourceFiles)
            {
                if (!inventoryHash.Contains(item))  // efficient since it requires O(1) time.
                {
                    finalList.Add(item); // file need to by uploaded to s3.
                }
            }            
            return finalList;
        }
    }
}
