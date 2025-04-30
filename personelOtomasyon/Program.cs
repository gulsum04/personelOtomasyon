using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Data;
using Microsoft.AspNetCore.Identity;
using personelOtomasyon.Models;
using personelOtomasyon.Connected_Services.KpsService;
using personelOtomasyon.Services;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı bağlantısı
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Identity servislerini ekle
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


//tc giriş
builder.Services.AddScoped<IKpsService, MockKpsService>();

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

Rotativa.AspNetCore.RotativaConfiguration.Setup(builder.Environment.WebRootPath, "Rotativa");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Admin", "Aday", "Yönetici", "Jüri" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Admin kullanıcıyı oluştur
    string adminTc = "66666666666";
    string adminEmail = "admin@kou.edu.tr";
    string adminPassword = "Admin123!";

    var adminUser = await userManager.FindByNameAsync(adminTc);
    if (adminUser == null)
    {
        var newAdmin = new ApplicationUser
        {
            FullName = "Sistem Admini",
            TcKimlikNo = adminTc,
            UserName = adminTc,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(newAdmin, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Admin");
        }
    }
}

app.Run();