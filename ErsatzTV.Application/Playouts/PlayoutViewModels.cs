using System;
using System.Collections.Generic;

namespace ErsatzTV.Application.Playouts
{
    public class PlayoutEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ChannelId { get; set; }
        public int PlayoutTemplateId { get; set; }
    }

    public class PlayoutTemplateEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class PlayoutAlternateScheduleEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public int ScheduleId { get; set; }
    }
}
