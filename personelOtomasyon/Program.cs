using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Data;
using Microsoft.AspNetCore.Identity;
using personelOtomasyon.Models;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı bağlantısı
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Identity servislerini ekle
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

var app = builder.Build();

//  ROL EKLEME BLOĞU (Admin ve User rolleri oluşturulur)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    Task.Run(async () =>
    {
        string[] roleNames = { "Admin", "User", "Yonetici", "Juri" };

        foreach (var role in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }).GetAwaiter().GetResult();
}

// Hata yönetimi
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
