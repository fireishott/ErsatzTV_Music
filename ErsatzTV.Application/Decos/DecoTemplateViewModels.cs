using System;
using System.Collections.Generic;

namespace ErsatzTV.Application.Decos
{
    public class DecoTemplateItemsEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<DecoTemplateItemEditViewModel> Items { get; set; } = new();
    }

    public class DecoTemplateItemEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? CollectionId { get; set; }
        public int? SmartCollectionId { get; set; }
        public int? MediaItemId { get; set; }
        public string MediaItemType { get; set; } = string.Empty;
        public int DurationSeconds { get; set; }
        public int Order { get; set; }
    }

    public class DecoTemplateTreeItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<DecoTemplateTreeItemViewModel> Children { get; set; } = new();
    }
}
