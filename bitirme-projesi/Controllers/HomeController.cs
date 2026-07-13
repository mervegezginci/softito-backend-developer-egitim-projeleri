using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using seyahat_projesi.Data;
using seyahat_projesi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace seyahat_projesi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DapperRepository _dapper;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public HomeController(ApplicationDbContext context, DapperRepository dapper, IMemoryCache cache, IConfiguration configuration)
        {
            _context = context;
            _dapper = dapper;
            _cache = cache;
            _configuration = configuration;
        }

         [HttpGet]
        public async Task<IActionResult> Index(string? search, int? categoryId, int? minDuration)
        {
            // 1. Fetch Categories (Cached using Dapper!)
            const string cacheCategoriesKey = "CategoriesList";
            if (!_cache.TryGetValue(cacheCategoriesKey, out List<Category>? categories))
            {
                var cats = await _dapper.QueryAsync<Category>("SELECT * FROM Categories");
                categories = cats.ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                _cache.Set(cacheCategoriesKey, categories, cacheEntryOptions);
            }
            ViewBag.Categories = categories;

            // 1.5 Fetch Marquee Tour Titles (Cached)
            const string cacheMarqueeKey = "MarqueeToursList";
            if (!_cache.TryGetValue(cacheMarqueeKey, out List<string>? marqueeTours))
            {
                var titles = await _dapper.QueryAsync<string>("SELECT Title FROM Tours WHERE IsActive = 1 ORDER BY Id DESC");
                marqueeTours = titles.ToList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                _cache.Set(cacheMarqueeKey, marqueeTours, cacheEntryOptions);
            }
            ViewBag.MarqueeTours = marqueeTours;

            // 1.7 Search Results Logic
            bool isSearching = !string.IsNullOrEmpty(search) || categoryId.HasValue || minDuration.HasValue;
            ViewBag.IsSearching = isSearching;
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.MinDuration = minDuration;

            if (isSearching)
            {
                var searchSql = @"SELECT t.Id, t.Title, t.Description, t.Location, t.StartDate, t.EndDate, t.DurationDays, t.Price, t.Capacity, t.ImageUrl, t.IsActive, t.CategoryId, t.GuideId,
                                         COALESCE(AVG(CAST(r.Rating AS FLOAT)), 4.8) as AverageRating,
                                         COUNT(r.Id) as ReviewCount,
                                         c.Id, c.Name, c.Description,
                                         g.Id, g.FullName, g.Mail, g.Phone, g.Bio, g.GuideImageUrl
                                  FROM Tours t
                                  INNER JOIN Categories c ON t.CategoryId = c.Id
                                  INNER JOIN Guides g ON t.GuideId = g.Id
                                  LEFT JOIN Reviews r ON t.Id = r.TourId
                                  WHERE t.IsActive = 1";

                var searchParams = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(search))
                {
                    searchSql += " AND (t.Title LIKE @Search OR t.Location LIKE @Search)";
                    searchParams.Add("Search", $"%{search}%");
                }
                if (categoryId.HasValue)
                {
                    searchSql += " AND t.CategoryId = @CategoryId";
                    searchParams.Add("CategoryId", categoryId.Value);
                }
                if (minDuration.HasValue)
                {
                    searchSql += " AND t.DurationDays >= @MinDuration";
                    searchParams.Add("MinDuration", minDuration.Value);
                }

                searchSql += @" GROUP BY t.Id, t.Title, t.Description, t.Location, t.StartDate, t.EndDate, t.DurationDays, t.Price, t.Capacity, t.ImageUrl, t.IsActive, t.CategoryId, t.GuideId,
                                         c.Id, c.Name, c.Description,
                                         g.Id, g.FullName, g.Mail, g.Phone, g.Bio, g.GuideImageUrl
                               ORDER BY AverageRating DESC";

                var searchResults = await _dapper.QueryToursAsync(searchSql, searchParams);
                ViewBag.SearchResults = searchResults.ToList();
            }
            else
            {
                ViewBag.SearchResults = null;
            }

            // Fetch top 6 popular tours ordered by AverageRating DESC (Cached)
            const string cacheToursKey = "PopularActiveTours";
            if (!_cache.TryGetValue(cacheToursKey, out List<Tour>? tours))
            {
                var sql = @"SELECT TOP 6 t.Id, t.Title, t.Description, t.Location, t.StartDate, t.EndDate, t.DurationDays, t.Price, t.Capacity, t.ImageUrl, t.IsActive, t.CategoryId, t.GuideId,
                                   COALESCE(AVG(CAST(r.Rating AS FLOAT)), 4.8) as AverageRating,
                                   COUNT(r.Id) as ReviewCount,
                                   c.Id, c.Name, c.Description,
                                   g.Id, g.FullName, g.Mail, g.Phone, g.Bio, g.GuideImageUrl
                            FROM Tours t
                            INNER JOIN Categories c ON t.CategoryId = c.Id
                            INNER JOIN Guides g ON t.GuideId = g.Id
                            LEFT JOIN Reviews r ON t.Id = r.TourId
                            WHERE t.IsActive = 1
                            GROUP BY t.Id, t.Title, t.Description, t.Location, t.StartDate, t.EndDate, t.DurationDays, t.Price, t.Capacity, t.ImageUrl, t.IsActive, t.CategoryId, t.GuideId,
                                     c.Id, c.Name, c.Description,
                                     g.Id, g.FullName, g.Mail, g.Phone, g.Bio, g.GuideImageUrl
                            ORDER BY AverageRating DESC";

                var fetched = await _dapper.QueryToursAsync(sql);
                tours = fetched.ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(cacheToursKey, tours, cacheEntryOptions);
            }

            // Fetch FAQs dynamically
            ViewBag.Faqs = await _context.Faqs.OrderBy(f => f.SortOrder).ToListAsync();

            return View(tours);
        }

        [HttpGet("Tours")]
        public async Task<IActionResult> Tours(string? search, int? categoryId, double? maxPrice, int? minDuration)
        {
            // Fetch Categories for filters
            var cats = await _dapper.QueryAsync<Category>("SELECT * FROM Categories");
            ViewBag.Categories = cats.ToList();

            var sql = @"SELECT t.Id, t.Title, t.Description, t.Location, t.StartDate, t.EndDate, t.DurationDays, t.Price, t.Capacity, t.ImageUrl, t.IsActive, t.CategoryId, t.GuideId,
                               COALESCE(AVG(CAST(r.Rating AS FLOAT)), 4.8) as AverageRating,
                               COUNT(r.Id) as ReviewCount,
                               c.Id, c.Name, c.Description,
                               g.Id, g.FullName, g.Mail, g.Phone, g.Bio, g.GuideImageUrl
                        FROM Tours t
                        INNER JOIN Categories c ON t.CategoryId = c.Id
                        INNER JOIN Guides g ON t.GuideId = g.Id
                        LEFT JOIN Reviews r ON t.Id = r.TourId
                        WHERE t.IsActive = 1";

            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(search))
            {
                sql += " AND (t.Title LIKE @Search OR t.Location LIKE @Search)";
                parameters.Add("Search", $"%{search}%");
            }
            if (categoryId.HasValue)
            {
                sql += " AND t.CategoryId = @CategoryId";
                parameters.Add("CategoryId", categoryId.Value);
            }
            if (maxPrice.HasValue)
            {
                sql += " AND t.Price <= @MaxPrice";
                parameters.Add("MaxPrice", maxPrice.Value);
            }
            if (minDuration.HasValue)
            {
                sql += " AND t.DurationDays >= @MinDuration";
                parameters.Add("MinDuration", minDuration.Value);
            }

            sql += @" GROUP BY t.Id, t.Title, t.Description, t.Location, t.StartDate, t.EndDate, t.DurationDays, t.Price, t.Capacity, t.ImageUrl, t.IsActive, t.CategoryId, t.GuideId,
                               c.Id, c.Name, c.Description,
                               g.Id, g.FullName, g.Mail, g.Phone, g.Bio, g.GuideImageUrl
                     ORDER BY AverageRating DESC";

            var tours = await _dapper.QueryToursAsync(sql, parameters);

            // Keep filters selected in UI
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.MinDuration = minDuration;

            return View(tours);
        }

        [HttpGet("Categories")]
        public async Task<IActionResult> Categories()
        {
            var cats = await _context.Categories.ToListAsync();
            return View(cats);
        }

        [HttpGet("Category/Detail/{id}")]
        public async Task<IActionResult> CategoryDetail(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Fetch tours in this category
            var sql = @"SELECT t.Id, t.Title, t.Description, t.Location, t.StartDate, t.EndDate, t.DurationDays, t.Price, t.Capacity, t.ImageUrl, t.IsActive, t.CategoryId, t.GuideId,
                               COALESCE(AVG(CAST(r.Rating AS FLOAT)), 4.8) as AverageRating,
                               COUNT(r.Id) as ReviewCount,
                               c.Id, c.Name, c.Description,
                               g.Id, g.FullName, g.Mail, g.Phone, g.Bio, g.GuideImageUrl
                        FROM Tours t
                        INNER JOIN Categories c ON t.CategoryId = c.Id
                        INNER JOIN Guides g ON t.GuideId = g.Id
                        LEFT JOIN Reviews r ON t.Id = r.TourId
                        WHERE t.IsActive = 1 AND t.CategoryId = @CategoryId
                        GROUP BY t.Id, t.Title, t.Description, t.Location, t.StartDate, t.EndDate, t.DurationDays, t.Price, t.Capacity, t.ImageUrl, t.IsActive, t.CategoryId, t.GuideId,
                                 c.Id, c.Name, c.Description,
                                 g.Id, g.FullName, g.Mail, g.Phone, g.Bio, g.GuideImageUrl
                        ORDER BY AverageRating DESC";

            var parameters = new Dictionary<string, object> { { "CategoryId", id } };
            var tours = await _dapper.QueryToursAsync(sql, parameters);

            ViewBag.Category = category;
            return View(tours);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var tour = await _context.Tours
                .Include(t => t.Category)
                .Include(t => t.Guide)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tour == null || !tour.IsActive)
            {
                return NotFound();
            }

            // Fetch reviews
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.TourId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            ViewBag.Reviews = reviews;

            // Fetch recent tours for sidebar list
            ViewBag.RecentTours = await _context.Tours
                .Where(t => t.IsActive && t.Id != id)
                .OrderByDescending(t => t.Id)
                .Take(3)
                .ToListAsync();

            return View(tour);
        }

        [HttpGet]
        public async Task<IActionResult> ValidateCoupon(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Json(new { success = false, message = "Kupon kodu boş olamaz." });
            }

            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == code.ToUpper() && c.IsActive && c.ExpiryDate >= DateTime.Now);

            if (coupon == null)
            {
                return Json(new { success = false, message = "Geçersiz veya süresi dolmuş kupon kodu." });
            }

            return Json(new { 
                success = true, 
                discountType = coupon.DiscountType, 
                discountValue = coupon.DiscountValue,
                message = coupon.DiscountType == "percentage" ? $"%{coupon.DiscountValue} indirim uygulandı." : $"{coupon.DiscountValue} TL indirim uygulandı."
            });
        }

        [HttpPost]
        public async Task<IActionResult> SaveContactMessage(string name, string email, string subject, string message)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(message))
            {
                return Json(new { success = false, message = "Lütfen gerekli alanları doldurun." });
            }

            var contactMessage = new ContactMessage
            {
                Name = name,
                Email = email,
                Subject = subject ?? "Yeni Seyahat Talebi",
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _context.ContactMessages.Add(contactMessage);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Mesajınız başarılı bir şekilde gönderildi." });
        }

        [HttpGet]
        public IActionResult Policy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetChatMessages(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId)) return BadRequest();
            var messages = await _context.ChatMessages
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
            return Json(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendChatMessage(string sessionId, string message)
        {
            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(message)) return BadRequest();

            // Save user message
            var userMsg = new ChatMessage
            {
                SessionId = sessionId,
                Sender = "user",
                MessageText = message,
                Timestamp = DateTime.Now
            };
            _context.ChatMessages.Add(userMsg);
            await _context.SaveChangesAsync();

            // Check if user is logged in
            bool isAuthenticated = User.Identity != null && User.Identity.IsAuthenticated;

            // 1. Attempt to call Gemini Generative AI API
            string? replyText = await CallGeminiApiAsync(message, isAuthenticated);

            // 2. Fallback to automatic rule-based reply if Gemini is not configured or fails
            if (string.IsNullOrEmpty(replyText))
            {
                string cleanMsg = message.ToLower();
                replyText = "Talebinizi aldık. Destek ekibimiz (Gamze veya Merve) en kısa sürede size geri dönüş sağlayacaktır. Turlarımızın detaylarına ana sayfamızdan da ulaşabilirsiniz.";

                // Coupon Token Check
                if (cleanMsg.Contains("indirim") || cleanMsg.Contains("kupon") || cleanMsg.Contains("promosyon") || cleanMsg.Contains("fırsat") || cleanMsg.Contains("ucuz"))
                {
                    if (isAuthenticated)
                    {
                        var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.IsActive && c.ExpiryDate >= DateTime.Now);
                        if (coupon != null)
                        {
                            string discountDetail = coupon.DiscountType == "percentage" ? $"%{coupon.DiscountValue} indirim" : $"{coupon.DiscountValue} TL indirim";
                            replyText = $"Size özel indirim kuponunuz: {coupon.Code} ({discountDetail} kazanmak için ödeme adımında bu kuponu uygulayabilirsiniz).";
                        }
                        else
                        {
                            replyText = "Şu anda aktif bir indirim kuponu tanımlı değildir. Güncel kampanyalarımız için ana sayfamızı takip edebilirsiniz.";
                        }
                    }
                    else
                    {
                        replyText = "İndirim kuponlarından ve özel fırsatlardan yararlanabilmek için lütfen öncelikle sisteme üye girişi yapın. Giriş yaptıktan sonra indirim kodunuzu memnuniyetle paylaşabilirim.";
                    }
                }
                // FAQ database match
                else
                {
                    var faqs = await _context.Faqs.ToListAsync();
                    var matchedFaq = faqs.FirstOrDefault(f => 
                        cleanMsg.Contains(f.Question.ToLower()) || 
                        f.Question.ToLower().Split(' ').Any(w => w.Length > 4 && cleanMsg.Contains(w.Replace("?", "").Replace(".", ""))));

                    if (matchedFaq != null)
                    {
                        replyText = matchedFaq.Answer;
                    }
                    else if (cleanMsg.Contains("fiyat") || cleanMsg.Contains("para") || cleanMsg.Contains("tutar") || cleanMsg.Contains("tl"))
                    {
                        replyText = "Turlarımızın güncel kişi başı fiyatları ana sayfamızda listelenmektedir. Ödemelerinizi güvenli sanal ödeme sistemiyle taksitli veya tek çekim olarak yapabilirsiniz.";
                    }
                    else if (cleanMsg.Contains("nerede") || cleanMsg.Contains("rota") || cleanMsg.Contains("konum"))
                    {
                        replyText = "Turlarımız Kapadokya, Likya Yolu ve Ege kıyılarını kapsamaktadır. Detaylar sekmesinden rotaları görebilirsiniz.";
                    }
                    else if (cleanMsg.Contains("öner") || cleanMsg.Contains("tavsiye") || cleanMsg.Contains("seyahat") || cleanMsg.Contains("hangisi"))
                    {
                        replyText = "Doğa severler için 'Likya Yolu Doğa Yürüyüşü' turunu, deniz/güneş arayanlar için 'Ege Mavi Yolculuk' turunu, kültür ve balon deneyimi içinse 'Kapadokya Balon Turu'nu tavsiye ederiz. Hangi kategori ilginizi çekiyor?";
                    }
                    else if (cleanMsg.Contains("başka var") || cleanMsg.Contains("başka tur") || cleanMsg.Contains("başka rota") || cleanMsg.Contains("başka destinasyon") || cleanMsg.Contains("başka seçene"))
                    {
                        replyText = "Şu anda aktif olan 3 ana seyahat programımız bulunmaktadır: Kapadokya Balon Turu, Likya Yolu Doğa Yürüyüşü ve Ege Mavi Yolculuk. Çok yakında yeni ve heyecan verici rotalarımız da sisteme eklenecektir! Takipte kalın.";
                    }
                    else if (cleanMsg.Contains("kapadokya"))
                    {
                        replyText = "Kapadokya Balon Turumuz 3 gün sürmektedir. Peri bacaları, yeraltı şehirleri gezisi ve balon uçuşu fiyata dahildir. Kişi başı fiyatı 12.000 TL'dir.";
                    }
                    else if (cleanMsg.Contains("likya"))
                    {
                        replyText = "Likya Yolu doğa yürüyüşü turumuz 5 gün sürmektedir. Fethiye'den Kaş'a kadar uzanan eşsiz koyları ve antik kentleri rehber eşliğinde yürürüz. Fiyatı 18.000 TL'dir.";
                    }
                    else if (cleanMsg.Contains("ege") || cleanMsg.Contains("mavi") || cleanMsg.Contains("yolculuk"))
                    {
                        replyText = "Ege Mavi Yolculuk turumuz Bodrum çıkışlı olup 7 gün süren ultra lüks bir yat gezisidir. Kişi başı fiyatı 28.000 TL'dir.";
                    }
                    else if (cleanMsg.Contains("merhaba") || cleanMsg.Contains("selam") || cleanMsg.Contains("hey") || cleanMsg.Contains("mrb"))
                    {
                        replyText = "Merhaba! GM Travel Canlı Destek Merkezine hoş geldiniz. Size nasıl yardımcı olabiliriz? Rotalarımız, fiyatlar veya indirim kuponları hakkında sorabilirsiniz.";
                    }
                    else if (cleanMsg.Contains("teşekkür") || cleanMsg.Contains("sagol") || cleanMsg.Contains("sağol") || cleanMsg.Contains("tşk") || cleanMsg.Contains("eyvallah"))
                    {
                        replyText = "Rica ederiz! Yardımcı olabildiysek ne mutlu bize. Keyifli ve unutulmaz bir seyahat dileriz! Başka bir sorunuz olursa her zaman buradayız.";
                    }
                }
            }

            var adminMsg = new ChatMessage
            {
                SessionId = sessionId,
                Sender = "admin",
                MessageText = replyText,
                Timestamp = DateTime.Now.AddSeconds(1)
            };
            _context.ChatMessages.Add(adminMsg);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        private async Task<string?> CallGeminiApiAsync(string userMessage, bool isAuthenticated)
        {
            var apiKey = _configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return null; // Fallback to rule-based chatbot
            }

            // Try multiple models in case the API key or region restricts specific models (Updated for 2026 models)
            var models = new[] { "gemini-3.5-flash", "gemini-3.1-flash-lite" };
            
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(8);

            string authInstruction = isAuthenticated 
                ? "Kullanıcı şu anda sisteme GİRİŞ YAPMIŞ durumda. İndirim kuponu sorarsa aktif kupon kodumuz olan 'GEZGIN2026' ile indirim kazanabileceğini belirtebilirsin." 
                : "Kullanıcı şu anda sisteme GİRİŞ YAPMAMIŞ (ziyaretçi). İndirim kuponu sorarsa, kupon kodunu vermemelisin! Bunun yerine nazikçe önce sisteme üye girişi yapması gerektiğini, giriş yaptıktan sonra indirim kodunu alabileceğini belirtmelisin.";

            string systemText = $"Sen GM Travel (Gezgin Seyahat) acentesinin canli destek asistanisin. Gamze ve Merve tarafindan tasarlanmis bir seyahat platformunda gorev aliyorsun. Nazik, yardimsever ve samimi bir dille cevap ver. Kullaniciya turlar, fiyatlar, lokasyonlar ve indirim kuponlari hakkinda yardimci ol. Cevaplarinda Turkceyi kusursuz kullan ve kisa, net yanitlar ver. Turlarimiz: 1) Kapadokya Balon Turu (3 Gün, Balon uçuşu dahil, 12.000 TL), 2) Likya Yolu Doğa Yürüyüşü (5 Gün, antik kentler gezisi, 18.000 TL), 3) Ege Mavi Yolculuk (7 Gün, Bodrum çıkışlı yat turu, 28.000 TL). {authInstruction}";
            string promptText = $"[TALİMATLAR: {systemText}]\n\nKullanıcı Sorusu: {userMessage}";

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = promptText }
                        }
                    }
                }
            };
            
            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
            string lastError = "";

            foreach (var model in models)
            {
                try
                {
                    var url = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";
                    var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseJson = await response.Content.ReadAsStringAsync();
                        using var doc = System.Text.Json.JsonDocument.Parse(responseJson);
                        
                        var text = doc.RootElement
                            .GetProperty("candidates")[0]
                            .GetProperty("content")
                            .GetProperty("parts")[0]
                            .GetProperty("text")
                            .GetString();

                        return text?.Trim();
                    }
                    else
                    {
                        var errStr = await response.Content.ReadAsStringAsync();
                        lastError = $"[Model: {model}, Hata: {response.StatusCode}]: {errStr}";
                    }
                }
                catch (Exception ex)
                {
                    lastError = $"[Model: {model} İstisna]: {ex.Message}";
                }
            }

            // Return the detailed log if all models fail
            return $"[Gemini API Hatası - Tüm Modeller Başarısız]: {lastError}";
        }

        [HttpGet]
        public async Task<IActionResult> Blogs()
        {
            var blogsList = await _context.Blogs.OrderByDescending(b => b.CreatedAt).ToListAsync();
            return View(blogsList);
        }

        [HttpGet]
        public async Task<IActionResult> BlogDetail(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }
    }
}
