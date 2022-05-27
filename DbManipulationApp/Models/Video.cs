using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbManipulationApp.Models
{
    public partial class Video
    {
        [Key]
        public int IdVideo { get; set; }
        public DateTime Data { get; set; }
        public string? TypCzytania { get; set; }
        public string? YoutubeId { get; set; }
        public DateTime RowVersion { get; set; }
    }
}
