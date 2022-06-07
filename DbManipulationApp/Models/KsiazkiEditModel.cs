namespace DbManipulationApp.Models
{
    public class KsiazkiEditModel
    {
        private Ksiazki? _mainRecord;
        private Ksiazki? _editedRecord;

        public Ksiazki MainRecord
        {
            get { return _mainRecord; }
            set { _mainRecord = value; }
        }
        public Ksiazki EditedRecord
        {
            get { return _editedRecord; }
            set { _editedRecord = value; }
        }
    }
}
