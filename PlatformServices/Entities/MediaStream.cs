﻿namespace AzureSkyMedia.PlatformServices
{
    public struct MediaStream
    {
        public string Name { get; set; }

        public string SourceUrl { get; set; }

        public MediaTrack[] TextTracks { get; set; }

        public MediaInsight[] CognitiveInsights { get; set; }

        public MediaProtection[] ContentProtection { get; set; }
    }
}