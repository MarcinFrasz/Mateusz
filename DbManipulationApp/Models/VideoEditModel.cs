namespace DbManipulationApp.Models
{
    public class VideoEditModel
    {
        private Video? _mainRecord;
        private Video? _editedRecord;

        public Video MainRecord
            {
            get { return _mainRecord; }
            set { _mainRecord = value; }
            }
        public Video EditedRecord
        {
            get { return _editedRecord; }
            set { _editedRecord = value; }
        }


    }
}
