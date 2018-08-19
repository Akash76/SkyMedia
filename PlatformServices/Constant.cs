﻿using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AzureSkyMedia.WebApp")]
[assembly: InternalsVisibleTo("AzureSkyMedia.FunctionApp")]

namespace AzureSkyMedia.PlatformServices
{
    public struct Constant
    {
        public const string AppSettings = "appsettings.json";
        public const string ModelsDirectory = "Models";

        public struct TextDelimiter
        {
            public const char Connection = ';';
            public const char Application = ',';
            public const string File = ".";
        }

        public struct TextFormatter
        {
            public const string NumericLong = "N0";

            public static readonly string[] SpacingPatterns = new string[] { "([a-z])([A-Z])", "(H264)([A-Z])", "([a-z])(720p|1080p)" };
            public static readonly string[] SpacingInserts = new string[] { "$1 $2", "$1 $2", "$1 $2" };

            public static string GetValue(string value)
            {
                for (int i = 0; i < SpacingPatterns.Length; i++)
                {
                    value = Regex.Replace(value, SpacingPatterns[i], SpacingInserts[i]);
                }
                return value;
            }
        }

        public struct AppSettingKey
        {
            public const string AppTitle = "App.Title";
            public const string AppRegionName = "App.RegionName";

            public const string AppApiVersion = "App.Api.Version";
            public const string AppApiEndpointUrl = "App.Api.EndpointUrl";
            public const string AppApiDescription = "App.Api.Description";

            public const string AzureResourceManagementEndpointUrl = "Arm.EndpointUrl";
            public const string AzureResourceManagementAudienceUrl = "Arm.AudienceUrl";

            public const string DirectoryTenantId = "Directory.TenantId";
            public const string DirectoryIssuerUrl = "Directory.IssuerUrl";
            public const string DirectoryDiscoveryPath = "Directory.DiscoveryPath";

            public const string DirectoryClientId = "Directory.ClientId";
            public const string DirectoryClientSecret = "Directory.ClientSecret";

            public const string DirectoryPolicyIdSignUpIn = "Directory.PolicyId.SignUpIn";
            public const string DirectoryPolicyIdProfileEdit = "Directory.PolicyId.ProfileEdit";
            public const string DirectoryPolicyIdPasswordReset = "Directory.PolicyId.PasswordReset";

            public const string DatabaseRegionsRead = "Database.Regions.Read";
            public const string DatabaseCollectionThroughputUnits = "Database.Collection.ThroughputUnits";

            public const string StorageCdnEndpointUrl = "Storage.CdnEndpointUrl";
            public const string StorageSharedAccessMinutes = "Storage.SharedAccessMinutes";

            public const string MediaStreamTunerPageSize = "Media.Stream.TunerPageSize";

            public const string MediaStream1Name = "Media.Stream1.Name";
            public const string MediaStream1SourceUrl = "Media.Stream1.SourceUrl";
            public const string MediaStream1TextTracks = "Media.Stream1.TextTracks";

            public const string MediaStream2Name = "Media.Stream2.Name";
            public const string MediaStream2SourceUrl = "Media.Stream2.SourceUrl";
            public const string MediaStream2TextTracks = "Media.Stream2.TextTracks";

            public const string MediaStream3Name = "Media.Stream3.Name";
            public const string MediaStream3SourceUrl = "Media.Stream3.SourceUrl";
            public const string MediaStream3TextTracks = "Media.Stream3.TextTracks";

            public const string MediaStream4Name = "Media.Stream4.Name";
            public const string MediaStream4SourceUrl = "Media.Stream4.SourceUrl";
            public const string MediaStream4TextTracks = "Media.Stream4.TextTracks";

            public const string MediaPublishUrl = "Media.Publish.Url";
            public const string MediaPublishQueue = "Media.Publish.Queue";

            public const string MediaPlayerVersion = "Media.Player.Version";
            public const string MediaPlayerSkin = "Media.Player.Skin";

            public const string MediaClipperVersion = "Media.Clipper.Version";

            public const string MediaIndexerApiUrl = "Media.Indexer.ApiUrl";
            public const string MediaIndexerLocation = "Media.Indexer.Location";

            public const string AccountEndpointPrefix = "AccountEndpoint=";
            public const string AccountNamePrefix = "AccountName=";
            public const string AccountKeyPrefix = "AccountKey=";
            public const string DatabaseIdPrefix = "DatabaseId=";

            public const string AzureStorage = "Storage";
            public const string AzureDatabase = "Database";

            public const string TwilioAccountId = "Twilio.Account.Id";
            public const string TwilioAccountToken = "Twilio.Account.Token";
            public const string TwilioMessageFrom = "Twilio.Message.From";
        }

        public struct HttpHeader
        {
            public const string ApiManagementKey = "Ocp-Apim-Subscription-Key";
        }

