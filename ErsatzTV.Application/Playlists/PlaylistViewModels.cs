using System;
using System.Collections.Generic;

namespace ErsatzTV.Application.Playlists
{
    public class PlaylistItemsEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<PlaylistItemEditViewModel> Items { get; set; } = new();
    }

    public class PlaylistItemEditViewModel
    {
        public int Id { get; set; }
        public int PlaylistId { get; set; }
        public int? CollectionId { get; set; }
        public int? SmartCollectionId { get; set; }
        public int? MediaItemId { get; set; }
        public string MediaItemType { get; set; } = string.Empty;
        public int Order { get; set; }
    }

    public class PlaylistTreeItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<PlaylistTreeItemViewModel> Children { get; set; } = new();
    }
}
