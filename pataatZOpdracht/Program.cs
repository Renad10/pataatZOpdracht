using Microsoft.EntityFrameworkCore;
using pataatZOpdracht.Models;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<PataatZaakDbContext>(
                DbContextOptions =>
                DbContextOptions.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

// Add the session service
builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(10); });

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
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=LogIn}/{id?}");

app.Run();