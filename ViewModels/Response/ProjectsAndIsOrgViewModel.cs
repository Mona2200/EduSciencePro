namespace EduSciencePro.ViewModels.Response
{
    public class ProjectsAndIsOrgViewModel
    {
    public bool IsOrg { get; set; }
    public ProjectViewModel[] Projects { get; set; }
    public List<string> Tags { get; set; }
    }
}
