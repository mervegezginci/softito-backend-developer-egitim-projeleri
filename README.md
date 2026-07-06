# 🚀 Softito 2026 — Backend Developer Eğitim Projeleri

Bu repo, **Softito Akademi Backend Developer** eğitim sürecinde geliştirilen 12 adet C# ve .NET tabanlı backend projesini içermektedir. Projeler; temel veritabanı bağlantılarından (ADO.NET) başlayarak, modern ORM araçlarına (Dapper, Entity Framework Core), katmanlı mimarilerden (N-Layer), RESTful Web API servislerine ve gelişmiş raporlama panellerine kadar uzanan geniş bir teknoloji yelpazesini kapsamaktadır.

---

## 📁 Proje Listesi

### 📗 Proje 1 — [adonet_hastaneproje](./01-adonet-hastane-projesi/)
**Teknoloji:** Windows Forms · ADO.NET · SQL Server · Stored Procedure

ADO.NET veri erişim mimarisi ve SQL Server Saklı Yordamları (Stored Procedures) kullanılarak geliştirilmiş Windows Forms tabanlı **Hastane Yönetim Sistemi**. Doktor, hasta, poliklinik ve randevu kayıtlarının yönetimi parametreli sorgularla güvenli bir şekilde gerçekleştirilir.

* **Öne Çıkan Özellikler:**
  - SQL Server Stored Procedure entegrasyonu (CRUD ve Arama)
  - ADO.NET (`SqlConnection`, `SqlCommand`, `SqlDataReader`)
  - Parametreli SQL sorguları (SQL Injection Koruması)
  - Kullanıcı yetkilendirme (Giriş Yap / Üye Ol)

---

### 📘 Proje 2 — [efdb_arac_kiralama_proje](./02-efdb-arac-kiralama-projesi/)
**Teknoloji:** Windows Forms · EF Core Database First · SQL Server

Entity Framework Core Database First yaklaşımıyla geliştirilmiş **Araç Kiralama Takip Sistemi**. Önceden tasarlanan SQL Server veritabanı şeması `Scaffold` araçlarıyla modellere dönüştürülmüş ve araç müsaitlik durumları, kiralama sözleşmeleri ve müşteri kayıtları EF Core ORM yetenekleriyle yönetilmiştir.

* **Öne Çıkan Özellikler:**
  - EF Core Database First & Scaffolding
  - Linq-to-Entities ile araç filtreleme ve listeleme
  - Fluent API veritabanı ilişkilendirmeleri
  - Müşteri ve araç bazlı kiralama geçmişi takibi

---

### 📙 Proje 3 — [adonetPastaneProje](./03-adonet-pastane-projesi/)
**Teknoloji:** Windows Forms · ADO.NET · SQL Server

ADO.NET bağlantılı veri tabanı modeli üzerine inşa edilmiş **Pastane Sipariş ve Stok Takip Sistemi**. Ham madde envanter kontrolü, ürün reçeteleri (maliyet hesabı), sipariş fişleri ve anlık stok durumları Windows Forms arayüzünden veritabanına doğrudan yansıtılır.

* **Öne Çıkan Özellikler:**
  - ADO.NET veri komutları ve veri tablosu (DataTable) haritalama
  - Kritik stok uyarıları ve ham madde takibi
  - Sipariş ve satış tutarı hesaplama mantığı

---

### 📕 Proje 4 — [cafe_codefirstmvcproje](./04-cafe-codefirst-mvc-projesi/)
**Teknoloji:** ASP.NET Core MVC · EF Core Code-First · SQL Server · Chart.js

Entity Framework Core Code-First yaklaşımıyla veritabanı modellenmiş **Kafe Sipariş Yönetimi ve Müşteri Raporlama Sistemi**. Admin paneline entegre edilen dual-axis Chart.js grafik modülleri ve KPI metrik kartları ile ürün kategorileri ve müşteri yorum puanı dağılımları görselleştirilmiştir.

