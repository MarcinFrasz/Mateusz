using System;
using System.Collections.Generic;

namespace DbManipulationApp.Models
{
    public partial class Komentarze
    {
        public int Idkom { get; set; }
        public string DzienLiturgiczny { get; set; } = null!;
        public string? KomZrodlo { get; set; }
        public string? Tekst { get; set; }
    }
}
