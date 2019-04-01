using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using trident;

namespace testing
{
    [TestClass]
    public class InventoryCoreTest
    {
        [TestMethod]
        public void FileExclusionTest()
        {
            var expexted = expectedFinalList_FileExclusionTest();
            InventoryCore inventoryCore = new InventoryCore(getSourceFiles(), getInventoryFiles(), getExtension(), getSetting());
            var ret = inventoryCore.generateDelta();
            Assert.AreEqual(expexted.Count, ret.Count);
        }



        private List<string> getSourceFiles()
        {
            List<string> sourceFiles = new List<string>();
            sourceFiles.Add(@"\\big\user\iPhone\IMG_0001.JPG");
            sourceFiles.Add(@"\\big\user\iPhone\IMG_0002.JPG");
            sourceFiles.Add(@"\\big\user\iPhone\IMG_0003.JPG");
            sourceFiles.Add(@"\\big\user\iPhone\IMG_0004.pdf");
            sourceFiles.Add(@"\\big\user\iPhone\IMG_0004.JPG");
            sourceFiles.Add(@"\\big\user\iPhone\thumbs.db");
            return sourceFiles;
        }
        private List<string> getInventoryFiles()
        {
            List<string> sourceFiles = new List<string>();
            sourceFiles.Add(@"\\big\user\iPhone\IMG_0001.JPG");
            sourceFiles.Add(@"\\big\user\iPhone\IMG_0002.JPG");
            sourceFiles.Add(@"\\big\user\iPhone\IMG_0004.JPG");
            sourceFiles.Add(@"\\big\user\iPhone\IMG_0003.JPG");
            return sourceFiles;
        }
        private string getExtension()
        {
            return ".db;.pdf";
        }
        private Setting getSetting()
        {
            return new Setting() { inventoryFileName = "my-s3-bucket-1.csv", s3BucketName = "my-s3-bucket-1", sourceFolderPath = "c:\\test" };
        }
        private List<string> expectedFinalList_FileExclusionTest()
        {
            List<string> sourceFiles = new List<string>();
            sourceFiles.Add(@"\\big\user\iPhone\IMG_0001.JPG");
            sourceFiles.Add(@"\\big\user\iPhone\IMG_0002.JPG");
            sourceFiles.Add(@"\\big\user\iPhone\IMG_0003.JPG");
            sourceFiles.Add(@"\\big\user\iPhone\IMG_0004.JPG");
            return sourceFiles;
        }


    }
}
