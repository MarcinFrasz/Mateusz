using System.ComponentModel.DataAnnotations;

namespace DbManipulationApp.Models
{
    public class PatroniViewModel
    {
        private string _date = "";
        private IEnumerable<Patroni> _videos = Enumerable.Empty<Patroni>();
        public string Date
        {
            get { return _date; }
            set { _date = value; }
        }
        public IEnumerable<Patroni> Patroni
        {
            get { return _videos; }
            set { _videos = value; }
        }
    }
}
