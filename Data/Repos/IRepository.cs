using EduSciencePro.Models;

namespace EduSciencePro.Data.Repos;

public interface IRepository<T> where T : IModel
{
    Task<List<T>> Get();
    Task<T?> GetById(Guid id);
    Task Save(T entity);
    Task Delete(Guid id);
}
