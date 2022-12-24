using AutoMapper;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;

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

         CreateMap<User, UserViewModel>().ForMember(m => m.FullName, opt => opt.MapFrom(u => $"{u.LastName} {u.FirstName} {u.MiddleName}"));
         //CreateMap<UserViewModel, AddUserViewModel>().ForMember(m => m.FirstName, opt => opt.MapFrom(u => u.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray()[0]))
         //                                            .ForMember(m => m.LastName, opt => opt.MapFrom(u => u.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray()[1]))
         //                                            .ForMember(m => m.LastName, opt => opt.MapFrom(u => u.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray()[2]))
         //                                            .ForMember(m => m.TypeUsers, opt => opt.MapFrom(u => u.TypeUsers.))
         CreateMap<Resume, ResumeViewModel>();
      }
   }
}
