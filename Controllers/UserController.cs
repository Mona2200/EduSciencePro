using EduSciencePro.Data.Repos;
using EduSciencePro.Models.User;
using EduSciencePro.ViewModels.Request;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using EduSciencePro.ViewModels.Response;

namespace EduSciencePro.Controllers
{
   public class UserController : Controller
   {
      private readonly IUserRepository _users;
      private readonly IRoleRepository _roles;
      private readonly ITypeRepository _types;
      private readonly IResumeRepository _resumes;
      private readonly IMapper _mapper;
      public UserController(IUserRepository users, IRoleRepository roles, ITypeRepository types, IResumeRepository resumes, IMapper mapper)
      {
         _users = users;
         _roles = roles;
         _types = types;
         _resumes = resumes;
         _mapper = mapper;
      }

      public async Task<IActionResult> Index()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;

         if (ident.IsAuthenticated)
         {
            return RedirectToAction("Main");
         }
         else
            return RedirectToAction("Authenticate");
      }

      [HttpGet]
      [Route("Authenticate")]
      public async Task<IActionResult> Authenticate()
      {
         var authenticateUserViewModel = new AuthenticateViewModel();
         return View(authenticateUserViewModel);
      }

      [HttpPost]
      [Route("Authenticate")]
      public async Task<IActionResult> Authenticate(AuthenticateViewModel model)
      {
         var user = await _users.GetUserByEmail(model.Email);
         if (user == null)
         {
            ModelState.AddModelError("Email", "Пользователь не найдён");
            return View(model);
         }

         var hash = new PasswordHasher<User>();
         //var hashPassword = hash.HashPassword(user, model.AddUserViewModel.Password);

         PasswordVerificationResult ver = hash.VerifyHashedPassword(user, user.Password, model.Password);
         if (ver == PasswordVerificationResult.Failed)
         {
            ModelState.AddModelError("Password", "Неверный пароль");
            return View(model);
         }

         var role = await _roles.GetRoleById(user.RoleId);

         var claims = new List<Claim>
            {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Name),
            };

         var types = await _types.GetTypesByUserId(user.Id);
         foreach (var type in types)
         {
            claims.Add(new Claim(ClaimTypes.Upn, type.Name));
         }

         var claimsIdentity = new ClaimsIdentity(claims, "AppCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
         if (model.RememberMe)
         {
            var authProperties = new AuthenticationProperties
            {
               AllowRefresh = true,
               ExpiresUtc = DateTimeOffset.UtcNow.AddDays(90)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
         }
         else
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

         return RedirectToAction("Main");
      }

      [HttpGet]
      [Route("Register")]
      public async Task<IActionResult> Register()
      {
         var registerUserViewModel = new RegisterViewModel();
         registerUserViewModel.AddUserViewModel = new AddUserViewModel();
         registerUserViewModel.Types = await _types.GetTypes();
         return View(registerUserViewModel);
      }

      [HttpPost]
      [Route("Register")]
      public async Task<IActionResult> Register(RegisterViewModel model)
      {
         if (!ModelState.IsValid)
         {
            foreach (var key in ModelState.Keys)
            {
               if (ModelState[key].Errors.Count > 0)
                  ModelState.AddModelError($"{key}", $"{ModelState[key].Errors[0].ErrorMessage}");
            }
            model.Types = await _types.GetTypes();
            return View(model);
         }

         if (!model.Consent)
         {
            ModelState.AddModelError("Consent", "Подтвердите согласие");
            model.Types = await _types.GetTypes();
            return View(model);
         }

         if ((DateTime.Now.Year - DateTime.Parse(model.AddUserViewModel.Birthday).Year) < 6)
         {
            ModelState.AddModelError("AddUserViewModel.Birthday", "Вы должны быть не моложе 6 лет");
            model.Types = await _types.GetTypes();
            return View(model);
         }

         var tryFindUser = await _users.GetUserByEmail(model.AddUserViewModel.Email);
         if (tryFindUser != null)
         {
            ModelState.AddModelError("AddUserViewModel.Email", "Пользователь с таким Email уже существует");
            model.Types = await _types.GetTypes();
            return View(model);
         }

         var user = _mapper.Map<AddUserViewModel, User>(model.AddUserViewModel);

         user.Password = "";

         var hash = new PasswordHasher<User>();
         var hashPassword = hash.HashPassword(user, model.AddUserViewModel.Password);

         model.AddUserViewModel.Password = hashPassword;
         await _users.Save(model.AddUserViewModel);
         return RedirectToAction("Authenticate");
         //return RedirectToAction("EmailConfirm", model.AddUserViewModel);
      }

      //[HttpGet]
      //[Route("EmailConfirm")]
      //public async Task<IActionResult> EmailConfirm(AddUserViewModel model)
      //{

      //}

      [HttpGet]
      [Route("AddTypes")]
      public async Task<IActionResult> AddTypes()
      {
         var type = new TypeModel() { Name = "Школьник" };
         await _types.Save(type);

         type = new TypeModel() { Name = "Студент, Аспирант или Соискатель" };
         await _types.Save(type);

         type = new TypeModel() { Name = "Научный сотрудник" };
         await _types.Save(type);

         type = new TypeModel() { Name = "Профессорско-преподавательский состав сферы высшего образования" };
         await _types.Save(type);

         type = new TypeModel() { Name = "Представитель сферы среднего профессионального образования" };
         await _types.Save(type);

         type = new TypeModel() { Name = "Представитель реального сектора экономики" };
         await _types.Save(type);

         var role = new Role() { Name = "Пользователь" };
         await _roles.Save(role);

         role = new Role() { Name = "Модератор" };
         await _roles.Save(role);

         role = new Role() { Name = "Администратор" };
         await _roles.Save(role);

         return Ok();
      }

      [HttpGet]
      [Route("Main")]
      public async Task<IActionResult> Main()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var userViewModel = await _users.GetUserViewModelByEmail(claimEmail);
         return View(userViewModel);
      }

