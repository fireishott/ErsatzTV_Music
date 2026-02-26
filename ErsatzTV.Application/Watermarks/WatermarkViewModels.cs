using System;

namespace ErsatzTV.Application.Watermarks
{
    public class WatermarkEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public int Opacity { get; set; }
        public string Location { get; set; } = string.Empty;
    }
}
