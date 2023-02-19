namespace EduSciencePro.ViewModels.Response
{
    public class ConferencesAndIsOrgViewModel
    {
        public bool IsOrg { get; set; }
        public ConferenceViewModel[] Conferences { get; set; }
        public List<string> Tags { get; set; }
    }
}
