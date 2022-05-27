using System;
using System.Collections.Generic;

namespace DbManipulationApp.Models
{
    public partial class Lekcjonarz
    {
        public int IdLlekcjonarz { get; set; }
        public string DzienLiturgiczny { get; set; } = null!;
        public string? TypCzytania { get; set; }
        public string? Siglum { get; set; }
        public string? Tekst { get; set; }
        public DateTime RowVersion { get; set; }
    }
}
