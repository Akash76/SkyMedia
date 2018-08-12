﻿using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Management.Media.Models;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.WebApp.Controllers
{
    public class JobController : Controller
    {
        private static SelectListItem[] GetTransforms(MediaClient mediaClient)
        {
            List<SelectListItem> transforms = new List<SelectListItem>
            {
                new SelectListItem()
            };
            Transform[] mediaTransforms = mediaClient.GetAllEntities<Transform>(MediaEntity.Transform);
            foreach (Transform mediaTransform in mediaTransforms)
            {
                SelectListItem transform = new SelectListItem()
                {
                    Text = Constant.TextFormatter.GetValue(mediaTransform.Name),
                    Value = mediaTransform.Name
                };
                transforms.Add(transform);
            }
            return transforms.ToArray();
        }

        private static SelectListItem[] GetStreamingPolicies(MediaClient mediaClient)
        {
            List<SelectListItem> policies = new List<SelectListItem>
            {
                new SelectListItem()
            };
            StreamingPolicy[] streamingPolicies = mediaClient.GetAllEntities<StreamingPolicy>(MediaEntity.StreamingPolicy);
            foreach (StreamingPolicy streamingPolicy in streamingPolicies)
            {
                SelectListItem policy = new SelectListItem()
                {
                    Text = Constant.TextFormatter.GetValue(streamingPolicy.Name),
                    Value = streamingPolicy.Name
                };
                policies.Add(policy);
            }
            return policies.ToArray();
        }

        public JsonResult Create(string transformName, string jobName, string jobDescription, string jobPriority, string inputAssetName, string inputFileUrl, string streamingPolicyName)
        {
            Job job = null;
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                bool videoAnalyzerPreset = false;
                bool audioAnalyzerPreset = false;
                Transform transform = mediaClient.GetEntity<Transform>(MediaEntity.Transform, transformName);
                foreach (TransformOutput transformOutput in transform.Outputs)
                {
                    if (transformOutput.Preset is VideoAnalyzerPreset)
                    {
                        videoAnalyzerPreset = true;
                    }
                    else if (transformOutput.Preset is AudioAnalyzerPreset)
                    {
                        audioAnalyzerPreset = true;
                    }
                }
                string indexId = null;
                if (mediaClient.IndexerIsEnabled() && (videoAnalyzerPreset || audioAnalyzerPreset))
                {
                    bool audioOnly = !videoAnalyzerPreset && audioAnalyzerPreset;
                    string videoUrl = inputFileUrl;
                    string videoName = null;
                    string videoDescription = null;
                    if (string.IsNullOrEmpty(videoUrl))
                    {
                        Asset inputAsset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, inputAssetName);
                        BlobClient blobClient = new BlobClient(mediaClient.MediaAccount, inputAsset.StorageAccountName);
                        MediaAsset mediaAsset = new MediaAsset(mediaClient.MediaAccount, inputAsset, false);
                        string fileName = mediaAsset.Files[0].Name;
                        videoUrl = blobClient.GetDownloadUrl(inputAsset.Container, fileName, false);
                        videoName = inputAsset.Name;
                        videoDescription = inputAsset.Description;
                    }
                    indexId = mediaClient.IndexerUploadVideo(mediaClient.MediaAccount, videoUrl, videoName, videoDescription, string.Empty, audioOnly);
                }
                string[] inputFileUrls = string.IsNullOrEmpty(inputFileUrl) ? null : new string[] { inputFileUrl };
                string outputAssetDescription = string.Empty;
                if (!string.IsNullOrEmpty(inputAssetName))
                {
                    Asset asset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, inputAssetName);
                    outputAssetDescription = asset.Description;
                }
                if (!string.IsNullOrEmpty(transformName))
                {
                    job = mediaClient.CreateJob(transformName, jobName, jobDescription, jobPriority, inputAssetName, inputFileUrl, null, outputAssetDescription, indexId, streamingPolicyName);
                }
            }
            return Json(job);
        }

        public void Cancel(string jobName, string transformName)
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                mediaClient.CancelJob(transformName, jobName);
            }
        }

        public JsonResult Publish(string jobName)
        {
            string publishMessage = string.Empty;
            using (DatabaseClient databaseClient = new DatabaseClient())
            {
                string collectionId = Constant.Database.Collection.ContentPublish;
                MediaPublish mediaPublish = databaseClient.GetDocument<MediaPublish>(collectionId, jobName);
                if (mediaPublish != null)
                {
                    publishMessage = MediaClient.PublishJobOutput(mediaPublish);
                }
            }
            return Json(publishMessage);
        }

        public JsonResult Get()
        {
            Job[] jobs;
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                jobs = mediaClient.GetAllEntities<Job, Transform>(MediaEntity.TransformJob, MediaEntity.Transform);
            }
            return Json(jobs);
        }

        public IActionResult Index()
        {
            string authToken = HomeController.GetAuthToken(Request, Response);
            using (MediaClient mediaClient = new MediaClient(authToken))
            {
                ViewData["transforms"] = GetTransforms(mediaClient);
                ViewData["transformJobs"] = mediaClient.GetAllEntities<Job, Transform>(MediaEntity.TransformJob, MediaEntity.Transform);
                ViewData["streamingPolicies"] = GetStreamingPolicies(mediaClient);
            }
            return View();
        }
    }
}