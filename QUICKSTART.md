# NuclearWeb - Quick Start Guide

## 🚀 Quick Start (Docker)

### Prerequisites
- Docker Desktop installed
- Git installed

### 1. Clone and Start
```bash
# Clone the repository
git clone <repository-url>
cd NuclearWeb

# Start all services
docker-compose up -d

# Check status
docker-compose ps
```

### 2. Access the Application
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **API Documentation**: http://localhost:5000/swagger

### 3. Default Credentials
Create an admin user (run this after database is initialized):
```bash
# Connect to backend container
docker exec -it nuclearweb-backend bash

# Use EF Core to add seed data or create via API
curl -X POST http://localhost:5000/api/v1/users \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "email": "admin@example.com",
    "password": "Admin123!",
    "displayName": "System Administrator",
    "role": "Admin"
  }'
```

## 💻 Local Development

### Backend Setup (.NET 9)

```bash
cd backend

# Restore dependencies
dotnet restore

# Update database connection string in appsettings.Development.json
# Default: Server=localhost;Port=3306;Database=nuclearweb;User=root;Password=password;

# Run database migrations
cd src/NuclearWeb.Infrastructure
dotnet ef database update --startup-project ../NuclearWeb.API

# Run the API
cd ../NuclearWeb.API
dotnet run
```

**Backend will be available at**: http://localhost:5000

### Frontend Setup (Vue.js 3)

```bash
cd frontend

# Install dependencies
npm install

# Configure API endpoint in .env.development
# VITE_API_BASE_URL=http://localhost:5000/api/v1

# Run development server
npm run dev
```

**Frontend will be available at**: http://localhost:5173

## 🔧 Configuration

### Backend Configuration
Edit `backend/src/NuclearWeb.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=nuclearweb;User=root;Password=password;"
  },
  "JwtSettings": {
    "Secret": "your-super-secret-key-minimum-32-characters-long",
    "Issuer": "NuclearWeb",
    "Audience": "NuclearWebUsers",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  },
  "CorsSettings": {
    "AllowedOrigins": ["http://localhost:3000", "http://localhost:5173"]
  },
  "FileStorageSettings": {
    "UploadPath": "./uploads",
    "MaxFileSizeBytes": 10485760
  }
}
```

### Frontend Configuration
Edit `frontend/.env.development`:

```env
VITE_API_BASE_URL=http://localhost:5000/api/v1
```

## 🧪 Running Tests

### Backend Tests
```bash
cd backend

# Run all tests
dotnet test

# Run specific test project
dotnet test tests/NuclearWeb.Tests.Unit
dotnet test tests/NuclearWeb.Tests.Contract
dotnet test tests/NuclearWeb.Tests.Integration
```

### Frontend Tests
```bash
cd frontend

# Run unit tests (when implemented)
npm run test:unit

# Run E2E tests (when implemented)
npm run test:e2e
```

## 📊 Database Management

### Create New Migration
```bash
cd backend/src/NuclearWeb.Infrastructure

dotnet ef migrations add <MigrationName> \
  --startup-project ../NuclearWeb.API \
  --context ApplicationDbContext
```

### Apply Migrations
```bash
dotnet ef database update \
  --startup-project ../NuclearWeb.API \
  --context ApplicationDbContext
```

### Remove Last Migration
```bash
dotnet ef migrations remove \
  --startup-project ../NuclearWeb.API \
  --context ApplicationDbContext
```

## 🔐 Authentication Flow

### 1. Login
```bash
POST /api/v1/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin123!"
}

Response:
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "550e8400...",
  "user": {
    "id": 1,
    "username": "admin",
    "email": "admin@example.com",
    "displayName": "System Administrator",
    "role": "Admin"
  }
}
```

### 2. Use Access Token
```bash
GET /api/v1/auth/me
Authorization: Bearer eyJhbGc...
```

### 3. Refresh Token
```bash
POST /api/v1/auth/refresh
Content-Type: application/json

{
  "refreshToken": "550e8400..."
}

Response:
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "661f9500..."
}
```

### 4. Logout
```bash
POST /api/v1/auth/logout
Content-Type: application/json

{
  "refreshToken": "550e8400..."
}
```

## 📡 API Endpoints

### Authentication
- `POST /api/v1/auth/login` - User login
- `POST /api/v1/auth/refresh` - Refresh access token
- `POST /api/v1/auth/logout` - User logout
- `GET /api/v1/auth/me` - Get current user profile

