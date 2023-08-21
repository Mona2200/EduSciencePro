using AutoMapper;
using EduSciencePro.Data.Services;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos;

/// <summary>
/// Репозиторий для работы с сотрудничествами.
/// </summary>
public class CooperationRepository : ICooperationRepository
{
    /// <summary>
    /// Контекст базы данных.
    /// </summary>
    private readonly ApplicationDbContext _db;

    /// <summary>
    /// Маппер для преобразования типов.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    /// Репозиторий для работы с организациями.
    /// </summary>
    private readonly IOrganizationRepository _organizations;

    /// <summary>
    /// Сервис для преобразование формата времени.
    /// </summary>
    private readonly DateTimeService _timeService;

    public CooperationRepository(ApplicationDbContext db, IMapper mapper, IOrganizationRepository organizations, DateTimeService timeService)
    {
        _db = db;
        _mapper = mapper;
        _organizations = organizations;
        _timeService = timeService;
    }

    /// <inheritdoc/>
    public async Task<List<Cooperation>> GetCooperations()
    {
        return await _db.Cooperations.OrderByDescending(p => p.EndDate).Where(p => p.EndDate > DateTime.Now).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<CooperationViewModel>> GetCooperationViewModels(string[]? skillNames = null, int take = 5, int skip = 0)
    {
        List<Cooperation> cooperations = new();
        if (skillNames != null && skillNames.Length != 0)
        {
            foreach (string skillName in skillNames)
            {
                Skill? skill = await _db.Skills.FirstOrDefaultAsync(t => t.Name == skillName);
                if (skill is null)
                    continue;

                List<SkillCooperation> skillCooperations = await _db.SkillCooperations.Where(t => t.SkillId == skill.Id).ToListAsync();
                foreach (SkillCooperation skillCooperation in skillCooperations)
                {
                    Cooperation? cooperation = await _db.Cooperations.FirstOrDefaultAsync(p => p.Id == skillCooperation.CooperationId);
                    if (cooperation != null && cooperations.SingleOrDefault(n => n.Id == cooperation.Id) == null)
                        cooperations.Add(cooperation);
                }
            }
        }
        else
            cooperations = await _db.Cooperations.ToListAsync();
        cooperations = cooperations.OrderByDescending(n => n.EndDate).Where(p => p.EndDate > DateTime.Now).Take(take).Skip(skip).ToList();

        List<CooperationViewModel> cooperationViewModels = new();
        foreach (Cooperation cooperation in cooperations)
        {
            CooperationViewModel cooperationViewModel = _mapper.Map<Cooperation, CooperationViewModel>(cooperation);
            cooperationViewModel.EndDate = _timeService.GetDate(cooperation.EndDate);

            List<SkillCooperation> skillCooperations = await _db.SkillCooperations.Where(t => t.CooperationId == cooperation.Id).ToListAsync();
            List<Skill> skills = new();
            foreach (SkillCooperation skillCooperation in skillCooperations)
            {
                Skill? skill = await _db.Skills.SingleOrDefaultAsync(t => t.Id == skillCooperation.SkillId);
                if (skill != null)
                    skills.Add(skill);
            }
            cooperationViewModel.Skills = skills.Take(4).ToList();
            cooperationViewModel.Organization = await _db.Organizations.SingleOrDefaultAsync(o => o.Id == cooperation.OrganizationId);

            cooperationViewModels.Add(cooperationViewModel);
        }
        return cooperationViewModels;
    }

    /// <inheritdoc/>
    public async Task<List<Cooperation>> GetCooperationsByOrganizationId(Guid organizationid)
    {
        return await _db.Cooperations.OrderByDescending(p => p.EndDate).Where(t => t.OrganizationId == organizationid).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<CooperationViewModel>> GetCooperationViewModelsByOrganizationId(Guid organizationid, int take = 5, int skip = 0)
    {
        List<Cooperation> cooperations = await GetCooperationsByOrganizationId(organizationid);
        cooperations = cooperations.Take(take).Skip(skip).ToList();

        List<CooperationViewModel> cooperationViewModels = new List<CooperationViewModel>();
        foreach (Cooperation cooperation in cooperations)
        {
            CooperationViewModel cooperationViewModel = _mapper.Map<Cooperation, CooperationViewModel>(cooperation);
            cooperationViewModel.EndDate = _timeService.GetDate(cooperation.EndDate);

            List<SkillCooperation> skillCooperations = await _db.SkillCooperations.Where(t => t.CooperationId == cooperation.Id).ToListAsync();
            List<Skill> skills = new();
            foreach (SkillCooperation skillCooperation in skillCooperations)
            {
                Skill? skill = await _db.Skills.SingleOrDefaultAsync(t => t.Id == skillCooperation.SkillId);
                if (skill != null)
                    skills.Add(skill);
            }
            cooperationViewModel.Skills = skills.Take(4).ToList();
            cooperationViewModel.Organization = await _db.Organizations.SingleOrDefaultAsync(o => o.Id == cooperation.OrganizationId);

            cooperationViewModels.Add(cooperationViewModel);
        }
        return cooperationViewModels;
    }

    /// <inheritdoc/>
    public async Task<Cooperation?> GetCooperationById(Guid id)
    {
        return await _db.Cooperations.SingleOrDefaultAsync(c => c.Id == id);
    }

    /// <inheritdoc/>
    public async Task<CooperationViewModel?> GetCooperationViewModelById(Guid id)
    {
        Cooperation? cooperation = await GetCooperationById(id);
        if (cooperation is null)
            return null;

        CooperationViewModel cooperationViewModel = _mapper.Map<Cooperation, CooperationViewModel>(cooperation);
        cooperationViewModel.EndDate = _timeService.GetDate(cooperation.EndDate);

        List<SkillCooperation> skillCooperations = await _db.SkillCooperations.Where(t => t.CooperationId == cooperation.Id).ToListAsync();
        List<Skill> skills = new();
        foreach (SkillCooperation skillCooperation in skillCooperations)
        {
            Skill? skill = await _db.Skills.SingleOrDefaultAsync(t => t.Id == skillCooperation.SkillId);
            if (skill != null)
                skills.Add(skill);
        }
        cooperationViewModel.Skills = skills.Take(4).ToList();
        cooperationViewModel.Organization = await _db.Organizations.SingleOrDefaultAsync(o => o.Id == cooperation.OrganizationId);

        return cooperationViewModel;
    }

    /// <inheritdoc/>
    public async Task Save(AddCooperationViewModel model)
    {
        Cooperation cooperation = _mapper.Map<AddCooperationViewModel, Cooperation>(model);
        Organization? organization = await _organizations.GetOrganizationByName(model.OrganizationName);
        if (organization is null)
            return;

        cooperation.OrganizationId = organization.Id;
        if (model.Skills != null)
        {
            string[] skillNames = model.Skills.Split('/', StringSplitOptions.RemoveEmptyEntries);
            List<Skill> skills = new List<Skill>();
            foreach (string skillName in skillNames)
            {
                Skill? skill = await _db.Skills.FirstOrDefaultAsync(t => t.Name == skillName);
                if (skill != null)
                {
                    SkillCooperation skillCooperation = new SkillCooperation() { CooperationId = cooperation.Id, SkillId = skill.Id };
                    await _db.SkillCooperations.AddAsync(skillCooperation);
                }
                else
                {
                    Skill newSkill = new Skill() { Name = skillName };
                    await _db.Skills.AddAsync(newSkill);

                    SkillCooperation skillCooperation = new SkillCooperation() { CooperationId = cooperation.Id, SkillId = newSkill.Id };
                    await _db.SkillCooperations.AddAsync(skillCooperation);
                }
            }
        }
        await _db.Cooperations.AddAsync(cooperation);
        await _db.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task Delete(Guid id)
    {
        Cooperation? cooperation = await GetCooperationById(id);
        if (cooperation is null)
            return;

        List<SkillCooperation> skillCooperations = await _db.SkillCooperations.Where(t => t.CooperationId == cooperation.Id).ToListAsync();
        foreach (SkillCooperation skillCooperation in skillCooperations)
            _db.SkillCooperations.Remove(skillCooperation);

        _db.Cooperations.Remove(cooperation);
        await _db.SaveChangesAsync();
    }
}

/// <summary>
/// Интерфейс для работы с сотрудничествами.
/// </summary>
public interface ICooperationRepository
{
/// <summary>
/// Возвращает все сотрудничества, сортированные по убыванию даты.
/// </summary>
    Task<List<Cooperation>> GetCooperations();

    /// <summary>
    /// Возвращает все сотрудничества, сортированные по убыванию даты.
    /// </summary>
    /// <param name="skillNames">Навыки</param>
    /// <param name="take">Количество возвращаемых сотрудничеств</param>
    /// <param name="skip">Количество пропускаемых сотрудничеств</param>
    /// <returns></returns>
    Task<List<CooperationViewModel>> GetCooperationViewModels(string[]? skillNames = null, int take = 5, int skip = 0);

    /// <summary>
    /// Возвращает все сотрудничества организации, сортированные по убыванию даты.
    /// </summary>
    /// <param name="organizationid">Идентификатор организации</param>
    Task<List<Cooperation>> GetCooperationsByOrganizationId(Guid organizationid);

    /// <summary>
    /// Возвращает все сотрудничества организации, сортированные по убыванию даты.
    /// </summary>
    /// <param name="organizationid">Идентификатор организации</param>
    /// <param name="take">Количество возвращаемых сотрудничеств</param>
    /// <param name="skip">Количество пропускаемых сотрудничеств</param>
    Task<List<CooperationViewModel>> GetCooperationViewModelsByOrganizationId(Guid organizationid, int take = 5, int skip = 0);

    /// <summary>
    /// Возвращает сотрудничество по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сотрудничества</param>
    Task<Cooperation?> GetCooperationById(Guid id);

    /// <summary>
    /// Возвращает сотрудничество по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор сотрудничества</param>
    Task<CooperationViewModel?> GetCooperationViewModelById(Guid id);

    /// <summary>
    /// Сохраняет сотрудничество.
    /// </summary>
    /// <param name="model">Сотрудничество</param>
    Task Save(AddCooperationViewModel model);

    /// <summary>
    /// Удаляет сотрудничество.
    /// </summary>
    /// <param name="id">Идентификатор сотрудничества</param>
    Task Delete(Guid id);
}
