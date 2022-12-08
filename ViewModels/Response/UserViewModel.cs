﻿using EduSciencePro.Models.User;

namespace EduSciencePro.ViewModels.Response
{
   public class UserViewModel
   {
      public Guid Id { get; set; }
      public string FullName { get; set; }
      public string Gender { get; set; }
      public string Birthday { get; set; }
      public TypeModel[] TypeUsers { get; set; }
      public string Email { get; set; }
      public byte[] Image { get; set; }
      public Link[]? Links { get; set; } = null;
      public Resume? Resume { get; set; } = null;
      public Role Role { get; set; }
   }
}