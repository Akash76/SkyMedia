using System;
using System.IO;
using System.Collections.Generic;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaIngestStorageBlob
    {
        private static readonly StorageBlobClient _blobClient = new StorageBlobClient();
        private static readonly DatabaseClient _databaseClient = new DatabaseClient(true);

        [FunctionName("MediaIngestStorageBlob")]
        public static void Run([BlobTrigger(Constant.Storage.BlobContainer.MediaServices + "/{blobName}", Connection = "Storage")] Stream blobStream, string blobName, ILogger logger)
        {
            try
            {
                StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
                if (!blobName.EndsWith(Constant.Media.IngestManifest.FileExtensionLog, stringComparison))
                {
                    logger.LogInformation("Media File: {0}", blobName);
                    if (blobName.StartsWith(Constant.Media.IngestManifest.TriggerPrefix, stringComparison))
                    {
                        ProcessManifest(blobStream, blobName, logger);
                    }
                    else
                    {
                        string collectionId = Constant.Database.Collection.MediaIngestManifest;
                        MediaIngestManifest[] ingestManifests = _databaseClient.GetDocuments<MediaIngestManifest>(collectionId);
                        foreach (MediaIngestManifest ingestManifest in ingestManifests)
                        {
                            List<string> missingFiles = new List<string>();
                            foreach (string missingFile in ingestManifest.MissingFiles)
                            {
                                if (!string.Equals(missingFile, blobName, stringComparison))
                                {
                                    missingFiles.Add(blobName);
                                }
                            }
                            if (missingFiles.Count == 0)
                            {
                                _databaseClient.DeleteDocument(collectionId, ingestManifest.Name);
                                string containerName = Constant.Storage.BlobContainer.MediaServices;
                                CloudBlockBlob manifestBlob = _blobClient.GetBlockBlob(containerName, null, ingestManifest.Name);
                                blobStream = manifestBlob.OpenReadAsync().Result;
                                ProcessManifest(blobStream, ingestManifest.Name, logger);
                            }
                            else if (missingFiles.Count != ingestManifest.MissingFiles.Length)
                            {
                                ingestManifest.MissingFiles = missingFiles.ToArray();
                                _databaseClient.UpsertDocument(collectionId, ingestManifest);
                            }
                        }
                    }
                }
            }
            catch (ApiErrorException ex)
            {
                string logData = ex.Response.Content;
                WriteLog(blobName, logData, null, true);
                logger.LogError(logData);
                throw;
            }
            catch (Exception ex)
            {
                string logData = ex.ToString();
                WriteLog(blobName, logData, null, true);
                logger.LogError(logData);
                throw;
            }
        }

        private static MediaIngestManifest GetManifest(Stream manifestStream, string manifestName)
        {
            MediaIngestManifest ingestManifest;
            using (StreamReader manifestReader = new StreamReader(manifestStream))
            {
                string manifestData = manifestReader.ReadToEnd();
                ingestManifest = JsonConvert.DeserializeObject<MediaIngestManifest>(manifestData);
                ingestManifest.Name = manifestName;
            }
            List<string> missingFiles = new List<string>();
            foreach (string fileName in ingestManifest.FileNames)
            {
                string containerName = Constant.Storage.BlobContainer.MediaServices;
                CloudBlockBlob blobFile = _blobClient.GetBlockBlob(containerName, null, fileName);
                if (!blobFile.ExistsAsync().Result)
                {
                    missingFiles.Add(fileName);
                }
            }
            ingestManifest.MissingFiles = missingFiles.ToArray();
            return ingestManifest;
        }

        private static void ProcessManifest(Stream blobStream, string manifestName, ILogger logger)
        {
            string logData = string.Concat("Process Manifest @ ", DateTime.UtcNow);
            WriteLog(manifestName, logData, logger, true);
            MediaIngestManifest ingestManifest = GetManifest(blobStream, manifestName);
            if (ingestManifest.MissingFiles.Length > 0)
            {
                logData = string.Concat("Missing Files: ", string.Join(", ", ingestManifest.MissingFiles));
                WriteLog(ingestManifest.Name, logData, logger, false);
                string collectionId = Constant.Database.Collection.MediaIngestManifest;
                _databaseClient.UpsertDocument(collectionId, ingestManifest);
            }
            else if (ingestManifest.FileNames.Length > 0 || !string.IsNullOrEmpty(ingestManifest.JobInputFileUrl))
            {
                if (string.IsNullOrEmpty(ingestManifest.JobInputFileUrl) && ingestManifest.JobInputMode == MediaJobInputMode.UploadFile)
                {
                    string sourceContainer = Constant.Storage.BlobContainer.MediaServices;
                    string fileName = ingestManifest.FileNames[0];
                    ingestManifest.JobInputFileUrl = _blobClient.GetDownloadUrl(sourceContainer, fileName, false);
                }
                using (MediaClient mediaClient = new MediaClient(ingestManifest.MediaAccount, null))
                {
                    Asset inputAsset = null;
                    if (ingestManifest.FileNames.Length > 0)
                    {
                        ingestManifest = CreateAsset(mediaClient, ingestManifest, out inputAsset, logger);
                    }
                    if (ingestManifest.TransformPresets != null && ingestManifest.TransformPresets.Length > 0)
                    {
                        CreateJob(mediaClient, ingestManifest, inputAsset, logger);
                    }
                }
            }
        }

        private static MediaIngestManifest CreateAsset(MediaClient mediaClient, MediaIngestManifest ingestManifest, out Asset inputAsset, ILogger logger)
        {
            string storageAccount = mediaClient.PrimaryStorageAccount;
            string assetName = ingestManifest.AssetName;
            string assetDescription = ingestManifest.AssetDescription;
            string assetAlternateId = ingestManifest.AssetAlternateId;
            string sourceContainer = Constant.Storage.BlobContainer.MediaServices;
            string[] fileNames = ingestManifest.FileNames;
            StorageBlobClient assetBlobClient = new StorageBlobClient(ingestManifest.MediaAccount, storageAccount);
            inputAsset = mediaClient.CreateAsset(_blobClient, assetBlobClient, storageAccount, assetName, assetDescription, assetAlternateId, sourceContainer, fileNames);
            string logData = string.Concat("New Asset: ", JsonConvert.SerializeObject(inputAsset));
            WriteLog(ingestManifest.Name, logData, logger, false);
            ingestManifest.AssetName = inputAsset.Name;
            if (string.IsNullOrEmpty(ingestManifest.JobInputFileUrl) && ingestManifest.JobInputMode == MediaJobInputMode.AssetFile)
            {
                string fileName = fileNames[0];
                ingestManifest.JobInputFileUrl = assetBlobClient.GetDownloadUrl(inputAsset.Container, fileName, false);
            }
            return ingestManifest;
        }

        private static void CreateJob(MediaClient mediaClient, MediaIngestManifest ingestManifest, Asset inputAsset, ILogger logger)
        {
            string logData = string.Concat("Input File Url: ", ingestManifest.JobInputFileUrl);
            WriteLog(ingestManifest.Name, logData, logger, false);
            int videoIndex = Array.IndexOf(ingestManifest.TransformPresets, MediaTransformPreset.VideoIndexer);
            int audioIndex = Array.IndexOf(ingestManifest.TransformPresets, MediaTransformPreset.AudioIndexer);
            if (mediaClient.IndexerEnabled() && (videoIndex > -1 || audioIndex > -1))
            {
                if (ingestManifest.JobInputMode != MediaJobInputMode.Asset)
                {
                    inputAsset = null;
                }
                bool audioOnly = videoIndex == -1 && audioIndex > -1;
                bool videoOnly = false;
                string insightId = mediaClient.IndexerUploadVideo(mediaClient.MediaAccount, inputAsset, ingestManifest.JobInputFileUrl, ingestManifest.JobPriority, false, audioOnly, videoOnly);
                logData = string.Concat("Insight Id: ", insightId);
                WriteLog(ingestManifest.Name, logData, logger, false);
            }
            Transform transform = mediaClient.CreateTransform(ingestManifest.TransformPresets);
            if (transform != null)
            {
                Job job = mediaClient.CreateJob(null, transform.Name, ingestManifest.JobName, ingestManifest.JobDescription, ingestManifest.JobPriority, ingestManifest.JobData, ingestManifest.JobInputFileUrl, ingestManifest.AssetName, ingestManifest.JobOutputMode, ingestManifest.JobOutputAssetAlternateIds, ingestManifest.JobOutputAssetDescriptions, ingestManifest.StreamingPolicyName);
                logData = string.Concat("Transform Name: ", transform.Name);
                WriteLog(ingestManifest.Name, logData, logger, false);
                logData = string.Concat("Job Name: ", job.Name);
                WriteLog(ingestManifest.Name, logData, logger, false);
            }
        }

        private static void WriteLog(string manifestName, string logData, ILogger logger, bool createNew)
        {
            string containerName = Constant.Storage.BlobContainer.MediaServices;
            string logName = manifestName.Replace(Constant.Media.IngestManifest.FileExtension, Constant.Media.IngestManifest.FileExtensionLog);
            CloudAppendBlob logBlob = _blobClient.GetAppendBlob(containerName, logName);
            CloudBlobStream logStream = logBlob.OpenWriteAsync(createNew).Result;
            using (StreamWriter logWriter = new StreamWriter(logStream))
            {
                logWriter.WriteLine(logData);
            }
            if (logger != null)
            {
                logger.LogInformation(logData);
            }
        }
    }
}