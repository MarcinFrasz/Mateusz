using System;
using System.Collections.Generic;

namespace DbManipulationApp.Models
{
    public partial class Video
    {
        public int IdVideo { get; set; }
        public DateTime Data { get; set; }
        public string? TypCzytania { get; set; }
        public string? YoutubeId { get; set; }
    }
}
