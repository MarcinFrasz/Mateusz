using System;
using System.Collections.Generic;

namespace DbManipulationApp.Models
{
    public partial class Patroni
    {
        public int Id { get; set; }
        public string Data { get; set; } = null!;
        public string? Patron { get; set; }
        public bool Glowny { get; set; }
        public string? Opis { get; set; }
        public string? Tekst { get; set; }
        public bool? Wyswietl { get; set; }
    }
}