* **Öne Çıkan Özellikler:**
  - EF Core Code-First & Migration
  - Raporlar ekranında Chart.js entegrasyonu
  - Excel/CSV biçiminde veri indirme motoru
  - Strongly-typed (`ReportsViewModel`) raporlama yapısı

---

### 🏠 Proje 5 — [emlak_dbfirstmvcproje](./05-emlak-dbfirst-mvc-projesi/)
**Teknoloji:** ASP.NET Core MVC · EF Core Database First · SQL Server · Chart.js

Database First yaklaşımıyla scaffold edilmiş modeller üzerine kurulu **Rentiz Emlak İlan & Raporlama Portalı**. Kullanıcı tarafında çoklu parametreli ilan arama motoru ve ilan detay sayfaları, admin tarafında ise Chart.js grafik modülleri ile zenginleştirilmiş analiz paneli ve CRUD arama formları sunar.

* **Öne Çıkan Özellikler:**
  - EF Core Database First & Dinamik Arama Motoru
  - Chart.js ile Fiyat Trendi, Şehir Dağılımı ve İlan Türü grafikleri
  - Fotoğraflı Emlak Danışmanı (Realtor) iletişim kartı entegrasyonu
  - Admin CRUD sayfalarında sunucu taraflı arama kutuları

---

### 🛒 Proje 6 — [stok_codefirstmvcproje](./06-stok-codefirst-mvc-projesi/)
**Teknoloji:** ASP.NET Core MVC · EF Core Code-First · SQL Server

Ürün envanteri ve depo hareketlerini yönetmek üzere tasarlanmış **Stok Takip Sistemi**. Code-First yaklaşımı kullanılarak ürünler, kategoriler ve tedarikçiler ilişkilendirilmiş; stok giriş/çıkış hareketleri ile kritik stok seviyeleri kontrol altında tutulmuştur.

* **Öne Çıkan Özellikler:**
  - EF Core Code-First & İlişkisel Veritabanı Modeli
  - Kritik stok uyarı limiti filtreleme
  - Tedarikçi bazlı ürün envanteri raporlama

---

### 🏢 Proje 7 — [OfisTasarim_RazorPagesProje](./07-ofis-tasarim-razor-pages-projesi/)
**Teknoloji:** ASP.NET Core Razor Pages · EF Core · SQL Server

ASP.NET Core Razor Pages mimarisi kullanılarak geliştirilmiş **Ofis ve Toplantı Odası Rezervasyon Portalı**. Sayfa odaklı (Page-focused) Razor yapısı sayesinde modüler rezervasyon yönetimi ve tarih aralığı bazlı müsaitlik filtrelemesi sunar.

* **Öne Çıkan Özellikler:**
  - ASP.NET Core Razor Pages mimarisi ile temiz sayfa yapıları
  - Tarih aralığı bazlı oda doluluk algoritması
  - Sayfa tabanlı hızlı form ve rezervasyon CRUD işlemleri

---

### 🎓 Proje 8 — [kurs_javascriptcodefirstproje](./08-kurs-javascript-codefirst-projesi/)
**Teknoloji:** ASP.NET Core MVC · EF Core Code-First · SQL Server

JavaScript eğitimleri ve yazılım geliştirme kursları için hazırlanmış **Eğitim & Kurs Portalı**. Kurs kategorileri, eğitmen profilleri, kurs içerikleri ve üye kayıt modüllerinin yönetimi Code-First veritabanı altyapısıyla çalışmaktadır.

* **Öne Çıkan Özellikler:**
  - Kurs ve eğitmen ilişkisel veritabanı tasarımı
  - Arama motoru ve seviye bazlı kurs filtreleme
  - Eğitmen & Kurs yönetim modülleri

---

### 🐱 Proje 9 — [pet_apiproje](./09-pet-api-projesi/)
**Teknoloji:** ASP.NET Core Web API · EF Core · SQL Server · Swagger

