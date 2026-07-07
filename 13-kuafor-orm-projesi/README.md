# ✂️ Proje 13 — Kuaför Salonu Randevu ve Ödeme Yönetim Sistemi

Bu proje, 3 Katmanlı Mimari (N-Tier Architecture), **Repository ve Unit of Work** tasarım kalıpları kullanılarak geliştirilmiş, rol tabanlı kimlik doğrulama, önbellekleme ve loglama içeren kapsamlı bir **Kuaför Salonu Randevu ve Ödeme Yönetim Sistemi**'dir.

---

## 📁 Katmanlı Mimari Yapısı

Proje temiz kod prensiplerine ve gevşek bağlılığa (loose coupling) uygun olarak 3 ayrı katmanda yapılandırılmıştır:

1. **`kuafor_ORMproje.Model` (Varlık Katmanı):**
   * Veritabanı tablolarının modellerini (`Appointment`, `Customer`, `Employee`, `Payment`, `Service`) ve Identity sınıflarını (`ApplicationUser`, `ApplicationRole`) barındırır.
2. **`kuafor_ORMproje.Data` (Veri Erişim Katmanı):**
   * `DbContext` yapılandırması, EF Core Migration dosyaları, generic `IRepository` / `Repository` ve veri yönetimini tek bir transaction altında toplayan `IUnitOfWork` / `UnitOfWork` sınıflarını barındırır.
3. **`kuafor_ORMproje` (Sunum Katmanı):**
   * MVC denetleyicileri (Controllers), kullanıcı arayüzü sayfaları (Views), Admin Paneli ve iş mantığı servislerinin yer aldığı katmandır.

---

## 💎 Öne Çıkan Gelişmiş Özellikler

* **Repository & Unit of Work Kalıpları:** Veritabanı işlemleri doğrudan Context çağrıları yerine generic repository katmanları üzerinden soyutlanarak yönetilir. Tüm işlemler transaction güvenliği için `IUnitOfWork.Save()` ile tek seferde veritabanına yansıtılır.
* **Rol Tabanlı Yetkilendirme (Identity):** Sitede normal müşteriler (User) ve sistem yöneticileri (Admin) olmak üzere iki rol tanımlıdır. Yönetici sayfaları `[Authorize(Roles = "Admin")]` ile korunurken, müşteri randevu alma sayfası genel erişime açıktır.
* **Akıllı Randevu Alma Arayüzü:** 
  * Giriş yapan kullanıcılar için kişisel bilgi (ad, soyad, e-posta) doldurma zorunluluğu kaldırılmıştır; sistem bu bilgileri otomatik olarak eşleştirir.
  * Müşterinin form üzerinden gireceği güncel telefon numarasıyla veritabanındaki kayıtları dinamik olarak senkronize edilir.
  * Randevu alma sayfasında dikkati dağıtmamak için anasayfadaki büyük kaydırıcılar (Hero) `ViewData["HideHero"]` mekanizmasıyla otomatik gizlenir.
* **Otomatik Ücret Hesaplama (Ödemeler):** Yönetim panelinden yeni bir ödeme kaydı oluşturulurken, randevu seçildiği anda randevuya bağlı hizmetin ücreti JavaScript ile otomatik olarak okunur ve tutar alanına yansıtılır (elle yazma hatasını önler).
* **Performans & İzleme (Cache & Log):**
  * Finansal ve istatistiksel raporlar `IMemoryCache` (5 dakika kayan, 1 saat mutlak süre) ile önbelleğe alınarak veritabanı yükü azaltılmıştır.
  * Raporlara erişim, `ILogger` servisiyle arka planda loglanmaktadır.
* **İstisnai Kontroller:** İptal edilen randevular ödeme ekranında listelenmez.

---

## 🎨 Arayüz Tasarımları

* **Admin Paneli:** Sneat Bootstrap 5 Admin Şablonu (Grafikler, KPI Kartları, CRUD listeleri, Raporlar ve Sistem Günlükleri).
* **Müşteri Arayüzü:** Salone Modern Güzellik & Kuaför Şablonu (Hizmetler kataloğu, fiyat listesi ve hızlı randevu formu).

---

## 📸 Proje Ekran Görüntüleri

Ekran görüntülerine ve detaylı kullanım adımlarına projenin kök dizinindeki [README.md](../README.md#-ekran-görüntüleri) dosyasından ulaşabilirsiniz.

---

## 🚀 Çalıştırma Adımları

1. Yerel SQL Server bağlantı dizesini (`ConnectionStrings:DefaultConnection`) `kuafor_ORMproje/appsettings.json` dosyasında güncelleyin.
2. NuGet paketlerini geri yükleyin ve EF Core migration'larını veritabanına uygulayın:
   ```bash
   dotnet ef database update --project kuafor_ORMproje.Data --startup-project kuafor_ORMproje
   ```
3. Projeyi çalıştırın:
   ```bash
   dotnet run --project kuafor_ORMproje
   ```
4. Sistem başlangıçta otomatik olarak bir yönetici hesabı oluşturacaktır:
   * **E-posta:** `merve@gmail.com`
   * **Şifre:** `Merve123!` (Admin Yetkili)
