using System;

namespace ErsatzTV.Application.RemoteMedia
{
    public class RemoteMediaSourcePathReplacementEditViewModel
    {
        public int Id { get; set; }
        public string JellyfinPath { get; set; } = string.Empty;
        public string LocalPath { get; set; } = string.Empty;
        public string PlexPath { get; set; } = string.Empty;
        public string EmbyPath { get; set; } = string.Empty;
    }

    public class RemoteMediaSourceLibraryEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool ShouldSyncItems { get; set; }
        public int MediaSourceId { get; set; }
    }

    public class RemoteMediaSourceEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }
}
