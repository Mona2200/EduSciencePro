﻿using EduSciencePro.Models.User;

namespace EduSciencePro.ViewModels.Response
{
    public class CourseViewModel
    {
      public Guid Id { get; set; }
      public Education? Education { get; set; }
      public PlaceWork? PlaceWork { get; set; }
      public string? Specialization { get; set; }
      public List<string>? CompletedCourses { get; set; }
      public int NeedSkills { get; set; }
      public List<Skill> Skills { get; set; }
      public User User { get; set; }
   }
}
