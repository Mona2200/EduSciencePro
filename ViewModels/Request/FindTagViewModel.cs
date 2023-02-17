namespace EduSciencePro.ViewModels.Request
{
    public class FindTagViewModel
    {
        public string tags { get; set; } = "";
        public int take { get; set; }
        public int skip { get; set; }
    }
}
