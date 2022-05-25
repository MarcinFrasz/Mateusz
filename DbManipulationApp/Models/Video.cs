using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbManipulationApp.Models
{
    public partial class Video
    {
        [Key]
        public int IdVideo { get; set; }
        [Required(ErrorMessage ="Data jest pomen wymaganym.")]
        public DateTime Data { get; set; }
        [Required(ErrorMessage ="Typ czytania jest polem wymagany.")]
        public string? TypCzytania { get; set; }
        [Required(ErrorMessage ="Youtube id jest polem wymaganym")]
        public string? YoutubeId { get; set; }
    }
}
