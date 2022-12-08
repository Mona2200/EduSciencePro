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

         CreateMap<User, UserViewModel>().ForMember(m => m.FullName, opt => opt.MapFrom(u => $"{u.LastName} {u.FirstName} {u.MiddleName}"));
      }
   }
}
