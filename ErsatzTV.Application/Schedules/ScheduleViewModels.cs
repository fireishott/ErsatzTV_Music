using System;
using System.Collections.Generic;

namespace ErsatzTV.Application.Schedules
{
    public class ProgramScheduleEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ProgramScheduleItemEditViewModel
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public int? CollectionId { get; set; }
        public int? SmartCollectionId { get; set; }
        public int? MediaItemId { get; set; }
        public string MediaItemType { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public int? DurationMinutes { get; set; }
        public string PlaybackOrder { get; set; } = string.Empty;
    }

    public class ProgramScheduleItemsEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ProgramScheduleItemEditViewModel> Items { get; set; } = new();
    }
}
