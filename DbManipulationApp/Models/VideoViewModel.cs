namespace DbManipulationApp.Models
{
    public class VideoViewModel
    {
        Video _current_vid = new();
        List<Video> _videos_list = new();
        List<string> _czytania_list = new();

        public Video Current_video
        {
            get { return _current_vid; }
            set { _current_vid = value; }
        }
        public List<Video> Videos_list
        {
            get { return _videos_list; }
            set { _videos_list=value; }
        }
        public List<string> Czytania_list
        {
            get { return _czytania_list;}
            set { _czytania_list=value;}
        }
    }
}
