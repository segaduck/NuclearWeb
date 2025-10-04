# Data Model: Integrated Multi-Module Web Application

**Feature**: Integrated Multi-Module Web Application Platform
**Date**: 2025-10-03
**Database**: MySQL 8.0+
**ORM**: Entity Framework Core 9.0

## Overview

This document defines all entities, their fields, relationships, validation rules, and state transitions for the integrated platform. The model supports two main modules: Meeting Room Reservations and CMS (Content Management System).

---

## Entity Relationship Diagram

```
┌─────────────┐       ┌──────────────┐
│    User     │──────<│ Reservation  │
└─────────────┘       └──────────────┘
      │                      │
      │                      │
      │               ┌──────────────┐
      │               │ MeetingRoom  │
      │               └──────────────┘
      │
      │               ┌────────────────┐
      ├──────────────<│ ContentArticle │
      │               └────────────────┘
      │                      │
      │                      │
      │               ┌──────────────┐
      │               │   MenuItem   │
      │               └──────────────┘
      │
      │               ┌──────────────┐
      └──────────────<│ UploadedFile │
                      └──────────────┘
```

---

## Core Entities

### 1. User

Represents authenticated users of the system (both normal users and administrators).

#### Fields

| Field | Type | Nullable | Description | Validation |
|-------|------|----------|-------------|------------|
| Id | int | No | Primary key, auto-increment | - |
| Username | string(50) | No | Unique login identifier | Unique, 3-50 chars, alphanumeric + underscore |
| PasswordHash | string(255) | No | Bcrypt hashed password | Required, stored as hash only |
| DisplayName | string(100) | No | User's display name | 1-100 chars |
| Email | string(255) | No | User's email address | Valid email format, unique |
| Role | string(20) | No | User role | Enum: "Admin" or "User" |
| ThemePreference | string(10) | No | UI theme preference | Enum: "Light" or "Dark", default "Light" |
| SidebarCollapsed | bool | No | Sidebar UI state | Default false |
| IsActive | bool | No | Account active status | Default true |
| CreatedAt | datetime | No | Account creation timestamp | Auto-set on insert |
| UpdatedAt | datetime | No | Last modification timestamp | Auto-update on change |
| LastLoginAt | datetime | Yes | Last successful login | Nullable, updated on login |

#### Relationships

- **Has Many**: Reservations (created by user)
- **Has Many**: ContentArticles (authored by user)
- **Has Many**: UploadedFiles (uploaded by user)

#### Indexes

- Unique index on `Username`
- Unique index on `Email`
- Index on `Role` (for role-based queries)
- Index on `IsActive` (for active user queries)

#### Business Rules

- Username cannot be changed after creation
- Password must be hashed using bcrypt (cost factor 12)
- Email must be unique across all users
- Only one active session per user (handled via refresh token)
- Default role is "User" unless explicitly set to "Admin"

---

### 2. MeetingRoom

Represents physical meeting rooms available for reservation.

#### Fields

| Field | Type | Nullable | Description | Validation |
|-------|------|----------|-------------|------------|
| Id | int | No | Primary key, auto-increment | - |
| Name | string(100) | No | Room name/identifier | Unique, 1-100 chars |
| Capacity | int | No | Maximum occupancy | Min 1, max 1000 |
| Location | string(255) | Yes | Physical location description | Max 255 chars |
| Amenities | string(500) | Yes | JSON array of amenities | Valid JSON array |
| IsActive | bool | No | Room availability status | Default true |
| CreatedAt | datetime | No | Record creation timestamp | Auto-set on insert |
| UpdatedAt | datetime | No | Last modification timestamp | Auto-update on change |

#### Example Amenities JSON
```json
["Projector", "Whiteboard", "Video Conference", "Phone"]
```

#### Relationships

- **Has Many**: Reservations (bookings for this room)

#### Indexes

- Unique index on `Name`
- Index on `IsActive` (for available rooms query)

#### Business Rules

- Room name must be unique
- Inactive rooms cannot be reserved (soft delete pattern)
- Amenities stored as JSON for flexibility

---

### 3. Reservation

Represents a meeting room booking by a user.

#### Fields

