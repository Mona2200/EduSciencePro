using AutoMapper;
using EduSciencePro.Data.Services;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos;

/// <summary>
/// Репозиторий для работы с конференциями.
/// </summary>
public class ConferenceRepository : IConferenceRepository
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
    /// Репозиторий для работы с тегами.
    /// </summary>
    private readonly ITagRepository _tags;

    /// <summary>
    /// Сервис для преобразование формата времени.
    /// </summary>
    private readonly DateTimeService _timeService;

    public ConferenceRepository(ApplicationDbContext db, IMapper mapper, IOrganizationRepository organizations, ITagRepository tags, DateTimeService timeService)
    {
        _db = db;
        _mapper = mapper;
        _organizations = organizations;
        _tags = tags;
        _timeService = timeService;
    }

    /// <inheritdoc/>
    public async Task<List<Conference>> GetConferences()
    {
        return await _db.Conferences.OrderByDescending(p => p.EventDate).Where(p => p.EventDate > DateTime.Now).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<ConferenceViewModel>> GetConferenceViewModels(string[]? tagNames = null, int take = 5, int skip = 0)
    {
        List<Conference> conferences = new();
        if (tagNames != null && tagNames.Length != 0)
        {
            foreach (string tagName in tagNames)
            {
                Tag? tag = await _tags.GetTagByName(tagName);
                if (tag is null)
                    continue;

                List<TagConference> tagConferences = await _db.TagConferences.Where(t => t.TagId == tag.Id).ToListAsync();
                foreach (TagConference tagConference in tagConferences)
                {
                    Conference? conference = await GetConferenceById(tagConference.ConferenceId);
                    if (conference != null && conferences.SingleOrDefault(n => n.Id == conference.Id) == null)
                        conferences.Add(conference);
                }
            }
        }
        else
            conferences = await _db.Conferences.ToListAsync();
        conferences = conferences.OrderByDescending(n => n.EventDate).Where(p => p.EventDate > DateTime.Now).Take(take).Skip(skip).ToList();

        List<ConferenceViewModel> conferenceViewModels = new();
        foreach (Conference conference in conferences)
        {
            ConferenceViewModel conferenceViewModel = _mapper.Map<Conference, ConferenceViewModel>(conference);
            conferenceViewModel.EventDate = _timeService.GetDate(conference.EventDate);

            List<TagConference> tagConferences = await _db.TagConferences.Where(t => t.ConferenceId == conference.Id).ToListAsync();
            List<Tag> tags = new List<Tag>();
            foreach (var tagConference in tagConferences)
            {
                Tag? tag = await _tags.GetTagById(tagConference.TagId);
                if (tag != null)
                    tags.Add(tag);
            }
            conferenceViewModel.Tags = tags.Take(4).ToList();
            conferenceViewModel.Organization = await _organizations.GetOrganizationById(conference.OrganizationId);

            conferenceViewModels.Add(conferenceViewModel);
        }
        return conferenceViewModels;
    }

    /// <inheritdoc/>
    public async Task<List<Conference>> GetConferencesByOrganizationId(Guid organizationid)
    {
        return await _db.Conferences.OrderByDescending(p => p.EventDate).Where(t => t.OrganizationId == organizationid).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<ConferenceViewModel>> GetConferenceViewModelsByOrganizationId(Guid organizationid, int take = 5, int skip = 0)
    {
        List<Conference> conferences = await GetConferencesByOrganizationId(organizationid);
        conferences = conferences.Take(take).Skip(skip).ToList();

        List<ConferenceViewModel> conferenceViewModels = new();
        foreach (Conference conference in conferences)
        {
            ConferenceViewModel conferenceViewModel = _mapper.Map<Conference, ConferenceViewModel>(conference);
            conferenceViewModel.EventDate = _timeService.GetDate(conference.EventDate);

            List<TagConference> tagConferences = await _db.TagConferences.Where(t => t.ConferenceId == conference.Id).ToListAsync();
            List<Tag> tags = new List<Tag>();
            foreach (var tagConference in tagConferences)
            {
                Tag? tag = await _tags.GetTagById(tagConference.TagId);
                if (tag != null)
                    tags.Add(tag);
            }
            conferenceViewModel.Tags = tags.Take(4).ToList();
            conferenceViewModel.Organization = await _organizations.GetOrganizationById(conference.OrganizationId);

            conferenceViewModels.Add(conferenceViewModel);
        }
        return conferenceViewModels;
    }

    /// <inheritdoc/>
    public async Task<Conference?> GetConferenceById(Guid id)
    {
        return await _db.Conferences.SingleOrDefaultAsync(c => c.Id == id);
    }

    /// <inheritdoc/>
    public async Task<ConferenceViewModel?> GetConferenceViewModelById(Guid id)
    {
        Conference? conference = await GetConferenceById(id);
        if (conference is null)
            return null;

        ConferenceViewModel conferenceViewModel = _mapper.Map<Conference, ConferenceViewModel>(conference);
        conferenceViewModel.EventDate = _timeService.GetDate(conference.EventDate);

        List<TagConference> tagConferences = await _db.TagConferences.Where(t => t.ConferenceId == conference.Id).ToListAsync();
        List<Tag> tags = new List<Tag>();
        foreach (var tagConference in tagConferences)
        {
            Tag? tag = await _tags.GetTagById(tagConference.TagId);
            if (tag != null)
                tags.Add(tag);
        }
        conferenceViewModel.Tags = tags.Take(4).ToList();
        conferenceViewModel.Organization = await _organizations.GetOrganizationById(conference.OrganizationId);

        return conferenceViewModel;
    }

    /// <inheritdoc/>
    public async Task Save(AddConferenceViewModel model)
    {
        Conference conference = _mapper.Map<AddConferenceViewModel, Conference>(model);
        Organization? organization = await _organizations.GetOrganizationByName(model.OrganizationName);
        if (organization is null)
            return;

        conference.OrganizationId = organization.Id;
        if (model.Tags != null)
        {
            string[] tagNames = model.Tags.Split('/', StringSplitOptions.RemoveEmptyEntries);
            List<Tag> tags = new();
            foreach (string tagName in tagNames)
            {
                Tag? tag = await _tags.GetTagByName(tagName);
                if (tag != null)
                {
                    TagConference tagConference = new TagConference() { ConferenceId = conference.Id, TagId = tag.Id };
                    await _db.TagConferences.AddAsync(tagConference);
                }
                else
                {
                    Tag newTag = new Tag() { Name = tagName };
                    await _db.Tags.AddAsync(newTag);

                    TagConference tagConference = new TagConference() { ConferenceId = conference.Id, TagId = newTag.Id };
                    await _db.TagConferences.AddAsync(tagConference);
                }
            }
        }
        await _db.Conferences.AddAsync(conference);
        await _db.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task Delete(Guid id)
    {
        Conference? conference = await GetConferenceById(id);
        if (conference is null)
            return;

        List<TagConference> tagConferences = await _db.TagConferences.Where(t => t.ConferenceId == conference.Id).ToListAsync();
        foreach (TagConference tagConference in tagConferences)
            _db.TagConferences.Remove(tagConference);

        _db.Conferences.Remove(conference);
        await _db.SaveChangesAsync();
    }
}

/// <summary>
/// Интерфейс для работы с конференциями.
/// </summary>
public interface IConferenceRepository
{
    /// <summary>
    /// Возвращает все предстоящие конференции, сортированные по убыванию даты.
    /// </summary>
    Task<List<Conference>> GetConferences();

    /// <summary>
    /// Возвращает все предстоящие конференции, сортированные по убыванию даты.
    /// </summary>
    /// <param name="tagNames">Теги</param>
    /// <param name="take">Количество извлекаемых конференций</param>
    /// <param name="skip">Количество пропускаемых конференций</param>
    Task<List<ConferenceViewModel>> GetConferenceViewModels(string[]? tagNames = null, int take = 5, int skip = 0);

    /// <summary>
    /// Возвращает все предстоящие конференции организации, сортированные по убыванию даты.
    /// </summary>
    /// <param name="organizationid">Идентификатор организации</param>
    Task<List<Conference>> GetConferencesByOrganizationId(Guid organizationid);

    /// <summary>
    /// Возвращает все предстоящие конференции организации, сортированные по убыванию даты.
    /// </summary>
    /// <param name="organizationid">Идентификатор организации</param>
    /// <param name="take">Количество извлекаемых конференций</param>
    /// <param name="skip">Количество пропускаемых конференций</param>
    Task<List<ConferenceViewModel>> GetConferenceViewModelsByOrganizationId(Guid organizationid, int take = 5, int skip = 0);

    /// <summary>
    /// Возвращает конференцию по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор конференции</param>
    Task<Conference?> GetConferenceById(Guid id);

    /// <summary>
    /// Возвращает конференцию по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор конференции</param>
    Task<ConferenceViewModel?> GetConferenceViewModelById(Guid id);

    /// <summary>
    /// Сохраняет конференцию.
    /// </summary>
    /// <param name="model">Конференция</param>
    Task Save(AddConferenceViewModel model);

    /// <summary>
    /// Удаляет конференцию.
    /// </summary>
    /// <param name="id">Идентификатор конференции</param>
    Task Delete(Guid id);
}
