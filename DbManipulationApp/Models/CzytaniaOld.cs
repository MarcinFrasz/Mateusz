using System;
using System.Collections.Generic;

namespace DbManipulationApp.Models
{
    public partial class CzytaniaOld
    {
        public string? Typ { get; set; }
        public DateTime? Data { get; set; }
        public string? Sigl { get; set; }
        public string? Dzien { get; set; }
        public string? Tekst { get; set; }
        public string? Rozwazania { get; set; }
        public string? Kmtauthor { get; set; }
        public string? Kmttitle { get; set; }
        public string? Kmtcontent { get; set; }
        public string? Kmtlink { get; set; }
        public string? Kmtfotolink { get; set; }
        public string? Uwagi { get; set; }
    }
}
