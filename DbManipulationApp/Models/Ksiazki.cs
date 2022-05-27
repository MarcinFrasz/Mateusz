using System;
using System.Collections.Generic;

namespace DbManipulationApp.Models
{
    public partial class Ksiazki
    {
        public int IdKsiazki { get; set; }
        public string? IdKmt { get; set; }
        public string? Tytul { get; set; }
        public string? Autor { get; set; }
        public string? Foto { get; set; }
        public string? Opis { get; set; }
        public DateTime RowVersion { get; set; }
    }
}
