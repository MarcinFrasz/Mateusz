using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbManipulationApp.Models
{
    public partial class Ksiazki
    {
        [Key]
        public int IdKsiazki { get; set; }
        [Required(ErrorMessage ="Pole jest wymagane.")]
        public string? IdKmt { get; set; }
        [Required(ErrorMessage = "Pole jest wymagane.")]
        public string? Tytul { get; set; }
        public string? Autor { get; set; }
        [Required(ErrorMessage = "Pole jest wymagane.")]
        public string? Foto { get; set; }
        [Required(ErrorMessage = "Pole jest wymagane.")]
        public string? Opis { get; set; }
        public DateTime RowVersion { get; set; }
    }
}
