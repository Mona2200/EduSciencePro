using EduSciencePro.Models;
using EduSciencePro.Models.User;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net;

namespace EduSciencePro.Data
{
   public class ApplicationDbContext : DbContext
   {
      public DbSet<User> Users { get; set; }
      public DbSet<Role> Roles { get; set; }
      public DbSet<Education> Educations { get; set; }
      public DbSet<Link> Links { get; set; }
      public DbSet<Organization> Organizations { get; set; }
      public DbSet<PlaceWork> PlaceWorks { get; set; }
      public DbSet<Resume> Resumes { get; set; }
      public DbSet<Skill> Skills { get; set; }
      public DbSet<TypeModel> TypeModels { get; set; }
      public DbSet<TypeUser> TypeUsers { get; set; }
      public DbSet<ResumeSkill> ResumeSkills { get; set; }
      public DbSet<UserOrganization> UserOrganizations { get; set; }

      public DbSet<Post> Posts { get; set; }

      public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
      {
         Database.EnsureCreated();
         Database.Migrate();
      }

      protected override void OnModelCreating(ModelBuilder builder)
      {
         builder.Entity<User>().ToTable("Users");
         builder.Entity<Role>().ToTable("Roles");
         builder.Entity<Education>().ToTable("Educations");
         builder.Entity<Link>().ToTable("Links");
         builder.Entity<Organization>().ToTable("Organizations");
         builder.Entity<PlaceWork>().ToTable("PlaceWorks");
         builder.Entity<Resume>().ToTable("Resumes");
         builder.Entity<Skill>().ToTable("Skills");
         builder.Entity<TypeModel>().ToTable("TypeModels");
         builder.Entity<TypeUser>().ToTable("TypeUsers");
         builder.Entity<ResumeSkill>().ToTable("ResumeSkills");
         builder.Entity<UserOrganization>().ToTable("UserOrganizations");

         builder.Entity<Post>().ToTable("Posts");
      }
   }
}
