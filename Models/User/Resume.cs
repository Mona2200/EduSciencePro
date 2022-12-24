using System.ComponentModel.DataAnnotations.Schema;

namespace EduSciencePro.Models.User
{
   public class Resume
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? EducationId { get; set; }
        public string? DateGraduationEducation { get; set; }
        public string? Specialization { get; set; }
        public Guid? PlaceWorkId { get; set; }
        public Guid? OrganizationId { get; set; }
        //public QualificationImprovement QualificationImprovement { get; set; }
        //public DateOnly DateGraduationImprovement { get; set; }
        public string? AboutYourself { get; set; }
   }
}
