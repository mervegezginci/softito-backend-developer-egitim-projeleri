using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using seyahat_projesi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace seyahat_projesi.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Apply migrations automatically
            await context.Database.MigrateAsync();

            // Seed Roles
            var roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Gamze (Admin)
            var gamzeEmail = "gamze@gmtravel.com";
            var gamzeUser = await userManager.FindByEmailAsync(gamzeEmail);
            if (gamzeUser == null)
            {
                gamzeUser = new ApplicationUser
                {
                    UserName = gamzeEmail,
                    Email = gamzeEmail,
                    Name = "Gamze Yılmaz",
                    EmailConfirmed = true,
                    Status = "active"
                };
                await userManager.CreateAsync(gamzeUser, "admin123");
                await userManager.AddToRoleAsync(gamzeUser, "Admin");
            }

            // Seed Merve (Admin)
            var merveEmail = "merve@gmtravel.com";
            var merveUser = await userManager.FindByEmailAsync(merveEmail);
            if (merveUser == null)
            {
                merveUser = new ApplicationUser
                {
                    UserName = merveEmail,
                    Email = merveEmail,
                    Name = "Merve Kaya",
                    EmailConfirmed = true,
                    Status = "active"
                };
                await userManager.CreateAsync(merveUser, "admin123");
                await userManager.AddToRoleAsync(merveUser, "Admin");
            }

            // Seed Legacy Admin User
            var adminEmail = "admin@gezgin.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "GM Admin",
                    EmailConfirmed = true,
                    Status = "active"
                };
                await userManager.CreateAsync(adminUser, "admin123");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Seed Regular User
            var userEmail = "ahmet@gmail.com";
            var regularUser = await userManager.FindByEmailAsync(userEmail);
            if (regularUser == null)
            {
                regularUser = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    Name = "Ahmet Yılmaz",
                    EmailConfirmed = true,
                    Status = "active"
                };
                await userManager.CreateAsync(regularUser, "user123");
                await userManager.AddToRoleAsync(regularUser, "User");
            }

            // Seed Guides
            if (!context.Guides.Any())
            {
                var guides = new List<Guide>
                {
                    new Guide { FullName = "Caner Şen", Mail = "caner@gmtravel.com", Phone = "0555 111 2233", Bio = "Kültür ve doğa turlarında 10 yıllık deneyimli rehber.", GuideImageUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?w=500&auto=format&fit=crop&q=60" },
                    new Guide { FullName = "Elif Kaya", Mail = "elif@gmtravel.com", Phone = "0555 222 3344", Bio = "Gastronomi ve doğa sporları uzmanı kokartlı rehber.", GuideImageUrl = "https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=500&auto=format&fit=crop&q=60" }
                };
                context.Guides.AddRange(guides);
                await context.SaveChangesAsync();
            }

            // Seed Categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Kültür", Description = "Tarihi ve kültürel miras turları" },
                    new Category { Name = "Doğa & Macera", Description = "Doğa yürüyüşü, kamp ve adrenalin dolu aktiviteler" },
                    new Category { Name = "Ege-Akdeniz", Description = "Yaz sezonu deniz ve sahil turları" }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Seed Tours
            if (!context.Tours.Any())
            {
                var culture = context.Categories.First(c => c.Name == "Kültür");
                var adventure = context.Categories.First(c => c.Name == "Doğa & Macera");
                var ege = context.Categories.First(c => c.Name == "Ege-Akdeniz");
                
                var guide1 = context.Guides.First();
                var guide2 = context.Guides.Last();

                var tours = new List<Tour>
                {
                    new Tour
                    {
                        Title = "Kapadokya Balon Turu",
                        Description = "Peri bacaları üzerinde balon keyfi ve yeraltı şehirleri turu.",
                        Location = "Nevşehir, Türkiye",
                        StartDate = DateTime.Now.AddDays(10),
                        EndDate = DateTime.Now.AddDays(13),
                        DurationDays = 3,
                        Price = 12000,
                        Capacity = 20,
                        ImageUrl = "https://images.unsplash.com/photo-1507608869274-d3177c8bb4c7?w=800&auto=format&fit=crop&q=60",
                        IsActive = true,
                        CategoryId = culture.Id,
                        GuideId = guide1.Id
                    },
                    new Tour
                    {
                        Title = "Likya Yolu Doğa Yürüyüşü",
                        Description = "Fethiye'den Kaş'a uzanan eşsiz doğa ve tarih rotası yürüyüşü.",
                        Location = "Muğla, Türkiye",
                        StartDate = DateTime.Now.AddDays(20),
                        EndDate = DateTime.Now.AddDays(25),
                        DurationDays = 5,
                        Price = 18000,
                        Capacity = 15,
                        ImageUrl = "https://images.unsplash.com/photo-1501785888041-af3ef285b470?w=800&auto=format&fit=crop&q=60",
                        IsActive = true,
                        CategoryId = adventure.Id,
                        GuideId = guide2.Id
                    },
                    new Tour
                    {
                        Title = "Ege Mavi Yolculuk",
                        Description = "Bodrum çıkışlı ultra lüks yat turu ve koy gezileri.",
                        Location = "Muğla, Türkiye",
                        StartDate = DateTime.Now.AddDays(30),
                        EndDate = DateTime.Now.AddDays(37),
                        DurationDays = 7,
                        Price = 28000,
                        Capacity = 12,
                        ImageUrl = "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?w=800&auto=format&fit=crop&q=60",
                        IsActive = true,
                        CategoryId = ege.Id,
                        GuideId = guide1.Id
                    }
                };
                context.Tours.AddRange(tours);
                await context.SaveChangesAsync();
            }

            // Seed Coupons
            if (!context.Coupons.Any())
            {
                var coupons = new List<Coupon>
                {
                    new Coupon { Code = "MERHABA10", DiscountType = "percentage", DiscountValue = 10, ExpiryDate = DateTime.Now.AddYears(1), IsActive = true },
                    new Coupon { Code = "YAZ2026", DiscountType = "fixed", DiscountValue = 1500, ExpiryDate = DateTime.Now.AddYears(1), IsActive = true }
                };
                context.Coupons.AddRange(coupons);
                await context.SaveChangesAsync();
            }

            // Seed Blogs
            if (!context.Blogs.Any())
            {
                var blogs = new List<Blog>
                {
                    new Blog { Title = "Kapadokya Gezi Rehberi", Content = "Balon turlarından vadilere, yeraltı şehirlerinden kaya otellere uzanan Kapadokya seyahati hakkında bilmeniz gereken her şey bu yazıda.", ImageUrl = "https://images.unsplash.com/photo-1507608869274-d3177c8bb4c7?w=800&auto=format&fit=crop&q=60", CreatedAt = DateTime.Now },
                    new Blog { Title = "Likya Yolu Kamp Önerileri", Content = "Akdeniz'in en güzel kıyılarını barındıran Likya Yolu'nda kamp yaparken dikkat etmeniz gereken ekipmanlar ve rota ipuçları.", ImageUrl = "https://images.unsplash.com/photo-1501785888041-af3ef285b470?w=800&auto=format&fit=crop&q=60", CreatedAt = DateTime.Now }
                };
                context.Blogs.AddRange(blogs);
                await context.SaveChangesAsync();
            }

            // Seed Faqs
            if (!context.Faqs.Any())
            {
                var faqs = new List<Faq>
                {
                    new Faq { Question = "Turlar için iptal politikanız nedir?", Answer = "Tur tarihine 7 gün kala yapılan iptallerde kesintisiz %100 ücret iadesi yapmaktayız.", SortOrder = 1 },
                    new Faq { Question = "Ödemelerde taksit imkanı var mı?", Answer = "Kredi kartlarıyla yapılan rezervasyonlarda 3-6-9 taksit seçeneklerimiz mevcuttur.", SortOrder = 2 }
                };
                context.Faqs.AddRange(faqs);
                await context.SaveChangesAsync();
            }

            // Seed Bookings, Payments, Reviews, ChatMessages, ContactMessages, SystemLogs
            if (!context.Bookings.Any())
            {
                var tour1 = context.Tours.First(t => t.Title == "Kapadokya Balon Turu");
                var tour2 = context.Tours.First(t => t.Title == "Likya Yolu Doğa Yürüyüşü");
                var userObj = await userManager.FindByEmailAsync("ahmet@gmail.com");

                var booking1 = new Booking
                {
                    TourId = tour1.Id,
                    UserId = userObj.Id,
                    BookingDate = DateTime.Now.AddDays(10),
                    GuestsCount = 2,
                    TotalPrice = 24000,
                    PaymentStatus = "paid",
                    Status = "approved"
                };

                var booking2 = new Booking
                {
                    TourId = tour2.Id,
                    UserId = userObj.Id,
                    BookingDate = DateTime.Now.AddDays(20),
                    GuestsCount = 1,
                    TotalPrice = 18000,
                    PaymentStatus = "pending",
                    Status = "pending"
                };

                context.Bookings.AddRange(booking1, booking2);
                await context.SaveChangesAsync();

                // Seed Payment
                var payment = new Payment
                {
                    BookingId = booking1.Id,
                    Amount = 24000,
                    PaymentDate = DateTime.Now.AddDays(-2),
                    TransactionId = "TX100293021",
                    PaymentMethod = "credit_card",
                    Status = "completed"
                };
                context.Payments.Add(payment);

                // Seed Review
                var review = new Review
                {
                    TourId = tour1.Id,
                    UserId = userObj.Id,
                    Rating = 5,
                    Comment = "Kapadokya balon turu harikaydı, GM Travel ve rehberimiz Caner beye çok teşekkürler!",
                    CreatedAt = DateTime.Now.AddDays(-1)
                };
                context.Reviews.Add(review);
                await context.SaveChangesAsync();
            }

            if (!context.ChatMessages.Any())
            {
                var chats = new List<ChatMessage>
                {
                    new ChatMessage { SessionId = "session_ahmet_001", Sender = "user", MessageText = "Merhaba, Kapadokya turu için hava durumu iptal olursa ne yapıyorsunuz?", Timestamp = DateTime.Now.AddHours(-3) },
                    new ChatMessage { SessionId = "session_ahmet_001", Sender = "admin", MessageText = "Merhaba Ahmet Bey, rüzgar sebebiyle uçuşlar iptal edilirse %100 ücret iadesi yapıyoruz.", Timestamp = DateTime.Now.AddHours(-2.9) }
                };
                context.ChatMessages.AddRange(chats);
                await context.SaveChangesAsync();
            }

            if (!context.ContactMessages.Any())
            {
                var messages = new List<ContactMessage>
                {
                    new ContactMessage { Name = "Zeynep Demir", Email = "zeynep@hotmail.com", Subject = "Balayı Tur Önerisi", Message = "Merhaba, Eylül ayında 5 günlük Ege kıyılarında özel bir tur düşünüyoruz, önerileriniz nelerdir?", IsRead = false, CreatedAt = DateTime.Now.AddDays(-3) },
                    new ContactMessage { Name = "Mustafa Çelik", Email = "mustafa@yahoo.com", Subject = "Grup İndirimi", Message = "12 kişilik iş arkadaşları grubu olarak Likya yolu turuna katılmak istiyoruz, grup indirimi tanımlayabilir misiniz?", IsRead = true, CreatedAt = DateTime.Now.AddDays(-5) }
                };
                context.ContactMessages.AddRange(messages);
                await context.SaveChangesAsync();
            }

            if (!context.SystemLogs.Any())
            {
                var userObj = await userManager.FindByEmailAsync("ahmet@gmail.com");
                var gamzeUserObj = await userManager.FindByEmailAsync("gamze@gmtravel.com");

                var logs = new List<SystemLog>
                {
                    new SystemLog { UserId = gamzeUserObj.Id, Action = "LOGIN", Level = "info", IpAddress = "::1", Details = "Yönetici girişi başarılı (Gamze).", CreatedAt = DateTime.Now.AddDays(-4) },
                    new SystemLog { UserId = userObj.Id, Action = "BOOKING_CREATE", Level = "info", IpAddress = "::1", Details = "Kapadokya Balon Turu rezervasyonu yapıldı.", CreatedAt = DateTime.Now.AddDays(-2) }
                };
                context.SystemLogs.AddRange(logs);
                await context.SaveChangesAsync();
            }
        }
    }
}
