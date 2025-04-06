// Data/SeedData.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using personelOtomasyon.Models;

namespace personelOtomasyon.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            string[] roleNames = { "Admin", "User", "Yonetici", "Juri" };

            foreach (var role in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Admin kullanıcı
            string adminTc = "66666666666";
            var adminUser = await userManager.FindByNameAsync(adminTc);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    FullName = "Sistem Yöneticisi",
                    TcKimlikNo = adminTc,
                    UserName = adminTc,
                    Email = "admin@kou.edu.tr",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Jüri kullanıcı
            string juriTc = "77777777777";
            var juriUser = await userManager.FindByNameAsync(juriTc);
            if (juriUser == null)
            {
                juriUser = new ApplicationUser
                {
                    FullName = "Jüri Üyesi 1",
                    TcKimlikNo = juriTc,
                    UserName = juriTc,
                    Email = "juri@kou.edu.tr",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(juriUser, "Juri123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(juriUser, "Juri");
            }

            // Aday kullanıcı
            string userTc = "88888888888";
            var adayUser = await userManager.FindByNameAsync(userTc);
            if (adayUser == null)
            {
                adayUser = new ApplicationUser
                {
                    FullName = "Aday Kullanıcı",
                    TcKimlikNo = userTc,
                    UserName = userTc,
                    Email = "aday@kou.edu.tr",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adayUser, "Aday123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adayUser, "User");
            }

            // Örnek ilan
            if (!context.AkademikIlanlar.Any())
            {
                context.AkademikIlanlar.Add(new AkademikIlan
                {
                    Baslik = "Bilişim Sistemleri Mühendisliği Öğretim Üyesi İlanı ",
                    Kategori = "Dr. Öğretim Üyesi",
                    Aciklama = "Bilişim Sistemleri Mühendisliği bölümüne öğretim görevlisi alınacaktır.",
                    BasvuruBaslangicTarihi = DateTime.Today,
                    BasvuruBitisTarihi = DateTime.Today.AddDays(15),
                    KullaniciAdminId = adminUser.Id
                });

                context.AkademikIlanlar.Add(new AkademikIlan
                {
                    Baslik = "Güzel Sanatlar Fakültesi Doçent İlanı ",
                    Kategori = "Doçent",
                    Aciklama = "Güzel Sanatlar fakültesi'ne doçent alınacaktır.",
                    BasvuruBaslangicTarihi = DateTime.Today,
                    BasvuruBitisTarihi = DateTime.Today.AddDays(15),
                    KullaniciAdminId = adminUser.Id
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
