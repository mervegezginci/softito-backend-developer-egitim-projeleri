# 🎓 Proje 11 — Spor Kulübü & Antrenman Yönetim Sistemi (Web API & MVC)

ASP .NET CORE İLE DAPPER ORM KÜTÜPHANESİ KULLANILARAK STORE PROCEDÜR VE ADO .NET TEKNOLOJİSİYLE APİ VE MVC PROJESİ YAPILMIŞTIR. PROJEDE KULLANICI ARAYÜZDEN API ARACILIĞI İLE CRUD İŞLEMLERİ, ÜYE ARAMA, PDF VE EXCEL ÇIKTILARI, KULLANICI GİRİŞ/KAYIT İŞLEMLERİ VE MEMORY CACHE İLE HIZLANDIRILMIŞ GEÇİŞLER GERÇEKLEŞTİRİLMİŞTİR.

---

## 📁 Proje Yapısı

Proje, istemci-sunucu (Client-Server) mimarisinde iki ana projeden oluşmaktadır:

1. **`sporkulubu_proje` (Web API - Port: 7045):**
   * Veritabanı bağlantısı, iş mantığı, Dapper sorguları ve Stored Procedure çağrılarının yapıldığı backend servis katmanı.
   * **ASP.NET Core Identity** altyapısı ile kimlik doğrulama tabloları yönetimi.
   * **JWT Bearer Token** tabanlı kimlik doğrulama (Authentication) modeli.
   * PDF (iTextSharp) ve Excel (EPPlus) dinamik belge üretme servisleri.

2. **`sporkulubu_mvc` (MVC Web Arayüzü - Port: 7138):**
   * Kullanıcı dostu arayüz (Frontend) katmanı.
   * API ile haberleşmek için özelleştirilmiş **`ApiService` (Typed HttpClient)** yapısı.
   * Oturum yönetimi için **Cookie-based Authentication** kullanır ve alınan JWT token değerini Cookie'de saklayarak API isteklerine otomatik ekler.
   * Spor branşları verileri için **IMemoryCache** entegrasyonu.

---

## 🗄️ Veritabanı Tasarımı (DB Schema)

Sistem ilişkisel bir veritabanı şeması üzerine kurulmuştur. Veritabanı kurulum scriptleri [SQL/database_setup.sql](./sporkulubu_proje/SQL/database_setup.sql) ve Identity tabloları için [SQL/identity_setup.sql](./sporkulubu_proje/SQL/identity_setup.sql) altında bulunmaktadır.

* **`SportsBranches` (Spor Branşları):** Fitness, Tenis, Yüzme vb. branş bilgileri.
* **`Coaches` (Antrenörler):** Antrenör bilgileri ve branş ilişkisi.
* **`Members` (Kulüp Üyeleri):** Kulüp üyeleri, iletişim bilgileri, kayıt tarihleri ve aktiflik durumları.
* **`Trainings` (Antrenman Kayıtları):** Hangi üyenin hangi antrenörle ne zaman antrenman yaptığı, seans süresi ve seans ücreti.

---

## ⚙️ Teknolojiler & Kütüphaneler

* **Dapper ORM:** Hızlı, yüksek performanslı ve doğrudan Stored Procedure çağrılarıyla veri yönetimi.
* **iTextSharp:** Üye listelerini dinamik olarak PDF belgesine dönüştürmek için.
* **EPPlus:** Excel (.xlsx) formatında üye raporu çıktısı almak için.
* **JWT (JSON Web Token):** Web API isteklerinde güvenli kimlik denetimi için.
* **Cookie Authentication:** MVC tarafında oturum yönetimi için.
* **Memory Cache:** Sık erişilen branş verilerini önbelleğe alıp performans artırmak için.

---

## 🛠️ Stored Procedure'lar (Veri Katmanı)

Tüm CRUD işlemleri SQL tarafında güvenli ve performanslı Stored Procedure'lar ile yürütülür:
* `sp_GetBranches`, `sp_GetBranchById`, `sp_InsertBranch`, `sp_UpdateBranch`, `sp_DeleteBranch` (Branşlar)
* `sp_GetCoaches`, `sp_GetCoachById`, `sp_InsertCoach`, `sp_UpdateCoach`, `sp_DeleteCoach` (Antrenörler)
* `sp_GetMembers`, `sp_GetMemberById`, `sp_SearchMembers`, `sp_InsertMember`, `sp_UpdateMember`, `sp_DeleteMember` (Üyeler)
* `sp_GetTrainings`, `sp_GetTrainingById`, `sp_InsertTraining`, `sp_UpdateTraining`, `sp_DeleteTraining` (Antrenmanlar)

---

## 🚀 Kurulum ve Çalıştırma

### 1. Veritabanını Hazırlayın
SQL Server Management Studio (SSMS) veya başka bir SQL aracı yardımıyla sırasıyla [database_setup.sql](./sporkulubu_proje/SQL/database_setup.sql) ve [identity_setup.sql](./sporkulubu_proje/SQL/identity_setup.sql) dosyalarını SQL Server'ınızda çalıştırarak veritabanı tablolarını, örnek verileri ve Stored Procedure'ları oluşturun.

### 2. API Projesini Başlatın
```bash
cd sporkulubu_proje
dotnet run
```
API varsayılan olarak `https://localhost:7045` adresinde ayağa kalkacaktır. Tarayıcıdan `https://localhost:7045/swagger` adresine giderek API dokümantasyonunu inceleyebilirsiniz.

### 3. MVC Projesini Başlatın
```bash
cd ../sporkulubu_mvc
dotnet run
```
MVC uygulaması varsayılan olarak `https://localhost:7138` adresinde çalışacaktır. Tarayıcınızdan giriş yaparak sistemi kullanabilirsiniz.

---

## 💡 Çoklu Proje Çalıştırma (Multiple Startup Projects)
Çözüm dosyası (Solution) Visual Studio üzerinde açıldığında, yeşil **Başlat (Start)** butonuna tıklandığında **hem API hem de MVC** projesinin aynı anda çalışması için `sporkulubu.slnLaunch.user` dosyası hazır olarak eklenmiştir. F5 tuşuna basmanız durumunda her iki proje de otomatik olarak ayağa kalkacaktır.
