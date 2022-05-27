using System;
using System.Collections.Generic;

namespace DbManipulationApp.Models
{
    public partial class Kalendarz
    {
        public DateTime Data { get; set; }
        public string? DzienLiturgiczny { get; set; }
        public string? NazwaDnia { get; set; }
        public string? KomZrodloD { get; set; }
        public string? KomZrodloM { get; set; }
        public int? IdKsiazka1 { get; set; }
        public int? IdKsiazka2 { get; set; }
        public int? IdKsiazka3 { get; set; }
        public int? NumerTygodnia { get; set; }
        public string? Patron1 { get; set; }
        public string? Patron2 { get; set; }
        public string? Patron3 { get; set; }
        public DateTime RowVersion { get; set; }
    }
}
