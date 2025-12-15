using HabitTracker.Data;
using HabitTracker.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantýsý servisi (En önemli kýsým burasý)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Giriþ Çýkýþ (Authentication) Servisi
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Login";
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- VERÝTABANI BAÞLANGIÇ VERÝLERÝ (SEED DATA) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    context.Database.EnsureCreated();

    if (!context.Badges.Any())
    {
        context.Badges.AddRange(
            new Badge { Name = "Hýzlý Baþlangýç", Description = "Ýlk alýþkanlýðýný tamamladýn!", Criteria = "1_completion" },
            new Badge { Name = "Acemi", Description = "5 kez alýþkanlýk tamamladýn.", Criteria = "5_completions" },
            new Badge { Name = "Ýstikrar Ustasý", Description = "10 kez alýþkanlýk tamamladýn.", Criteria = "10_completions" },
            new Badge { Name = "Efsane", Description = "50 kez alýþkanlýk tamamladýn.", Criteria = "50_completions" }
        );
        context.SaveChanges();
    }
}
// ------------------------------------------------

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();