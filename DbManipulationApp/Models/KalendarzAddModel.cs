using System.ComponentModel.DataAnnotations;

namespace DbManipulationApp.Models
{
    public class KalendarzAddModel
    {
        private bool _show_patron = false;
        private string _book_id="";
        private Kalendarz _kalendarz=new();
        [Required(ErrorMessage = "Pole jest wymagane.")]
        public string BookId
        {
            get { return _book_id; }
            set { _book_id = value; }
        }
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
