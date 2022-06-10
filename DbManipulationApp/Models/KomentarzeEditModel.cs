namespace DbManipulationApp.Models
{
    public class KomentarzeEditModel
    {
        private Komentarze? _mainRecord;
        private Komentarze? _editedRecord;

        public Komentarze MainRecord
        {
            get { return _mainRecord; }
            set { _mainRecord = value; }
        }
        public Komentarze EditedRecord
        {
            get { return _editedRecord; }
            set { _editedRecord = value; }
        }
    }
}
