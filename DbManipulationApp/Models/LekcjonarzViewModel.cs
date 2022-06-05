using System.ComponentModel.DataAnnotations;

namespace DbManipulationApp.Models
{
    public class LekcjonarzViewModel
    {
        private string dzien_liturgiczny = "";
        private IEnumerable<Lekcjonarz> _lekcjonarze = Enumerable.Empty<Lekcjonarz>();
        public string DzienLiturgiczny
        {
            get { return dzien_liturgiczny; }
            set { dzien_liturgiczny = value; }
        }
        public IEnumerable<Lekcjonarz> Lekcjonarze
        {
            get { return _lekcjonarze; }
            set { _lekcjonarze = value; }
        }
    }
}
