-- ============================================================
-- SPOR KULUBU REZERVASYON SISTEMI - Veritabanı Kurulum Scripti
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SporKulubuDB')
BEGIN
    CREATE DATABASE SporKulubuDB;
END
GO

USE SporKulubuDB;
GO

-- ============================================================
-- TABLOLAR
-- ============================================================

-- 1. SportsBranches (Spor Branşları)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SportsBranches')
BEGIN
    CREATE TABLE SportsBranches (
        BranchId    INT IDENTITY(1,1) PRIMARY KEY,
        BranchName  NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL
    );
END
GO

-- 2. Coaches (Antrenörler)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Coaches')
BEGIN
    CREATE TABLE Coaches (
        CoachId     INT IDENTITY(1,1) PRIMARY KEY,
        FirstName   NVARCHAR(50)  NOT NULL,
        LastName    NVARCHAR(50)  NOT NULL,
        Email       NVARCHAR(100) NOT NULL UNIQUE,
        Phone       NVARCHAR(20)  NULL,
        BranchId    INT NOT NULL REFERENCES SportsBranches(BranchId),
        CreatedAt   DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- 3. Members (Üyeler)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Members')
BEGIN
    CREATE TABLE Members (
        MemberId    INT IDENTITY(1,1) PRIMARY KEY,
        FirstName   NVARCHAR(50)  NOT NULL,
        LastName    NVARCHAR(50)  NOT NULL,
        Email       NVARCHAR(100) NOT NULL UNIQUE,
        Phone       NVARCHAR(20)  NULL,
        JoinDate    DATETIME      NOT NULL DEFAULT GETDATE(),
        IsActive    BIT           NOT NULL DEFAULT 1
    );
END
GO

-- 4. Trainings (Antrenman Kayıtları)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Trainings')
BEGIN
    CREATE TABLE Trainings (
        TrainingId      INT IDENTITY(1,1) PRIMARY KEY,
        MemberId        INT NOT NULL REFERENCES Members(MemberId),
        CoachId         INT NOT NULL REFERENCES Coaches(CoachId),
        TrainingDate    DATETIME NOT NULL,
        DurationMinutes INT NOT NULL DEFAULT 60,
        Fee             DECIMAL(10,2) NOT NULL DEFAULT 0.00,
        CreatedAt       DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- ============================================================
-- ÖRNEK SEED VERİLERİ (En az 5 kayıt)
-- ============================================================

-- 1. Branşlar
IF NOT EXISTS (SELECT * FROM SportsBranches)
BEGIN
    INSERT INTO SportsBranches (BranchName, Description) VALUES
    (N'Fitness & Vücut Geliştirme', N'Kişiye özel ağırlık ve kardiyo antrenmanları'),
    (N'Tenis', N'Bireysel ve çiftler kort tenis eğitimleri'),
    (N'Yüzme', N'Kapalı ve açık olimpik havuz yüzme dersleri'),
    (N'Boks / Kick Boks', N'Savunma sanatları ve kondisyon dersleri'),
    (N'Pilates / Yoga', N'Esneklik ve postür düzeltici grup dersleri');
END
GO

-- 2. Antrenörler
IF NOT EXISTS (SELECT * FROM Coaches)
BEGIN
    INSERT INTO Coaches (FirstName, LastName, Email, Phone, BranchId) VALUES
    (N'Hakan', N'Yılmaz', N'hakan.yilmaz@sporkulubu.com', N'0555 101 20 30', 1),
    (N'Deniz', N'Kaya', N'deniz.kaya@sporkulubu.com', N'0555 202 30 40', 2),
    (N'Aylin', N'Şahin', N'aylin.sahin@sporkulubu.com', N'0555 303 40 50', 3),
    (N'Murat', N'Demir', N'murat.demir@sporkulubu.com', N'0555 404 50 60', 4),
    (N'Selin', N'Öztürk', N'selin.ozturk@sporkulubu.com', N'0555 505 60 70', 5);
END
GO

-- 3. Üyeler
IF NOT EXISTS (SELECT * FROM Members)
BEGIN
    INSERT INTO Members (FirstName, LastName, Email, Phone, JoinDate, IsActive) VALUES
    (N'Merve', N'Gezginci', N'merve.gezginci@gmail.com', N'0532 111 22 33', DATEADD(day, -35, GETDATE()), 1),
    (N'Can', N'Öztürk', N'can.ozturk@gmail.com', N'0532 222 33 44', DATEADD(day, -20, GETDATE()), 1),
    (N'Elif', N'Şen', N'elif.sen@gmail.com', N'0532 333 44 55', DATEADD(day, -10, GETDATE()), 1),
    (N'Bora', N'Demir', N'bora.demir@gmail.com', N'0532 444 55 66', DATEADD(day, -15, GETDATE()), 0),
    (N'Aslı', N'Kaya', N'asli.kaya@gmail.com', N'0532 555 66 77', DATEADD(day, -5, GETDATE()), 1),
    (N'Oğuz', N'Tuna', N'oguz.tuna@gmail.com', N'0532 666 77 88', DATEADD(day, -45, GETDATE()), 1);
END
GO

-- 4. Antrenman Seansları
IF NOT EXISTS (SELECT * FROM Trainings)
BEGIN
    DECLARE @M1 INT, @M2 INT, @M3 INT, @M4 INT, @M5 INT;
    SELECT @M1 = MemberId FROM Members WHERE Email = N'merve.gezginci@gmail.com';
    SELECT @M2 = MemberId FROM Members WHERE Email = N'can.ozturk@gmail.com';
    SELECT @M3 = MemberId FROM Members WHERE Email = N'elif.sen@gmail.com';
    SELECT @M4 = MemberId FROM Members WHERE Email = N'asli.kaya@gmail.com';
    SELECT @M5 = MemberId FROM Members WHERE Email = N'oguz.tuna@gmail.com';

    DECLARE @C1 INT, @C2 INT, @C3 INT, @C4 INT, @C5 INT;
    SELECT @C1 = CoachId FROM Coaches WHERE Email = N'hakan.yilmaz@sporkulubu.com';
    SELECT @C2 = CoachId FROM Coaches WHERE Email = N'deniz.kaya@sporkulubu.com';
    SELECT @C3 = CoachId FROM Coaches WHERE Email = N'aylin.sahin@sporkulubu.com';
    SELECT @C4 = CoachId FROM Coaches WHERE Email = N'murat.demir@sporkulubu.com';
    SELECT @C5 = CoachId FROM Coaches WHERE Email = N'selin.ozturk@sporkulubu.com';

    INSERT INTO Trainings (MemberId, CoachId, TrainingDate, DurationMinutes, Fee) VALUES
    (@M1, @C1, DATEADD(hour, 10, DATEADD(day, -2, GETDATE())), 60, 250.00),
    (@M1, @C1, DATEADD(hour, 14, DATEADD(day, -1, GETDATE())), 60, 250.00),
    (@M2, @C2, DATEADD(hour, 9, DATEADD(day, -3, GETDATE())), 90, 400.00),
    (@M2, @C2, DATEADD(hour, 11, DATEADD(day, -1, GETDATE())), 90, 400.00),
    (@M3, @C3, DATEADD(hour, 15, DATEADD(day, -4, GETDATE())), 60, 300.00),
    (@M3, @C3, DATEADD(hour, 16, DATEADD(day, -2, GETDATE())), 60, 300.00),
    (@M4, @C4, DATEADD(hour, 18, DATEADD(day, -1, GETDATE())), 60, 350.00),
    (@M5, @C5, DATEADD(hour, 8, DATEADD(day, -5, GETDATE())), 60, 200.00),
    (@M5, @C5, DATEADD(hour, 8, DATEADD(day, -3, GETDATE())), 60, 200.00),
    (@M1, @C5, DATEADD(hour, 19, DATEADD(day, -7, GETDATE())), 60, 200.00);
END
GO


-- ============================================================
-- STORED PROCEDURES
-- ============================================================

-- 1. SportsBranches (Branşlar) SP'leri
CREATE OR ALTER PROCEDURE sp_GetBranches AS
BEGIN
    SELECT BranchId, BranchName, Description FROM SportsBranches ORDER BY BranchName;
END;
GO

CREATE OR ALTER PROCEDURE sp_GetBranchById @BranchId INT AS
BEGIN
    SELECT BranchId, BranchName, Description FROM SportsBranches WHERE BranchId = @BranchId;
END;
GO

CREATE OR ALTER PROCEDURE sp_InsertBranch @BranchName NVARCHAR(100), @Description NVARCHAR(500) AS
BEGIN
    INSERT INTO SportsBranches (BranchName, Description) VALUES (@BranchName, @Description);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END;
GO

CREATE OR ALTER PROCEDURE sp_UpdateBranch @BranchId INT, @BranchName NVARCHAR(100), @Description NVARCHAR(500) AS
BEGIN
    UPDATE SportsBranches SET BranchName = @BranchName, Description = @Description WHERE BranchId = @BranchId;
END;
GO

CREATE OR ALTER PROCEDURE sp_DeleteBranch @BranchId INT AS
BEGIN
    DELETE FROM SportsBranches WHERE BranchId = @BranchId;
END;
GO

-- 2. Coaches (Antrenörler) SP'leri
CREATE OR ALTER PROCEDURE sp_GetCoaches AS
BEGIN
    SELECT c.CoachId, c.FirstName, c.LastName, c.Email, c.Phone, c.BranchId, b.BranchName
    FROM Coaches c
    INNER JOIN SportsBranches b ON c.BranchId = b.BranchId
    ORDER BY c.FirstName, c.LastName;
END;
GO

CREATE OR ALTER PROCEDURE sp_GetCoachById @CoachId INT AS
BEGIN
    SELECT c.CoachId, c.FirstName, c.LastName, c.Email, c.Phone, c.BranchId, b.BranchName
    FROM Coaches c
    INNER JOIN SportsBranches b ON c.BranchId = b.BranchId
    WHERE c.CoachId = @CoachId;
END;
GO

CREATE OR ALTER PROCEDURE sp_InsertCoach @FirstName NVARCHAR(50), @LastName NVARCHAR(50), @Email NVARCHAR(100), @Phone NVARCHAR(20), @BranchId INT AS
BEGIN
    INSERT INTO Coaches (FirstName, LastName, Email, Phone, BranchId)
    VALUES (@FirstName, @LastName, @Email, @Phone, @BranchId);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END;
GO

CREATE OR ALTER PROCEDURE sp_UpdateCoach @CoachId INT, @FirstName NVARCHAR(50), @LastName NVARCHAR(50), @Email NVARCHAR(100), @Phone NVARCHAR(20), @BranchId INT AS
BEGIN
    UPDATE Coaches
    SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Phone = @Phone, BranchId = @BranchId
    WHERE CoachId = @CoachId;
END;
GO

CREATE OR ALTER PROCEDURE sp_DeleteCoach @CoachId INT AS
BEGIN
    DELETE FROM Coaches WHERE CoachId = @CoachId;
END;
GO

-- 3. Members (Üyeler) SP'leri
CREATE OR ALTER PROCEDURE sp_GetMembers AS
BEGIN
    SELECT MemberId, FirstName, LastName, Email, Phone, JoinDate, IsActive FROM Members ORDER BY JoinDate DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_GetMemberById @MemberId INT AS
BEGIN
    SELECT MemberId, FirstName, LastName, Email, Phone, JoinDate, IsActive FROM Members WHERE MemberId = @MemberId;
END;
GO

CREATE OR ALTER PROCEDURE sp_SearchMembers @Keyword NVARCHAR(100) = NULL, @IsActive BIT = NULL AS
BEGIN
    SELECT MemberId, FirstName, LastName, Email, Phone, JoinDate, IsActive
    FROM Members
    WHERE 
        (@Keyword IS NULL OR FirstName LIKE '%' + @Keyword + '%' OR LastName LIKE '%' + @Keyword + '%' OR Email LIKE '%' + @Keyword + '%')
        AND (@IsActive IS NULL OR IsActive = @IsActive)
    ORDER BY JoinDate DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_InsertMember @FirstName NVARCHAR(50), @LastName NVARCHAR(50), @Email NVARCHAR(100), @Phone NVARCHAR(20) AS
BEGIN
    INSERT INTO Members (FirstName, LastName, Email, Phone) VALUES (@FirstName, @LastName, @Email, @Phone);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END;
GO

CREATE OR ALTER PROCEDURE sp_UpdateMember @MemberId INT, @FirstName NVARCHAR(50), @LastName NVARCHAR(50), @Email NVARCHAR(100), @Phone NVARCHAR(20), @IsActive BIT AS
BEGIN
    UPDATE Members
    SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Phone = @Phone, IsActive = @IsActive
    WHERE MemberId = @MemberId;
END;
GO

CREATE OR ALTER PROCEDURE sp_DeleteMember @MemberId INT AS
BEGIN
    DELETE FROM Trainings WHERE MemberId = @MemberId;
    DELETE FROM Members WHERE MemberId = @MemberId;
END;
GO

-- 4. Trainings (Antrenman Seansları) SP'leri
CREATE OR ALTER PROCEDURE sp_GetTrainings AS
BEGIN
    SELECT t.TrainingId, t.MemberId, (m.FirstName + ' ' + m.LastName) AS MemberName,
           t.CoachId, (c.FirstName + ' ' + c.LastName) AS CoachName,
           t.TrainingDate, t.DurationMinutes, t.Fee, b.BranchName
    FROM Trainings t
    INNER JOIN Members m ON t.MemberId = m.MemberId
    INNER JOIN Coaches c ON t.CoachId = c.CoachId
    INNER JOIN SportsBranches b ON c.BranchId = b.BranchId
    ORDER BY t.TrainingDate DESC;
END;
GO

CREATE OR ALTER PROCEDURE sp_GetTrainingById @TrainingId INT AS
BEGIN
    SELECT t.TrainingId, t.MemberId, (m.FirstName + ' ' + m.LastName) AS MemberName,
           t.CoachId, (c.FirstName + ' ' + c.LastName) AS CoachName,
           t.TrainingDate, t.DurationMinutes, t.Fee, b.BranchName
    FROM Trainings t
    INNER JOIN Members m ON t.MemberId = m.MemberId
    INNER JOIN Coaches c ON t.CoachId = c.CoachId
    INNER JOIN SportsBranches b ON c.BranchId = b.BranchId
    WHERE t.TrainingId = @TrainingId;
END;
GO

CREATE OR ALTER PROCEDURE sp_InsertTraining @MemberId INT, @CoachId INT, @TrainingDate DATETIME, @DurationMinutes INT, @Fee DECIMAL(10,2) AS
BEGIN
    INSERT INTO Trainings (MemberId, CoachId, TrainingDate, DurationMinutes, Fee)
    VALUES (@MemberId, @CoachId, @TrainingDate, @DurationMinutes, @Fee);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewId;
END;
GO

CREATE OR ALTER PROCEDURE sp_UpdateTraining @TrainingId INT, @MemberId INT, @CoachId INT, @TrainingDate DATETIME, @DurationMinutes INT, @Fee DECIMAL(10,2) AS
BEGIN
    UPDATE Trainings
    SET MemberId = @MemberId, CoachId = @CoachId, TrainingDate = @TrainingDate, DurationMinutes = @DurationMinutes, Fee = @Fee
    WHERE TrainingId = @TrainingId;
END;
GO

CREATE OR ALTER PROCEDURE sp_DeleteTraining @TrainingId INT AS
BEGIN
    DELETE FROM Trainings WHERE TrainingId = @TrainingId;
END;
GO

PRINT 'SporKulubuDB Veritabanı, Tabloları, Örnek Verileri ve Stored Procedure''ları başarıyla oluşturuldu.';
