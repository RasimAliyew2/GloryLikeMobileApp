namespace MetanetA_MobileApp.Model
{
    public class VideoItem
    {
        public string Title { get; set; }     // məsələn: "Nümunə video 1"
        public string Description { get; set; } // məsələn: "YouTube embed test."
        public string Url { get; set; }       // YouTube link (watch/shorts/youtu.be ola bilər)
        public string Thumbnail { get; set; } // şəkil istəyirsənsə (optional)
    }
}