| Field | Type | Nullable | Description | Validation |
|-------|------|----------|-------------|------------|
| Id | int | No | Primary key, auto-increment | - |
| MeetingRoomId | int | No | Foreign key to MeetingRoom | Required, must exist |
| UserId | int | No | Foreign key to User (creator) | Required, must exist |
| StartTime | datetime | No | Reservation start | Required, future datetime |
| EndTime | datetime | No | Reservation end | Required, after StartTime |
| Purpose | string(500) | Yes | Reservation description | Max 500 chars |
| AttendeeCount | int | Yes | Expected number of attendees | Min 1, max room capacity |
| Status | string(20) | No | Reservation status | Enum: "Confirmed", "Cancelled" |
| CreatedAt | datetime | No | Record creation timestamp | Auto-set on insert |
| UpdatedAt | datetime | No | Last modification timestamp | Auto-update on change |
| CreatedBy | int | No | User who created (denormalized) | Same as UserId initially |
| ModifiedBy | int | Yes | Last user who modified | Nullable, set on updates |

#### Relationships

- **Belongs To**: MeetingRoom
- **Belongs To**: User (creator)

#### Indexes

- Index on `MeetingRoomId` (for room schedule queries)
- Index on `UserId` (for user reservations queries)
- Index on `StartTime, EndTime` (for conflict detection)
- Index on `Status` (for filtering)
- Composite index on `MeetingRoomId, StartTime, EndTime` (conflict checking)

#### Business Rules

- **Conflict Detection**: No overlapping reservations for same room
  - Query: `WHERE MeetingRoomId = {roomId} AND Status = 'Confirmed' AND NOT (EndTime <= {newStart} OR StartTime >= {newEnd})`
- EndTime must be after StartTime
- Cannot modify or cancel reservations created by other users (except Admins)
- Cancelled reservations remain in database for audit trail
- AttendeeCount cannot exceed room capacity

#### State Transitions

```
[New] → Confirmed (on creation)
Confirmed → Cancelled (user cancels)
```

No way back from Cancelled state (audit trail).

---

### 4. ContentArticle

Represents CMS articles in the knowledge database.

#### Fields

| Field | Type | Nullable | Description | Validation |
|-------|------|----------|-------------|------------|
| Id | int | No | Primary key, auto-increment | - |
| Title | string(255) | No | Article title | Required, 1-255 chars |
| Content | text | No | Article body (TipTap JSON) | Required, valid JSON |
| AuthorId | int | No | Foreign key to User | Required, must exist |
| PublicationStatus | string(20) | No | Current status | Enum: "Draft", "PendingApproval", "Published", "Rejected" |
| AvailableFrom | datetime | Yes | Start of availability period | Required for Published status |
| AvailableUntil | datetime | Yes | End of availability period | Nullable (null = indefinite) |
| ViewCount | int | No | Number of views | Default 0, auto-increment on view |
| CreatedAt | datetime | No | Article creation timestamp | Auto-set on insert |
| UpdatedAt | datetime | No | Last modification timestamp | Auto-update on change |
| PublishedAt | datetime | Yes | Publication timestamp | Set when status → Published |
| PublishedBy | int | Yes | Admin who approved | Foreign key to User, nullable |

#### Relationships

- **Belongs To**: User (author)
- **Belongs To**: User (publisher - admin who approved)
- **Has Many**: MenuItems (articles can be linked from multiple menu items)

#### Indexes

