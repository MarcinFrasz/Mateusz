namespace DbManipulationApp.Models
{
    public class SlownikDniEditModel
    {
        private SlownikDni? _mainRecord;
        private SlownikDni? _editedRecord;

        public SlownikDni MainRecord
        {
            get { return _mainRecord; }
            set { _mainRecord = value; }
        }
        public SlownikDni EditedRecord
        {
            get { return _editedRecord; }
            set { _editedRecord = value; }
        }
    }
}