      [HttpGet]
      [Route("EditUser")]
      public async Task<IActionResult> EditUser()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var userViewModel = await _users.GetUserViewModelByEmail(claimEmail);
         var editUserViewModel = new EditUserViewModel();
         editUserViewModel.UserViewModel = userViewModel;
         editUserViewModel.AddUserViewModel = new AddUserViewModel();
         editUserViewModel.Types = await _types.GetTypes();

         editUserViewModel.AddUserViewModel.FirstName = editUserViewModel.UserViewModel?.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray()[0];
         editUserViewModel.AddUserViewModel.LastName = editUserViewModel.UserViewModel?.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray()[1];
         editUserViewModel.AddUserViewModel.MiddleName = editUserViewModel.UserViewModel?.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray()[2];

         var year = editUserViewModel.UserViewModel?.Birthday.Split('.', StringSplitOptions.RemoveEmptyEntries).ToArray()[2];
         var month = editUserViewModel.UserViewModel?.Birthday.Split('.', StringSplitOptions.RemoveEmptyEntries).ToArray()[1];
         if (month != null && month.Length == 1)
            month = $"0{month}";
         var day = editUserViewModel.UserViewModel?.Birthday.Split('.', StringSplitOptions.RemoveEmptyEntries).ToArray()[0];
         if (day != null && day.Length == 1)
            day = $"0{day}";
         if (year != null && month != null && day != null)
            editUserViewModel.AddUserViewModel.Birthday = $"{year}-{month}-{day}";

         foreach (var typeU in editUserViewModel.UserViewModel?.TypeUsers)
         {
            editUserViewModel.AddUserViewModel.TypeUsers += typeU.Id + " ";
         }

         var editResumeViewModel = new EditResumeViewModel();
         editResumeViewModel.AddResumeViewModel = new AddResumeViewModel();
         var resume = await _resumes.GetResumeByUserId(userViewModel.Id);
         editResumeViewModel.Resume = _mapper.Map<Resume, ResumeViewModel>(resume);
         if (editResumeViewModel.Resume != null)
            editResumeViewModel.Resume.Skills = await _resumes.GetSkills();

         editUserViewModel.EditResumeViewModel = editResumeViewModel;

