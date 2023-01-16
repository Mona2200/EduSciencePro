using AutoMapper;
using EduSciencePro.Models;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos
{
   public class ConferenceRepository : IConferenceRepository
   {
      private readonly ApplicationDbContext _db;
      private readonly IMapper _mapper;
      private readonly IOrganizationRepository _organizations;

      public ConferenceRepository(ApplicationDbContext db, IMapper mapper, IOrganizationRepository organizations)
      {
         _db = db;
         _mapper = mapper;
         _organizations = organizations;
      }

      public async Task<Conference[]> GetConferences() => await _db.Conferences.ToArrayAsync();
      public async Task<ConferenceViewModel[]> GetConferenceViewModels()
      {
         var conferences = await GetConferences();
         List<ConferenceViewModel> conferenceViewModels = new List<ConferenceViewModel>();
         foreach (var conference in conferences)
         {
            ConferenceViewModel conferenceViewModel = _mapper.Map<Conference, ConferenceViewModel>(conference);

            string date = "";

            if (conference.EventDate.Day.ToString().Length == 1)
               date += "0" + conference.EventDate.Day.ToString() + ".";
            else
               date += conference.EventDate.Day.ToString() + ".";

            if (conference.EventDate.Month.ToString().Length == 1)
               date += "0" + conference.EventDate.Month.ToString() + ".";
            else
               date += conference.EventDate.Month.ToString() + ".";
            date += conference.EventDate.Year.ToString();

            conferenceViewModel.EventDate = date;

            var tagConferences = await _db.TagConferences.Where(t => t.ConferenceId == conference.Id).ToArrayAsync();
            List<Tag> tags = new List<Tag>();
            foreach (var tagConference in tagConferences)
            {
               var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == tagConference.TagId);
               tags.Add(tag);
            }
            conferenceViewModel.Tags = tags.ToArray();
            conferenceViewModel.Organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == conference.OrganizationId);

            conferenceViewModels.Add(conferenceViewModel);
         }
         return conferenceViewModels.ToArray();
      }

      public async Task<Conference[]> GetConferencesByOrganizationId(Guid organizationid) => await _db.Conferences.Where(t => t.OrganizationId == organizationid).ToArrayAsync();

      public async Task<ConferenceViewModel[]> GetConferenceViewModelsByOrganizationId(Guid organizationid)
      {
         var conferences = await GetConferencesByOrganizationId(organizationid);
         List<ConferenceViewModel> conferenceViewModels = new List<ConferenceViewModel>();
         foreach (var conference in conferences)
         {
            ConferenceViewModel conferenceViewModel = _mapper.Map<Conference, ConferenceViewModel>(conference);

            string date = "";

            if (conference.EventDate.Day.ToString().Length == 1)
               date += "0" + conference.EventDate.Day.ToString() + ".";
            else
               date += conference.EventDate.Day.ToString() + ".";

            if (conference.EventDate.Month.ToString().Length == 1)
               date += "0" + conference.EventDate.Month.ToString() + ".";
            else
               date += conference.EventDate.Month.ToString() + ".";
            date += conference.EventDate.Year.ToString();

            conferenceViewModel.EventDate = date;

            var tagConferences = await _db.TagConferences.Where(t => t.ConferenceId == conference.Id).ToArrayAsync();
            List<Tag> tags = new List<Tag>();
            foreach (var tagConference in tagConferences)
            {
               var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == tagConference.TagId);
               tags.Add(tag);
            }
            conferenceViewModel.Tags = tags.ToArray();
            conferenceViewModel.Organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == conference.OrganizationId);

            conferenceViewModels.Add(conferenceViewModel);
         }
         return conferenceViewModels.ToArray();
      }

      public async Task<Conference> GetConferenceById(Guid id) => await _db.Conferences.FirstOrDefaultAsync(c => c.Id == id);

      public async Task<ConferenceViewModel> GetConferenceViewModelById(Guid id)
      {
         var conference = await GetConferenceById(id);
         List<ConferenceViewModel> conferenceViewModels = new List<ConferenceViewModel>();

         ConferenceViewModel conferenceViewModel = _mapper.Map<Conference, ConferenceViewModel>(conference);

         string date = "";

         if (conference.EventDate.Day.ToString().Length == 1)
            date += "0" + conference.EventDate.Day.ToString() + ".";
         else
            date += conference.EventDate.Day.ToString() + ".";

         if (conference.EventDate.Month.ToString().Length == 1)
            date += "0" + conference.EventDate.Month.ToString() + ".";
         else
            date += conference.EventDate.Month.ToString() + ".";
         date += conference.EventDate.Year.ToString();

         conferenceViewModel.EventDate = date;

         var tagConferences = await _db.TagConferences.Where(t => t.ConferenceId == conference.Id).ToArrayAsync();
         List<Tag> tags = new List<Tag>();
         foreach (var tagConference in tagConferences)
         {
            var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == tagConference.TagId);
            tags.Add(tag);
         }
         conferenceViewModel.Tags = tags.ToArray();
         conferenceViewModel.Organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == conference.OrganizationId);

         return conferenceViewModel;
      }

      public async Task Save(AddConferenceViewModel model)
      {
         Conference conference = _mapper.Map<AddConferenceViewModel, Conference>(model);
         var organization = await _organizations.GetOrganizationByName(model.OrganizationName);
         if (organization != null)
         {
            conference.OrganizationId = organization.Id;
            string[] tagNames = model.Tags.Split('/', StringSplitOptions.RemoveEmptyEntries);
            List<Tag> tags = new List<Tag>();
            foreach (var tagName in tagNames)
            {
               Tag? tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
               if (tag != null)
               {
                  TagConference tagConference = new TagConference() {  ConferenceId = conference.Id, TagId = tag.Id};
                  var entry = _db.TagConferences.Entry(tagConference);
                  if (entry.State == EntityState.Detached)
                     await _db.TagConferences.AddAsync(tagConference);
               }
               else
               {
                  Tag newTag = new Tag() { Name = tagName };
                  var entry = _db.Tags.Entry(newTag);
                  if (entry.State == EntityState.Detached)
                     await _db.Tags.AddAsync(newTag);

                  TagConference tagConference = new TagConference() { ConferenceId = conference.Id, TagId = newTag.Id };
                  var newentry = _db.TagConferences.Entry(tagConference);
                  if (newentry.State == EntityState.Detached)
                     await _db.TagConferences.AddAsync(tagConference);
               }
            }
            var conferenceEntry = _db.Conferences.Entry(conference);
            if (conferenceEntry.State == EntityState.Detached)
               await _db.Conferences.AddAsync(conference);
            await _db.SaveChangesAsync();
         }
      }

      public async Task Delete(Guid id)
      {
         var conference = await GetConferenceById(id);
         if (conference != null)
         {
            var tagsConference = await _db.TagConferences.Where(t => t.ConferenceId == conference.Id).ToArrayAsync();
            foreach (var tagConference in tagsConference)
            {
               _db.TagConferences.Remove(tagConference);
            }
            _db.Conferences.Remove(conference);
            await _db.SaveChangesAsync();
         }
      }
   }

   public interface IConferenceRepository
   {
      Task<Conference[]> GetConferences();
      Task<ConferenceViewModel[]> GetConferenceViewModels();
      Task<Conference[]> GetConferencesByOrganizationId(Guid organizationid);
      Task<ConferenceViewModel[]> GetConferenceViewModelsByOrganizationId(Guid organizationid);
      Task<Conference> GetConferenceById(Guid id);
      Task<ConferenceViewModel> GetConferenceViewModelById(Guid id);
      Task Save(AddConferenceViewModel model);
      Task Delete(Guid id);
   }
}
