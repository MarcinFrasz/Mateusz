using System.ComponentModel.DataAnnotations;

namespace DbManipulationApp.Models
{
    public class KomentarzeViewModel
    {
        private string dzien_liturgiczny = "";
        private IEnumerable<Komentarze> _komentarze = Enumerable.Empty<Komentarze>();
        public string DzienLiturgiczny
        {
            get { return dzien_liturgiczny; }
            set { dzien_liturgiczny = value; }
        }
        public IEnumerable<Komentarze> Komentarzes
        {
            get { return _komentarze; }
            set { _komentarze = value; }
        }
    }
}
