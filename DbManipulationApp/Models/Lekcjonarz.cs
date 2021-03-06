using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbManipulationApp.Models
{
    public partial class Lekcjonarz
    {
        [Key]
        public int IdLlekcjonarz { get; set; }
        [Required]
        public string DzienLiturgiczny { get; set; } = null!;
        [Required]
        public string? TypCzytania { get; set; }
        public string? Siglum { get; set; }
        public string? Tekst { get; set; }
        public DateTime RowVersion { get; set; }
    }
}
