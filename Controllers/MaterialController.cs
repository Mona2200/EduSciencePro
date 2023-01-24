using EduSciencePro.Data.Repos;
using EduSciencePro.Models;
using EduSciencePro.ViewModels.Request;
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
      public async Task<IActionResult> Materials()
      {
         var materials = await _materials.GetMaterialViewModels();
         return View(materials);
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
