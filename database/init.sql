-- =============================================================================
-- NuclearWeb Database Initialization Script
-- =============================================================================
-- Purpose: Complete database setup (schema, indexes, seed data)
-- Database: MySQL 8.0+
-- Generated from: specs/002-build-an-integrated/data-model.md
-- Date: 2025-10-04
--
-- IMPORTANT: This is the SINGLE source of truth for database initialization.
-- All schema changes, indexes, and seed data must be centralized here.
-- =============================================================================

-- Set timezone and character set
SET TIME_ZONE = '+08:00';
SET NAMES utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Use the database
USE nuclearweb;

-- =============================================================================
-- SCHEMA CREATION
-- =============================================================================

-- 1. Users Table
CREATE TABLE IF NOT EXISTS `Users` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Username` VARCHAR(50) NOT NULL,
    `PasswordHash` VARCHAR(255) NOT NULL,
    `DisplayName` VARCHAR(100) NOT NULL,
    `Email` VARCHAR(255) NOT NULL,
    `Role` VARCHAR(20) NOT NULL DEFAULT 'User',
    `ThemePreference` VARCHAR(10) NOT NULL DEFAULT 'Light',
    `SidebarCollapsed` BOOLEAN NOT NULL DEFAULT FALSE,
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `CreatedAt` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedAt` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
    `LastLoginAt` DATETIME(6) NULL,

    PRIMARY KEY (`Id`),
    UNIQUE KEY `UX_Users_Username` (`Username`),
    UNIQUE KEY `UX_Users_Email` (`Email`),
    INDEX `IX_Users_Role` (`Role`),
    INDEX `IX_Users_IsActive` (`IsActive`),

    CONSTRAINT `CK_Users_Role` CHECK (`Role` IN ('Admin', 'User')),
    CONSTRAINT `CK_Users_ThemePreference` CHECK (`ThemePreference` IN ('Light', 'Dark'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='使用者資料表 - 儲存系統使用者（管理員和一般使用者）';

-- 2. MeetingRooms Table
CREATE TABLE IF NOT EXISTS `MeetingRooms` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Name` VARCHAR(100) NOT NULL,
    `Capacity` INT NOT NULL,
    `Location` VARCHAR(255) NULL,
    `Amenities` JSON NULL,
    `IsActive` BOOLEAN NOT NULL DEFAULT TRUE,
    `CreatedAt` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedAt` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),

    PRIMARY KEY (`Id`),
    UNIQUE KEY `UX_MeetingRooms_Name` (`Name`),
    INDEX `IX_MeetingRooms_IsActive` (`IsActive`),

    CONSTRAINT `CK_MeetingRooms_Capacity` CHECK (`Capacity` >= 1 AND `Capacity` <= 1000)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='會議室資料表 - 可預約的會議室資訊';

-- 3. Reservations Table
CREATE TABLE IF NOT EXISTS `Reservations` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `MeetingRoomId` INT NOT NULL,
    `UserId` INT NOT NULL,
    `StartTime` DATETIME(6) NOT NULL,
    `EndTime` DATETIME(6) NOT NULL,
    `Purpose` VARCHAR(500) NULL,
    `AttendeeCount` INT NULL,
    `Status` VARCHAR(20) NOT NULL DEFAULT 'Confirmed',
    `CreatedAt` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedAt` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
    `CreatedBy` INT NOT NULL,
    `ModifiedBy` INT NULL,

    PRIMARY KEY (`Id`),
    INDEX `IX_Reservations_MeetingRoomId` (`MeetingRoomId`),
    INDEX `IX_Reservations_UserId` (`UserId`),
    INDEX `IX_Reservations_StartTime_EndTime` (`StartTime`, `EndTime`),
    INDEX `IX_Reservations_Status` (`Status`),
    INDEX `IX_Reservations_Conflict` (`MeetingRoomId`, `StartTime`, `EndTime`),

    CONSTRAINT `FK_Reservations_MeetingRooms` FOREIGN KEY (`MeetingRoomId`) REFERENCES `MeetingRooms`(`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Reservations_Users` FOREIGN KEY (`UserId`) REFERENCES `Users`(`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Reservations_CreatedBy` FOREIGN KEY (`CreatedBy`) REFERENCES `Users`(`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Reservations_ModifiedBy` FOREIGN KEY (`ModifiedBy`) REFERENCES `Users`(`Id`) ON DELETE RESTRICT,
    CONSTRAINT `CK_Reservations_Status` CHECK (`Status` IN ('Confirmed', 'Cancelled')),
    CONSTRAINT `CK_Reservations_Times` CHECK (`EndTime` > `StartTime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='會議室預約資料表 - 儲存使用者的會議室預約紀錄';

-- 4. ContentArticles Table
CREATE TABLE IF NOT EXISTS `ContentArticles` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Title` VARCHAR(255) NOT NULL,
    `Content` JSON NOT NULL,
    `AuthorId` INT NOT NULL,
    `PublicationStatus` VARCHAR(20) NOT NULL DEFAULT 'Draft',
    `AvailableFrom` DATETIME(6) NULL,
    `AvailableUntil` DATETIME(6) NULL,
    `ViewCount` INT NOT NULL DEFAULT 0,
    `CreatedAt` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedAt` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),
    `PublishedAt` DATETIME(6) NULL,
    `PublishedBy` INT NULL,

    PRIMARY KEY (`Id`),
    INDEX `IX_ContentArticles_AuthorId` (`AuthorId`),
    INDEX `IX_ContentArticles_PublicationStatus` (`PublicationStatus`),
    INDEX `IX_ContentArticles_Availability` (`PublicationStatus`, `AvailableFrom`, `AvailableUntil`),
    INDEX `IX_ContentArticles_PublishedAt` (`PublishedAt`),

    CONSTRAINT `FK_ContentArticles_Authors` FOREIGN KEY (`AuthorId`) REFERENCES `Users`(`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_ContentArticles_Publishers` FOREIGN KEY (`PublishedBy`) REFERENCES `Users`(`Id`) ON DELETE RESTRICT,
    CONSTRAINT `CK_ContentArticles_Status` CHECK (`PublicationStatus` IN ('Draft', 'PendingApproval', 'Published', 'Rejected'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='內容文章資料表 - CMS系統的知識文章';

-- 5. MenuItems Table
CREATE TABLE IF NOT EXISTS `MenuItems` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Name` VARCHAR(100) NOT NULL,
    `ParentId` INT NULL,
    `DisplayOrder` INT NOT NULL DEFAULT 0,
    `LinkType` VARCHAR(20) NOT NULL,
    `ArticleId` INT NULL,
    `ExternalUrl` VARCHAR(500) NULL,
    `IsVisible` BOOLEAN NOT NULL DEFAULT TRUE,
    `CreatedAt` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `UpdatedAt` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6) ON UPDATE CURRENT_TIMESTAMP(6),

    PRIMARY KEY (`Id`),
    INDEX `IX_MenuItems_ParentId` (`ParentId`),
    INDEX `IX_MenuItems_DisplayOrder` (`DisplayOrder`),
    INDEX `IX_MenuItems_ArticleId` (`ArticleId`),

    CONSTRAINT `FK_MenuItems_Parent` FOREIGN KEY (`ParentId`) REFERENCES `MenuItems`(`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_MenuItems_Articles` FOREIGN KEY (`ArticleId`) REFERENCES `ContentArticles`(`Id`) ON DELETE RESTRICT,
    CONSTRAINT `CK_MenuItems_LinkType` CHECK (`LinkType` IN ('Article', 'ExternalUrl'))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='選單項目資料表 - CMS前端導航選單';

-- 6. UploadedFiles Table
CREATE TABLE IF NOT EXISTS `UploadedFiles` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `FileName` VARCHAR(255) NOT NULL,
    `StoredFileName` VARCHAR(255) NOT NULL,
    `FileType` VARCHAR(50) NOT NULL,
    `FileExtension` VARCHAR(10) NOT NULL,
    `FileSizeBytes` BIGINT NOT NULL,
    `UploadedBy` INT NOT NULL,
    `UploadedAt` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `Description` VARCHAR(500) NULL,
    `Category` VARCHAR(50) NULL,
    `DownloadCount` INT NOT NULL DEFAULT 0,

    PRIMARY KEY (`Id`),
    UNIQUE KEY `UX_UploadedFiles_StoredFileName` (`StoredFileName`),
    INDEX `IX_UploadedFiles_UploadedBy` (`UploadedBy`),
    INDEX `IX_UploadedFiles_Category` (`Category`),
    INDEX `IX_UploadedFiles_FileType` (`FileType`),

    CONSTRAINT `FK_UploadedFiles_Users` FOREIGN KEY (`UploadedBy`) REFERENCES `Users`(`Id`) ON DELETE RESTRICT,
    CONSTRAINT `CK_UploadedFiles_FileSize` CHECK (`FileSizeBytes` > 0 AND `FileSizeBytes` <= 104857600)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='上傳檔案資料表 - 系統中可下載的檔案';

-- 7. RefreshTokens Table (Authentication)
CREATE TABLE IF NOT EXISTS `RefreshTokens` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `UserId` INT NOT NULL,
    `Token` VARCHAR(255) NOT NULL,
    `ExpiresAt` DATETIME(6) NOT NULL,
    `CreatedAt` DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    `RevokedAt` DATETIME(6) NULL,
    `ReplacedByToken` VARCHAR(255) NULL,

    PRIMARY KEY (`Id`),
    UNIQUE KEY `UX_RefreshTokens_Token` (`Token`),
    INDEX `IX_RefreshTokens_UserId` (`UserId`),
    INDEX `IX_RefreshTokens_ExpiresAt` (`ExpiresAt`),

    CONSTRAINT `FK_RefreshTokens_Users` FOREIGN KEY (`UserId`) REFERENCES `Users`(`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='刷新權杖資料表 - JWT認證的刷新權杖';

-- =============================================================================
-- SEED DATA
-- =============================================================================

-- Insert default admin user
-- Password: Admin@123 (bcrypt hash with cost factor 11, generated with BCrypt.Net-Next 4.0.3)
INSERT INTO `Users` (`Username`, `PasswordHash`, `DisplayName`, `Email`, `Role`, `IsActive`)
VALUES
('admin', '$2a$11$JiUUXtMAzuFufhKNBdV88.0DCveoFU1jUKZ1UfTqWK9G3iGB6n3kG', '系統管理員', 'admin@nuclearweb.local', 'Admin', TRUE),
('user1', '$2a$11$JiUUXtMAzuFufhKNBdV88.0DCveoFU1jUKZ1UfTqWK9G3iGB6n3kG', '測試使用者', 'user1@nuclearweb.local', 'User', TRUE),
('李主管', '$2a$11$JiUUXtMAzuFufhKNBdV88.0DCveoFU1jUKZ1UfTqWK9G3iGB6n3kG', '李主管', 'lee@nuclearweb.local', 'User', TRUE),
('王經理', '$2a$11$JiUUXtMAzuFufhKNBdV88.0DCveoFU1jUKZ1UfTqWK9G3iGB6n3kG', '王經理', 'wang@nuclearweb.local', 'User', TRUE),
('張組長', '$2a$11$JiUUXtMAzuFufhKNBdV88.0DCveoFU1jUKZ1UfTqWK9G3iGB6n3kG', '張組長', 'chang@nuclearweb.local', 'User', TRUE),
('陳工程師', '$2a$11$JiUUXtMAzuFufhKNBdV88.0DCveoFU1jUKZ1UfTqWK9G3iGB6n3kG', '陳工程師', 'chen@nuclearweb.local', 'User', TRUE)
ON DUPLICATE KEY UPDATE `Username` = `Username`;

-- Insert sample meeting rooms
INSERT INTO `MeetingRooms` (`Name`, `Capacity`, `Location`, `Amenities`, `IsActive`)
VALUES
('大會議室A', 20, '1樓', JSON_ARRAY('投影機', '白板', '視訊會議設備', '電話'), TRUE),
('小會議室B', 8, '2樓', JSON_ARRAY('白板', '電話'), TRUE),
('會議室C', 12, '2樓', JSON_ARRAY('投影機', '白板', '視訊會議設備'), TRUE),
('訓練教室D', 30, '3樓', JSON_ARRAY('投影機', '白板', '音響設備'), TRUE)
ON DUPLICATE KEY UPDATE `Name` = `Name`;

-- Insert sample reservations (matching prototype examples with realistic daily schedule)
-- User IDs: 1=admin, 2=user1, 3=李主管, 4=王經理, 5=張組長, 6=陳工程師
INSERT INTO `Reservations` (`MeetingRoomId`, `UserId`, `StartTime`, `EndTime`, `Purpose`, `AttendeeCount`, `Status`, `CreatedBy`)
VALUES
-- Today's reservations (Full day schedule 8:00-18:00)
-- Morning (8:00-12:00)
(1, 5, '2025-10-06 08:00:00', '2025-10-06 09:00:00', '晨會', 15, 'Confirmed', 5),
(2, 6, '2025-10-06 09:00:00', '2025-10-06 10:00:00', '技術研討', 6, 'Confirmed', 6),
(3, 1, '2025-10-06 10:00:00', '2025-10-06 11:00:00', '週會', 8, 'Confirmed', 1),
(4, 4, '2025-10-06 08:30:00', '2025-10-06 10:30:00', '新人訓練', 20, 'Confirmed', 4),
(1, 3, '2025-10-06 10:00:00', '2025-10-06 11:30:00', '部門報告', 12, 'Confirmed', 3),

-- Afternoon - Currently happening (13:00-15:00)
(2, 4, '2025-10-06 13:00:00', '2025-10-06 15:00:00', '專案進度檢討', 6, 'Confirmed', 4),
(3, 5, '2025-10-06 14:00:00', '2025-10-06 15:30:00', '預算會議', 10, 'Confirmed', 5),

-- Later today - Next meetings (15:00-18:00)
(1, 3, '2025-10-06 15:00:00', '2025-10-06 16:00:00', '客戶簡報', 15, 'Confirmed', 3),
(4, 2, '2025-10-06 15:30:00', '2025-10-06 17:00:00', '安全審查', 25, 'Confirmed', 2),
(2, 6, '2025-10-06 16:00:00', '2025-10-06 17:00:00', '系統維護討論', 5, 'Confirmed', 6),
(3, 1, '2025-10-06 16:30:00', '2025-10-06 18:00:00', '月度總結', 12, 'Confirmed', 1),

-- Tomorrow's reservations
(1, 2, '2025-10-07 09:00:00', '2025-10-07 10:00:00', '團隊會議', 12, 'Confirmed', 2),
(2, 1, '2025-10-07 14:00:00', '2025-10-07 15:00:00', '技術討論', 5, 'Confirmed', 1),
(3, 3, '2025-10-07 10:00:00', '2025-10-07 12:00:00', '設計評審', 10, 'Confirmed', 3),
(4, 2, '2025-10-07 13:00:00', '2025-10-07 17:00:00', '員工訓練', 25, 'Confirmed', 2),

-- This week (next 2-6 days)
(1, 4, '2025-10-08 10:00:00', '2025-10-08 11:30:00', '供應商會議', 8, 'Confirmed', 4),
(2, 5, '2025-10-09 14:00:00', '2025-10-09 16:00:00', '品質檢討', 6, 'Confirmed', 5),
(3, 6, '2025-10-10 09:00:00', '2025-10-10 10:00:00', '技術分享', 12, 'Confirmed', 6),
(4, 1, '2025-10-11 13:00:00', '2025-10-11 15:00:00', '緊急應變演練', 30, 'Confirmed', 1),

-- Next week reservations
(1, 1, '2025-10-13 10:00:00', '2025-10-13 12:00:00', '月度審查會議', 18, 'Confirmed', 1),
(3, 2, '2025-10-13 14:00:00', '2025-10-13 16:00:00', '專案啟動會議', 10, 'Confirmed', 2),
(2, 3, '2025-10-14 09:00:00', '2025-10-14 11:00:00', '策略規劃', 8, 'Confirmed', 3),
(4, 4, '2025-10-15 15:00:00', '2025-10-15 17:00:00', '教育訓練', 28, 'Confirmed', 4)
ON DUPLICATE KEY UPDATE `Purpose` = `Purpose`;

-- Insert sample content articles
INSERT INTO `ContentArticles` (`Title`, `Content`, `AuthorId`, `PublicationStatus`, `AvailableFrom`, `PublishedAt`, `PublishedBy`)
VALUES
(
    '歡迎使用核能安全委員會知識平台',
    JSON_OBJECT(
        'type', 'doc',
        'content', JSON_ARRAY(
            JSON_OBJECT(
                'type', 'paragraph',
                'content', JSON_ARRAY(
                    JSON_OBJECT('type', 'text', 'text', '歡迎使用本系統！這是一個整合的會議室預約與內容管理平台。')
                )
            )
        )
    ),
    1,
    'Published',
    NOW(),
    NOW(),
    1
),
(
    '系統使用指南',
    JSON_OBJECT(
        'type', 'doc',
        'content', JSON_ARRAY(
            JSON_OBJECT(
                'type', 'heading',
                'attrs', JSON_OBJECT('level', 2),
                'content', JSON_ARRAY(JSON_OBJECT('type', 'text', 'text', '系統功能'))
            ),
            JSON_OBJECT(
                'type', 'paragraph',
                'content', JSON_ARRAY(
                    JSON_OBJECT('type', 'text', 'text', '本系統提供會議室預約、內容管理、檔案下載等功能。')
                )
            )
        )
    ),
    1,
    'Published',
    NOW(),
    NOW(),
    1
)
ON DUPLICATE KEY UPDATE `Title` = `Title`;

-- Insert sample menu items
INSERT INTO `MenuItems` (`Name`, `ParentId`, `DisplayOrder`, `LinkType`, `ArticleId`, `IsVisible`)
VALUES
('首頁', NULL, 1, 'Article', 1, TRUE),
('使用指南', NULL, 2, 'Article', 2, TRUE),
('相關連結', NULL, 3, 'ExternalUrl', NULL, TRUE)
ON DUPLICATE KEY UPDATE `Name` = `Name`;

-- Update ExternalUrl for the menu item (cannot be done in INSERT due to constraint)
UPDATE `MenuItems` SET `ExternalUrl` = 'https://www.aec.gov.tw/' WHERE `Name` = '相關連結';

-- =============================================================================
-- VERIFICATION QUERIES (commented out - for manual verification)
-- =============================================================================

-- SELECT 'Users Created:' AS Info, COUNT(*) AS Count FROM Users;
-- SELECT 'MeetingRooms Created:' AS Info, COUNT(*) AS Count FROM MeetingRooms;
-- SELECT 'ContentArticles Created:' AS Info, COUNT(*) AS Count FROM ContentArticles;
-- SELECT 'MenuItems Created:' AS Info, COUNT(*) AS Count FROM MenuItems;

-- =============================================================================
-- END OF INITIALIZATION SCRIPT
-- =============================================================================
