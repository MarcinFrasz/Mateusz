namespace DbManipulationApp.Models
{
    public class KsiazkiViewModel
    {
        private string _title = "";
        private IEnumerable<Ksiazki> _books = Enumerable.Empty<Ksiazki>();
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        public IEnumerable<Ksiazki> Books
        {
            get { return _books; }
            set { _books = value; }
        }
    }
}
