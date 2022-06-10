using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbManipulationApp.Models
{
    public partial class Komentarze
    {
        [Key]
        public int Idkom { get; set; }
        [Required(ErrorMessage ="Pole jest wymagane.")]
        public string DzienLiturgiczny { get; set; } = null!;
        [Required(ErrorMessage = "Pole jest wymagane.")]
        public string? KomZrodlo { get; set; }
        [Required(ErrorMessage = "Pole jest wymagane.")]
        public string? Tekst { get; set; }
        public DateTime RowVersion { get; set; }
    }
}
