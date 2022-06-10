using System.ComponentModel.DataAnnotations;

namespace DbManipulationApp.Models
{
    public class KalendarzEditModel
    {

        private string _book_id = "";
        private Kalendarz? _mainRecord;
        private Kalendarz? _editedRecord;
   
        [Required]
        public string BookId
        {
            get { return _book_id; }
            set { _book_id = value; }
        }
        public Kalendarz MainRecord
        {
            get { return _mainRecord; }
            set { _mainRecord = value; }
        }
        public Kalendarz EditedRecord
        {
            get { return _editedRecord; }
            set { _editedRecord = value; }
        }
    }
}
