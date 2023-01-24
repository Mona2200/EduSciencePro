namespace EduSciencePro.Models
{
    public class TagMaterial
    {
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MaterialId { get; set; }
    public Guid TagId { get; set; }
    }
}
