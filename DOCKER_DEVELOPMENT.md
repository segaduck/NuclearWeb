# Docker-Based Development Guide

## 🐳 Development Environment Setup

**IMPORTANT**: All development, testing, and package installation happens **inside Docker containers**.
**DO NOT** install Node.js, .NET, or MySQL on your host machine.

---

## 🚀 Quick Start

### 1. Start Development Environment
```bash
# Start all services in development mode
docker-compose up -d

# Check status
docker-compose ps

# View logs
docker-compose logs -f
```

### 2. Access Services
- **Frontend Dev Server**: http://localhost:3000
- **Backend API**: http://localhost:5000
- **API Documentation**: http://localhost:5000/swagger
- **MySQL**: localhost:3306 (accessible from containers)

---

## 🔧 Development Workflow

### Backend Development (Inside Container)

#### Access Backend Container
```bash
# Enter backend container shell
docker exec -it nuclearweb-backend bash

# Or use docker-compose
docker-compose exec backend bash
```

#### Inside Backend Container:
```bash
# Install new NuGet package
dotnet add package <PackageName>

# Run migrations
cd src/NuclearWeb.Infrastructure
dotnet ef database update --startup-project ../NuclearWeb.API

# Create new migration
dotnet ef migrations add <MigrationName> --startup-project ../NuclearWeb.API

# Run tests
dotnet test

# Run specific test project
dotnet test tests/NuclearWeb.Tests.Contract
dotnet test tests/NuclearWeb.Tests.Integration
dotnet test tests/NuclearWeb.Tests.Unit

# Build
dotnet build

# Run backend directly
dotnet run --project src/NuclearWeb.API
```

### Frontend Development (Inside Container)

#### Access Frontend Container
```bash
# Enter frontend container shell
docker exec -it nuclearweb-frontend sh

# Or use docker-compose
docker-compose exec frontend sh
```

#### Inside Frontend Container:
```bash
# Install new npm package
npm install <package-name>

# Install development dependency
npm install -D <package-name>

# Install remaining UI libraries (when ready)
npm install @fullcalendar/vue3 @fullcalendar/daygrid @fullcalendar/interaction
npm install @tiptap/vue-3 @tiptap/starter-kit @tiptap/extension-image @tiptap/extension-youtube

# Run tests (when implemented)
npm run test:unit
npm run test:e2e

# Build for production
npm run build

# Lint
npm run lint
```

### Database Management (Inside Container)

#### Access MySQL Container
```bash
# Connect to MySQL shell
docker exec -it nuclearweb-mysql mysql -u root -p
# Password: password (from docker-compose.yml)

# Or use docker-compose
docker-compose exec mysql mysql -u root -p
```

#### Inside MySQL:
```sql
-- Show databases
SHOW DATABASES;

-- Use NuclearWeb database
USE nuclearweb;

-- Show tables
SHOW TABLES;

-- Check users
SELECT * FROM Users;

-- Create admin user manually
INSERT INTO Users (Username, Email, PasswordHash, DisplayName, Role, IsActive, CreatedAt, UpdatedAt)
VALUES (
  'admin',
  'admin@example.com',
  '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYIr.yGG7i6', -- BCrypt hash of "Admin123!"
  'System Administrator',
  'Admin',
  1,
  NOW(),
  NOW()
);
```

---

## 📦 Package Management

### Backend (NuGet Packages)
All packages must be installed **inside the backend container**:

```bash
# Enter container
docker exec -it nuclearweb-backend bash

# Install packages
dotnet add src/NuclearWeb.API/NuclearWeb.API.csproj package <PackageName>
dotnet add src/NuclearWeb.Infrastructure/NuclearWeb.Infrastructure.csproj package <PackageName>

# Restore all packages
dotnet restore

# List installed packages
dotnet list package
```

### Frontend (npm Packages)
All packages must be installed **inside the frontend container**:

```bash
# Enter container
docker exec -it nuclearweb-frontend sh

# Install packages
npm install <package-name>

# Install dev dependencies
npm install -D <package-name>

# List installed packages
npm list --depth=0

# Update package.json and package-lock.json
npm install
```

---

## 🧪 Running Tests (Inside Containers)

### Backend Tests

```bash
# Enter backend container
docker exec -it nuclearweb-backend bash

# Run all tests
dotnet test

# Run with verbosity
dotnet test --verbosity detailed

# Run specific test project
dotnet test tests/NuclearWeb.Tests.Contract
dotnet test tests/NuclearWeb.Tests.Integration
dotnet test tests/NuclearWeb.Tests.Unit

# Run with coverage (if configured)
dotnet test --collect:"XPlat Code Coverage"

# Run tests in watch mode
dotnet watch test --project tests/NuclearWeb.Tests.Unit
```

### Frontend Tests

```bash
# Enter frontend container
docker exec -it nuclearweb-frontend sh

# Run unit tests (when implemented)
npm run test:unit

# Run unit tests in watch mode
npm run test:unit -- --watch

# Run E2E tests (when implemented)
npm run test:e2e

# Run E2E tests headless
npm run test:e2e -- --headless
```

---

## 🔄 Development Cycle

### Typical Workflow:

1. **Edit Code on Host** (using your IDE/editor)
   - Files are mounted via Docker volumes
   - Changes are reflected immediately in containers

