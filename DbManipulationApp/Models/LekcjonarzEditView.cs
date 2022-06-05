namespace DbManipulationApp.Models
{
    public class LekcjonarzEditView
    {
        private Lekcjonarz? _mainRecord;
        private Lekcjonarz? _editedRecord;

        public Lekcjonarz MainRecord
        {
            get { return _mainRecord; }
            set { _mainRecord = value; }
        }
        public Lekcjonarz EditedRecord
        {
            get { return _editedRecord; }
            set { _editedRecord = value; }
        }


    }
}
