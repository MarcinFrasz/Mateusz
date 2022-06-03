namespace DbManipulationApp.Models
{
    public class PatroniEditModel
    {
        private Patroni? _mainRecord;
        private Patroni? _editedRecord;

        public Patroni MainRecord
        {
            get { return _mainRecord; }
            set { _mainRecord = value; }
        }
        public Patroni EditedRecord
        {
            get { return _editedRecord; }
            set { _editedRecord = value; }
        }


    }
}
