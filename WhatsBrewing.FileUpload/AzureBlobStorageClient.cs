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
    public class AzureBlobStorageClient
    {
        string _connectionString = string.Empty;

        public AzureBlobStorageClient(string connectionString)
        {
            _connectionString = connectionString;
        }

        private CloudBlockBlob InitializeTransfer(string containerName, string blobName, bool isPublic = false)
        {
            var container = InitializeContainer(containerName, isPublic);

            // Retrieve reference to a blob named blobname.
            var blockBlob = container.GetBlockBlobReference(blobName);
            return blockBlob;
        }

        private CloudBlobContainer InitializeContainer(string containerName, bool isPublic)
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            var container = blobClient.GetContainerReference(containerName);

            if (isPublic)
            {
                container.CreateIfNotExists(accessType: BlobContainerPublicAccessType.Blob);
            }
            else
            {
                container.CreateIfNotExists(accessType: BlobContainerPublicAccessType.Off);
            }

            return container;
        }

        public void SetPublicPermission(string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);

            var blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            var container = blobClient.GetContainerReference(containerName);

            container.SetPermissions(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob });
        }

        public bool IsFileInContainer(string containerName, string blobName)
        {
            var blockBlob = InitializeTransfer(containerName, blobName);

            return blockBlob.Exists();
        }

        public void UploadBytes(byte[] bytesToUpload, string containerName, string blobName)
        {
            var blockBlob = InitializeTransfer(containerName, blobName);

            blockBlob.UploadFromByteArray(bytesToUpload, 0, bytesToUpload.Length);
        }

        public void UploadStream(Stream streamToUpload, string containerName, string blobname, bool isPublic = false)
        {
            var blockBlob = InitializeTransfer(containerName, blobname, isPublic);

            blockBlob.UploadFromStream(streamToUpload);
        }

        public Stream DownloadToStream(string containerName, string blobName)
        {
            var blockBlob = InitializeTransfer(containerName, blobName);

            var blobExists = blockBlob.Exists();

            return blobExists ? blockBlob.OpenRead() : null;
        }

        //public DataSet DownloadDataSet(string containerName, string blobName)
        //{
        //    DataSet dataSet = null;

        //    var blockBlob = InitializeTransfer(containerName, blobName);

        //    if (!blockBlob.Exists())
        //    {
        //        throw new Exception(string.Format("Blob {0} does not exist in container: {1}.", blobName, containerName));
        //    }

        //    using (var stream = new MemoryStream())
        //    {
        //        blockBlob.DownloadToStream(stream);

        //        // seek to beginning of stream.
        //        stream.Seek(0, SeekOrigin.Begin);

        //        if (stream.Length > 0)
        //        {
        //            dataSet = new DataSet("InitialData");
        //            dataSet.ReadXml(stream);
        //        }
        //    }

        //    return dataSet;
        //}

        //public void UploadDataSet(DataSet xmlData, string containerName, string blobName)
        //{
        //    var blockBlob = InitializeTransfer(containerName, blobName);

        //    // Create or overwrite the blob with contents from xml data.
        //    using (var stream = new MemoryStream())
        //    {
        //        xmlData.WriteXml(stream, XmlWriteMode.WriteSchema);

        //        // seek to beginning of stream.
        //        stream.Seek(0, SeekOrigin.Begin);

        //        if (stream.Length > 0)
        //        {
        //            blockBlob.UploadFromStream(stream);
        //        }
        //    }
        //}

        public void DeleteBlob(string containerName, string blobName)
        {
            var blockBlob = InitializeTransfer(containerName, blobName);

            blockBlob.DeleteIfExists();
        }
    }
}
