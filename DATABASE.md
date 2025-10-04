# Database Management Guidelines

## Single Source of Truth Principle

**CRITICAL RULE**: All MySQL database initialization must be centralized in a **single file**.

### File Location
```
database/init.sql
```

### What Must Be Included

This single file must contain (in order):

1. **Schema Creation**
   - All `CREATE TABLE` statements
   - Primary keys, foreign keys, constraints
   - Indexes (unique, composite, single-column)
   - Check constraints
   - Table comments (in Traditional Chinese)

2. **Seed Data**
   - Default admin user
   - Sample/initial data for testing
   - Reference data (meeting rooms, etc.)

3. **Verification Queries** (commented out)
   - Useful for manual verification
   - Should not execute automatically

### Why Single File?

✅ **Benefits:**
- Single source of truth for database state
- Easier to track changes in git
- Prevents initialization order issues
- Simplifies docker-compose volume mounting
- Reduces debugging complexity

❌ **Avoid:**
- Multiple numbered files (01-schema.sql, 02-seed.sql, etc.)
- Separate directories for migrations
- Splitting schema and data into different files

### Docker Integration

The `docker-compose.yml` mounts this file as read-only:

```yaml
volumes:
  - ./database/init.sql:/docker-entrypoint-initdb.d/init.sql:ro
```

### Making Changes

When modifying the database:

1. **During Development**: Edit `database/init.sql` directly
2. **Reset Database**:
   ```bash
   docker-compose down -v  # Remove volumes
   docker-compose up -d mysql  # Recreate with fresh init
   ```

3. **Production Migrations**: Use EF Core migrations (separate from init.sql)

### Entity Framework Core Migrations

- **Development**: Use `init.sql` for quick setup
- **Production**: Use EF Core migrations for schema changes
- **Separation**: EF migrations are in `backend/src/NuclearWeb.Infrastructure/Migrations/`

### File Structure

```
NuclearWeb/
├── database/
│   └── init.sql              # SINGLE source of truth
├── backend/
│   └── src/
│       └── NuclearWeb.Infrastructure/
│           └── Migrations/   # EF Core migrations (production)
└── docker-compose.yml        # Mounts init.sql
```

## Default Credentials

### Admin User
- **Username**: `admin`
- **Password**: `Admin@123`
- **Email**: `admin@nuclearweb.local`
- **Role**: Admin

### Test User
- **Username**: `user1`
- **Password**: `Admin@123`
- **Email**: `user1@nuclearweb.local`
- **Role**: User

**⚠️ SECURITY**: Change default passwords in production!

## Database Connection

### Docker Compose
```
Server: mysql
Database: nuclearweb
User: nuclearweb_user
Password: nuclearweb_pass_2024
```

### Local Development
```
Server: localhost:3306
Database: nuclearweb
User: nuclearweb_user
Password: nuclearweb_pass_2024
```

## Verification

After database initialization, verify with:

```sql
USE nuclearweb;
SELECT 'Users' AS Table, COUNT(*) AS Count FROM Users
UNION ALL
SELECT 'MeetingRooms', COUNT(*) FROM MeetingRooms
UNION ALL
SELECT 'ContentArticles', COUNT(*) FROM ContentArticles
UNION ALL
SELECT 'MenuItems', COUNT(*) FROM MenuItems;
```

Expected results:
- Users: 2
- MeetingRooms: 4
- ContentArticles: 2
- MenuItems: 3
