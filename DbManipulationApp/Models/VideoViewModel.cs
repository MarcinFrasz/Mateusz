namespace DbManipulationApp.Models
{
    public class VideoViewModel
    {
        private DateTime _date= new();
        private IEnumerable<Video> _videos=Enumerable.Empty<Video>();
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }
        public IEnumerable<Video> Videos
        {
            get { return _videos; }
            set { _videos = value; }
        }
    }
}
