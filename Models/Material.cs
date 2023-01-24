namespace EduSciencePro.Models
{
    public class Material
    {
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public string Specialization { get; set; }
    public string Type { get; set; }
    public string Url { get; set; }
    public string Annotation { get; set; }
    public string Publication { get; set; }
    }
}
