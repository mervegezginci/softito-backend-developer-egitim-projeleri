# 🎓 Proje 12 — Öğrenci Yönetim Sistemi (Web API & MVC)

ASP .NET CORE İLE DAPPER KÜTÜPHANESİ KULLANILARAK STORE PROCEDÜR VE ADO .NET TEKNOLOJİSİYLE API VE MVC PROJESİ YAPILMIŞTIR. PROJEDE KULLANICI ARAYÜZDEN API ARACILIĞI İLE CRUD İŞLEMLERİ, ARAMA ,PDF ,EXCEL,LOGİN ,REGİSTER İŞLEMLERİ GERÇEKLEŞTİRİLMİŞTİR.

---

## 📁 Proje Yapısı

Proje, istemci-sunucu (Client-Server) mimarisinde iki ana projeden oluşmaktadır:

1. **`ogrenciyonetimi_proje` (Web API):**
   * Veritabanı bağlantısı, iş mantığı, Dapper sorguları ve Stored Procedure çağrılarının yapıldığı backend servis katmanı.
   * Swagger UI ile API dokümantasyonu ve test imkanı sunar.
   * **JWT Bearer Token** tabanlı kimlik doğrulama (Authentication) uygular.
   * PDF (iTextSharp) ve Excel (EPPlus) rapor oluşturma servislerini barındırır.

2. **`ogrenciyonetimi_mvc` (MVC Web Arayüzü):**
   * Kullanıcı arayüzü (Frontend) katmanı.
   * API ile haberleşmek için özelleştirilmiş **`ApiService` (Typed HttpClient)** yapısını kullanır.
   * Kullanıcı oturumu için **Cookie-based Authentication** kullanır ve alınan JWT token değerini Cookie'de saklayarak API isteklerine otomatik ekler.

---

## 🗄️ Veritabanı Tasarımı (DB Schema)

Sistem ilişkisel bir veritabanı şeması üzerine kurulmuştur. Veritabanı scripti [SQL/database_setup.sql](./ogrenciyonetimi_proje/SQL/database_setup.sql) altında bulunmaktadır.

* **`Users` (Kullanıcılar):** Kimlik doğrulama, rol (Admin/User) ve hesap yönetimi bilgileri.
* **`Departments` (Bölümler):** Bölüm tanımları (Bilgisayar, Elektrik-Elektronik, Makine vb.).
* **`Students` (Öğrenciler):** Öğrenci bilgileri, sınıf seviyesi, durumu ve bölüm ilişkisi.
* **`Grades` (Notlar):** Öğrencilere ait ders notları ve sınav sonuçları.

---

## ⚙️ Teknolojiler & Kütüphaneler

* **Dapper ORM:** Hızlı, yüksek performanslı ve doğrudan Stored Procedure çağrılarıyla veri yönetimi.
* **iTextSharp:** Öğrenci listelerini dinamik olarak PDF belgesine dönüştürmek için.
* **EPPlus:** Excel (.xlsx) formatında öğrenci raporu çıktısı almak için.
* **JWT (JSON Web Token):** Web API isteklerinde güvenli kimlik denetimi için.
* **Cookie Authentication:** MVC tarafında oturum yönetimi için.

---

## 🛠️ Stored Procedure'lar (Veri Katmanı)

Tüm CRUD ve kimlik doğrulama işlemleri SQL tarafında güvenli ve performanslı Stored Procedure'lar ile yürütülür:
* `sp_Register` & `sp_Login` (Kullanıcı İşlemleri)
* `sp_GetAllStudents` & `sp_GetStudentById` (Öğrenci Listeleme)
* `sp_SearchStudents` (Dinamik Arama ve Filtreleme)
* `sp_InsertStudent`, `sp_UpdateStudent`, `sp_DeleteStudent` (CRUD)
* `sp_GetAllDepartments` (Bölüm Listesi)
* `sp_GetGradesByStudent` (Öğrenci Not Detayları)

---

## 📸 Ekran Görüntüleri

Sistemin güncel tasarımına ve premium Mor-İndigo temalı arayüzüne ait ekran görüntüleri aşağıdadır:

1. **Yönetici Giriş Arayüzü (Secure Login):**
   ![Giriş Ekranı](./images/login.png)

2. **Dinamik Raporlar ve Analizler Paneli (Interactive Dashboard):**
   ![Gösterge Paneli](./images/dashboard.png)

3. **Öğrenci Listesi ve Gelişmiş Arama Arayüzü (Students):**
   ![Öğrenci Listesi](./images/students_list.png)

4. **Yeni Öğrenci Ekleme ve Bilgi Formu (Add Student):**
   ![Öğrenci Kayıt Formu](./images/add_student.png)

5. **Akademik Bölüm Yönetimi Arayüzü (Departments):**
   ![Bölüm Yönetimi](./images/departments.png)

6. **Not Sistemi ve Sınav Kayıt Arayüzü (Grades):**
   ![Not Sistemi](./images/grades.png)

---

## 🚀 Kurulum ve Çalıştırma

### 1. Veritabanını Hazırlayın
SQL Server Management Studio (SSMS) veya başka bir SQL aracı yardımıyla [database_setup.sql](./ogrenciyonetimi_proje/SQL/database_setup.sql) dosyasını SQL Server'ınızda çalıştırarak veritabanı tablolarını, örnek verileri ve Stored Procedure'ları oluşturun.

### 2. Bağlantı Bilgilerini Düzenleyin
`ogrenciyonetimi_proje/appsettings.json` dosyası içerisindeki `ConnectionStrings:DefaultConnection` alanını kendi SQL Server adresinize göre düzenleyin.

### 3. API Projesini Başlatın
```bash
cd ogrenciyonetimi_proje
dotnet run
```
API varsayılan olarak `https://localhost:7101` adresinde ayağa kalkacaktır. Tarayıcıdan `https://localhost:7101/swagger` adresine giderek API dokümantasyonunu inceleyebilirsiniz.

### 4. MVC Projesini Başlatın
```bash
cd ../ogrenciyonetimi_mvc
dotnet run
```
MVC uygulaması varsayılan olarak `https://localhost:7227` adresinde çalışacaktır. Tarayıcınızdan giriş yaparak sistemi kullanabilirsiniz.
