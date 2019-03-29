using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trident
{
    class InventoryCore
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

        public List<string> generateDelta()
        {
            if (this.sourceFiles == null)
                throw new ArgumentNullException("sourceFiles", string.Format("sourceFiles object is null for folder: {0}", syncSetting.sourceFolderPath));
            if (this.inventoryFiles == null)
                throw new ArgumentNullException("inventoryFiles", "inventoryFiles object is null.");
            // remove the excluded extention files from source files.
           

            throw new NotImplementedException();
        }
    }
}
