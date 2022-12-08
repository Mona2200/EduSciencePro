using TypeModel = EduSciencePro.Models.User.TypeModel;

namespace EduSciencePro.Data.Repos
{
    public interface ITypeRepository
    {
      public Task<TypeModel[]> GetTypes();
      public Task<TypeModel> GetTypeById(Guid id);
      public Task<TypeModel[]> GetTypesByUserId(Guid id);
      public Task Save(TypeModel type);
      public Task Update(TypeModel updateType, TypeModel newType);
      public Task Delete(TypeModel type);
    }
}
