-- ============================================================
-- OGRENCI YONETIM SISTEMI - Veritabanı Kurulum Scripti
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OgrenciYonetimDB')
BEGIN
    CREATE DATABASE OgrenciYonetimDB;
END
GO

USE OgrenciYonetimDB;
GO

-- ============================================================
-- TABLOLAR
-- ============================================================

-- 1. Users (Kullanıcılar)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        UserId       INT IDENTITY(1,1) PRIMARY KEY,
        Username     NVARCHAR(50)  NOT NULL UNIQUE,
        Email        NVARCHAR(100) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(256) NOT NULL,
        Role         NVARCHAR(20)  NOT NULL DEFAULT 'User',
        CreatedAt    DATETIME      NOT NULL DEFAULT GETDATE()
    );
END
GO

-- 2. Departments (Bölümler)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Departments')
BEGIN
    CREATE TABLE Departments (
        DepartmentId   INT IDENTITY(1,1) PRIMARY KEY,
        DepartmentName NVARCHAR(100) NOT NULL
    );
END
GO

-- 3. Students (Öğrenciler)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Students')
BEGIN
    CREATE TABLE Students (
        StudentId   INT IDENTITY(1,1) PRIMARY KEY,
        FirstName   NVARCHAR(50)  NOT NULL,
        LastName    NVARCHAR(50)  NOT NULL,
        Email       NVARCHAR(100) NOT NULL,
        Phone       NVARCHAR(20),
        DepartmentId INT NOT NULL REFERENCES Departments(DepartmentId),
        GradeLevel  INT NOT NULL DEFAULT 1,
        IsActive    BIT NOT NULL DEFAULT 1,
        CreatedAt   DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- 4. Grades (Notlar)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Grades')
BEGIN
    CREATE TABLE Grades (
        GradeId    INT IDENTITY(1,1) PRIMARY KEY,
        StudentId  INT NOT NULL REFERENCES Students(StudentId),
        CourseName NVARCHAR(100) NOT NULL,
        Score      DECIMAL(5,2)  NOT NULL,
        CreatedAt  DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- ============================================================
-- ÖRNEK VERİLER
-- ============================================================

IF NOT EXISTS (SELECT * FROM Departments)
BEGIN
    INSERT INTO Departments (DepartmentName) VALUES
    ('Bilgisayar Mühendisliği'),
    ('Elektrik-Elektronik Mühendisliği'),
    ('Makine Mühendisliği'),
    ('İşletme');
END
GO

-- ============================================================
-- STORED PROCEDURES
-- ============================================================

-- SP: Register
CREATE OR ALTER PROCEDURE sp_Register
    @Username     NVARCHAR(50),
    @Email        NVARCHAR(100),
    @PasswordHash NVARCHAR(256),
    @Role         NVARCHAR(20) = 'User'
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email OR Username = @Username)
    BEGIN
        SELECT -1 AS Result, 'Kullanıcı zaten mevcut.' AS Message;
        RETURN;
    END

    INSERT INTO Users (Username, Email, PasswordHash, Role)
    VALUES (@Username, @Email, @PasswordHash, @Role);

    SELECT SCOPE_IDENTITY() AS Result, 'Kayıt başarılı.' AS Message;
END
GO

-- SP: Login
CREATE OR ALTER PROCEDURE sp_Login
    @Email        NVARCHAR(100),
    @PasswordHash NVARCHAR(256)
AS
BEGIN
    SELECT UserId, Username, Email, Role
    FROM Users
    WHERE Email = @Email AND PasswordHash = @PasswordHash;
END
GO

-- SP: GetAllStudents
CREATE OR ALTER PROCEDURE sp_GetAllStudents
AS
BEGIN
    SELECT s.StudentId, s.FirstName, s.LastName, s.Email, s.Phone,
           s.DepartmentId, d.DepartmentName, s.GradeLevel, s.IsActive, s.CreatedAt
    FROM Students s
    INNER JOIN Departments d ON s.DepartmentId = d.DepartmentId
    ORDER BY s.CreatedAt DESC;
END
GO

-- SP: GetStudentById
CREATE OR ALTER PROCEDURE sp_GetStudentById
    @StudentId INT
AS
BEGIN
    SELECT s.StudentId, s.FirstName, s.LastName, s.Email, s.Phone,
           s.DepartmentId, d.DepartmentName, s.GradeLevel, s.IsActive, s.CreatedAt
    FROM Students s
    INNER JOIN Departments d ON s.DepartmentId = d.DepartmentId
    WHERE s.StudentId = @StudentId;
END
GO

-- SP: SearchStudents
CREATE OR ALTER PROCEDURE sp_SearchStudents
    @Keyword     NVARCHAR(100) = NULL,
    @DepartmentId INT = NULL
AS
BEGIN
    SELECT s.StudentId, s.FirstName, s.LastName, s.Email, s.Phone,
           s.DepartmentId, d.DepartmentName, s.GradeLevel, s.IsActive, s.CreatedAt
    FROM Students s
    INNER JOIN Departments d ON s.DepartmentId = d.DepartmentId
    WHERE
        (@Keyword IS NULL OR
         s.FirstName  LIKE '%' + @Keyword + '%' OR
         s.LastName   LIKE '%' + @Keyword + '%' OR
         s.Email      LIKE '%' + @Keyword + '%')
        AND
        (@DepartmentId IS NULL OR s.DepartmentId = @DepartmentId)
    ORDER BY s.CreatedAt DESC;
END
GO

-- SP: InsertStudent
CREATE OR ALTER PROCEDURE sp_InsertStudent
    @FirstName    NVARCHAR(50),
    @LastName     NVARCHAR(50),
    @Email        NVARCHAR(100),
    @Phone        NVARCHAR(20),
    @DepartmentId INT,
    @GradeLevel   INT
AS
BEGIN
    INSERT INTO Students (FirstName, LastName, Email, Phone, DepartmentId, GradeLevel)
    VALUES (@FirstName, @LastName, @Email, @Phone, @DepartmentId, @GradeLevel);

    SELECT SCOPE_IDENTITY() AS NewId;
END
GO

-- SP: UpdateStudent
CREATE OR ALTER PROCEDURE sp_UpdateStudent
    @StudentId    INT,
    @FirstName    NVARCHAR(50),
    @LastName     NVARCHAR(50),
    @Email        NVARCHAR(100),
    @Phone        NVARCHAR(20),
    @DepartmentId INT,
    @GradeLevel   INT,
    @IsActive     BIT
AS
BEGIN
    UPDATE Students
    SET FirstName    = @FirstName,
        LastName     = @LastName,
        Email        = @Email,
        Phone        = @Phone,
        DepartmentId = @DepartmentId,
        GradeLevel   = @GradeLevel,
        IsActive     = @IsActive
    WHERE StudentId = @StudentId;
END
GO

-- SP: DeleteStudent
CREATE OR ALTER PROCEDURE sp_DeleteStudent
    @StudentId INT
AS
BEGIN
    DELETE FROM Grades  WHERE StudentId = @StudentId;
    DELETE FROM Students WHERE StudentId = @StudentId;
END
GO

-- SP: GetAllDepartments
CREATE OR ALTER PROCEDURE sp_GetAllDepartments
AS
BEGIN
    SELECT DepartmentId, DepartmentName FROM Departments ORDER BY DepartmentName;
END
GO

-- SP: GetGradesByStudent
CREATE OR ALTER PROCEDURE sp_GetGradesByStudent
    @StudentId INT
AS
BEGIN
    SELECT GradeId, StudentId, CourseName, Score, CreatedAt
    FROM Grades
    WHERE StudentId = @StudentId
    ORDER BY CreatedAt DESC;
END
GO

PRINT 'Veritabanı ve Stored Procedure''lar başarıyla oluşturuldu.';
