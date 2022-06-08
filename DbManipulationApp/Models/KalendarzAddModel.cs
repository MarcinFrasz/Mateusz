namespace DbManipulationApp.Models
{
    public class KalendarzAddModel
    {
        private bool _show_patron = false;
        private Kalendarz _kalendarz=new();
        public bool ShowPatron
        {
            get { return _show_patron; }
            set { _show_patron = value; }
        }
        public Kalendarz Kalendarz
        {
            get { return _kalendarz; }
            set { _kalendarz = value; }
        }
    }
}
