using System;
using System.Collections.Generic;

namespace ErsatzTV.Application.Templates
{
    public class TemplateItemsEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<TemplateItemEditViewModel> Items { get; set; } = new();
    }

    public class TemplateItemEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? CollectionId { get; set; }
        public int? SmartCollectionId { get; set; }
        public int? MediaItemId { get; set; }
        public string MediaItemType { get; set; } = string.Empty;
        public int? DurationSeconds { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class CalendarItem
    {
        public DateTime Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
