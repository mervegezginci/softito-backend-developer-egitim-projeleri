# 📚 Kütüphane Otomasyonu, Yorum ve Ödünç Takip Sistemi (Web API & MVC)

Bu proje, Softito Akademi eğitimi kapsamında ASP.NET Core Web API ve MVC yapıları bir arada kullanılarak geliştirilmiş, **ASP.NET Core Identity** güvenlik entegrasyonuna sahip gelişmiş bir **Kütüphane Yönetim Portalı ve API** uygulamasıdır.

## 🛠️ Kullanılan Teknolojiler

- **Programlama Dili:** C#
- **Framework:** ASP.NET Core Web API & ASP.NET Core MVC (Net 6.0/8.0)
- **Güvenlik & Kimlik Doğrulama:** ASP.NET Core Identity (`ApplicationUser` ile genişletilmiş)
- **ORM Teknolojisi:** Entity Framework Core
- **Veritabanı:** MS SQL Server (`kutuphanedb` veya `LibraryDb` veritabanı)
- **Dokümantasyon:** Swagger UI

## 🗄️ Model / Entity Yapısı

Proje, kütüphane iş süreçlerini ve kullanıcı yetkilerini kapsayan geniş bir veritabanı şemasına sahiptir:

- **ApplicationUser:** ASP.NET Identity yapısını devralarak kütüphaneye üye olan kişilerin ve kütüphane görevlilerinin (admin) profil bilgilerini saklar.
- **Book (Kitap):** Kitap adı, ISBN, sayfa sayısı, yayın yılı, stok durumu ve Kategori (`CategoryId`) / Yazar (`AuthorId`) ilişkileri.
- **Author (Yazar):** Yazarların ad, soyad ve kısa biyografi bilgileri.
- **Category (Kategori):** Roman, Bilim, Tarih vb. kitap türleri.
- **Borrowing (Ödünç Alma):** Hangi üyenin (`UserId`), hangi kitabı (`BookId`) ne zaman aldığını, iade tarihini ve teslim durumunu takip eder.
- **Review (Kitap Yorumları):** Üyelerin kitaplara bıraktığı yorumları, puanları (`Rating`) ve onay durumlarını tutar.

## 🌟 Öne Çıkan Özellikler

- **ASP.NET Core Identity Yetkilendirmesi:** Rol bazlı erişim kontrolü (Öğrenci kitap talep edebilir, Kütüphaneci kitap ekleyebilir/güncelleyebilir).
- **Dashboard ve İstatistikler (`DashboardViewModel`):** Kütüphanedeki toplam kitap sayısı, aktif ödünçteki kitaplar ve en çok ödünç alınan kitapların (`MostBorrowedBookViewModel`) analitik gösterimi.
- **Yorum ve Puanlama Filtresi:** Kitap detay sayfasında onaylanmış kitap yorumlarının listelenmesi.
- **RESTful API Uçları:** Mobil veya harici uygulamaların kütüphane envanterine erişebilmesi için Swagger dokümantasyonlu API desteği.

## 📸 Ekran Görüntüleri

Uygulamaya ait arayüz ekran görüntüleri aşağıda yer almaktadır:

### 🏠 Kullanıcı & Kitap İşlemleri Arayüzü

<table width="100%">
  <tr>
    <td width="50%" align="center">
      <b>1. Ana Sayfa (Kitap Listesi & Karşılama)</b><br/>
      <img src="images/01_anasayfa.png" width="100%" alt="Ana Sayfa"/>
    </td>
    <td width="50%" align="center">
      <b>2. Kitap Detay Sayfası</b><br/>
      <img src="images/02_kitap_detay.png" width="100%" alt="Kitap Detayı"/>
    </td>
  </tr>
  <tr>
    <td width="50%" align="center">
      <b>3. Kitap Ödünç Alma Talebi (Modal)</b><br/>
      <img src="images/03_odunc_talep_modal.png" width="100%" alt="Ödünç Alma Modalı"/>
    </td>
    <td width="50%" align="center">
      <b>4. Ödünç Aldığım Kitaplar (Kullanıcı Paneli)</b><br/>
      <img src="images/04_kullanici_odunc_paneli.png" width="100%" alt="Ödünç Alınan Kitaplar"/>
    </td>
  </tr>
</table>

### 🛡️ Yönetici (Admin) Paneli & CRUD Yönetimi

<table width="100%">
  <tr>
    <td width="50%" align="center">
      <b>5. Yönetici Kontrol Paneli (Dashboard)</b><br/>
      <img src="images/05_admin_dashboard.png" width="100%" alt="Yönetici Dashboard"/>
    </td>
    <td width="50%" align="center">
      <b>6. Kitap Yönetim Paneli (CRUD)</b><br/>
      <img src="images/06_admin_kitap_yonetimi.png" width="100%" alt="Kitap Yönetimi"/>
    </td>
  </tr>
  <tr>
    <td width="50%" align="center">
      <b>7. Yazar Yönetim Paneli (CRUD)</b><br/>
      <img src="images/07_admin_yazar_yonetimi.png" width="100%" alt="Yazar Yönetimi"/>
    </td>
    <td width="50%" align="center">
      <b>8. Kategori Yönetim Paneli (CRUD)</b><br/>
      <img src="images/08_admin_kategori_yonetimi.png" width="100%" alt="Kategori Yönetimi"/>
    </td>
  </tr>
  <tr>
    <td width="50%" align="center">
      <b>9. Ödünç İşlemleri & Takip Paneli</b><br/>
      <img src="images/09_admin_odunc_yonetimi.png" width="100%" alt="Ödünç Yönetimi"/>
    </td>
    <td width="50%" align="center">
      <b>10. Kullanıcı & Rol Yönetimi</b><br/>
      <img src="images/10_admin_kullanici_yonetimi.png" width="100%" alt="Kullanıcı Yönetimi"/>
    </td>
  </tr>
</table>

### 🔌 Web API Dokümantasyonu

<table width="100%">
  <tr>
    <td width="100%" align="center">
      <b>11. Swagger Web API Dokümantasyonu</b><br/>
      <img src="images/11_swagger_api_dokumantasyonu.png" width="90%" alt="Swagger API Dokümantasyonu"/>
    </td>
  </tr>
</table>


