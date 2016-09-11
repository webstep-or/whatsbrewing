using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WhatsBrewing.FileUpload
{
    public class BlobStorageUploader
    {
        public string StorageConnectionString { get; set; }
        public string BlobStorageDirectory { get; set; }
        public string BlobStorageContainer { get; set; }

        public bool Upload(string uploadDirectoryPath)
        {
            var storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var cloudContainer = blobClient.GetContainerReference(BlobStorageContainer);

            var blobStorageDirectory = cloudContainer.GetDirectoryReference(BlobStorageDirectory);
            UploadDirectory(uploadDirectoryPath, blobStorageDirectory);

            return true;
        }

        private void UploadDirectory(string directoryPath, CloudBlobDirectory blobDirectory)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);

            // Enumerate files in folder
            foreach (var fileInfo in directoryInfo.GetFiles())
            {
                // Upload file
                UploadFile(blobDirectory, fileInfo);
            }

            // Enumerate sub directories
            foreach (var subDirectoryInfo in directoryInfo.GetDirectories())
            {
                var subBlobDir = blobDirectory.GetDirectoryReference(subDirectoryInfo.Name);
                // Upload sub directory
                UploadDirectory(subDirectoryInfo.FullName, subBlobDir);
            }
        }

        private void UploadFile(CloudBlobDirectory blobDirectory, FileInfo fileInfo)
        {
            var blob = blobDirectory.GetBlockBlobReference(fileInfo.Name);

            using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open))
            {
                blob.UploadFromStream(fileStream);
            }
        }
    }
}
