using Microsoft.EntityFrameworkCore;
using Lab5;
using Lab5.Models.DataAccess;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Injecting Session services to application pipeline
builder.Services.AddSession();
// Add services to the DB access
string dbConStr = builder.Configuration.GetConnectionString("StudentRecord");
builder.Services.AddDbContext<StudentRecordContext>(options => options.UseSqlServer(dbConStr));
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

// Telling program to use the session services
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=AcademicRecords}/{action=Index}/{id?}");

app.Run();
