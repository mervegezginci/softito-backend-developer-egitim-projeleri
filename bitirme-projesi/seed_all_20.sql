SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

-- 1. Clean existing records in dependency order
DELETE FROM Payments;
DELETE FROM Reviews;
DELETE FROM Bookings;
DELETE FROM SystemLogs;
DELETE FROM Tours;
DELETE FROM Guides;
DELETE FROM Categories;
DELETE FROM Coupons;
DELETE FROM Blogs;
DELETE FROM Faqs;
DELETE FROM ContactMessages;
DELETE FROM ChatMessages;
DELETE FROM AspNetUsers WHERE Email NOT IN ('gamze@gmtravel.com', 'merve@gmtravel.com', 'admin@gezgin.com', 'ahmet@gmail.com');

-- 2. Insert Categories
DECLARE @CategoriesTable TABLE (Id INT IDENTITY(1,1), Name NVARCHAR(100), Description NVARCHAR(250));
INSERT INTO @CategoriesTable (Name, Description) VALUES
(N'Kültür Rotaları', N'Kültür Rotaları için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Doğa ve Macera', N'Doğa ve Macera için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Mavi Yolculuk', N'Mavi Yolculuk için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Gastronomi', N'Gastronomi için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Kış Turizmi', N'Kış Turizmi için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Balayı Paketleri', N'Balayı Paketleri için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Arkeoloji ve Tarih', N'Arkeoloji ve Tarih için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Fotoğrafçılık', N'Fotoğrafçılık için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Sağlık ve Termal', N'Sağlık ve Termal için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Yayla Turizmi', N'Yayla Turizmi için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Kamp ve Karavan', N'Kamp ve Karavan için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Ekstrem Sporlar', N'Ekstrem Sporlar için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Kanyon Geçişi', N'Kanyon Geçişi için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Kuş Gözlemciliği', N'Kuş Gözlemciliği için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'İnanç Turizmi', N'İnanç Turizmi için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Bisiklet Rotaları', N'Bisiklet Rotaları için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Mağara Keşifleri', N'Mağara Keşifleri için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Şehir Turları', N'Şehir Turları için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Ekoturizm', N'Ekoturizm için özel olarak hazırlanmış en prestijli seyahat programları.'),
(N'Bağ Bozumu Turları', N'Bağ Bozumu Turları için özel olarak hazırlanmış en prestijli seyahat programları.');

INSERT INTO Categories (Name, Description)
SELECT Name, Description FROM @CategoriesTable;

-- 3. Insert Guides
DECLARE @GuidesTable TABLE (Id INT IDENTITY(1,1), FullName NVARCHAR(100), Mail NVARCHAR(100), Phone NVARCHAR(50), Bio NVARCHAR(500));
INSERT INTO @GuidesTable (FullName, Mail, Phone, Bio) VALUES
(N'Mehmet Şahin', N'mehmet.sahin@gmtravel.com', N'0555 100 2030', N'Mehmet Şahin rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Ayşe Çelik', N'ayse.celik@gmtravel.com', N'0555 101 2131', N'Ayşe Çelik rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Hakan Öztürk', N'hakan.ozturk@gmtravel.com', N'0555 102 2232', N'Hakan Öztürk rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Fatma Yılmaz', N'fatma.yilmaz@gmtravel.com', N'0555 103 2333', N'Fatma Yılmaz rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Mustafa Yıldız', N'mustafa.yildiz@gmtravel.com', N'0555 104 2434', N'Mustafa Yıldız rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Zeynep Demir', N'zeynep.demir@gmtravel.com', N'0555 105 2535', N'Zeynep Demir rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Ali Koç', N'ali.koc@gmtravel.com', N'0555 106 2636', N'Ali Koç rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Elif Kaya', N'elif.kaya@gmtravel.com', N'0555 107 2737', N'Elif Kaya rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Ömer Arslan', N'omer.arslan@gmtravel.com', N'0555 108 2838', N'Ömer Arslan rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Hatice Polat', N'hatice.polat@gmtravel.com', N'0555 109 2939', N'Hatice Polat rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Süleyman Bulut', N'suleyman.bulut@gmtravel.com', N'0555 110 3040', N'Süleyman Bulut rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Emine Şen', N'emine.sen@gmtravel.com', N'0555 111 3141', N'Emine Şen rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'İbrahim Aksoy', N'ibrahim.aksoy@gmtravel.com', N'0555 112 3242', N'İbrahim Aksoy rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Merve Kılıç', N'merve.kilic@gmtravel.com', N'0555 113 3343', N'Merve Kılıç rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Ahmet Aydın', N'ahmet.aydin@gmtravel.com', N'0555 114 3444', N'Ahmet Aydın rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Selin Öztürk', N'selin.ozturk@gmtravel.com', N'0555 115 3545', N'Selin Öztürk rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Caner Ünal', N'caner.unal@gmtravel.com', N'0555 116 3646', N'Caner Ünal rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Büşra Çetin', N'busra.cetin@gmtravel.com', N'0555 117 3747', N'Büşra Çetin rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Murat Özdemir', N'murat.ozdemir@gmtravel.com', N'0555 118 3848', N'Murat Özdemir rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.'),
(N'Gökhan Güler', N'gokhan.guler@gmtravel.com', N'0555 119 3949', N'Gökhan Güler rehberimiz, kendi alanında uzmanlaşmış ve uzun yıllar boyunca yerli/yabancı turist gruplarına eşlik etmiş kokartlı profesyonel bir rehberdir.');

INSERT INTO Guides (FullName, Mail, Phone, Bio, GuideImageUrl)
SELECT FullName, Mail, Phone, Bio, 'https://images.unsplash.com/photo-1544005313-94ddf0286df2?w=500&auto=format&fit=crop&q=60' FROM @GuidesTable;

-- 4. Insert AspNetUsers
DECLARE @AhmetHash NVARCHAR(MAX), @AhmetSecurityStamp NVARCHAR(MAX), @AhmetConcurrencyStamp NVARCHAR(MAX);
SELECT TOP 1 @AhmetHash = PasswordHash, @AhmetSecurityStamp = SecurityStamp, @AhmetConcurrencyStamp = ConcurrencyStamp 
FROM AspNetUsers WHERE Email = 'ahmet@gmail.com';

IF @AhmetHash IS NULL
BEGIN
    SET @AhmetHash = 'AQAAAAIAAYagAAAAENnS5w9s...'; 
    SET @AhmetSecurityStamp = 'STAMP123';
    SET @AhmetConcurrencyStamp = 'CONCURRENCY123';
END;

DECLARE @j INT = 1;
WHILE @j <= 20
BEGIN
    DECLARE @userId NVARCHAR(450) = CAST(NEWID() AS NVARCHAR(450));
    DECLARE @userEmail NVARCHAR(256) = 'gezgin_user' + CAST(@j AS NVARCHAR(10)) + '@gmail.com';
    
    INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Name, Status)
    VALUES (
        @userId, 
        @userEmail, 
        UPPER(@userEmail), 
        @userEmail, 
        UPPER(@userEmail), 
        1, 
        @AhmetHash, 
        @AhmetSecurityStamp, 
        @AhmetConcurrencyStamp, 
        0, 0, 1, 0, 
        N'Gezgin Müşteri ' + CAST(@j AS NVARCHAR(10)), 
        'active'
    );
    SET @j = @j + 1;
END;

-- 5. Insert Tours
DECLARE @CatIds TABLE (RowNo INT IDENTITY(1,1), Id INT);
INSERT INTO @CatIds (Id) SELECT Id FROM Categories;

DECLARE @GuideIds TABLE (RowNo INT IDENTITY(1,1), Id INT);
INSERT INTO @GuideIds (Id) SELECT Id FROM Guides;

DECLARE @ToursTable TABLE (Id INT IDENTITY(1,1), Title NVARCHAR(100), Location NVARCHAR(100));
INSERT INTO @ToursTable (Title, Location) VALUES
(N'Göbeklitepe ve Şanlıurfa Kültür Turu', N'Şanlıurfa'),
(N'Fırtına Deresi Rize Yaylaları', N'Rize'),
(N'Marmaris Mavi Koylar Yolculuğu', N'Muğla'),
(N'Gaziantep Gastronomi ve Lezzet Rotası', N'Gaziantep'),
(N'Uludağ ve Kartalkaya Kayak Turu', N'Bursa'),
(N'Kapadokya Balayı Rüyası', N'Nevşehir'),
(N'Efes Antik Kenti ve Şirince Gezisi', N'İzmir'),
(N'Ihlara Vadisi Fotoğraf Safari', N'Aksaray'),
(N'Pamukkale Travertenleri ve Termal Keyfi', N'Denizli'),
(N'Ayder Yaylası Doğa Yürüyüşü', N'Rize'),
(N'Kaz Dağları Kamp ve Karavan Deneyimi', N'Çanakkale'),
(N'Fethiye Yamaç Paraşütü ve Macera', N'Muğla'),
(N'Köprülü Kanyon Rafting Turu', N'Antalya'),
(N'Manyas Kuş Cenneti Gözlem Turu', N'Balıkesir'),
(N'Mardin Taş Evler ve İnanç Rotası', N'Mardin'),
(N'Gökçeada ve Bozcaada Bisiklet Turu', N'Çanakkale'),
(N'Karain Mağarası Keşif Gezisi', N'Antalya'),
(N'İstanbul Boğazı ve Tarihi Yarımada', N'İstanbul'),
(N'Küre Dağları Ekoturizm Yolu', N'Kastamonu'),
(N'Urla Bağ Bozumu ve Şarap Rotası', N'İzmir');

DECLARE @k INT = 1;
WHILE @k <= 20
BEGIN
    DECLARE @catId INT, @guideId INT;
    SELECT @catId = Id FROM @CatIds WHERE RowNo = ((@k - 1) % 20) + 1;
    SELECT @guideId = Id FROM @GuideIds WHERE RowNo = ((@k - 1) % 20) + 1;
    
    DECLARE @title NVARCHAR(100), @location NVARCHAR(100);
    SELECT @title = Title, @location = Location FROM @ToursTable WHERE Id = @k;
    
    INSERT INTO Tours (Title, Description, Location, StartDate, EndDate, DurationDays, Price, Capacity, ImageUrl, IsActive, CategoryId, GuideId)
    VALUES (
        @title,
        @title + N' ile Türkiye''nin en eşsiz yerlerini keşfedin. Konforlu ulaşım ve konaklama dahildir.',
        @location + N', Türkiye',
        DATEADD(day, 15 + @k * 2, GETDATE()),
        DATEADD(day, 18 + @k * 2, GETDATE()),
        3 + (@k % 4),
        10000 + @k * 1200,
        10 + @k,
        'https://images.unsplash.com/photo-1507608869274-d3177c8bb4c7?w=800&auto=format&fit=crop&q=60',
        1,
        @catId,
        @guideId
    );
    SET @k = @k + 1;
END;

-- 6. Insert Coupons
DECLARE @c INT = 1;
WHILE @c <= 20
BEGIN
    INSERT INTO Coupons (Code, DiscountType, DiscountValue, ExpiryDate, IsActive)
    VALUES (
        N'TURKCE' + RIGHT('00' + CAST(@c AS NVARCHAR(10)), 2),
        CASE WHEN @c % 2 = 0 THEN 'percentage' ELSE 'fixed' END,
        CASE WHEN @c % 2 = 0 THEN 5 + @c ELSE 500 + @c * 100 END,
        DATEADD(month, 6, GETDATE()),
        1
    );
    SET @c = @c + 1;
END;

-- 7. Insert Blogs
DECLARE @BlogTitlesTable TABLE (Id INT IDENTITY(1,1), Title NVARCHAR(200));
INSERT INTO @BlogTitlesTable (Title) VALUES
(N'Şanlıurfa''da Ne Yenir? En İyi Kebapçılar'),
(N'Rize Yaylalarında Kamp Yapmanın Püf Noktaları'),
(N'Marmaris''in En Sakin Koyları'),
(N'Gaziantep Gastronomi Müzesi Gezi Rehberi'),
(N'Kayak Yaparken Dikkat Edilmesi Gerekenler'),
(N'Kapadokya Balon Turu Fiyatları 2026'),
(N'Efes Harabeleri Tarihi ve Giriş Detayları'),
(N'Doğa Fotoğrafçılığı İçin Gerekli Ekipmanlar'),
(N'Pamukkale''de Şifalı Termal Sular'),
(N'Ayder Yaylası Otel Tavsiyeleri'),
(N'Kaz Dağları''nda Oksijen Depolayacağınız Rotalar'),
(N'Fethiye Ölüdeniz Paraşüt Firmaları'),
(N'Antalya Rafting Alanları Karşılaştırması'),
(N'Kuş Gözlemciliğine Başlama Kılavuzu'),
(N'Mardin''in Büyülü Taş Mimarisi'),
(N'Bozcaada Bisiklet Parkurları Haritası'),
(N'Antalya Karain Mağarası Giriş Ücreti'),
(N'İstanbul Boğaz Turu Saatleri ve Rotaları'),
(N'Ekoturizm Nedir? Küre Dağları Örneği'),
(N'Urla Bağ Bozumu Şenlikleri Takvimi');

DECLARE @b INT = 1;
WHILE @b <= 20
BEGIN
    DECLARE @blogTitle NVARCHAR(200);
    SELECT @blogTitle = Title FROM @BlogTitlesTable WHERE Id = @b;
    
    INSERT INTO Blogs (Title, Content, ImageUrl, CreatedAt)
    VALUES (
        @blogTitle,
        @blogTitle + N' başlığı altında hazırladığımız rehber yazımızda, bölgeye yapacağınız seyahatte işinize yarayacak tüm ipuçlarını detaylıca inceledik.',
        'https://images.unsplash.com/photo-1501785888041-af3ef285b470?w=800&auto=format&fit=crop&q=60',
        DATEADD(day, -@b, GETDATE())
    );
    SET @b = @b + 1;
END;

-- 8. Insert Faqs
DECLARE @f INT = 1;
WHILE @f <= 20
BEGIN
    INSERT INTO Faqs (Question, Answer, SortOrder)
    VALUES (
        N'Seyahat planlarında ' + CAST(@f AS NVARCHAR(10)) + N'. sıkça sorulan soru başlığı nedir?',
        N'Bu soruya istinaden yapılan geri dönüşlere göre, ödeme iadeleri ve rezervasyon değişiklikleri koşullarımız web sitemizde yer almaktadır.',
        @f
    );
    SET @f = @f + 1;
END;

-- 9. Insert ContactMessages
DECLARE @cm INT = 1;
WHILE @cm <= 20
BEGIN
    INSERT INTO ContactMessages (Name, Email, Subject, Message, IsRead, CreatedAt)
    VALUES (
        N'Müşteri Danışma ' + CAST(@cm AS NVARCHAR(10)),
        'danisma' + CAST(@cm AS NVARCHAR(10)) + '@gmail.com',
        N'Bilgi Talebi ve Sorular - Konu ' + CAST(@cm AS NVARCHAR(10)),
        N'Merhaba, ' + CAST(@cm AS NVARCHAR(10)) + N' numaralı seyahat paketi hakkında detaylı bilgi ve müsaitlik durumunu öğrenmek istiyorum. Dönüş yaparsanız sevinirim.',
        CASE WHEN @cm % 3 = 0 THEN 1 ELSE 0 END,
        DATEADD(day, -@cm, GETDATE())
    );
    SET @cm = @cm + 1;
END;

-- 10. Insert ChatMessages
DECLARE @ch INT = 1;
WHILE @ch <= 20
BEGIN
    INSERT INTO ChatMessages (SessionId, Sender, MessageText, Timestamp)
    VALUES (
        'session_client_' + CAST(100 + @ch AS NVARCHAR(10)),
        CASE WHEN @ch % 2 = 0 THEN 'user' ELSE 'admin' END,
        CASE WHEN @ch % 2 = 0 THEN N'Merhaba, canlı destek üzerinden ' + CAST(@ch AS NVARCHAR(10)) + N'. sorumu sormak istiyorum.' ELSE N'Elbette, size seyahat paketlerimiz hakkında yardımcı olmaktan memnuniyet duyarım.' END,
        DATEADD(minute, -10 * @ch, GETDATE())
    );
    SET @ch = @ch + 1;
END;

-- 11. Insert Bookings (using Tour and User IDs)
DECLARE @TourIds TABLE (RowNo INT IDENTITY(1,1), Id INT, Price FLOAT);
INSERT INTO @TourIds (Id, Price) SELECT Id, Price FROM Tours;

DECLARE @UserIds TABLE (RowNo INT IDENTITY(1,1), Id NVARCHAR(450));
INSERT INTO @UserIds (Id) SELECT Id FROM AspNetUsers;

DECLARE @CouponIds TABLE (RowNo INT IDENTITY(1,1), Id INT);
INSERT INTO @CouponIds (Id) SELECT Id FROM Coupons;

DECLARE @bk INT = 1;
WHILE @bk <= 20
BEGIN
    DECLARE @tId INT, @tPrice FLOAT, @uId NVARCHAR(450), @cpId INT;
    SELECT @tId = Id, @tPrice = Price FROM @TourIds WHERE RowNo = ((@bk - 1) % 20) + 1;
    SELECT @uId = Id FROM @UserIds WHERE RowNo = ((@bk - 1) % 20) + 1;
    SELECT @cpId = Id FROM @CouponIds WHERE RowNo = ((@bk - 1) % 20) + 1;
    
    INSERT INTO Bookings (TourId, UserId, BookingDate, GuestsCount, TotalPrice, PaymentStatus, Status, CouponId)
    VALUES (
        @tId,
        @uId,
        DATEADD(day, @bk, GETDATE()),
        1 + (@bk % 4),
        @tPrice * (1 + (@bk % 4)),
        CASE WHEN @bk % 3 = 0 THEN 'paid' WHEN @bk % 3 = 1 THEN 'unpaid' ELSE 'refunded' END,
        CASE WHEN @bk % 4 = 0 THEN 'approved' WHEN @bk % 4 = 1 THEN 'pending' ELSE 'cancelled' END,
        CASE WHEN @bk % 5 = 0 THEN @cpId ELSE NULL END
    );
    SET @bk = @bk + 1;
END;

-- 12. Insert Payments
DECLARE @BookingList TABLE (RowNo INT IDENTITY(1,1), Id INT, TotalPrice FLOAT, PaymentStatus NVARCHAR(50));
INSERT INTO @BookingList (Id, TotalPrice, PaymentStatus) SELECT Id, TotalPrice, PaymentStatus FROM Bookings;

DECLARE @p INT = 1;
WHILE @p <= 20
BEGIN
    DECLARE @bId INT, @amount FLOAT, @pStatus NVARCHAR(50);
    SELECT @bId = Id, @amount = TotalPrice, @pStatus = PaymentStatus FROM @BookingList WHERE RowNo = @p;
    
    INSERT INTO Payments (BookingId, Amount, PaymentDate, TransactionId, PaymentMethod, Status)
    VALUES (
        @bId,
        @amount,
        DATEADD(day, -@p, GETDATE()),
        CASE WHEN @pStatus = 'paid' THEN 'TRX987654' + RIGHT('00' + CAST(@p AS NVARCHAR(10)), 2) ELSE NULL END,
        CASE WHEN @p % 2 = 0 THEN 'credit_card' ELSE 'bank_transfer' END,
        CASE WHEN @pStatus = 'paid' THEN 'completed' WHEN @pStatus = 'refunded' THEN 'failed' ELSE 'pending' END
    );
    SET @p = @p + 1;
END;

-- 13. Insert Reviews
DECLARE @CommentsTable TABLE (Id INT IDENTITY(1,1), Comment NVARCHAR(500));
INSERT INTO @CommentsTable (Comment) VALUES
(N'Harika bir geziydi, emeği geçen herkese teşekkürler!'),
(N'Rehberin bilgisi ve ilgisi mükemmel seviyedeydi.'),
(N'Ulaşım ve oteller çok konforluydu, tavsiye ederim.'),
(N'Hayatımın en güzel tatillerinden birini yaşadım.'),
(N'Fiyat performans oranı oldukça iyi bir tur programı.'),
(N'Yemekler harikaydı, gastronomi turundan çok memnun kaldık.'),
(N'Doğa yürüyüşü biraz yorucuydu ama manzaraya kesinlikle değdi.'),
(N'Balon turundaki manzaralar rüya gibiydi.'),
(N'Her şey en ince ayrıntısına kadar düşünülmüştü.'),
(N'Rehberimiz çok eğlenceli ve bilgiliydi.'),
(N'Bir dahaki seyahatimi kesinlikle yine bu acenteyle yapacağım.'),
(N'Kamp alanları temiz ve düzenliydi.'),
(N'Rafting macerası adrenalin doluydu, süperdi!'),
(N'Tarihi bilgileri rehberimizden dinlemek çok keyifliydi.'),
(N'Taş konakta konaklama harika bir deneyimdi.'),
(N'Bisiklet turu parkuru mükemmel seçilmişti.'),
(N'Mağara içi aydınlatma ve rehberlik mükemmeldi.'),
(N'İstanbul boğaz turundaki anlatım harikaydı.'),
(N'Küre dağlarının temiz havası bize çok iyi geldi.'),
(N'Urla''daki şarap tadım etkinliği son derece şıktı.');

DECLARE @rv INT = 1;
WHILE @rv <= 20
BEGIN
    DECLARE @revTourId INT, @revUserId NVARCHAR(450);
    SELECT @revTourId = Id FROM @TourIds WHERE RowNo = ((@rv - 1) % 20) + 1;
    SELECT @revUserId = Id FROM @UserIds WHERE RowNo = ((@rv - 1) % 20) + 1;
    
    DECLARE @comment NVARCHAR(500);
    SELECT @comment = Comment FROM @CommentsTable WHERE Id = @rv;
    
    INSERT INTO Reviews (TourId, UserId, Rating, Comment, CreatedAt)
    VALUES (
        @revTourId,
        @revUserId,
        4 + (@rv % 2),
        @comment,
        DATEADD(day, -@rv, GETDATE())
    );
    SET @rv = @rv + 1;
END;

-- 14. Insert SystemLogs
DECLARE @lg INT = 1;
WHILE @lg <= 20
BEGIN
    DECLARE @logUserId NVARCHAR(450);
    SELECT @logUserId = Id FROM @UserIds WHERE RowNo = ((@lg - 1) % 20) + 1;
    
    INSERT INTO SystemLogs (UserId, Action, Level, IpAddress, Details, CreatedAt)
    VALUES (
        @logUserId,
        CASE WHEN @lg % 2 = 0 THEN 'LOGIN_SUCCESS' ELSE 'SEARCH_PERFORMED' END,
        CASE WHEN @lg % 10 = 0 THEN 'warn' ELSE 'info' END,
        '192.168.1.' + CAST(10 + @lg AS NVARCHAR(10)),
        N'Gezgin seyahat sistemi üzerinde ' + CAST(@lg AS NVARCHAR(10)) + N'. işlem adımını gerçekleştirdi.',
        DATEADD(hour, -@lg, GETDATE())
    );
    SET @lg = @lg + 1;
END;
