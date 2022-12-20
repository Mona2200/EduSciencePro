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
         CreateMap<AddResumeViewModel, Resume>();

         CreateMap<User, UserViewModel>().ForMember(m => m.FullName, opt => opt.MapFrom(u => $"{u.LastName} {u.FirstName} {u.MiddleName}"));
         //CreateMap<UserViewModel, AddUserViewModel>().ForMember(m => m.FirstName, opt => opt.MapFrom(u => u.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray()[0]))
         //                                            .ForMember(m => m.LastName, opt => opt.MapFrom(u => u.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray()[1]))
         //                                            .ForMember(m => m.LastName, opt => opt.MapFrom(u => u.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray()[2]))
         //                                            .ForMember(m => m.TypeUsers, opt => opt.MapFrom(u => u.TypeUsers.))
         CreateMap<Resume, ResumeViewModel>();
      }
   }
}
