-- Login System Database Schema
-- This file contains the SQL schema for the Active Directory Login System

-- Create database (uncomment if needed)
-- CREATE DATABASE LoginSystemDB;
-- GO

-- Use the database
USE LoginSystemDB;
GO

-- Create Users table
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AdUsername NVARCHAR(256) NOT NULL UNIQUE,
    Email NVARCHAR(256) NULL,
    DisplayName NVARCHAR(256) NULL,
    Role NVARCHAR(50) NOT NULL DEFAULT 'Viewer',
    LastLogin DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Create LoginHistory table
CREATE TABLE LoginHistories (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Username NVARCHAR(256) NOT NULL,
    LoginTimeUtc DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Success BIT NOT NULL,
    IpAddress NVARCHAR(45) NULL,
    UserAgent NVARCHAR(500) NULL,
    FailureReason NVARCHAR(500) NULL,
    UserId UNIQUEIDENTIFIER NULL,
    CONSTRAINT FK_LoginHistories_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE SET NULL
);

-- Create Contents table
CREATE TABLE Contents (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Title NVARCHAR(200) NOT NULL,
    Body NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(256) NOT NULL,
    UpdatedBy NVARCHAR(256) NULL,
    IsPublished BIT NOT NULL DEFAULT 0
);

-- Create indexes for better performance
CREATE INDEX IX_Users_AdUsername ON Users(AdUsername);
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_Role ON Users(Role);
CREATE INDEX IX_Users_IsActive ON Users(IsActive);

CREATE INDEX IX_LoginHistories_LoginTimeUtc ON LoginHistories(LoginTimeUtc);
CREATE INDEX IX_LoginHistories_Username ON LoginHistories(Username);
CREATE INDEX IX_LoginHistories_UserId ON LoginHistories(UserId);

CREATE INDEX IX_Contents_CreatedAt ON Contents(CreatedAt);
CREATE INDEX IX_Contents_IsPublished ON Contents(IsPublished);
CREATE INDEX IX_Contents_CreatedBy ON Contents(CreatedBy);

-- Insert sample data for demonstration
INSERT INTO Contents (Id, Title, Body, CreatedBy, IsPublished, CreatedAt) VALUES
(NEWID(), 'Welcome to the Login System', 'This is a sample content item that demonstrates the role-based access control system. Viewers can see this content, editors can modify it, and admins have full control.', 'system', 1, GETUTCDATE()),
(NEWID(), 'System Documentation', 'This content is only visible to authenticated users. Different roles have different levels of access to content management features.', 'system', 1, GETUTCDATE()),
(NEWID(), 'Admin Guide', 'This content is only visible to administrators. It contains sensitive information about system administration.', 'system', 0, GETUTCDATE());

-- Create a view for recent login activity
CREATE VIEW vw_RecentLogins AS
SELECT TOP 100
    lh.Username,
    lh.LoginTimeUtc,
    lh.Success,
    lh.IpAddress,
    u.DisplayName,
    u.Role
FROM LoginHistories lh
LEFT JOIN Users u ON lh.UserId = u.Id
ORDER BY lh.LoginTimeUtc DESC;

-- Create a stored procedure for user statistics
CREATE PROCEDURE sp_GetUserStatistics
AS
BEGIN
    SELECT 
        COUNT(*) as TotalUsers,
        COUNT(CASE WHEN IsActive = 1 THEN 1 END) as ActiveUsers,
        COUNT(CASE WHEN Role = 'Admin' THEN 1 END) as AdminUsers,
        COUNT(CASE WHEN Role = 'Editor' THEN 1 END) as EditorUsers,
        COUNT(CASE WHEN Role = 'Viewer' THEN 1 END) as ViewerUsers
    FROM Users;
END;

-- Create a stored procedure for login statistics
CREATE PROCEDURE sp_GetLoginStatistics
    @Days INT = 30
AS
BEGIN
    SELECT 
        COUNT(*) as TotalLogins,
        COUNT(CASE WHEN Success = 1 THEN 1 END) as SuccessfulLogins,
        COUNT(CASE WHEN Success = 0 THEN 1 END) as FailedLogins,
        COUNT(DISTINCT Username) as UniqueUsers
    FROM LoginHistories
    WHERE LoginTimeUtc >= DATEADD(DAY, -@Days, GETUTCDATE());
END;

GO

-- Grant permissions (adjust as needed for your environment)
-- GRANT SELECT, INSERT, UPDATE, DELETE ON Users TO [YourAppUser];
-- GRANT SELECT, INSERT, UPDATE, DELETE ON LoginHistories TO [YourAppUser];
-- GRANT SELECT, INSERT, UPDATE, DELETE ON Contents TO [YourAppUser];
-- GRANT EXECUTE ON sp_GetUserStatistics TO [YourAppUser];
-- GRANT EXECUTE ON sp_GetLoginStatistics TO [YourAppUser];

PRINT 'Database schema created successfully!'; 