### Reservations
- `GET /api/v1/reservations` - List reservations (with filters)
- `POST /api/v1/reservations` - Create reservation
- `GET /api/v1/reservations/{id}` - Get reservation details
- `PUT /api/v1/reservations/{id}` - Update reservation
- `DELETE /api/v1/reservations/{id}` - Cancel reservation
- `POST /api/v1/reservations/check-availability` - Check time slot availability

### Rooms
- `GET /api/v1/rooms` - List meeting rooms
- `POST /api/v1/rooms` - Create room (Admin only)
- `GET /api/v1/rooms/{id}` - Get room details
- `PUT /api/v1/rooms/{id}` - Update room (Admin only)
- `DELETE /api/v1/rooms/{id}` - Delete room (Admin only)

### Articles (CMS)
- `GET /api/v1/articles` - List articles (with filters)
- `POST /api/v1/articles` - Create article (Admin only)
- `GET /api/v1/articles/{id}` - Get article details
- `PUT /api/v1/articles/{id}` - Update article (Admin only)
- `DELETE /api/v1/articles/{id}` - Delete article (Admin only)
- `POST /api/v1/articles/{id}/submit` - Submit for approval
- `POST /api/v1/articles/{id}/approve` - Approve article (Admin only)
- `POST /api/v1/articles/{id}/reject` - Reject article (Admin only)
- `GET /api/v1/articles/published` - List published articles (Public)

### Menus
- `GET /api/v1/menus` - List menu items
- `POST /api/v1/menus` - Create menu item (Admin only)
- `GET /api/v1/menus/{id}` - Get menu item
- `PUT /api/v1/menus/{id}` - Update menu item (Admin only)
- `DELETE /api/v1/menus/{id}` - Delete menu item (Admin only)
- `PUT /api/v1/menus/reorder` - Reorder menu items (Admin only)

### Users
- `GET /api/v1/users` - List users (Admin only)
- `POST /api/v1/users` - Create user (Admin only)
- `GET /api/v1/users/{id}` - Get user details (Admin only)
- `PUT /api/v1/users/{id}` - Update user (Admin only)
- `DELETE /api/v1/users/{id}` - Delete user (Admin only)
- `POST /api/v1/users/{id}/reset-password` - Reset password (Admin only)
- `PUT /api/v1/users/me/preferences` - Update own preferences

### Files
- `GET /api/v1/files` - List files
- `POST /api/v1/files` - Upload file
- `GET /api/v1/files/{id}` - Get file metadata
- `PUT /api/v1/files/{id}` - Update file metadata
- `DELETE /api/v1/files/{id}` - Delete file
- `GET /api/v1/files/{id}/download` - Download file
- `GET /api/v1/files/categories` - List file categories

## 🎨 Frontend Features

### Implemented
- ✅ Login page with authentication
- ✅ Responsive layout with collapsible sidebar
- ✅ Light/Dark theme switching
- ✅ Role-based navigation menu
- ✅ Auto token refresh
- ✅ Protected routes with auth guards

### Placeholder Pages (Ready for Implementation)
- Reservations calendar
- CMS article list & editor
- Menu management
- File upload
- User management
- Room management
- User profile

## 🐛 Troubleshooting

### Backend won't start
```bash
# Check if MySQL is running
docker ps | grep mysql

# Check logs
dotnet run --project backend/src/NuclearWeb.API --verbosity detailed
```

### Database connection issues
```bash
# Test MySQL connection
mysql -h localhost -P 3306 -u root -p

# Verify connection string in appsettings.json
```

### Frontend can't connect to backend
```bash
# Check CORS settings in backend appsettings.json
# Verify VITE_API_BASE_URL in frontend/.env.development
# Check browser console for CORS errors
```

### JWT token errors
```bash
# Ensure JWT secret is at least 32 characters
# Check token expiry settings
# Verify Authorization header format: "Bearer <token>"
```

## 📚 Additional Resources

- **API Documentation**: http://localhost:5000/swagger
- **Frontend Dev Server**: http://localhost:5173
- **MySQL Admin**: Use phpMyAdmin or MySQL Workbench

## 🎯 What's Working

✅ **Backend**:
- All 7 API controllers fully functional
- JWT authentication with refresh tokens
- Role-based authorization
- Global exception handling
- Request logging
- File upload/download
- Database migrations

✅ **Frontend**:
- Authentication flow (login/logout)
- Auto token refresh
- Theme switching
- Responsive layout
- API service layer
- State management with Pinia
- Protected routes

⚠️ **Pending Full UI**:
- Rich calendar component (reservations)
- TipTap editor (CMS)
- Drag-and-drop (files, menus)
- User management forms
- Room management forms

---

**Need Help?** Check `IMPLEMENTATION_STATUS.md` for detailed progress report.
