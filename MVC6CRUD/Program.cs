using MVC6CRUD.Data;
using Microsoft.EntityFrameworkCore;
using MVC6CRUD.Repository;
using MVC6CRUD.Models;
using Microsoft.AspNetCore.Http;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

#region Service Injected
builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));


#endregion

// Add services to the container.
builder.Services.AddControllersWithViews();
//Comment
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

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
