using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using seyahat_projesi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace seyahat_projesi.Data
{
    public static class DbSeeder
    {
        public static async Task Seed20Async(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            // Cleanup any garbled tours from previous manual powershell inserts
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Tours WHERE Title LIKE '%Ã%'");

            // Guard: check if we already seeded 20 items to prevent duplicate seeding on every startup
            if (context.Categories.Count() >= 20)
            {
                return; // Already seeded
            }

            // 1. Seed Categories (Ensure at least 20)
            var categoryNames = new[] {
                "Kültür Rotaları", "Doğa ve Macera", "Mavi Yolculuk", "Gastronomi", 
                "Kış Turizmi", "Balayı Paketleri", "Arkeoloji ve Tarih", "Fotoğrafçılık", 
                "Sağlık ve Termal", "Yayla Turizmi", "Kamp ve Karavan", "Ekstrem Sporlar",
                "Kanyon Geçişi", "Kuş Gözlemciliği", "İnanç Turizmi", "Bisiklet Rotaları",
                "Mağara Keşifleri", "Şehir Turları", "Ekoturizm", "Bağ Bozumu Turları"
            };
            
            var categories = new List<Category>();
            for (int i = 0; i < 20; i++)
            {
                var cat = new Category 
                { 
                    Name = categoryNames[i], 
                    Description = $"{categoryNames[i]} için özel olarak hazırlanmış en prestijli seyahat programları." 
                };
                categories.Add(cat);
            }
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            // 2. Seed Guides (Ensure at least 20)
            var guideNames = new[] {
                "Mehmet Şahin", "Ayşe Çelik", "Hakan Öztürk", "Fatma Yılmaz", "Mustafa Yıldız",
                "Zeynep Demir", "Ali Koç", "Elif Kaya", "Ömer Arslan", "Hatice Polat",
                "Süleyman Bulut", "Emine Şen", "İbrahim Aksoy", "Merve Kılıç", "Ahmet Aydın",
                "Selin Öztürk", "Caner Ünal", "Büşra Çetin", "Murat Özdemir", "Gökhan Güler"
            };

            var guides = new List<Guide>();
            for (int i = 0; i < 20; i++)
            {
                var guide = new Guide
                {
                    FullName = guideNames[i],
                    Mail = $"{guideNames[i].ToLower().Replace(" ", "").Replace("ş", "s").Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i").Replace("ö", "o").Replace("ü", "u")}@gmtravel.com",
                    Phone = $"0555 {100 + i} {20 + i}{30 + i}",
                    Bio = $"{guideNames[i]} rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.",
                    GuideImageUrl = $"https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=500&auto=format&fit=crop&q=60"
                };
                guides.Add(guide);
            }
            context.Guides.AddRange(guides);
            await context.SaveChangesAsync();

            // 3. Seed Users (Ensure at least 20)
            var users = new List<ApplicationUser>();
            for (int i = 1; i <= 20; i++)
            {
                var email = $"gezgin_user{i}@gmail.com";
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        Name = $"Gezgin Müşteri {i}",
                        EmailConfirmed = true,
                        Status = "active"
                    };
                    await userManager.CreateAsync(user, "user123");
                }
                users.Add(user);
            }

            // 4. Seed Tours (Ensure at least 20)
            var tourTitles = new[] {
                "Göbeklitepe ve Şanlıurfa Kültür Turu", "Fırtına Deresi Rize Yaylaları", "Marmaris Mavi Koylar Yolculuğu",
                "Gaziantep Gastronomi ve Lezzet Rotası", "Uludağ ve Kartalkaya Kayak Turu", "Kapadokya Balayı Rüyası",
                "Efes Antik Kenti ve Şirince Gezisi", "Ihlara Vadisi Fotoğraf Safari", "Pamukkale Travertenleri ve Termal keyfi",
                "Ayder Yaylası Doğa Yürüyüşü", "Kaz Dağları Kamp ve Karavan Deneyimi", "Fethiye Yamaç Paraşütü ve Macera",
                "Köprülü Kanyon Rafting Turu", "Manyas Kuş Cenneti Gözlem Turu", "Mardin Taş Evler ve İnanç Rotası",
                "Gökçeada ve Bozcaada Bisiklet Turu", "Karain Mağarası Keşif Gezisi", "İstanbul Boğazı ve Tarihi Yarımada",
                "Küre Dağları Ekoturizm Yolu", "Urla Bağ Bozumu ve Şarap Rotası"
            };

            var tourLocations = new[] {
                "Şanlıurfa", "Rize", "Muğla", "Gaziantep", "Bursa", "Nevşehir", "İzmir", "Aksaray", "Denizli", "Rize",
                "Çanakkale", "Muğla", "Antalya", "Balıkesir", "Mardin", "Çanakkale", "Antalya", "İstanbul", "Kastamonu", "İzmir"
            };

            var dbTours = new List<Tour>();
            for (int i = 0; i < 20; i++)
            {
                var category = categories[i % categories.Count];
                var guide = guides[i % guides.Count];
                var tour = new Tour
                {
                    Title = tourTitles[i],
                    Description = $"{tourTitles[i]} ile Türkiye'nin en eşsiz yerlerini, uzman rehberimiz {guide.FullName} eşliğinde keşfedin. Konforlu ulaşım ve konaklama dahildir.",
                    Location = $"{tourLocations[i]}, Türkiye",
                    StartDate = DateTime.Now.AddDays(15 + i * 2),
                    EndDate = DateTime.Now.AddDays(18 + i * 2),
                    DurationDays = 3 + (i % 4),
                    Price = 10000 + i * 1200,
                    Capacity = 10 + i,
                    ImageUrl = "https://images.unsplash.com/photo-1507608869274-d3177c8bb4c7?w=800&auto=format&fit=crop&q=60",
                    IsActive = true,
                    CategoryId = category.Id,
                    GuideId = guide.Id
                };
                dbTours.Add(tour);
            }
            context.Tours.AddRange(dbTours);
            await context.SaveChangesAsync();

            // 5. Seed Coupons (Ensure at least 20)
            var coupons = new List<Coupon>();
            for (int i = 1; i <= 20; i++)
            {
                var coupon = new Coupon
                {
                    Code = $"TURKCE{i:D2}",
                    DiscountType = i % 2 == 0 ? "percentage" : "fixed",
                    DiscountValue = i % 2 == 0 ? 5 + i : 500 + i * 100,
                    ExpiryDate = DateTime.Now.AddMonths(6),
                    IsActive = true
                };
                coupons.Add(coupon);
            }
            context.Coupons.AddRange(coupons);
            await context.SaveChangesAsync();

            // 6. Seed Blogs (Ensure at least 20)
            var blogTitles = new[] {
                "Şanlıurfa'da Ne Yenir? En İyi Kebapçılar", "Rize Yaylalarında Kamp Yapmanın Püf Noktaları", "Marmaris'in En Sakin Koyları",
                "Gaziantep Gastronomi Müzesi Gezi Rehberi", "Kayak Yaparken Dikkat Edilmesi Gerekenler", "Kapadokya Balon Turu Fiyatları 2026",
                "Efes Harabeleri Tarihi ve Giriş Detayları", "Doğa Fotoğrafçılığı İçin Gerekli Ekipmanlar", "Pamukkale'de Şifalı Termal Sular",
                "Ayder Yaylası Otel Tavsiyeleri", "Kaz Dağları'nda Oksijen Depolayacağınız Rotalar", "Fethiye Ölüdeniz Paraşüt Firmaları",
                "Antalya Rafting Alanları Karşılaştırması", "Kuş Gözlemciliğine Başlama Kılavuzu", "Mardin'in Büyülü Taş Mimarisi",
                "Bozcaada Bisiklet Parkurları Haritası", "Antalya Karain Mağarası Giriş Ücreti", "İstanbul Boğaz Turu Saatleri ve Rotaları",
                "Ekoturizm Nedir? Küre Dağları Örneği", "Urla Bağ Bozumu Şenlikleri Takvimi"
            };

            var blogs = new List<Blog>();
            for (int i = 0; i < 20; i++)
            {
                var blog = new Blog
                {
                    Title = blogTitles[i],
                    Content = $"{blogTitles[i]} başlığı altında hazırladığımız rehber yazımızda, bölgeye yapacağınız seyahatte işinize yarayacak tüm ipuçlarını, konaklama önerilerini ve gezilecek yerleri detaylıca inceledik.",
                    ImageUrl = "https://images.unsplash.com/photo-1501785888041-af3ef285b470?w=800&auto=format&fit=crop&q=60",
                    CreatedAt = DateTime.Now.AddDays(-i)
                };
                blogs.Add(blog);
            }
            context.Blogs.AddRange(blogs);
            await context.SaveChangesAsync();

            // 7. Seed Faqs (Ensure at least 20)
            var faqs = new List<Faq>();
            for (int i = 1; i <= 20; i++)
            {
                var faq = new Faq
                {
                    Question = $"Seyahat planlarında {i}. sıkça sorulan soru başlığı nedir?",
                    Answer = $"Bu soruya istinaden yapılan geri dönüşlere göre, ödeme iadeleri ve rezervasyon değişiklikleri koşullarımız web sitemizde yer almaktadır.",
                    SortOrder = i
                };
                faqs.Add(faq);
            }
            context.Faqs.AddRange(faqs);
            await context.SaveChangesAsync();

            // 8. Seed ContactMessages (Ensure at least 20)
            var contactMessages = new List<ContactMessage>();
            for (int i = 1; i <= 20; i++)
            {
                var msg = new ContactMessage
                {
                    Name = $"Müşteri Danışma {i}",
                    Email = $"danisma{i}@gmail.com",
                    Subject = $"Bilgi Talebi ve Sorular - Konu {i}",
                    Message = $"Merhaba, {i} numaralı seyahat paketi hakkında detaylı bilgi ve müsaitlik durumunu öğrenmek istiyorum. Dönüş yaparsanız sevinirim.",
                    IsRead = i % 3 == 0,
                    CreatedAt = DateTime.Now.AddDays(-i)
                };
                contactMessages.Add(msg);
            }
            context.ContactMessages.AddRange(contactMessages);
            await context.SaveChangesAsync();

            // 9. Seed ChatMessages (Ensure at least 20)
            var chatMessages = new List<ChatMessage>();
            for (int i = 1; i <= 20; i++)
            {
                var chat = new ChatMessage
                {
                    SessionId = $"session_client_{100 + i}",
                    Sender = i % 2 == 0 ? "user" : "admin",
                    MessageText = i % 2 == 0 ? $"Merhaba, canlı destek üzerinden {i}. sorumu sormak istiyorum." : $"Elbette, size seyahat paketlerimiz hakkında yardımcı olmaktan memnuniyet duyarım.",
                    Timestamp = DateTime.Now.AddMinutes(-10 * i)
                };
                chatMessages.Add(chat);
            }
            context.ChatMessages.AddRange(chatMessages);
            await context.SaveChangesAsync();

            // 10. Seed SystemLogs (Ensure at least 20)
            var systemLogs = new List<SystemLog>();
            for (int i = 0; i < 20; i++)
            {
                var user = users[i % users.Count];
                var log = new SystemLog
                {
                    UserId = user.Id,
                    Action = i % 2 == 0 ? "LOGIN_SUCCESS" : "SEARCH_PERFORMED",
                    Level = i % 10 == 0 ? "warn" : "info",
                    IpAddress = $"192.168.1.{10 + i}",
                    Details = $"{user.Email} kullanıcısı seyahat sistemi üzerinde {i}. işlem adımını gerçekleştirdi.",
                    CreatedAt = DateTime.Now.AddHours(-i)
                };
                systemLogs.Add(log);
            }
            context.SystemLogs.AddRange(systemLogs);
            await context.SaveChangesAsync();

            // 11. Seed Bookings (Ensure at least 20)
            var bookings = new List<Booking>();
            for (int i = 0; i < 20; i++)
            {
                var tour = dbTours[i % dbTours.Count];
                var user = users[i % users.Count];
                var coupon = coupons[i % coupons.Count];
                var booking = new Booking
                {
                    TourId = tour.Id,
                    UserId = user.Id,
                    BookingDate = DateTime.Now.AddDays(i),
                    GuestsCount = 1 + (i % 4),
                    TotalPrice = tour.Price * (1 + (i % 4)),
                    PaymentStatus = i % 3 == 0 ? "paid" : (i % 3 == 1 ? "unpaid" : "refunded"),
                    Status = i % 4 == 0 ? "approved" : (i % 4 == 1 ? "pending" : "cancelled"),
                    CouponId = i % 5 == 0 ? coupon.Id : null
                };
                bookings.Add(booking);
            }
            context.Bookings.AddRange(bookings);
            await context.SaveChangesAsync();

            // 12. Seed Payments (Ensure at least 20)
            var payments = new List<Payment>();
            for (int i = 0; i < 20; i++)
            {
                var booking = bookings[i];
                if (booking.PaymentStatus == "paid")
                {
                    var payment = new Payment
                    {
                        BookingId = booking.Id,
                        Amount = booking.TotalPrice,
                        PaymentDate = DateTime.Now.AddDays(-i),
                        TransactionId = $"TRX987654{i:D2}",
                        PaymentMethod = i % 2 == 0 ? "credit_card" : "bank_transfer",
                        Status = "completed"
                    };
                    payments.Add(payment);
                }
                else
                {
                    var payment = new Payment
                    {
                        BookingId = booking.Id,
                        Amount = booking.TotalPrice,
                        PaymentDate = DateTime.Now.AddDays(-i),
                        TransactionId = null,
                        PaymentMethod = "credit_card",
                        Status = booking.PaymentStatus == "refunded" ? "failed" : "pending"
                    };
                    payments.Add(payment);
                }
            }
            context.Payments.AddRange(payments);
            await context.SaveChangesAsync();

            // 13. Seed Reviews (Ensure at least 20)
            var comments = new[] {
                "Harika bir geziydi, emeği geçen herkese teşekkürler!", "Rehberin bilgisi ve ilgisi mükemmel seviyedeydi.",
                "Ulaşım ve oteller çok konforluydu, tavsiye ederim.", "Hayatımın en güzel tatillerinden birini yaşadım.",
                "Fiyat performans oranı oldukça iyi bir tur programı.", "Yemekler harikaydı, gastronomi turundan çok memnun kaldık.",
                "Doğa yürüyüşü biraz yorucuydu ama manzaraya kesinlikle değdi.", "Balon turundaki manzaralar rüya gibiydi.",
                "Her şey en ince ayrıntısına kadar düşünülmüştü.", "Rehberimiz çok eğlenceli ve bilgiliydi.",
                "Bir dahaki seyahatimi kesinlikle yine bu acenteyle yapacağım.", "Kamp alanları temiz ve düzenliydi.",
                "Rafting macerası adrenalin doluydu, süperdi!", "Tarihi bilgileri rehberimizden dinlemek çok keyifliydi.",
                "Taş konakta konaklama harika bir deneyimdi.", "Bisiklet turu parkuru mükemmel seçilmişti.",
                "Mağara içi aydınlatma ve rehberlik mükemmeldi.", "İstanbul boğaz turundaki anlatım harikaydı.",
                "Küre dağlarının temiz havası bize çok iyi geldi.", "Urla'daki şarap tadım etkinliği son derece şıktı."
            };

            var reviews = new List<Review>();
            for (int i = 0; i < 20; i++)
            {
                var tour = dbTours[i % dbTours.Count];
                var user = users[i % users.Count];
                var review = new Review
                {
                    TourId = tour.Id,
                    UserId = user.Id,
                    Rating = 4 + (i % 2), // 4 or 5 stars
                    Comment = comments[i],
                    CreatedAt = DateTime.Now.AddDays(-i)
                };
                reviews.Add(review);
            }
            context.Reviews.AddRange(reviews);
            await context.SaveChangesAsync();
        }
    }
}
