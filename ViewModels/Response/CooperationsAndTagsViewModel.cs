namespace EduSciencePro.ViewModels.Response
{
    public class CooperationsAndTagsViewModel
    {
    public bool IsOrg { get; set; }
    public CooperationViewModel[] Cooperations { get; set; }
    public List<string> Tags { get; set; }
    }
}
