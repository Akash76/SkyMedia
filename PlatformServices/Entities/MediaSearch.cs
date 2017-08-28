﻿namespace AzureSkyMedia.PlatformServices
{
    public class MediaSearchCriteria
    {
        public string IndexId { get; set; }
        
        public string AssetId { get; set; }

        public bool PublicVideo { get; set; }

        public string SearchPartition { get; set; }

        public string TextScope { get; set; }

        public string TextQuery { get; set; }

        public string Owner { get; set; }

        public string Face { get; set; }

        public string Language { get; set; }

        public int PageSize { get; set; }

        public int SkipCount { get; set; }
    }
}