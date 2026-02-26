using System;
using System.Collections.Generic;

namespace ErsatzTV.Application.Decos
{
    public class DecoEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<DecoBreakContentEditViewModel> BreakContents { get; set; } = new();
    }

    public class DecoBreakContentEditViewModel
    {
        public int Id { get; set; }
        public int DecoId { get; set; }
        public int? CollectionId { get; set; }
        public int? SmartCollectionId { get; set; }
        public int? MediaItemId { get; set; }
        public string MediaItemType { get; set; } = string.Empty;
        public int DurationSeconds { get; set; }
        public int Order { get; set; }
    }

    public class DecoTreeItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<DecoTreeItemViewModel> Children { get; set; } = new();
    }
}