2. **Run Commands in Container**
   ```bash
   # Backend changes
   docker exec -it nuclearweb-backend dotnet build

   # Frontend changes - auto-reload via Vite HMR
   # Just save file, browser auto-refreshes
   ```

3. **Run Tests in Container**
   ```bash
   # Backend
   docker exec -it nuclearweb-backend dotnet test

   # Frontend
   docker exec -it nuclearweb-frontend npm run test:unit
   ```

4. **View Logs**
   ```bash
   # All services
   docker-compose logs -f

   # Specific service
   docker-compose logs -f backend
   docker-compose logs -f frontend
   docker-compose logs -f mysql
   ```

---

## 🛠️ Useful Docker Commands

### Container Management
```bash
# Start all services
docker-compose up -d

# Stop all services
docker-compose down

# Restart specific service
docker-compose restart backend
docker-compose restart frontend

# Rebuild and restart (after Dockerfile changes)
docker-compose up -d --build

# View running containers
docker-compose ps

# Remove all containers and volumes
docker-compose down -v
```

### Logs and Debugging
```bash
# View logs
docker-compose logs -f

# View logs for specific service
docker-compose logs -f backend
docker-compose logs -f frontend
docker-compose logs -f mysql

# View last 100 lines
docker-compose logs --tail=100 backend

# Follow logs from now
docker-compose logs -f --tail=0 backend
```

### Execute Commands in Containers
```bash
# Backend
docker-compose exec backend bash
docker-compose exec backend dotnet --version
docker-compose exec backend dotnet build
docker-compose exec backend dotnet test

# Frontend
docker-compose exec frontend sh
docker-compose exec frontend node --version
docker-compose exec frontend npm --version
docker-compose exec frontend npm install <package>

# MySQL
docker-compose exec mysql mysql -u root -p
docker-compose exec mysql mysqldump -u root -p nuclearweb > backup.sql
```

### Cleanup
```bash
# Remove stopped containers
docker-compose rm

# Remove all (including volumes - CAUTION: deletes database)
docker-compose down -v

# Prune Docker system
docker system prune -a
```

---

## 🔍 Troubleshooting

### Backend Container Won't Start
```bash
# Check logs
docker-compose logs backend

# Check if port 5000 is in use
netstat -ano | findstr :5000  # Windows
lsof -i :5000                 # Linux/Mac

# Rebuild
docker-compose up -d --build backend
```

### Frontend Container Won't Start
```bash
# Check logs
docker-compose logs frontend

# Check if port 3000 is in use
netstat -ano | findstr :3000  # Windows
lsof -i :3000                 # Linux/Mac

# Rebuild
docker-compose up -d --build frontend
```

### Database Connection Issues
```bash
# Check MySQL is running
docker-compose ps mysql

# Check MySQL logs
docker-compose logs mysql

# Test connection from backend
docker-compose exec backend dotnet ef database update --startup-project src/NuclearWeb.API

# Test connection from MySQL client
docker-compose exec mysql mysql -u root -p -e "SHOW DATABASES;"
```

### Package Installation Issues
```bash
# Frontend - clear npm cache
docker-compose exec frontend npm cache clean --force
docker-compose exec frontend rm -rf node_modules package-lock.json
docker-compose exec frontend npm install

# Backend - clear NuGet cache
docker-compose exec backend dotnet nuget locals all --clear
docker-compose exec backend dotnet restore
```

---

## 📝 Important Notes

### ✅ DO:
- ✅ Edit code on your host machine (with IDE)
- ✅ Run all commands inside Docker containers
- ✅ Install packages inside containers
- ✅ Run tests inside containers
- ✅ Use `docker-compose exec` for commands
- ✅ Use volume mounts for live code updates

### ❌ DON'T:
- ❌ Install Node.js on host machine
- ❌ Install .NET SDK on host machine
- ❌ Install MySQL on host machine
- ❌ Run `npm install` on host
- ❌ Run `dotnet restore` on host
- ❌ Run tests on host

---

## 🎯 Next Steps for Implementation

### To Complete UI Components (T121-T147):

```bash
# 1. Enter frontend container
docker exec -it nuclearweb-frontend sh

# 2. Install required UI libraries
npm install @fullcalendar/vue3 @fullcalendar/daygrid @fullcalendar/timegrid @fullcalendar/interaction
npm install @tiptap/vue-3 @tiptap/starter-kit @tiptap/extension-image @tiptap/extension-youtube
npm install @vueuse/core

# 3. Exit container
exit

# 4. Edit components on host machine
# Files will auto-reload in browser via Vite HMR
```

### To Run Tests (T148-T158):

```bash
# Backend Integration Tests
docker exec -it nuclearweb-backend bash
cd /app
dotnet test tests/NuclearWeb.Tests.Integration
exit

# Frontend E2E Tests (when implemented)
docker exec -it nuclearweb-frontend sh
npm run test:e2e
exit
```

---

## 📚 Additional Resources

- Docker Compose Docs: https://docs.docker.com/compose/
- .NET in Docker: https://docs.docker.com/samples/dotnet/
- Vue.js in Docker: https://v3.vuejs.org/guide/installation.html#docker

---

**Remember**: Everything happens in Docker! Your host machine only needs Docker Desktop and your code editor. 🐳
