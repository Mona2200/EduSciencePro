using EduSciencePro.Models.User;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using TypeModel = EduSciencePro.Models.User.TypeModel;

namespace EduSciencePro.Data.Repos;

/// <summary>
/// Репозиторий для работы с типами пользователей.
/// </summary>
public class TypeRepository : ITypeRepository
{
    /// <summary>
    /// Контекст базы данных.
    /// </summary>
    private readonly ApplicationDbContext _db;

    public TypeRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task<List<TypeModel>> GetTypes()
    {
        return await _db.TypeModels.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<TypeModel?> GetTypeById(Guid id)
    {
        return await _db.TypeModels.SingleOrDefaultAsync(t => t.Id == id);
    }

    /// <inheritdoc/>
    public async Task<List<TypeModel>> GetTypesByUserId(Guid id)
    {
        List<TypeUser> typeUsers = await _db.TypeUsers.Where(t => t.UserId == id).ToListAsync();
        List<TypeModel> types = new();
        foreach (TypeUser typeUser in typeUsers)
        {
            TypeModel? type = await GetTypeById(typeUser.TypeId);
            if (type != null)
                types.Add(type);
        }
        return types;
    }

    /// <inheritdoc/>
    public async Task<TypeModel?> GetTypeByName(string name)
    {
        return await _db.TypeModels.FirstOrDefaultAsync(t => t.Name == name);
    }

    /// <inheritdoc/>
    public async Task Save(TypeModel type)
    {
        var entry = _db.Entry(type);
        if (entry.State == EntityState.Detached)
            await _db.TypeModels.AddAsync(type);
        await _db.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task Update(TypeModel updateType, TypeModel newType)
    {
        updateType.Name = newType.Name;

        var entry = _db.Entry(updateType);
        if (entry.State == EntityState.Detached)
            _db.TypeModels.Update(updateType);
        await _db.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task Delete(Guid id)
    {
        TypeModel? type = await GetTypeById(id);
        if (type is null)
            return;

        _db.TypeModels.Remove(type);
        List<TypeUser> typeUsers = await _db.TypeUsers.Where(t => t.TypeId == type.Id).ToListAsync();
        foreach (var typeUser in typeUsers)
            _db.TypeUsers.Remove(typeUser);

        await _db.SaveChangesAsync();
    }
}

/// <summary>
/// Интерфейс для работы с типами пользователей.
/// </summary>
public interface ITypeRepository
{
    /// <summary>
    /// Возвращает все типы пользователей.
    /// </summary>
    Task<List<TypeModel>> GetTypes();

    /// <summary>
    /// Возвращает типы пользователя по идентификатору пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    Task<List<TypeModel>> GetTypesByUserId(Guid id);

    /// <summary>
    /// Возвращает тип пользователя по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор типа пользователя</param>
    Task<TypeModel?> GetTypeById(Guid id);

    /// <summary>
    /// Возвращает тип пользователя по названию.
    /// </summary>
    /// <param name="name">Название типа пользователя</param>
    Task<TypeModel?> GetTypeByName(string name);

    /// <summary>
    /// Добавляет пользователю тип пользователя.
    /// </summary>
    /// <param name="type">Тип пользователя</param>
    Task Save(TypeModel type);

    /// <summary>
    /// Обновляет тип пользователя.
    /// </summary>
    /// <param name="updateType">Текущий тип пользователя</param>
    /// <param name="newType">Новый тип пользователя</param>
    Task Update(TypeModel updateType, TypeModel newType);

    /// <summary>
    /// Удаляет тип пользователя.
    /// </summary>
    /// <param name="id">Идентификатор типа пользователя</param>
    Task Delete(Guid id);
}
