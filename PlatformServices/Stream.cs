using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public static class Stream
    {
        private static MediaTextTrack[] GetTextTracks(MediaClient mediaClient, IAsset asset, LocatorType locatorType)
        {
            List<MediaTextTrack> textTracks = new List<MediaTextTrack>();
            string fileExtension = Constant.Media.FileExtension.WebVtt;
            string[] fileNames = MediaClient.GetFileNames(asset, fileExtension);
            foreach (string fileName in fileNames)
            {
                MediaTextTrack textTrack = new MediaTextTrack();
                textTrack.Type = Constant.Media.Stream.TextTrackSubtitles;
                textTrack.SourceUrl = mediaClient.GetLocatorUrl(asset, locatorType, fileName);
                textTrack.LanguageCode = Language.GetLanguageCode(textTrack);
                textTrack.Label = Language.GetLanguageLabel(textTrack.LanguageCode);
                textTracks.Add(textTrack);
            }
            return textTracks.ToArray();
        }

        private static int OrderLocators(ILocator leftSide, ILocator rightSide)
        {
            return DateTime.Compare(leftSide.Asset.Created, rightSide.Asset.Created);
        }

        public static MediaStream[] GetMediaStreams(MediaClient mediaClient)
        {
            List<MediaStream> mediaStreams = new List<MediaStream>();
            string settingKey = Constant.AppSettingKey.MediaLocatorMaxStreamCount;
            int maxStreamCount = int.Parse(AppSetting.GetValue(settingKey));
            ILocator[] locators = mediaClient.GetEntities(MediaEntity.Locator) as ILocator[];
            Array.Sort<ILocator>(locators, OrderLocators);
            foreach (ILocator locator in locators)
            {
                IAsset asset = locator.Asset;
                if (asset.IsStreamable && asset.AssetFiles.Count() > 1)
                {
                    string locatorUrl = mediaClient.GetLocatorUrl(asset, locator.Type, null);
                    if (!string.IsNullOrEmpty(locatorUrl))
                    {
                        MediaTextTrack[] textTracks = GetTextTracks(mediaClient, asset, locator.Type);
                        MediaProtection[] protectionTypes = mediaClient.GetProtectionTypes(asset);
                        MediaMetadata[] analyticsMetadata = Processor.GetAnalyticsMetadata(asset);

                        MediaStream mediaStream = new MediaStream();
                        mediaStream.Name = asset.Name;
                        mediaStream.SourceUrl = locatorUrl;
                        mediaStream.TextTracks = textTracks;
                        mediaStream.ProtectionTypes = protectionTypes;
                        mediaStream.AnalyticsMetadata = analyticsMetadata;
                        mediaStreams.Add(mediaStream);

                        foreach (IStreamingAssetFilter filter in asset.AssetFilters)
                        {
                            mediaStream = new MediaStream();
                            mediaStream.Name = string.Concat(asset.Name, " (Filtered)");
                            mediaStream.SourceUrl = string.Concat(locatorUrl, "(filter=", filter.Name, ")");
                            mediaStream.TextTracks = new MediaTextTrack[] { };
                            mediaStream.ProtectionTypes = new MediaProtection[] { };
                            mediaStream.AnalyticsMetadata = new MediaMetadata[] { };
                            mediaStreams.Add(mediaStream);
                        }
                    }

                }
                if (mediaStreams.Count == maxStreamCount)
                {
                    break;
                }
            }
            return mediaStreams.ToArray();
        }

        public static string GetSourceUrl(string channelName, bool livePreview)
        {
            string liveSourceUrl = string.Empty;
            string settingKey = Constant.AppSettingKey.AzureMedia;
            string[] liveAccount = AppSetting.GetValue(settingKey, true);
            if (liveAccount.Length > 0)
            {
                MediaClient mediaClient = new MediaClient(liveAccount[0], liveAccount[1]);
                IChannel channel = mediaClient.GetEntityByName(MediaEntity.Channel, channelName, true) as IChannel;
                if (channel != null && channel.State == ChannelState.Running)
                {
                    if (livePreview)
                    {
                        liveSourceUrl = channel.Preview.Endpoints.First().Url.ToString();
                    }
                    else
                    {
                        IProgram program = channel.Programs.First();
                        if (program.State == ProgramState.Running)
                        {
                            liveSourceUrl = mediaClient.GetLocatorUrl(program.Asset, LocatorType.OnDemandOrigin, null);
                        }
                    }
                }
            }
            return liveSourceUrl;
        }

        public static DateTime? GetEventStart(string accountName, string channelName)
        {
            DateTime? eventStart = null;
            EntityClient entityClient = new EntityClient();
            string tableName = Constant.Storage.TableName.LiveEvent;
            LiveEvent liveEvent = entityClient.GetEntity<LiveEvent>(tableName, accountName, channelName);
            if (liveEvent != null)
            {
                eventStart = liveEvent.EventStart;
            }
            return eventStart;
        }
    }
}
