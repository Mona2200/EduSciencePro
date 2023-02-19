using EduSciencePro.Data.Repos;
using EduSciencePro.Models;
using EduSciencePro.ViewModels.Request;
using EduSciencePro.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;

namespace EduSciencePro.Controllers
{
   public class MaterialController : Controller
   {
      private readonly IMaterialRepository _materials;
      public MaterialController(IMaterialRepository materials)
      {
         _materials = materials;
      }

      [HttpGet]
      [Route("Materials")]
      public async Task<IActionResult> Materials(string? tagNamesString)
      {
            List<string> tags = new();
            string[] tagNames = null;
            if (tagNamesString != null)
            {
                tagNamesString = tagNamesString.Replace('_', '/');
                tagNames = tagNamesString.Split('/', StringSplitOptions.RemoveEmptyEntries);

                foreach (var tagName in tagNames)
                {
                    tags.Add(tagName);
                }
            }

            var materials = await _materials.GetMaterialViewModels(tagNames, 5, 0);
         return View(new MaterialsAndTagsViewModel() { Tags = tags, Materials = materials});
      }

        [HttpGet]
        [Route("MaterialsTag/{tags}")]
        public async Task<IActionResult> MaterialsTag([FromRoute] string? tags)
        {
            return RedirectToAction("Materials", "Material", new { tagNamesString = tags });
        }

        [HttpPost]
        [Route("MaterialsMore/{take}/{skip}/{tags?}")]
        public async Task<MaterialViewModel[]> MaterialsMore([FromRoute] int take, [FromRoute] int skip, [FromRoute] string? tags = null)
        {
            string[] tagNames = null;
            if (tags != null)
            {
                tags = tags.Replace('_', '/');
                tagNames = tags.Split('/', StringSplitOptions.RemoveEmptyEntries);
            }

            var news = await _materials.GetMaterialViewModels(tagNames, take, skip);
            return news;
        }

        [HttpGet]
      [Route("AddMaterial")]
      public async Task<IActionResult> AddMaterial()
      {
         var material = new AddMaterialViewModel();
         return View(material);
      }

      [HttpPost]
      [Route("AddMaterial")]
      public async Task<IActionResult> AddMaterial(AddMaterialViewModel model)
      {
         if (!ValidMaterial(model))
            return View(model);

         await _materials.Save(model);
         return RedirectToAction("Materials");
      }

      private bool ValidMaterial(AddMaterialViewModel model)
      {
         if (!ModelState.IsValid)
         {
            foreach (var key in ModelState.Keys)
            {
               if (ModelState[key].Errors.Count > 0)
                  ModelState.AddModelError($"{key}", $"{ModelState[key].Errors[0].ErrorMessage}");
            }
            return false;
         }
         return true;
      }
   }
}