- Index on `AuthorId` (for author's articles query)
- Index on `PublicationStatus` (for filtering)
- Composite index on `PublicationStatus, AvailableFrom, AvailableUntil` (for frontend queries)
- Index on `PublishedAt` (for recent articles)

#### Business Rules

- Content stored as TipTap JSON format
- AvailableFrom required when status changes to Published
- AvailableUntil can be null (indefinite availability)
- Frontend displays only: `PublicationStatus = 'Published' AND AvailableFrom <= NOW() AND (AvailableUntil IS NULL OR AvailableUntil >= NOW())`
- Admins can see all statuses; normal users see only Published (within availability)

#### State Transitions

```
[New] → Draft (on creation)
Draft → PendingApproval (author submits)
PendingApproval → Published (admin approves + sets period)
PendingApproval → Rejected (admin rejects)
Published → Draft (admin unpublishes)
Rejected → Draft (author revises)
```

---

### 5. MenuItem

Represents navigation menu items in the CMS frontend.

#### Fields

| Field | Type | Nullable | Description | Validation |
|-------|------|----------|-------------|------------|
| Id | int | No | Primary key, auto-increment | - |
| Name | string(100) | No | Menu item display name | Required, 1-100 chars |
| ParentId | int | Yes | Parent menu item ID | Nullable (null = top-level) |
| DisplayOrder | int | No | Sort order within level | Default 0, used for ordering |
| LinkType | string(20) | No | Type of link | Enum: "Article", "ExternalUrl" |
| ArticleId | int | Yes | Linked article ID | Required if LinkType = "Article" |
| ExternalUrl | string(500) | Yes | External URL | Required if LinkType = "ExternalUrl" |
| IsVisible | bool | No | Visibility flag | Default true |
| CreatedAt | datetime | No | Record creation timestamp | Auto-set on insert |
| UpdatedAt | datetime | No | Last modification timestamp | Auto-update on change |

#### Relationships

- **Belongs To**: MenuItem (parent - self-referential)
- **Has Many**: MenuItems (children)
- **Belongs To**: ContentArticle (if LinkType = "Article")

#### Indexes

- Index on `ParentId` (for hierarchical queries)
- Index on `DisplayOrder` (for sorting)
- Index on `ArticleId` (for article menu lookups)

#### Business Rules

- Two-level hierarchy maximum (main menu + sub-menu)
  - ParentId IS NULL → Main menu item
  - ParentId IS NOT NULL → Sub-menu item
- DisplayOrder determines sort within each level
- If LinkType = "Article", ArticleId must be set; ExternalUrl must be null
- If LinkType = "ExternalUrl", ExternalUrl must be set; ArticleId must be null
- Frontend shows only visible items linked to available articles

---

### 6. UploadedFile

Represents files uploaded to the system for download.

#### Fields

| Field | Type | Nullable | Description | Validation |
|-------|------|----------|-------------|------------|
| Id | int | No | Primary key, auto-increment | - |
| FileName | string(255) | No | Original filename | Required, 1-255 chars |
| StoredFileName | string(255) | No | UUID-based storage filename | Unique, auto-generated |
| FileType | string(50) | No | MIME type | Required, validated |
| FileExtension | string(10) | No | File extension | Required, validated |
| FileSizeBytes | long | No | File size in bytes | Required, max 104857600 (100MB) |
| UploadedBy | int | No | Foreign key to User | Required, must exist |
| UploadedAt | datetime | No | Upload timestamp | Auto-set on insert |
| Description | string(500) | Yes | File description | Max 500 chars |
| Category | string(50) | Yes | File category/tag | Max 50 chars |
| DownloadCount | int | No | Number of downloads | Default 0, auto-increment |

#### Relationships

- **Belongs To**: User (uploader)

#### Indexes

- Index on `UploadedBy` (for user uploads query)
- Index on `Category` (for category browsing)
- Index on `FileType` (for type filtering)
- Unique index on `StoredFileName`

#### Business Rules

- Allowed file types (validated on upload):
  - Documents: PDF, DOC, DOCX, XLS, XLSX, PPT, PPTX
  - Images: PNG, JPG, JPEG, GIF, SVG
  - Videos: MP4, AVI, MOV, WMV
  - Archives: ZIP, RAR, 7Z
- Max file size: 100MB (104,857,600 bytes)
- StoredFileName = UUID v4 + original extension (prevents filename conflicts and path traversal)
- Files stored outside web root (`/app/uploads/` in Docker container)
- Access control: Served via controller action, not direct file access

---

## Supporting Entities

### 7. RefreshToken (Authentication)

Represents refresh tokens for JWT authentication.

#### Fields

| Field | Type | Nullable | Description | Validation |
|-------|------|----------|-------------|------------|
| Id | int | No | Primary key, auto-increment | - |
| UserId | int | No | Foreign key to User | Required, must exist |
| Token | string(255) | No | Refresh token value | Unique, UUID |
| ExpiresAt | datetime | No | Token expiration | Required, typically +7 days |
| CreatedAt | datetime | No | Token creation timestamp | Auto-set on insert |
| RevokedAt | datetime | Yes | Token revocation timestamp | Nullable, set on logout/refresh |
| ReplacedByToken | string(255) | Yes | New token on rotation | Nullable |

#### Relationships

- **Belongs To**: User

#### Indexes

- Unique index on `Token`
- Index on `UserId` (for user token queries)
- Index on `ExpiresAt` (for cleanup queries)

#### Business Rules

- Only one active refresh token per user
- Tokens rotated on each refresh (old token revoked)
- Expired tokens auto-deleted by background job
- Tokens stored in HTTP-only cookies on client

---

## Database Schema Script (MySQL)

```sql
CREATE DATABASE IF NOT EXISTS nuclearweb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE nuclearweb;

-- Users table
CREATE TABLE Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL,
    DisplayName VARCHAR(100) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Role VARCHAR(20) NOT NULL DEFAULT 'User',
    ThemePreference VARCHAR(10) NOT NULL DEFAULT 'Light',
    SidebarCollapsed BOOLEAN NOT NULL DEFAULT FALSE,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    LastLoginAt DATETIME NULL,
    INDEX idx_role (Role),
    INDEX idx_is_active (IsActive)
) ENGINE=InnoDB;

-- MeetingRooms table
CREATE TABLE MeetingRooms (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL UNIQUE,
    Capacity INT NOT NULL,
    Location VARCHAR(255) NULL,
    Amenities JSON NULL,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    INDEX idx_is_active (IsActive)
) ENGINE=InnoDB;

-- Reservations table
CREATE TABLE Reservations (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    MeetingRoomId INT NOT NULL,
    UserId INT NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,
    Purpose VARCHAR(500) NULL,
    AttendeeCount INT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Confirmed',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CreatedBy INT NOT NULL,
    ModifiedBy INT NULL,
    FOREIGN KEY (MeetingRoomId) REFERENCES MeetingRooms(Id) ON DELETE RESTRICT,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE RESTRICT,
    INDEX idx_meeting_room_id (MeetingRoomId),
    INDEX idx_user_id (UserId),
    INDEX idx_time_range (StartTime, EndTime),
    INDEX idx_status (Status),
    INDEX idx_conflict_check (MeetingRoomId, StartTime, EndTime)
) ENGINE=InnoDB;

-- ContentArticles table
CREATE TABLE ContentArticles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Content TEXT NOT NULL,
    AuthorId INT NOT NULL,
    PublicationStatus VARCHAR(20) NOT NULL DEFAULT 'Draft',
    AvailableFrom DATETIME NULL,
    AvailableUntil DATETIME NULL,
    ViewCount INT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PublishedAt DATETIME NULL,
    PublishedBy INT NULL,
    FOREIGN KEY (AuthorId) REFERENCES Users(Id) ON DELETE RESTRICT,
    FOREIGN KEY (PublishedBy) REFERENCES Users(Id) ON DELETE SET NULL,
    INDEX idx_author_id (AuthorId),
    INDEX idx_publication_status (PublicationStatus),
    INDEX idx_availability (PublicationStatus, AvailableFrom, AvailableUntil),
    INDEX idx_published_at (PublishedAt)
) ENGINE=InnoDB;

-- MenuItems table
CREATE TABLE MenuItems (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    ParentId INT NULL,
    DisplayOrder INT NOT NULL DEFAULT 0,
    LinkType VARCHAR(20) NOT NULL,
    ArticleId INT NULL,
    ExternalUrl VARCHAR(500) NULL,
    IsVisible BOOLEAN NOT NULL DEFAULT TRUE,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (ParentId) REFERENCES MenuItems(Id) ON DELETE CASCADE,
    FOREIGN KEY (ArticleId) REFERENCES ContentArticles(Id) ON DELETE SET NULL,
    INDEX idx_parent_id (ParentId),
    INDEX idx_display_order (DisplayOrder),
    INDEX idx_article_id (ArticleId)
) ENGINE=InnoDB;

-- UploadedFiles table
CREATE TABLE UploadedFiles (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FileName VARCHAR(255) NOT NULL,
    StoredFileName VARCHAR(255) NOT NULL UNIQUE,
    FileType VARCHAR(50) NOT NULL,
    FileExtension VARCHAR(10) NOT NULL,
    FileSizeBytes BIGINT NOT NULL,
    UploadedBy INT NOT NULL,
    UploadedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description VARCHAR(500) NULL,
    Category VARCHAR(50) NULL,
    DownloadCount INT NOT NULL DEFAULT 0,
    FOREIGN KEY (UploadedBy) REFERENCES Users(Id) ON DELETE RESTRICT,
    INDEX idx_uploaded_by (UploadedBy),
    INDEX idx_category (Category),
    INDEX idx_file_type (FileType)
) ENGINE=InnoDB;

-- RefreshTokens table
CREATE TABLE RefreshTokens (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    Token VARCHAR(255) NOT NULL UNIQUE,
    ExpiresAt DATETIME NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    RevokedAt DATETIME NULL,
    ReplacedByToken VARCHAR(255) NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    INDEX idx_user_id (UserId),
    INDEX idx_expires_at (ExpiresAt)
) ENGINE=InnoDB;
```

---

## Validation Summary

### Cross-Entity Validation

1. **Reservation Conflict**: No overlapping reservations for same room (handled by composite index + application logic)
2. **Content Availability**: Published articles only visible within availability period
3. **Menu Links**: MenuItem can only link to published, available articles
4. **File Type Whitelist**: Strict file extension and MIME type validation
5. **User Permissions**: Users can only edit/delete their own reservations (except Admins)

### Data Integrity

- All foreign keys use `RESTRICT` or `CASCADE` appropriately
- Timestamps auto-managed by database
- Soft deletes via `IsActive` flag (Users, MeetingRooms)
- Hard deletes for RefreshTokens (security)
- Audit trail preserved for Reservations (Status = "Cancelled")

---

**Status**: ✅ Data model complete and ready for contract generation.
