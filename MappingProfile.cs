using AutoMapper;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using System.Text;

namespace EduSciencePro
{
   public class MappingProfile : Profile
   {
      public MappingProfile()
      {
         CreateMap<UserViewModel, User>();
         CreateMap<AddUserViewModel, User>();
         CreateMap<AddUserViewModel, UserViewModel>().ForMember(m => m.TypeUsers, opt => opt.Ignore()).ForMember(m => m.Links, opt => opt.Ignore());
         CreateMap<AddResumeViewModel, Resume>();
         CreateMap<AddResumeViewModel, ResumeViewModel>()
                                                         .ForMember(m => m.Education, opt => opt.Ignore())
                                                         .ForMember(m => m.Skills, opt => opt.Ignore())
                                                         .ForMember(m => m.Organization, opt => opt.Ignore())
                                                         .ForMember(m => m.PlaceWork, opt => opt.Ignore());
         //CreateMap<AddPostViewModel, NewsPost>().ForMember(m => m.Content, opt => opt.MapFrom(n => Encoding.UTF8.GetBytes(n.Content))).ForMember(m => m.Cover, opt => opt.Ignore());
         //CreateMap<AddPostViewModel, DiscussionPost>().ForMember(m => m.Content, opt => opt.MapFrom(n => Encoding.UTF8.GetBytes(n.Content)));
         CreateMap<AddPostViewModel, Post>().ForMember(m => m.Content, opt => opt.MapFrom(n => Encoding.UTF8.GetBytes(n.Content))).ForMember(m => m.Cover, opt => opt.Ignore());
         CreateMap<EditPostViewModel, Post>().ForMember(m => m.Content, opt => opt.MapFrom(n => Encoding.UTF8.GetBytes(n.Content))).ForMember(m => m.Cover, opt => opt.Ignore());
         CreateMap<AddCommentViewModel, Comment>();
         CreateMap<AddProjectViewModel, Project>()
                                                  .ForMember(m => m.StartDate, opt => opt.MapFrom(p => DateTime.Parse(p.StartDate)))
                                                  .ForMember(m => m.EndDate, opt => opt.MapFrom(p => DateTime.Parse(p.EndDate)));

         CreateMap<User, UserViewModel>().ForMember(m => m.FullName, opt => opt.MapFrom(u => $"{u.LastName} {u.FirstName} {u.MiddleName}"));
         //CreateMap<UserViewModel, AddUserViewModel>().ForMember(m => m.FirstName, opt => opt.MapFrom(u => u.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray()[0]))
         //                                            .ForMember(m => m.LastName, opt => opt.MapFrom(u => u.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray()[1]))
         //                                            .ForMember(m => m.LastName, opt => opt.MapFrom(u => u.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray()[2]))
         //                                            .ForMember(m => m.TypeUsers, opt => opt.MapFrom(u => u.TypeUsers.))
         CreateMap<Resume, ResumeViewModel>();
         CreateMap<Post, PostViewModel>().ForMember(m => m.Content, opt => opt.MapFrom(p => Encoding.UTF8.GetString(p.Content))).ForMember(m => m.CreatedDate, opt => opt.Ignore());
         CreateMap<Comment, CommentViewModel>().ForMember(m => m.CreatedDate, opt => opt.Ignore());
         CreateMap<Project, ProjectViewModel>().ForMember(m => m.StartDate, opt => opt.Ignore()).ForMember(m => m.EndDate, opt => opt.Ignore());
      }
   }
}
