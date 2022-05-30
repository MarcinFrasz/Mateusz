using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbManipulationApp.Models
{
    public partial class Video
    {
        [Key]
        public int IdVideo { get; set; }
        [Required(ErrorMessage = "Pole Data jest wymagane")]
        public DateTime Data { get; set; }
        [Required(ErrorMessage ="Pole TypCzytania jest wymagane.")]
        public string? TypCzytania { get; set; }
        [Required(ErrorMessage ="Pole YoutubeId jest wymagane.")]
        public string? YoutubeId { get; set; }
        [Required]
        public DateTime RowVersion { get; set; }
    }
}