        public struct HttpForm
        {
            public const string IdToken = "id_token";
        }

        public struct HttpCookie
        {
            public const string UserAuthToken = "UserAuthToken";
        }

        public struct UserAttribute
        {
            public const string EMail = "email";
            public const string EMails = "emails";
            public const string MobileNumber = "extension_MobileNumber";

            public const string MediaAccountName = "extension_MediaAccountName";
            public const string MediaAccountSubscriptionId = "extension_MediaAccountSubscriptionId";
            public const string MediaAccountResourceGroupName = "extension_MediaAccountResourceGroupName";
            public const string MediaAccountDirectoryTenantId = "extension_MediaAccountDirectoryTenantId";
            public const string MediaAccountServicePrincipalId = "extension_MediaAccountServicePrincipalId";
            public const string MediaAccountServicePrincipalKey = "extension_MediaAccountServicePrincipalKey";
            public const string MediaAccountVideoIndexerKey = "extension_MediaAccountVideoIndexerKey";

            public const string StorageAccount1Name = "extension_StorageAccount1Name";
            public const string StorageAccount1Key = "extension_StorageAccount1Key";
            public const string StorageAccount2Name = "extension_StorageAccount2Name";
            public const string StorageAccount2Key = "extension_StorageAccount2Key";
            public const string StorageAccount3Name = "extension_StorageAccount3Name";
            public const string StorageAccount3Key = "extension_StorageAccount3Key";
        }

        public struct Storage
        {
            public struct BlobContainer
            {
                public const string MediaServices = "ams";
                public const string FileUpload = "upload";
            }
        }

        public struct Database
        {
            public struct Collection
            {
                public const string ContentPublish = "ContentPublish";
                public const string ContentInsight = "ContentInsight";
                public const string IngestManifest = "IngestManifest";
            }

            public struct Script
            {
                public const string IsTimecodeFragment = "isTimecodeFragment.js";
                public const string GetTimecodeFragment = "getTimecodeFragment.js";
            }

            public struct Document
            {
                public const string SystemPropertyPrefix = "_";
            }
        }

        public struct Media
        {
            public const string PredefinedPrefix = "Predefined_";
            public const string AccountResourceId = "/subscriptions/{0}/resourceGroups/{1}/providers/microsoft.media/mediaServices/{2}";

            public struct Channel9
            {
                public const int IngestVideoCount = 8;
                public const string NamespacePrefix = "media";
                public const string NamespaceUrl = "http://search.yahoo.com/mrss/";
                public const string XPathQuery = "media:group/media:content[@type='video/mp4']";
                public const string UrlHttp = "http://video.ch9.ms/";
                public const string UrlHttps = "https://sec.ch9.ms/";
                public const string Http = "http://";
                public const string Https = "https://";
            }

            public struct IngestManifest
            {
                public const string GalleryPrefix = "Gallery.";
                public const string TriggerPrefix = "MediaIngestManifest";
                public const string FileExtension = ".json";
                public const string FileExtensionLog = ".log";
            }

            public struct ContentType
            {
                public const string IngestContent = "video/mp4";
                public const string IngestManifest = "application/json";
            }

            public struct ContentKey
            {
                public const string PolicyAES = "Content Key Policy AES";
                public const string PolicyDRM = "Content Key Policy DRM";
            }

            public struct Asset
            {
                public const string NameDelimiter = " - ";
                public const string SingleBitrate = "SBR";
            }

            public struct Transform
            {
                public const string PresetNameDelimiter = ", ";
                public const string PresetNameAnalyzerVideo = "VideoAnalyzer";
                public const string PresetNameAnalyzerAudio = "AudioAnalyzer";
            }

            public struct Transcript
            {
                public const string Analyzer = "transcript.vtt";
                public const string Indexer = "transcript-vi.vtt";
            }

            public struct Publish
            {
                public const string EventTriggerName = "AMS-Job-State";
                public static readonly string[] EventTriggerTypes = new string[] { "Microsoft.Media.JobStateChange" };
            }

            public struct Stream
            {
                public const string DefaultEndpoint = "default";
                public const string ManifestExtension = ".ism";
                public const string InsightExtension = ".json";
            }

            public struct Track
            {
                public const string CaptionsType = "captions";
                public const string CaptionsLabel = "Captions On";
            }

            public struct Thumbnail
            {
                public const string Best = "Thumbnail000001.jpg";
            }

            public struct Live
            {
                public const string AllowedPreviewAccess = "Allowed Preview Access";
            }
        }

        public struct Message
        {
            public const string UserPasswordForgotten = "AADB2C90118";
            public const string MobileNumberNotAvailable = "No message sent. Mobile number not available.";
            public const string StreamingEndpointNotStarted = "Your media account streaming endpoint has not been started.";
            public const string StorageAccountReadPermission = " (Your AMS Service Principal does not have storage account read permission)";
        }
    }
}