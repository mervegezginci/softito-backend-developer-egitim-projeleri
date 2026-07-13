# 🌍 GM Seyahat Acentesi (Travel Booking System)

**GM Seyahat Acentesi**, modern seyahat acentelerinin tur listeleme, arama, rezervasyon, blog yönetimi ve üyelik süreçlerini dijitalleştirmek amacıyla geliştirilmiş, **ASP.NET Core MVC** mimarisine sahip kapsamlı bir web uygulamasıdır. 

> 💡 **Proje Adı Hakkında:** Projede kullanılan "GM" ismi, geliştiriciler **Gamze Türker** ve **Merve Gezginci**'nin baş harflerini temsil etmektedir.

Projede yüksek performans için **Dapper** ve **IMemoryCache** entegrasyonu, güvenli kimlik doğrulama için **ASP.NET Core Identity**, raporlama için ise **PDFsharp (Fatura Oluşturma)** ve **ClosedXML (Excel Raporu)** kullanılmıştır.

---

## 👥 Proje Ekibi (Geliştiriciler)
Bu proje, aşağıdaki ekip tarafından ortak bir çalışma olarak tasarlanmış ve kodlanmıştır:
- **Gamze Türker** - [GitHub Profili](https://github.com/gamzeturker) (Kendi profil linkinizi ekleyebilirsiniz)
- **Merve Gezginci** - [GitHub Profili](https://github.com/mervegezginci) (Kendi profil linkinizi ekleyebilirsiniz)

---

## 📸 Ekran Görüntüleri (Screenshots)

*Aşağıdaki alanlara projenizin ekran görüntülerini ekleyerek GitHub'da görsel bir şölen sunabilirsiniz.*

| Ana Sayfa | Tur Detay & Yorumlar |
| :---: | :---: |
| ![Ana Sayfa](docs/images/home.png) | ![Tur Detay](docs/images/tour_detail.png) |

| Rezervasyon & Fatura (PDF) | Yönetici (Admin) Paneli |
| :---: | :---: |
| ![Fatura PDF](docs/images/invoice_pdf.png) | ![Admin Panel](docs/images/admin_dashboard.png) |

| Önbellek & Log Yönetimi | Kullanıcı Paneli |
| :---: | :---: |
| ![Cache Yönetimi](docs/images/cache_management.png) | ![Kullanıcı Paneli](docs/images/user_dashboard.png) |

---

## 🛠️ Kullanılan Teknolojiler ve Kütüphaneler

### Backend & Veritabanı
- **.NET 8.0 / C#** (ASP.NET Core MVC Framework)
- **Entity Framework Core (EF Core)**: Veri tabanı CRUD işlemleri, ilişki yönetimi ve Migrations için.
- **Dapper (Micro-ORM)**: Popüler turlar, kategoriler ve anlık sorgulamalar gibi kritik listeleme işlemlerinde maksimum veri tabanı performansı elde etmek için.
- **SQL Server**: İlişkisel veri tabanı yönetim sistemi.
- **IMemoryCache**: Sıkça kullanılan ve az değişen verilerin (kategoriler, marquee tur başlıkları, popüler turlar vb.) RAM'de tutularak sunucu yanıt sürelerinin minimize edilmesi için.
- **ASP.NET Core Identity**: Rol tabanlı yetkilendirme (Admin, User), şifreli üyelik, oturum yönetimi ve güvenlik çerezleri (Cookie Authentication) için.

### Raporlama & Yardımcı Araçlar
- **PDFsharp**: Kullanıcılara rezervasyon sonrasında otomatik Türkçe karakter destekli fatura ve seyahat belgesi (PDF) oluşturmak için.
- **ClosedXML**: Admin panelinde rezervasyon ve satış raporlarını Excel (.xlsx) formatında dışa aktarmak için.
- **LogService (Sistem Günlüğü)**: Yönetici paneli hareketleri, kritik silme/güncelleme işlemleri ve önbellek temizleme hareketlerinin IP adresleriyle birlikte SQL Server'a kaydedilmesi için.

### Frontend
- **Bootstrap 5** & **Vanilla CSS**
- **HTML5 & CSS3 & JavaScript (jQuery)**
- **SweetAlert2**: Dinamik, şık ve kullanıcı dostu bildirim/onay pencereleri için.
- **FontAwesome**: Modern ve vektörel ikon kütüphanesi.

---

## 🏢 Proje Mimari Yapısı (N-Tier Architecture)

Proje, temiz kod (clean code) prensiplerine uygun olarak 3 ana katmandan oluşmaktadır:

1. **`seyahat_projesi` (Sunum Katmanı - Web/MVC)**
   - **Controllers**: MVC ve Web API Controller sınıflarını barındırır.
   - **Views**: Kullanıcı arayüzleri ve Admin paneli görünümlerini içerir.
   - **Areas**: `Admin` ve `User` olarak ikiye ayrılmış özel panelleri içerir.
   - **Services**: PDF font çözücü (`MyFontResolver`) ve veritabanı loglama (`LogService`) sınıflarını barındırır.
   - **wwwroot**: CSS, JS, kütüphaneler ve statik dosyaların tutulduğu klasördür.

2. **`seyahat_projesi.Data` (Veri Erişim Katmanı)**
   - **ApplicationDbContext**: Entity Framework Core veri tabanı bağlamı.
   - **Repository & IRepository**: Unit of Work tasarımı ile EF Core sorgularını soyutlayan katman.
   - **DapperRepository**: Dapper sorgularını yöneten performans katmanı.
   - **DbInitializer & DbSeeder**: İlk kurulumda otomatik veri tabanı oluşturma, migration uygulama ve başlangıç (seed) verilerini (her tablo için 20'şer kayıt) yükleyen mekanizma.

3. **`seyahat_projesi.Model` (Varlık / Entity Katmanı)**
   - Veri tabanındaki tabloları temsil eden sınıflar (`Tour`, `Category`, `Booking`, `Payment`, `Review`, `Coupon`, `Blog`, `SystemLog`, `Guide` vb.) ve ViewModel sınıfları yer alır.

---

## 🌟 Öne Çıkan Özellikler

### 👥 Kullanıcı ve Ziyaretçi Deneyimi
- **Gelişmiş Arama Motoru**: Lokasyon, tur başlığı, kategori ve minimum gün süresine göre dinamik filtreleme.
- **Rezervasyon Sistemi**: Tur kapasite kontrolü, kişi sayısına göre dinamik fiyat hesaplama ve ödeme adımları.
- **Promosyon Kodu (Kupon)**: Sepette aktif kupon kodu kullanarak indirim kazanma.
- **Ödeme Simülasyonu**: Kredi Kartı ve EFT/Havale seçenekleriyle hızlı rezervasyon tamamlama.
- **Puanlama ve Değerlendirme**: Turlara yorum yapma ve 1-5 yıldız arası puan verme.
- **Dinamik Blog**: Seyahat hikayeleri okuma ve detaylarını inceleme.
- **Bilet / Fatura İndirme**: Kullanıcı panelinden geçmiş rezervasyonlar için PDF formatında şık bir fatura indirme.

### 👑 Yönetici (Admin) Paneli (`/Admin`)
- **İstatistik Kartları**: Toplam Gelir, Toplam Rezervasyon, Aktif Tur ve Kayıtlı Üye sayılarının anlık gösterimi.
- **Tur Yönetimi**: Yeni tur rotası oluşturma, rehber ve kategori atama, fiyat/kapasite/tarih belirleme (Pasife alma ile *Soft Delete* desteği).
- **Rezervasyon Yönetimi**: Gelen rezervasyon taleplerini inceleme, onaylama veya iptal etme. İptal durumunda tur kapasitesinin otomatik iade edilmesi.
- **Kullanıcı Yönetimi**: Sisteme yeni kullanıcı ekleme, mevcut kullanıcıyı düzenleme, üyelik durumunu askıya alma (status) veya rol değiştirme.
- **Kupon ve Promosyon Yönetimi**: İndirim kodu tanımlama, oran veya tutar bazlı indirim tipi seçme, son kullanma tarihi belirleme.
- **Yorum Moderasyonu**: Turlara yapılan kullanıcı yorumlarını inceleme ve uygunsuz içerikleri silme.
- **İletişim Portalı**: Ziyaretçilerin gönderdiği iletişim mesajlarını okundu/okunmadı olarak işaretleme veya silme.
- **Sistem Önbellek Yönetimi**: Sistemdeki aktif önbellek (Cache) durumunu izleme ve tek tıkla önbelleği temizleyerek güncel verilerin yüklenmesini sağlama.
- **Sistem Günlükleri (Audit Logs)**: Adminlerin yaptığı tüm kritik işlemlerin (Logins, CRUD, Cache Clear vb.) detaylı tarih, işlem tipi ve IP adresi bilgisiyle listelenmesi.

---

## 🔌 Veritabanı ve Seeding İşlemleri

Proje ilk ayağa kalktığında **EF Core Migrations** otomatik olarak çalışır ve veritabanı tablolarını SQL Server üzerinde oluşturur.

- **`DbInitializer.cs`**: Sistem yöneticisi (`admin@gezgin.com` / Şifre: `Admin123*`) ve standart test kullanıcısını otomatik olarak oluşturur. Ayrıca başlangıç rollerini tanımlar.
- **`DbSeeder.cs`**: Test süreçlerini kolaylaştırmak amacıyla turlar, rehberler, yorumlar, bloglar ve kuponlar dahil olmak üzere her tablo için **20 adet gerçekçi örnek veri** üretir.
- **SQL Dosyaları**:
  - `seed_all_20.sql`: Veritabanını manuel olarak doldurmak isteyenler için SQL scripti.
  - `update_tour_images.sql`: Turların görsellerini daha estetik ve gerçek seyahat görselleriyle güncellemek için hazırlanmış SQL scripti.

---

## 🚀 Kurulum Adımları

Projeyi kendi bilgisayarınızda çalıştırmak için aşağıdaki adımları takip edebilirsiniz:

1. **Projeyi İndirin:**
   ```bash
   git clone <github-repository-url>
   ```

2. **Veritabanı Bağlantısını Yapılandırın:**
   `seyahat_projesi` klasöründeki `appsettings.json` dosyasını açın ve `DefaultConnection` bağlantı dizesini kendi SQL Server bilgilerinize göre güncelleyin:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=SeyahatDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```

3. **Gerekli NuGet Paketlerini Yükleyin:**
   Proje dizininde terminali açarak paketleri restore edin:
   ```bash
   dotnet restore
   ```

4. **Veritabanını Oluşturun ve Verileri Yükleyin:**
   Proje ilk kez çalıştırıldığında veri tabanı otomatik olarak oluşturulacak ve seed verileri yüklenecektir. Ancak manuel olarak yapmak isterseniz Package Manager Console veya Terminal üzerinden şu komutları çalıştımak isteyebilirsiniz:
   ```bash
   dotnet ef database update
   ```

5. **Projeyi Çalıştırın:**
   ```bash
   dotnet run
   ```
   Uygulama çalıştıktan sonra tarayıcınızda `http://localhost:5000` (veya terminalde belirtilen port) adresine giderek projeyi test edebilirsiniz.

### Giriş Bilgileri (Default Test Hesapları)
- **Yönetici Girişi (Admin):**
  - **E-posta:** `admin@gezgin.com`
  - **Şifre:** `Admin123*`
- **Kullanıcı Girişi (User):**
  - **E-posta:** `uye@gezgin.com`
  - **Şifre:** `Uye123*`

---

## 📝 Lisans
Bu proje eğitim ve kişisel gelişim amacıyla Gamze Türker ve Merve Gezginci tarafından geliştirilmiştir.
