using System;
using System.Collections.Generic;

namespace DbManipulationApp.Models
{
    public partial class SlownikDni
    {
        public string DzienLiturgiczny { get; set; } = null!;
        public string? NazwaDnia { get; set; }
        public bool? Swieto { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