RESTful API standartlarında tasarlanmış **Hayvan Sahiplendirme ve Veteriner Klinik Yönetim API'si**. Dış istemcilerin tüketebileceği HTTP GET, POST, PUT, DELETE uç noktalarını barındırır ve Swagger entegrasyonu ile test edilebilir.

* **Öne Çıkan Özellikler:**
  - RESTful Web API Standartları (Controller-based)
  - Swagger UI Entegrasyonu & API Dokümantasyonu
  - İlişkili tablolarda JSON döngülerini (circular reference) engellemek için serileştirme ayarları

---

### 📚 Proje 10 — [kutuphane_apiproje](./10-kutuphane-api-projesi/)
**Teknoloji:** ASP.NET Core Web API · EF Core · SQL Server · Swagger

Kütüphane Kitap, Üye ve Ödünç Takip Sistemi **Web API Servisi**. Kitapların ödünç verilme süreleri, üye geçmişleri ve popüler kitap istatistikleri API uç noktaları üzerinden yönetilir ve dış platformlara veri servisi sağlar.

* **Öne Çıkan Özellikler:**
  - ASP.NET Core Web API & DTO (Data Transfer Object) kullanımı
  - Ödünç alma süre kontrolü ve gecikme cezası hesabı mantığı
  - Dış servis entegrasyonu için optimize edilmiş JSON çıktıları

---

### 🛠️ Proje 11 — Yakında Eklenecek
**Teknoloji:** -
* **Durum:** Yapım Aşamasında 🚧

---

### 🛠️ Proje 12 — Yakında Eklenecek
**Teknoloji:** -
* **Durum:** Yapım Aşamasında 🚧

---

### ✂️ Proje 13 — [kuafor_ORMproje](./13-kuafor-orm-projesi/)
**Teknoloji:** ASP.NET Core MVC · EF Core ORM · SQL Server

Kuaför salonları için randevu ve hizmet yönetim sistemi. Müşterilerin çalışan müsaitlik takvimine göre randevu oluşturması, hizmet kategorilerinin yönetimi ve admin onay mekanizması EF Core ORM altyapısıyla modellenmiştir.

* **Öne Çıkan Özellikler:**
  - EF Core ORM Randevu takvim modellemesi
  - Müşteri randevu oluşturma ve onay süreçleri
  - Hizmet & Fiyat listesi CRUD yönetimi

---



## 🛠️ Kullanılan Teknolojiler Matrisi

| Teknoloji | Kullanıldığı Projeler |
| :--- | :--- |
| **ASP.NET Core MVC** | Proje 4, 5, 6, 8, 13 |
| **ASP.NET Core Razor Pages** | Proje 7 |
| **ASP.NET Core Web API** | Proje 9, 10 |
| **Windows Forms** | Proje 1, 2, 3 |
| **EF Core Code-First** | Proje 4, 6, 8 |
| **EF Core Db-First** | Proje 2, 5 |
| **EF Core (Genel/ORM)** | Proje 7, 9, 10, 13 |
| **ADO.NET** | Proje 1, 3 |
| **Chart.js (Raporlama)** | Proje 4, 5 |
| **SQL Server** | Tüm projeler |

---

## ⚙️ Gereksinimler

- **.NET SDK:** .NET 8.0 veya .NET 9.0 SDK
- **Veritabanı:** MS SQL Server / LocalDB
- **IDE:** Visual Studio 2022+ veya Visual Studio Code (C# Dev Kit eklentisi yüklü)

---

## 🚀 Çalıştırma

Her proje bağımsız olarak kendi klasörü altındaki `.sln` veya `.csproj` dosyası üzerinden çalıştırılabilir. 

Terminal üzerinden çalıştırmak için:
```bash
# Proje dizinine gidin (Örn: Proje 4)
cd "04-cafe-codefirst-mvc-projesi/cafe_codefirstmvcproje"

# Bağımlılıkları geri yükleyin ve projeyi çalıştırın
dotnet run
```
