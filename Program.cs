using EduSciencePro;
using EduSciencePro.Data;
using EduSciencePro.Data.Repos;
using EduSciencePro.Hubs;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Swashbuckle.Swagger;
using EduSciencePro.Data.Services;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
//builder.Services.AddSwaggerGen();


var assembly = Assembly.GetAssembly(typeof(MappingProfile));
builder.Services.AddAutoMapper(assembly);

builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IRoleRepository, RoleRepository>();
builder.Services.AddSingleton<ITypeRepository, TypeRepository>();
builder.Services.AddSingleton<IResumeRepository, ResumeRepository>();

builder.Services.AddSingleton<IConfirmationCodeRepository, ConfirmationCodeRepository>();

builder.Services.AddSingleton<IEducationRepository, EducationRepository>();
builder.Services.AddSingleton<IPlaceWorkRepository, PlaceWorkRepository>();
builder.Services.AddSingleton<IOrganizationRepository, OrganizationRepository>();

builder.Services.AddSingleton<ITagRepository, TagRepository>();
builder.Services.AddSingleton<IPostRepository, PostRepository>();
builder.Services.AddSingleton<ILikePostRepository, LikePostRepository>();
builder.Services.AddSingleton<ICommentRepository, CommentRepository>();

builder.Services.AddSingleton<IProjectRepository, ProjectRepository>();
builder.Services.AddSingleton<IConferenceRepository, ConferenceRepository>();
builder.Services.AddSingleton<ICooperationRepository, CooperationRepository>();
builder.Services.AddSingleton<IInternshipRepository, InternshipRepository>();
builder.Services.AddSingleton<ICourseRepository, CourseRepository>();
builder.Services.AddSingleton<IMaterialRepository, MaterialRepository>();

builder.Services.AddSingleton<IMessageRepository, MessageRepository>();

builder.Services.AddSingleton<INotificationRepository, NotificationRepository>();

builder.Services.AddSingleton<DateTimeService>();

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection), ServiceLifetime.Singleton);

builder.Services.AddAuthentication(opt => opt.DefaultScheme = "Cookies")
.AddCookie("Cookies", opt =>
{
   opt.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
   {
      OnRedirectToLogin = redirectContext =>
      {
         redirectContext.HttpContext.Response.StatusCode = 401;
         return Task.CompletedTask;
      }
   };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
   app.UseExceptionHandler("/Home/Error");
   // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
   app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapHub<ChatHub>("/ChatHub");

app.UseRouting();

//app.UseWebSockets();

app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Index}/{id?}");

//app.MapHub<ChatHandler>("OpenMessage");

//app.UseSwagger();
//app.UseSwaggerUI(options =>
//{
//    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
//    options.RoutePrefix = string.Empty;
//});


app.Run();
