using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhatsBrewing.FileUpload;

namespace WhatsBrewing.Tests.FileUpload
{
    [TestClass]
    public class BlobStorageUploaderTests
    {
        [TestMethod]
        public void BlobStorageUploader_Upload()
        {
            var uploader = new BlobStorageUploader()
            {
                BlobStorageContainer = "whatsbrewingpublicfiles",
                BlobStorageDirectory = "clickonce",
                StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=whatsbrewing;AccountKey=7+rTyTItXBrLK0nfDYyFNXD80tBHr3IZCwjAv5Ac5BMtX9T8ZmCnzupSQqtQi7uW5jJEddfkgN3NxJDSKbq0rA=="
            };

            var isUploadComplete = uploader.Upload(@"D:\Webstep\BackupFolder\Documents\Sardinboksen\WhatsBrewing\clickonce");

            Assert.IsTrue(isUploadComplete);
        }
    }
}
