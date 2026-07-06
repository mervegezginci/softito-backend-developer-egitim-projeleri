# 🍰 Pastane Sipariş ve Stok Takip Uygulaması (ADO.NET)

Bu proje, Softito Akademi eğitimi kapsamında ADO.NET veri erişim teknolojisi kullanılarak geliştirilmiş bir **Pastane Sipariş ve Stok Takip** Windows Forms uygulamasıdır.

## 🛠️ Kullanılan Teknolojiler

- **Programlama Dili:** C#
- **Veri Erişim Teknolojisi:** ADO.NET (`SqlConnection`, `SqlCommand`, `SqlDataReader`, `SqlDataAdapter`)
- **Veritabanı:** MS SQL Server (`softPastane` veritabanı)
- **Arayüz:** Windows Forms

## 🌟 Temel Özellikler

- **Dinamik Sipariş Giriş Arayüzü (`Siparisler.cs`):** Form üzerindeki `FlowLayoutPanel` yapısı kullanılarak veritabanındaki ürünlerin dinamik CheckBox ve adet seçimi için NumericUpDown kontrolleriyle otomatik listelenmesi. Sipariş verilirken toplam tutar ve adetin dinamik olarak hesaplanması.
- **Ürün & Menü Yönetimi (`Urunler.cs`):** Pastane menüsündeki ürünlerin (pasta, börek, içecekler vb.) eklenmesi, stok güncellenmesi ve fiyatlandırılması.
- **Müşteri Kayıt Sistemi (`Musteriler.cs`):** Müşteri telefon, adres ve sipariş geçmişinin takibi.
- **Satış Raporları (`Rapor.cs`):** Belirli tarih aralıklarına göre satılan ürün miktarlarının ve elde edilen cironun analiz edilmesi.

## 🗄️ Veritabanı Yapısı

Uygulama, SQL Server üzerindeki `softPastane` veritabanını kullanır:
- **Kullanici:** Sistem giriş yetkilerini barındırır.
- **Urunler:** Ürün bilgileri ve mevcut stok durumlarını tutar.
- **Musteri:** Sipariş veren müşteri bilgilerini içerir.
- **Siparis:** Alınan siparişlerin detaylarını, tarihini ve toplam fiyatını kaydeder.

## 📸 Ekran Görüntüleri

Uygulamaya ait arayüz ekran görüntüleri aşağıda yer almaktadır:

### 1. Giriş ve Kayıt Ekranı (`Giris.cs`)
Sisteme güvenli giriş yapmayı veya yeni kullanıcı kaydetmeyi sağlayan giriş ekranı:
![Giriş Yap ve Kayıt Ol](images/giris_ekrani.png)

### 2. Ürün Yönetimi Ekranı (`Urunler.cs`)
Menüdeki ürünlerin (ekler, baklava, börek vb.) listelenmesi, yeni ürün ekleme, güncelleme ve silme işlemlerinin yapıldığı arayüz:
![Ürün Yönetimi](images/urun_yonetimi.png)

### 3. Müşteri Yönetimi Ekranı (`Musteriler.cs`)
Müşteri kaydetme, bilgilerini güncelleme, silme ve arama işlemlerinin yapıldığı arayüz:
![Müşteri Yönetimi](images/musteri_yonetimi.png)

### 4. Sipariş Yönetimi Ekranı (`Siparisler.cs`)
FlowLayoutPanel yapısı sayesinde veritabanındaki aktif ürünlerin dinamik CheckBox ve NumericUpDown kontrolleriyle listelendiği, sipariş adeti ve toplam tutarının dinamik olarak hesaplanarak sisteme kaydedildiği ekran:
![Sipariş Yönetimi](images/siparis_yonetimi.png)

### 5. Raporlar Ekranı (`Rapor.cs`)
Toplam ürün sayısı, ortalama ürün fiyatı, en yüksek/düşük fiyatlı ürünler, toplam müşteri/sipariş sayıları ve toplam satış geliri gibi veritabanı istatistiklerinin listelendiği raporlama ekranı:
![Raporlar](images/raporlar.png)

