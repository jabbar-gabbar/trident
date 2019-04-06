using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using trident;

namespace testing
{
    [TestClass]
    public class InventoryCoreTest
    {
        private Setting getSetting()
        {
            return new Setting() { inventoryFileName = "my-s3-bucket-1.csv", s3BucketName = "my-s3-bucket-1", sourceFolderPath = "c:\\test" };
        }

        [TestMethod]
        public void FileExt_Excluded()
        {
            List<string> sourceFiles = new List<string>();
            for (int i = 1; i < 5; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".JPG");
            }
            for (int i = 5; i < 10; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".PDF");
            }
            List<string> inventoryFiles = new List<string>();
            for (int i = 1; i < 3; i++)
            {
                inventoryFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".jpg");
            }
            string extFilter = ".pdf;";
            InventoryCore inventoryCore = new InventoryCore(sourceFiles, inventoryFiles, extFilter, getSetting());
            List<string> actual = inventoryCore.runInventory();
            
            Assert.AreEqual(false, actual.Exists(x => x.ToLower().EndsWith(".pdf")));
            Assert.AreEqual(true, actual.Exists(x => x.ToLower().EndsWith(".jpg")));
        }
                
        [TestMethod]
        public void FileExt_CaseInsensetive()
        {
            List<string> sourceFiles = new List<string>();
            for (int i = 1; i < 5; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".JPG");
            }
            for (int i = 5; i < 10; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".PDF");
            }
            List<string> inventoryFiles = new List<string>();
            for (int i = 1; i < 3; i++)
            {
                inventoryFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".jpg");
            }
            string extFilter = ".pdf;";
            InventoryCore inventoryCore = new InventoryCore(sourceFiles, inventoryFiles, extFilter, getSetting());
            List<string> actual = inventoryCore.runInventory();
            Assert.AreEqual(2, actual.Count);
        }

        [TestMethod]
        public void FileExt_EmptyFilterString()
        {
            List<string> sourceFiles = new List<string>();
            for (int i = 1; i < 5; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".JPG");
            }
            for (int i = 5; i < 10; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".PDF");
            }
            List<string> inventoryFiles = new List<string>();
            for (int i = 1; i < 3; i++)
            {
                inventoryFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".jpg");
            }
            string extFilter = "";
            InventoryCore inventoryCore = new InventoryCore(sourceFiles, inventoryFiles, extFilter, getSetting());
            List<string> actual = inventoryCore.runInventory();
            Assert.AreEqual(7, actual.Count);
            extFilter = ";";
            inventoryCore = new InventoryCore(sourceFiles, inventoryFiles, extFilter, getSetting());
            List<string> actual2 = inventoryCore.runInventory();
            Assert.AreEqual(7, actual2.Count);
        }

        // test file without extension (.) in the file name
        [TestMethod]
        public void FileExt_WithougExtensionInASourceFile()
        {
            List<string> sourceFiles = new List<string>();
            for (int i = 1; i < 5; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".JPG");
            }
            for (int i = 5; i < 10; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\IMG_000" + i);
            }
            List<string> inventoryFiles = new List<string>();
            for (int i = 1; i < 3; i++)
            {
                inventoryFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".jpg");
            }
            string extFilter = ".pdf;;";
            InventoryCore inventoryCore = new InventoryCore(sourceFiles, inventoryFiles, extFilter, getSetting());
            List<string> actual = inventoryCore.runInventory();
            Assert.AreEqual(7, actual.Count);
            extFilter = "";
            inventoryCore = new InventoryCore(sourceFiles, inventoryFiles, extFilter, getSetting());
            List<string> actual2 = inventoryCore.runInventory();
            Assert.AreEqual(7, actual2.Count);
        }

        // test file without extension (.) in the file name
        [TestMethod]
        public void FileExt_WithougExtensionInA_Source_Inventory_Files()
        {
            List<string> sourceFiles = new List<string>();
            for (int i = 1; i < 5; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".JPG");
            }
            for (int i = 5; i < 10; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\IMG_000" + i);
            }
            List<string> inventoryFiles = new List<string>();
            for (int i = 5; i < 7; i++)
            {
                inventoryFiles.Add(@"\\big\user\iPhone\IMG_000" + i);
            }
            string extFilter = ".pdf;;";
            InventoryCore inventoryCore = new InventoryCore(sourceFiles, inventoryFiles, extFilter, getSetting());
            List<string> actual = inventoryCore.runInventory();
            Assert.AreEqual(7, actual.Count);
            extFilter = "";
            inventoryCore = new InventoryCore(sourceFiles, inventoryFiles, extFilter, getSetting());
            List<string> actual2 = inventoryCore.runInventory();
            Assert.AreEqual(7, actual2.Count);
        }

        // test extension without . in the filter string.
        [TestMethod]
        public void FileExt_WithoutDotInFilterString()
        {
            List<string> sourceFiles = new List<string>();
            for (int i = 1; i < 5; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".JPG");
            }
            for (int i = 5; i < 10; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".PDF");
            }
            List<string> inventoryFiles = new List<string>();
            for (int i = 1; i < 3; i++)
            {
                inventoryFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".jpg");
            }
            string extFilter = "pdf;";
            InventoryCore inventoryCore = new InventoryCore(sourceFiles, inventoryFiles, extFilter, getSetting());
            List<string> actual = inventoryCore.runInventory();
            Assert.AreEqual(7, actual.Count);
        }
    }
}
