using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DbManipulationApp.Models
{
    public partial class Kalendarz
    {
        [Key]
        public DateTime Data { get; set; }
        [Required(ErrorMessage = "Pole jest wymagane.")]
        public string? DzienLiturgiczny { get; set; }
        [Required(ErrorMessage = "Pole jest wymagane.")]
        public string? NazwaDnia { get; set; }
        [Required(ErrorMessage = "Pole jest wymagane.")]
        public string? KomZrodloD { get; set; }
        [Required(ErrorMessage = "Pole jest wymagane.")]
        public string? KomZrodloM { get; set; }
        public int? IdKsiazka1 { get; set; }
        public int? IdKsiazka2 { get; set; }
        public int? IdKsiazka3 { get; set; }
        [Required(ErrorMessage = "Pole jest wymagane.")]
        [RegularExpression(@"^([2-9]{1}[0-9]{3}[0-5]{1}[0-9]{1})$",ErrorMessage ="Błędny format danych!")]
        public int? NumerTygodnia { get; set; }
        public string? Patron1 { get; set; }
        public string? Patron2 { get; set; }
        public string? Patron3 { get; set; }
        public DateTime RowVersion { get; set; }
    }
}
