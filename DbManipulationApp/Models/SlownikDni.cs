using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbManipulationApp.Models
{
    public partial class SlownikDni
    {
        [Key]
        [StringLength(8)]
        [RegularExpression(@"^([0-9A-Z]{8})$",ErrorMessage ="Pole DzienLiturgiczny musi zawierać 8 znaków [A-Z 0-9]")]
        public string DzienLiturgiczny { get; set; } = null!;
        [Required(ErrorMessage ="Pole NazwaDnia jest wymagane.")]
        [StringLength(255)]
        public string? NazwaDnia { get; set; }
        public bool Swieto { get; set; }
        public DateTime? Timestamp { get; set; }
        [Required]
        public DateTime RowVersion { get; set; }
    }
}
