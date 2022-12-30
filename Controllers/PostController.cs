using EduSciencePro.ViewModels.Request;
using Microsoft.AspNetCore.Mvc;

namespace EduSciencePro.Controllers
{
   public class PostController : Controller
   {
      public IActionResult Index()
      {
         return View();
      }

      [HttpGet]
      [Route("AddPost")]
      public async Task<IActionResult> AddPost() => View(new AddPostViewModel());
   }
}
