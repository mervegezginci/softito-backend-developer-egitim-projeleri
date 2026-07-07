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

1. **Yönetici Paneli - İstatistik Paneli (Dashboard):**
   ![İstatistik Paneli](./kuafor_ORMproje/wwwroot/img/01_admin_dashboard.png)
2. **Yönetici Paneli - Randevu Listesi:**
   ![Randevu Listesi](./kuafor_ORMproje/wwwroot/img/02_admin_randevular.png)
3. **Yönetici Paneli - Randevu Listesi (Güncel):**
   ![Randevu Listesi Güncel](./kuafor_ORMproje/wwwroot/img/03_admin_randevular_guncel.png)
4. **Yönetici Paneli - Müşteri Listesi:**
   ![Müşteri Listesi](./kuafor_ORMproje/wwwroot/img/04_admin_musteriler.png)
5. **Yönetici Paneli - Çalışan Uzman Listesi:**
   ![Çalışan Uzman Listesi](./kuafor_ORMproje/wwwroot/img/05_admin_calisanlar.png)
6. **Yönetici Paneli - Hizmet Kataloğu:**
   ![Hizmet Kataloğu](./kuafor_ORMproje/wwwroot/img/06_admin_hizmetler.png)
7. **Yönetici Paneli - Ödeme Kayıtları:**
   ![Ödeme Kayıtları](./kuafor_ORMproje/wwwroot/img/07_admin_odemeler.png)
8. **Yönetici Paneli - Gelişmiş Raporlama & Analiz:**
   ![Gelişmiş Raporlama](./kuafor_ORMproje/wwwroot/img/08_admin_raporlar.png)
9. **Müşteri Paneli - Anasayfa:**
   ![Müşteri Anasayfa](./kuafor_ORMproje/wwwroot/img/09_kullanici_anasayfa.png)
10. **Müşteri Paneli - Hizmetler Alanı:**
    ![Müşteri Hizmetler](./kuafor_ORMproje/wwwroot/img/10_kullanici_hizmetler.png)
11. **Müşteri Paneli - Randevu Talep Formu:**
    ![Randevu Talep Formu](./kuafor_ORMproje/wwwroot/img/11_kullanici_randevu_formu.png)

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
