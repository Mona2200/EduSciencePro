using EduSciencePro.Models;

namespace EduSciencePro.ViewModels.Response
{
    public class MaterialsAndTagsViewModel
    {
    public List<string> Tags { get; set; }
    public MaterialViewModel[] Materials{ get; set; }
    }
}