         return View(editUserViewModel);
      }

      [HttpPost]
      [Route("EditUser")]
      public async Task<IActionResult> EditUser(EditUserViewModel model)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         if (!model.Consent)
         {
            ModelState.AddModelError("Consent", "Подтвердите согласие");
            model.UserViewModel = await _users.GetUserViewModelById(user.Id);
            model.Types = await _types.GetTypes();
            return View(model);
         }

         if (!String.IsNullOrEmpty(model.AddUserViewModel.Birthday) && (DateTime.Now.Year - DateTime.Parse(model.AddUserViewModel.Birthday).Year) < 6)
         {
            ModelState.AddModelError("AddUserViewModel.Birthday", "Вы должны быть не моложе 6 лет");
            model.UserViewModel = await _users.GetUserViewModelById(user.Id);
            model.Types = await _types.GetTypes();
            return View(model);
         }

         if (ModelState["AddUserViewModel.TypeUsers"].Errors.Count > 0)
         {
            ModelState.AddModelError($"AddUserViewModel.TypeUsers", $"{ModelState["AddUserViewModel.TypeUsers"].Errors[0].ErrorMessage}");
            model.UserViewModel = await _users.GetUserViewModelById(user.Id);
            model.Types = await _types.GetTypes();
            return View(model);
         }

         IFormFileCollection files = Request.Form.Files;
         if (files != null && files.Count != 0)
         {
            model.AddUserViewModel.Img = files[0];
         }

         if (ModelState["AddUserViewModel.TelegramLink"].Errors.Count > 0)
         {
            ModelState.AddModelError($"AddUserViewModel.TelegramLink", $"{ModelState["AddUserViewModel.TelegramLink"].Errors[0].ErrorMessage}");
            model.UserViewModel = await _users.GetUserViewModelById(user.Id);
            model.Types = await _types.GetTypes();
            return View(model);
         }

         if (ModelState["AddUserViewModel.WhatsAppLink"].Errors.Count > 0)
         {
            ModelState.AddModelError($"AddUserViewModel.WhatsAppLink", $"{ModelState["AddUserViewModel.WhatsAppLink"].Errors[0].ErrorMessage}");
            model.UserViewModel = await _users.GetUserViewModelById(user.Id);
            model.Types = await _types.GetTypes();
            return View(model);
         }

         if (ModelState["AddUserViewModel.EmailLink"].Errors.Count > 0)
         {
            ModelState.AddModelError($"AddUserViewModel.EmailLink", $"{ModelState["AddUserViewModel.EmailLink"].Errors[0].ErrorMessage}");
            model.UserViewModel = await _users.GetUserViewModelById(user.Id);
            model.Types = await _types.GetTypes();
            return View(model);
         }

         if (ModelState["AddUserViewModel.AnotherLink"].Errors.Count > 0)
         {
            ModelState.AddModelError($"AddUserViewModel.AnotherLink", $"{ModelState["AddUserViewModel.AnotherLink"].Errors[0].ErrorMessage}");
            model.UserViewModel = await _users.GetUserViewModelById(user.Id);
            model.Types = await _types.GetTypes();
            return View(model);
         }

         await _users.Update(model.AddUserViewModel, user);
         user = await _users.GetUserById(user.Id);

         var oldClaimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name);
         ident.RemoveClaim(oldClaimEmail);

         var newClaimEmail = new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email);

         ident.AddClaim(newClaimEmail);

         var claimsIdentity = new ClaimsIdentity(ident.Claims, "AppCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
         await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

         return RedirectToAction("Main");
      }

      [HttpPost]
      [Route("EditResume")]
      public async Task<IActionResult> EditResume(EditResumeViewModel model)
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         if (!model.Consent)
         {
            ModelState.AddModelError("Consent", "Подтвердите согласие");
            return RedirectToAction("EditUser");
         }

         if (ModelState["AddResumeViewModel.DateGraduationEducation"].Errors.Count > 0)
         {
            ModelState.AddModelError($"AddResumeViewModel.DateGraduationEducation", $"{ModelState["AddResumeViewModel.DateGraduationEducation"].Errors[0].ErrorMessage}");
            return RedirectToAction("EditUser");
         }

         var resume = await _resumes.GetResumeByUserId(user.Id);
         if (resume == null)
         {
            await _resumes.Save(model.AddResumeViewModel);
         }
         else
         {
            await _resumes.Update(resume, model.AddResumeViewModel);
         }

         return RedirectToAction("Main");
      }

      [HttpGet]
      [Route("EditEmail")]
      public async Task<IActionResult> EditEmail()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var editViewModel = new EditUserViewModel();
         editViewModel.UserViewModel = await _users.GetUserViewModelByEmail(claimEmail);
         editViewModel.AddUserViewModel = new AddUserViewModel();
         return View(editViewModel);
      }

      [HttpGet]
      [Route("DeleteImage")]
      public async Task<IActionResult> DeleteImage()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);
         await _users.DeleteImage(user.Id);
         return RedirectToAction("EditUser");
      }

      [HttpGet]
      [Route("GetUsers")]
      public async Task<User[]> GetUsers()
      {
         return await _users.GetUsers();
      }

      [HttpGet]
      [Route("Logout")]
      public async Task<IActionResult> Logout()
      {
         ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
         var claimEmail = ident.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Name).Value;
         var user = await _users.GetUserByEmail(claimEmail);

         await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
         return RedirectToAction("Index");
      }
   }
}
