-- OfisDB Veritabanı Oluşturma ve Örnek Veri Betiği

-- 1. Veritabanını oluştur (eğer yoksa)
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'OfisDB')
BEGIN
    CREATE DATABASE OfisDB;
END
GO

USE OfisDB;
GO

-- 2. Tabloları oluştur (eğer yoksa)

-- Categories Tablosu
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type in (N'U'))
BEGIN
    CREATE TABLE Categories (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(150) NOT NULL
    );
END
GO

-- Services Tablosu
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Services]') AND type in (N'U'))
BEGIN
    CREATE TABLE Services (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX) NULL,
        IconUrl NVARCHAR(500) NULL
    );
END
GO

-- Projects Tablosu
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Projects]') AND type in (N'U'))
BEGIN
    CREATE TABLE Projects (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX) NULL,
        ImageUrl NVARCHAR(500) NULL,
        CategoryId INT NULL,
        CONSTRAINT FK_Projects_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(ID) ON DELETE SET NULL
    );
END
GO

-- Contacts Tablosu
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Contacts]') AND type in (N'U'))
BEGIN
    CREATE TABLE Contacts (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(150) NOT NULL,
        Email NVARCHAR(150) NOT NULL,
        Message NVARCHAR(MAX) NULL,
        Date NVARCHAR(50) NULL
    );
END
GO

-- 3. Örnek Verileri Temizle ve Yeniden Yükle (Tablo bütünlüğü için)

-- İlişkili verilerin silinmesi
DELETE FROM Projects;
DELETE FROM Services;
DELETE FROM Contacts;
DELETE FROM Categories;

-- IDENTITY Değerlerini sıfırla
DBCC CHECKIDENT ('Projects', RESEED, 0);
DBCC CHECKIDENT ('Services', RESEED, 0);
DBCC CHECKIDENT ('Contacts', RESEED, 0);
DBCC CHECKIDENT ('Categories', RESEED, 0);

-- Categories Örnek Veriler
INSERT INTO Categories (Name) VALUES 
('Yönetici Ofisleri'),
('Açık Ofis & Ortak Çalışma Alanları'),
('Toplantı & Konferans Salonları'),
('Sosyal Alanlar & Dinlenme Bölümleri'),
('Karşılama & Resepsiyon Alanları');

-- Services Örnek Veriler
INSERT INTO Services (Title, Description, IconUrl) VALUES 
('İç Mimari Planlama', 'Ofisinizin metrekaresine en uygun, ergonomik ve verimli yerleşim planlarını hazırlıyoruz.', 'fas fa-pencil-ruler'),
('Aydınlatma Tasarımı', 'Çalışan motivasyonunu artıran, gözü yormayan modern ve akıllı aydınlatma çözümleri sunuyoruz.', 'fas fa-lightbulb'),
('Akustik Çözümler', 'Açık ofislerdeki ses kirliliğini önlemek amacıyla akustik panel ve yalıtım tasarımları yapıyoruz.', 'fas fa-volume-mute'),
('Mobilya Seçimi & Üretim', 'Ofisinizin kimliğine uygun, konforlu ve kaliteli mobilyaların tedarik ve üretim sürecini yönetiyoruz.', 'fas fa-couch'),
('Proje Yönetimi', 'Tasarım aşamasından anahtar teslim ana kadar olan tüm şantiye ve uygulama süreçlerini takip ediyoruz.', 'fas fa-tasks');

-- Projects Örnek Veriler
-- Categories ID eşleşmeleri: 1=Yönetici Ofisleri, 2=Açık Ofis, 3=Toplantı, 4=Sosyal, 5=Karşılama
INSERT INTO Projects (Title, Description, ImageUrl, CategoryId) VALUES 
('Minimalist Yönetici Ofisi', 'Ahşap ve metal detayların hakim olduğu, ferah ve prestijli yönetici süiti tasarımı.', 'https://images.unsplash.com/photo-1497366216548-37526070297c?auto=format&fit=crop&w=800&q=80', 1),
('Kreatif Açık Ofis Alanı', 'Renkli dinlenme köşeleri ve ortak çalışma masaları ile harmanlanmış dinamik çalışma ortamı.', 'https://images.unsplash.com/photo-1497215728101-856f4ea42174?auto=format&fit=crop&w=800&q=80', 2),
('Akıllı Toplantı Odası', 'Video konferans sistemleri ve akustik duvar panelleri ile donatılmış 12 kişilik toplantı salonu.', 'https://images.unsplash.com/photo-1431540015161-0bf868a2d407?auto=format&fit=crop&w=800&q=80', 3),
('Premium Karşılama Alanı', 'Mermer resepsiyon bankosu ve modern bekleme koltukları ile kurumsal karşılama tasarımı.', 'https://images.unsplash.com/photo-1497366811353-6870744d04b2?auto=format&fit=crop&w=800&q=80', 5);

-- Contacts Örnek Veriler
INSERT INTO Contacts (Name, Email, Message, Date) VALUES 
('Ahmet Yılmaz', 'ahmet@gmail.com', 'Yeni açacağımız 150 metrekarelik yazılım ofisi için iç mimari tasarım teklifi almak istiyoruz.', '05.07.2026 10:15'),
('Elif Kaya', 'elif@kaya.co', 'Toplantı odalarımızın akustik yalıtımı ile ilgili görüşebilir miyiz?', '05.07.2026 11:30'),
('Can Demir', 'can.demir@outlook.com', 'Mobilya kataloglarınızı ve güncel fiyat listenizi gönderebilir misiniz?', '05.07.2026 14:02');

GO
