namespace DbManipulationApp.Models
{
    public class KalendarzViewModel
    {
        private DateTime _date = new();
        private IEnumerable<Kalendarz> _kalendarz = Enumerable.Empty<Kalendarz>();
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }
        public IEnumerable<Kalendarz> Kalendarz
        {
            get { return _kalendarz; }
            set { _kalendarz = value; }
        }
    }
}
