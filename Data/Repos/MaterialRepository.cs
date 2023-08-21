using AutoMapper;
using EduSciencePro.Models;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace EduSciencePro.Data.Repos
{
   public class MaterialRepository : IMaterialRepository
   {
      private readonly ApplicationDbContext _db;
      private readonly IMapper _mapper;
      public MaterialRepository(ApplicationDbContext db, IMapper mapper)
      {
         _db = db;
         _mapper = mapper;
      }

      public async Task<Material[]> GetMaterials() => await _db.Materials.ToArrayAsync();

      public async Task<MaterialViewModel[]> GetMaterialViewModels(string[] tagNames = null, int take = 5, int skip = 0)
      {
            List<Material> materials = new();
            if (tagNames != null && tagNames.Length != 0)
            {
                foreach (var tagName in tagNames)
                {
                    var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (tag != null)
                    {
                        var tagPosts = await _db.TagMaterials.Where(t => t.TagId == tag.Id).ToListAsync();
                        foreach (var tagPost in tagPosts)
                        {
                            var tagNew = await _db.Materials.FirstOrDefaultAsync(p => p.Id == tagPost.MaterialId);
                            if (tagNew != null && materials.FirstOrDefault(n => n.Id == tagNew.Id) == null)
                                materials.Add(tagNew);
                        }
                    }
                }
            }
            else
                materials = await _db.Materials.ToListAsync();
            materials = materials.Take(take).Skip(skip).ToList();

         var materialViewModels = new List<MaterialViewModel>();
         foreach (var material in materials)
         {
            var tagMaterials = await _db.TagMaterials.Where(t => t.MaterialId == material.Id).ToListAsync();
            var tags = new List<Tag>();
            foreach (var tagMaterial in tagMaterials)
            {
               var tag = await _db.Tags.FirstOrDefaultAsync(t => t.Id == tagMaterial.TagId);
               tags.Add(tag);
            }
            var materialViewModel = _mapper.Map<Material, MaterialViewModel>(material);
            materialViewModel.Tags = tags;
            materialViewModels.Add(materialViewModel);
         }
         return materialViewModels.ToArray();
      }

      public async Task Save(AddMaterialViewModel model)
      {
         Material material = _mapper.Map<AddMaterialViewModel, Material>(model);

         if (model.Tags != null)
         {
            var tags = model.Tags.Split('/', StringSplitOptions.RemoveEmptyEntries);
            foreach (var tag in tags)
            {
               var tryTag = await _db.Tags.FirstOrDefaultAsync(t => t.Name == tag);
               if (tryTag != null)
               {
                  var tagMaterial = new TagMaterial() { MaterialId = material.Id, TagId = tryTag.Id };
                  await _db.TagMaterials.AddAsync(tagMaterial);
               }
               else
               {
                  var newTag = new Tag() { Name = tag };
                  await _db.Tags.AddAsync(newTag);
                  var tagMaterial = new TagMaterial() { MaterialId = material.Id, TagId = newTag.Id };
                  await _db.TagMaterials.AddAsync(tagMaterial);
               }
            }
         }


         await _db.Materials.AddAsync(material);
         await _db.SaveChangesAsync();
      }
   }

   public interface IMaterialRepository
   {
      Task<Material[]> GetMaterials();
      Task<MaterialViewModel[]> GetMaterialViewModels(string[] tagNames = null, int take = 5, int skip = 0);
      Task Save(AddMaterialViewModel model);
   }
}
