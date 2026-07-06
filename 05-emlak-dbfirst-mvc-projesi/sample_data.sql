-- Emlak İlan Portalı (RealEstateDb) Örnek Veri Ekleme Betiği
-- SQL Server Management Studio (SSMS) üzerinden "RealEstateDb" veritabanı seçilerek çalıştırılabilir.

USE RealEstateDb;
GO

-- 1. ÇAKIŞMALARI ÖNLEMEK İÇİN MEVCUT VERİLERİ İLİŞKİ SIRASINA GÖRE TEMİZLE
-- Önce bağımlı tabloyu (Properties) siliyoruz, ardından diğer tabloları temizliyoruz.
DELETE FROM Properties;
DELETE FROM Cities;
DELETE FROM PropertyTypes;
DELETE FROM Realtors;
GO

-- 2. ŞEHİRLER (Cities) EKLE
SET IDENTITY_INSERT Cities ON;
INSERT INTO Cities (Id, Name) VALUES 
(1, N'Sakarya'),
(2, N'İstanbul'),
(3, N'Ankara'),
(4, N'İzmir'),
(5, N'Antalya'),
(6, N'Bursa');
SET IDENTITY_INSERT Cities OFF;
GO

-- 3. İLAN TÜRLERİ (PropertyTypes) EKLE
SET IDENTITY_INSERT PropertyTypes ON;
INSERT INTO PropertyTypes (Id, TypeName) VALUES 
(1, N'Kiralık'),
(2, N'Satılık'),
(3, N'Günlük Kiralık');
SET IDENTITY_INSERT PropertyTypes OFF;
GO

-- 4. GAYRİMENKUL DANIŞMANLARI (Realtors) EKLE
SET IDENTITY_INSERT Realtors ON;
INSERT INTO Realtors (Id, NameSurname, Phone, Email, ImageUrl) VALUES 
(1, N'Ahmet Yılmaz', N'0555 111 2233', N'ahmet@emlak.com', N'https://images.pexels.com/photos/220453/pexels-photo-220453.jpeg?auto=compress&cs=tinysrgb&w=150'),
(2, N'Merve Demir', N'0555 222 3344', N'merve@emlak.com', N'https://images.pexels.com/photos/774909/pexels-photo-774909.jpeg?auto=compress&cs=tinysrgb&w=150'),
(3, N'Caner Aslan', N'0555 333 4455', N'caner@emlak.com', N'https://images.pexels.com/photos/1222271/pexels-photo-1222271.jpeg?auto=compress&cs=tinysrgb&w=150');
SET IDENTITY_INSERT Realtors OFF;
GO

-- 5. İLANLAR (Properties) EKLE
SET IDENTITY_INSERT Properties ON;
INSERT INTO Properties (Id, Title, Description, Price, Address, SquareMeter, RoomCount, BathCount, ImageUrl, CityId, PropertyTypeId, RealtorId) VALUES 
(1, N'Boğaz Manzaralı Lüks Rezidans Dairesi', N'Beşiktaş''ın en prestijli bölgesinde, 360 derece boğaz manzaralı, akıllı ev sistemine sahip, sıfır 4+1 daire. Kapalı otopark ve havuz mevcuttur.', 45000000.00, N'Zorlu Center, Beşiktaş, İstanbul', 220, N'4+1', 3, N'https://images.pexels.com/photos/106399/pexels-photo-106399.jpeg?auto=compress&cs=tinysrgb&w=800', 2, 2, 1),
(2, N'Serdivan Mavi Durak Yakını Kiralık 2+1', N'Mavi durağa yürüyüş mesafesinde, tamamen eşyalı, öğrenciye veya çalışana uygun, doğalgaz kombili temiz daire.', 18000.00, N'Serdivan, Sakarya', 95, N'2+1', 1, N'https://images.pexels.com/photos/276724/pexels-photo-276724.jpeg?auto=compress&cs=tinysrgb&w=800', 1, 1, 2),
(3, N'Çankaya Konutkulelerinde Eşyalı Günlük Rezidans', N'Çankaya merkezde, lüks döşenmiş, tüm ev gereçleri mevcut, güvenlikli site içerisinde günlük kiralık 1+1 daire.', 2500.00, N'Çankaya, Ankara', 65, N'1+1', 1, N'https://images.pexels.com/photos/1571460/pexels-photo-1571460.jpeg?auto=compress&cs=tinysrgb&w=800', 3, 3, 3),
(4, N'Urla Kekliktepe Havuzlu Müstakil Villa', N'Urla Kekliktepe bölgesinde, 1.5 dönüm arsa içerisinde, 50m2 özel havuzlu, 5 odalı, tamamıyla müstakil lüks taş villa.', 65000000.00, N'Kekliktepe, Urla, İzmir', 400, N'5+2', 4, N'https://images.pexels.com/photos/2102587/pexels-photo-2102587.jpeg?auto=compress&cs=tinysrgb&w=800', 4, 2, 1),
(5, N'Konyaaltı Plajına Sıfır Kiralık Daire', N'Konyaaltı plajına 50 metre mesafede, geniş balkonlu, deniz manzaralı, klimalı ve mobilyalı 3+1 daire.', 35000.00, N'Konyaaltı, Antalya', 140, N'3+1', 2, N'https://images.pexels.com/photos/259588/pexels-photo-259588.jpeg?auto=compress&cs=tinysrgb&w=800', 5, 1, 2),
(6, N'Nilüfer Gümüştepe Doğa Manzaralı Dubleks Ev', N'Bursa Nilüfer Gümüştepe''de, yeşillikler içerisinde, kendine ait küçük bahçesi bulunan, otoparklı, 3+1 dubleks villa.', 12500000.00, N'Gümüştepe, Nilüfer, Bursa', 180, N'3+1', 2, N'https://images.pexels.com/photos/1396122/pexels-photo-1396122.jpeg?auto=compress&cs=tinysrgb&w=800', 6, 2, 3);
SET IDENTITY_INSERT Properties OFF;
GO

-- 6. KONTROL SORGULARI
SELECT N'Eklenen İlan Sayısı' AS Bilgi, COUNT(*) AS Sayi FROM Properties UNION ALL
SELECT N'Eklenen Şehir Sayısı', COUNT(*) FROM Cities UNION ALL
SELECT N'Eklenen Danışman Sayısı', COUNT(*) FROM Realtors;
GO
