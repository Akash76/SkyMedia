using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal static class Media
    {
        private static MediaTrack[] MapTextTracks(string textTracks)
        {
            List<MediaTrack> tracks = new List<MediaTrack>();
            if (!string.IsNullOrEmpty(textTracks))
            {
                string[] tracksInfo = textTracks.Split(Constant.TextDelimiter.Connection);
                foreach (string trackInfo in tracksInfo)
                {
                    MediaTrack track = new MediaTrack();
                    string[] textTrack = trackInfo.Split(Constant.TextDelimiter.Application);
                    track.Type = textTrack[0];
                    track.SourceUrl = textTrack[1];
                    string languageCode = Language.GetLanguageCode(track.SourceUrl);
                    track.Label = Language.GetLanguageLabel(languageCode);
                    tracks.Add(track);
                }
            }
            return tracks.ToArray();
        }

        private static MediaProtection[] GetContentProtection(string protectionTypes)
        {
            List<MediaProtection> contentProtection = new List<MediaProtection>();
            if (!string.IsNullOrEmpty(protectionTypes))
            {
                string[] typesInfo = protectionTypes.Split(Constant.TextDelimiter.Application);
                foreach (string typeInfo in typesInfo)
                {
                    MediaProtection protectionType = (MediaProtection)Enum.Parse(typeof(MediaProtection), typeInfo);
                    contentProtection.Add(protectionType);
                }
            }
            return contentProtection.ToArray();
        }

        private static void AddBaseStream(List<MediaStream> mediaStreams, string settingStreamName, string settingSourceUrl,
                                          string settingTextTrack, string settingContentProtection)
        {
            string streamName = AppSetting.GetValue(settingStreamName);
            string sourceUrl = AppSetting.GetValue(settingSourceUrl);
            string protectionTypes = AppSetting.GetValue(settingContentProtection);
            string textTracks = AppSetting.GetValue(settingTextTrack);
            if (!string.IsNullOrEmpty(streamName))
            {
                MediaStream mediaStream = new MediaStream()
                {
                    Name = streamName,
                    SourceUrl = sourceUrl,
                    TextTracks = MapTextTracks(textTracks),
                    CognitiveInsights = new MediaInsight[] { },
                    ContentProtection = GetContentProtection(protectionTypes)
                };
                mediaStreams.Add(mediaStream);
            }
        }

         private static int OrderLocators(ILocator leftSide, ILocator rightSide)
        {
            return DateTime.Compare(leftSide.Asset.Created, rightSide.Asset.Created);
        }

        private static MediaTrack[] GetTextTracks(MediaClient mediaClient, IndexerClient indexerClient, IAsset asset)
        {
            List<MediaTrack> textTracks = new List<MediaTrack>();

            string webVttUrl = mediaClient.GetWebVttUrl(asset);
            string indexId = indexerClient.GetIndexId(asset.Id);
            if (!string.IsNullOrEmpty(indexId))
            {
                webVttUrl = indexerClient.GetWebVttUrl(indexId, null);
            }
            if (!string.IsNullOrEmpty(webVttUrl))
            {
                MediaTrack textTrack = new MediaTrack()
                {
                    Type = Constant.Media.Stream.TextTrackCaptions,
                    SourceUrl = webVttUrl,
                    //LanguageCode = childAsset.AlternateId,
                    //Label = Language.GetLanguageLabel(textTrack.LanguageCode)
                };
                textTracks.Add(textTrack);
            }
            return textTracks.ToArray();
        }

        //private static MediaMetadata[] GetAnalyticsMetadata(MediaClient mediaClient, IAsset asset)
        //{
        //    List<MediaMetadata> analyticsMetadata = new List<MediaMetadata>();
        //    if (asset.ParentAssets.Count > 0)
        //    {
        //        string parentAssetId = asset.ParentAssets[0].Id;
        //        MediaAsset[] childAssets = mediaClient.GetAssets(parentAssetId);
        //        foreach (MediaAsset childAsset in childAssets)
        //        {
        //            string webVtt = childAsset.WebVtt;
        //            if (!string.IsNullOrEmpty(webVtt))
        //            {
        //                MediaMetadata mediaMetadata = new MediaMetadata();
        //                mediaMetadata.ProcessorName = Processor.GetProcessorName(MediaProcessor.SpeechToText);
        //                mediaMetadata.SourceUrl = mediaClient.GetLocatorUrl(childAsset.Asset, webVtt);
        //                analyticsMetadata.Insert(0, mediaMetadata);
        //            }
        //            else if (!string.IsNullOrEmpty(childAsset.AlternateId))
        //            {
        //                string[] assetMetadata = childAsset.AlternateId.Split(Constant.TextDelimiter.Identifier);
        //                MediaMetadata mediaMetadata = new MediaMetadata();
        //                mediaMetadata.ProcessorName = assetMetadata[0];
        //                mediaMetadata.DocumentId = assetMetadata[1];
        //                analyticsMetadata.Add(mediaMetadata);
        //            }
        //        }
        //    }
        //    return analyticsMetadata.ToArray();
        //}

        //private static MediaMetadata[] GetAnalyticsMetadata(MediaClient mediaClient, IAsset asset)
        //{
        //    List<MediaMetadata> analyticsMetadata = new List<MediaMetadata>();
        //    if (asset.ParentAssets.Count > 0)
        //    {
        //        IAsset parentAsset = asset.ParentAssets[0];
        //        if (!string.IsNullOrEmpty(parentAsset.AlternateId))
        //        {
        //            string[] assetMetadata = parentAsset.AlternateId.Split(Constant.TextDelimiter.Identifier);
        //            MediaMetadata mediaMetadata = new MediaMetadata();
        //            mediaMetadata.ProcessorName = assetMetadata[0];
        //            mediaMetadata.DocumentId = assetMetadata[1];
        //            analyticsMetadata.Add(mediaMetadata);
        //        }
        //    }
        //    return analyticsMetadata.ToArray();
        //}

        private static MediaStream GetMediaStream(MediaClient mediaClient, IndexerClient indexerClient, IAsset asset)
        {
            MediaStream mediaStream = new MediaStream()
            {
                Name = asset.Name,
                SourceUrl = mediaClient.GetLocatorUrl(asset),
                TextTracks = GetTextTracks(mediaClient, indexerClient, asset),
                //AnalyticsMetadata = GetAnalyticsMetadata(mediaClient, asset),
                ContentProtection = mediaClient.GetContentProtection(asset)
            };
            string indexId = indexerClient.GetIndexId(asset.Id);
            if (!string.IsNullOrEmpty(indexId) && indexerClient != null)
            {
                //mediaStream.InsightsUrl = indexerClient.GetInsightsUrl(indexId);
            }
            return mediaStream;
        }

        public static MediaStream[] GetMediaStreams(MediaClient mediaClient, IndexerClient indexerClient)
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();
            string settingKey = Constant.AppSettingKey.MediaLocatorMaxStreamCount;
            int maxStreamCount = int.Parse(AppSetting.GetValue(settingKey));
            ILocator[] locators = mediaClient.GetEntities(MediaEntity.Locator) as ILocator[];
            Array.Sort<ILocator>(locators, OrderLocators);
            foreach (ILocator locator in locators)
            {
                IAsset asset = locator.Asset;
                if (asset.IsStreamable && asset.AssetType != AssetType.SmoothStreaming) // Live Streaming
                {
                    MediaStream mediaStream = GetMediaStream(mediaClient, indexerClient, asset);
                    mediaStreams.Add(mediaStream);
                    if (!string.IsNullOrEmpty(mediaStream.SourceUrl))
                    {
                        foreach (IStreamingAssetFilter filter in asset.AssetFilters)
                        {
                            MediaStream streamFilter = mediaStream;
                            streamFilter.Name = string.Concat(streamFilter.Name, Constant.Media.Stream.AssetFilteredSuffix);
                            streamFilter.SourceUrl = string.Concat(streamFilter.SourceUrl, "(filter=", filter.Name, ")");
                            mediaStreams.Add(streamFilter);
                        }
                    }
                    if (mediaStreams.Count == maxStreamCount)
                    {
                        break;
                    }
                }
            }
            return mediaStreams.ToArray();
        }

        public static MediaStream[] GetMediaStreams()
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();

            string settingKey1 = Constant.AppSettingKey.MediaStream1Name;
            string settingKey2 = Constant.AppSettingKey.MediaStream1SourceUrl;
            string settingKey3 = Constant.AppSettingKey.MediaStream1TextTracks;
            string settingKey4 = Constant.AppSettingKey.MediaStream1ContentProtection;
            AddBaseStream(mediaStreams, settingKey1, settingKey2, settingKey3, settingKey4);

            settingKey1 = Constant.AppSettingKey.MediaStream2Name;
            settingKey2 = Constant.AppSettingKey.MediaStream2SourceUrl;
            settingKey3 = Constant.AppSettingKey.MediaStream2TextTracks;
            settingKey4 = Constant.AppSettingKey.MediaStream2ContentProtection;
            AddBaseStream(mediaStreams, settingKey1, settingKey2, settingKey3, settingKey4);

            settingKey1 = Constant.AppSettingKey.MediaStream3Name;
            settingKey2 = Constant.AppSettingKey.MediaStream3SourceUrl;
            settingKey3 = Constant.AppSettingKey.MediaStream3TextTracks;
            settingKey4 = Constant.AppSettingKey.MediaStream3ContentProtection;
            AddBaseStream(mediaStreams, settingKey1, settingKey2, settingKey3, settingKey4);

            return mediaStreams.ToArray();
        }

        public static MediaStream[] GetLiveStreams(MediaClient mediaClient, IndexerClient indexerClient)
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();
            IChannel[] channels = mediaClient.GetEntities(MediaEntity.Channel) as IChannel[];
            foreach (IChannel channel in channels)
            {
                foreach (IProgram program in channel.Programs)
                {
                    MediaStream mediaStream = GetMediaStream(mediaClient, indexerClient, program.Asset);
                    mediaStreams.Add(mediaStream);
                }
            }
            return mediaStreams.ToArray();
        }

        public static bool IsStreamingEnabled(MediaClient mediaClient)
        {
            bool streamingEnabled = false;
            IStreamingEndpoint[] streamingEndpoints = mediaClient.GetEntities(MediaEntity.StreamingEndpoint) as IStreamingEndpoint[];
            foreach (IStreamingEndpoint streamingEndpoint in streamingEndpoints)
            {
                if (streamingEndpoint.State == StreamingEndpointState.Running ||
                    streamingEndpoint.State == StreamingEndpointState.Scaling)
                {
                    streamingEnabled = true;
                }
            }
            return streamingEnabled;
        }
    }
}
