using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using trident;
using System.Collections.Generic;
using System.Linq;

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
        public void FileExt_WithougExtensionInSourceFile()
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
        public void FileExt_WithougExtensionIn_Source_Inventory_Files()
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

        // test same list in both source and inventory
        [TestMethod]
        public void Inventory_Is_Same_As_Source()
        {
            List<string> sourceFiles = new List<string>();
            for (int i = 1; i < 50; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".JPG");
            }
            
            List<string> inventoryFiles = new List<string>();
            for (int i = 1; i < 50; i++)
            {
                inventoryFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".jpg");
            }
            string extFilter = ".pdf;";
            InventoryCore inventoryCore = new InventoryCore(sourceFiles, inventoryFiles, extFilter, getSetting());
            List<string> actual = inventoryCore.runInventory();
            Assert.AreEqual(0, actual.Count);
        }

        // test both source and inventory first last elements same but middle is different
        [TestMethod]
        public void First_Last_Elements_Same_In_Both_But_Middle_Elements_AreNot()
        {
            List<string> sourceFiles = new List<string>();
            for (int i = 1; i < 10; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\A_IMG_000" + i + ".JPG");
            }
            for (int i = 1; i < 10; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\B_IMG_000" + i + ".JPG");
            }
            for (int i = 1; i < 10; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\C_IMG_000" + i + ".JPG");
            }

            List<string> inventoryFiles = new List<string>();
            for (int i = 1; i < 10; i++)
            {
                inventoryFiles.Add(@"\\big\user\iPhone\A_IMG_000" + i + ".jpg");
            }
            for (int i = 1; i < 10; i++)
            {
                inventoryFiles.Add(@"\\big\user\iPhone\C_IMG_000" + i + ".jpg");
            }
            string extFilter = ".pdf;";
            InventoryCore inventoryCore = new InventoryCore(sourceFiles, inventoryFiles, extFilter, getSetting());
            List<string> actual = inventoryCore.runInventory();
            Assert.AreEqual(9, actual.Count);
        }

        [TestMethod]
        public void SpotCheck_Detected_Anomalies()
        {
            List<string> sourceFiles = new List<string>();
            for (int i = 1; i < 30; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\A_IMG_000" + i + ".JPG");
            }
            for (int i = 1; i < 10; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\D_IMG_000" + i + ".JPG");
            }
            for (int i = 1; i < 5; i++) //amomalies
            {
                sourceFiles.Add(@"\\big\user\iPhone\B_IMG_000" + i + ".JPG");
            } 
            List<string> inventoryFiles = new List<string>();
            for (int i = 1; i < 30; i++)
            {
                inventoryFiles.Add(@"\\big\user\iPhone\A_IMG_000" + i + ".jpg");
            }
            for (int i = 1; i < 10; i++)
            {
                inventoryFiles.Add(@"\\big\user\iPhone\D_IMG_000" + i + ".jpg");
            }
            for (int i = 1; i < 5; i++) //amomalies
            {
                inventoryFiles.Add(@"\\big\user\iPhone\C_IMG_000" + i + ".jpg");
            }
            string extFilter = ".pdf;";
            InventoryCore inventoryCore = new InventoryCore(sourceFiles, inventoryFiles, extFilter, getSetting());
            List<string> actual = inventoryCore.runInventory();
            Assert.AreEqual(4, actual.Count);
        }

        [TestMethod]
        public void Performance_Test_Large_Collection()
        {

            List<string> sourceFiles = new List<string>();
            for (int i = 1; i < 200000; i++)
            {
                sourceFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".JPG");
            }
            
            List<string> inventoryFiles = new List<string>();
            for (int i = 1; i < 160000; i++)
            {
                inventoryFiles.Add(@"\\big\user\iPhone\IMG_000" + i + ".jpg");
            }
            //var ret = sourceFiles.Except(inventoryFiles,StringComparer.OrdinalIgnoreCase);
            //Assert.AreEqual(40000, ret.Count());

            string extFilter = ".pdf;";
            InventoryCore inventoryCore = new InventoryCore(sourceFiles, inventoryFiles, extFilter, getSetting());
            List<string> actual = inventoryCore.runInventory();
            Assert.AreEqual(40000, actual.Count);
        }
    }
}
