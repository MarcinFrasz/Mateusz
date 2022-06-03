using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbManipulationApp.Models
{
    public partial class Patroni
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(5)]
        [RegularExpression(@"^([0][1-9]|[1][0-2])[-]([0][1-9]|[1-2][0-9]|[3][0-1])$",ErrorMessage ="Niepoprawny format daty. Wymagany format to mm-dd.")]
        public string Data { get; set; } = null!;
        [Required]
        [StringLength(50,ErrorMessage ="Patron może zawierać maksymalnie 50 znaków")]
        public string? Patron { get; set; }
        public bool Glowny { get; set; }
        public string? Opis { get; set; }
        [StringLength(50, ErrorMessage = "Opis może zawierać maksymalnie 50 znaków")]
        public string? Tekst { get; set; }
        public bool? Wyswietl { get; set; }
        public DateTime RowVersion { get; set; }
    }
}
