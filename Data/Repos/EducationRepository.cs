using AutoMapper;
using EduSciencePro.Models.User;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos;

/// <summary>
/// Репозиторий для работы с образованием.
/// </summary>
public class EducationRepository : IEducationRepository
{
    /// <summary>
    /// Контекст базы данных.
    /// </summary>
    private readonly ApplicationDbContext _db;

    /// <summary>
    /// Маппер для преобразования типов.
    /// </summary>
    private readonly IMapper _mapper;

    public EducationRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<List<Education>> GetEducations()
    {
        return await _db.Educations.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Education?> GetEducationById(Guid id)
    {
        return await _db.Educations.SingleOrDefaultAsync(e => e.Id == id);
    }

    /// <inheritdoc/>
    public async Task<Education?> GetEducationByName(string name)
    {
        return await _db.Educations.FirstOrDefaultAsync(e => e.Name == name);
    }

    /// <inheritdoc/>
    public async Task<List<Education>> GetEducationsSearch(string search)
    {
        return await _db.Educations.Where(e => e.Name.ToLower().Contains(search.ToLower())).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task Save(Education education)
    {
        await _db.Educations.AddAsync(education);
        await _db.SaveChangesAsync();
    }
}

/// <summary>
/// Интерфейс для работы с образовательными учреждениями.
/// </summary>
public interface IEducationRepository
{
/// <summary>
/// Возвращает все образовательные учреждения.
/// </summary>
    Task<List<Education>> GetEducations();

    /// <summary>
    /// Возвращает образовательное учреждение по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор образовательного учреждения</param>
    Task<Education?> GetEducationById(Guid id);

    /// <summary>
    /// Возвращает образовательно учреждение по названию.
    /// </summary>
    /// <param name="name">Название образовательного учреждения</param>
    Task<Education?> GetEducationByName(string name);

    /// <summary>
    /// Возвращает образовательно учреждение по поиску названия.
    /// </summary>
    /// <param name="search">Строка поиска</param>
    Task<List<Education>> GetEducationsSearch(string search);

    /// <summary>
    /// Сохраняет образовательное учреждение.
    /// </summary>
    /// <param name="education">Образовательное учреждение</param>
    Task Save(Education education);
    //Task Update(Education education, string newName);
    //Task Delete(int id);
}
