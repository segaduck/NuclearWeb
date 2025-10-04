# Tasks: Integrated Multi-Module Web Application Platform

**Input**: Design documents from `/specs/002-build-an-integrated/`
**Prerequisites**: plan.md (required), research.md, data-model.md, contracts/

## Execution Flow (main)
```
1. Load plan.md from feature directory
   → Tech stack: .NET 9, Vue.js 3, MySQL 8.0+
   → Structure: backend/, frontend/ separated
2. Load design documents:
   → data-model.md: 7 entities extracted
   → contracts/: 7 API contract files found
   → research.md: 8 technical decisions
   → quickstart.md: 4 user journeys
3. Generate tasks by category:
   → **NEW GATE**: Phase 3.0 UI Prototypes (9 tasks)
   → Setup: Docker, dependencies, database
   → Tests: 7 contract files × ~4 endpoints each = ~44 tests
   → Core: 7 entities, services, endpoints, components
   → Integration: Auth, middleware, file storage
   → Polish: E2E tests, performance, documentation
4. Apply task rules:
   → Different files = mark [P] for parallel
   → Tests before implementation (TDD)
   → Backend and frontend can proceed in parallel after contracts
5. Total tasks: 173 tasks (including 9 prototype tasks)
```

## Format: `[ID] [P?] Description`
- **[P]**: Can run in parallel (different files, no dependencies)
- File paths use Windows paths (E:\AITest\NuclearWeb\...)

## Phase 3.0: UI Prototype Design & Approval ⚠️ GATE: USER MUST APPROVE BEFORE PHASE 3.1
**CRITICAL: Implementation cannot begin until user selects a UI prototype**

### Prototype Generation
- [x] T000 Create prototypes directory structure in specs/002-build-an-integrated/prototypes/
- [x] T001 [P] Generate Prototype A (Minimalist/Clean) with 4 key pages in specs/002-build-an-integrated/prototypes/prototype-A/
- [x] T002 [P] Generate Prototype B (Modern/Bold) with 4 key pages in specs/002-build-an-integrated/prototypes/prototype-B/
- [x] T003 [P] Generate Prototype C (Professional/Corporate) with 4 key pages in specs/002-build-an-integrated/prototypes/prototype-C/
- [x] T004 Create design comparison document in specs/002-build-an-integrated/prototypes/comparison.md

### Key Pages (each prototype includes):
1. **Login page** with theme toggle, light/dark mode preview
2. **Reservations calendar page** with sidebar navigation, month view, event cards
3. **CMS article editor page** with TipTap toolbar, preview panel
4. **Admin dashboard layout** with navigation, role-based menu items

### Design Tokens to Define:
- Color palette (primary, secondary, neutral, semantic colors)
- Typography scale (font families, sizes, weights)
- Spacing system (margins, padding, gaps)
- Component styles (buttons, inputs, cards, modals)
- Light and dark theme variations

### User Approval
- [x] T005 **GATE**: Present all 3 prototypes to user for selection
- [x] T006 **GATE**: User selects preferred prototype (A, B, or C)
- [x] T007 Document selected prototype in specs/002-build-an-integrated/prototypes/design-decision.md
- [x] T008 Extract design tokens from selected prototype to frontend/tailwind.config.js template

**GATE STATUS**: ✅ APPROVED - Prototype C selected, ready for Phase 3.1

---

## Phase 3.1: Setup & Infrastructure (UNBLOCKED - T008 complete)
- [x] T009 Create backend project structure (backend/src/, backend/tests/)
- [x] T010 Initialize .NET 9 solution with projects: NuclearWeb.API, NuclearWeb.Core, NuclearWeb.Infrastructure, NuclearWeb.Application
- [x] T011 Create frontend project structure (frontend/src/, frontend/tests/)
- [x] T012 Initialize Vue.js 3 project with Vite, TypeScript, Vue Router, Pinia, TailwindCSS (apply design tokens from T008)
- [x] T013 [P] Create backend Dockerfile using mcr.microsoft.com/dotnet/aspnet:9.0
- [x] T014 [P] Create frontend Dockerfile for production build
- [x] T015 Create docker-compose.yml with services: mysql, backend, frontend
- [x] T016 Create MySQL initialization script with all 7 entity schemas from data-model.md (centralized in database/init.sql)
- [x] T017 [P] Configure backend appsettings.json (connection strings, JWT settings, CORS)
- [x] T018 [P] Configure frontend environment files (.env.development, .env.production)
- [x] T019 [P] Install backend NuGet packages: EF Core, MySQL Connector, JWT, BCrypt, xUnit, Moq, FluentAssertions
- [x] T020 [P] Install frontend npm packages: axios, @fullcalendar/vue3, @tiptap/vue-3, @headlessui/vue

## Phase 3.2: Tests First - Backend Contract Tests (TDD) ⚠️ MUST COMPLETE BEFORE 3.3
**CRITICAL: These tests MUST be written and MUST FAIL before ANY implementation**

### Authentication API Tests (contracts/auth.yaml)
- [x] T021 [P] Contract test POST /api/v1/auth/login in backend/tests/NuclearWeb.Tests.Contract/AuthControllerTests.cs
- [x] T022 [P] Contract test POST /api/v1/auth/refresh in backend/tests/NuclearWeb.Tests.Contract/AuthControllerTests.cs
- [x] T023 [P] Contract test POST /api/v1/auth/logout in backend/tests/NuclearWeb.Tests.Contract/AuthControllerTests.cs
- [x] T024 [P] Contract test GET /api/v1/auth/me in backend/tests/NuclearWeb.Tests.Contract/AuthControllerTests.cs

### Reservations API Tests (contracts/reservations.yaml)
- [x] T025 [P] Contract test GET /api/v1/reservations in backend/tests/NuclearWeb.Tests.Contract/ReservationsControllerTests.cs
- [x] T026 [P] Contract test POST /api/v1/reservations in backend/tests/NuclearWeb.Tests.Contract/ReservationsControllerTests.cs
- [x] T027 [P] Contract test GET /api/v1/reservations/{id} in backend/tests/NuclearWeb.Tests.Contract/ReservationsControllerTests.cs
- [x] T028 [P] Contract test PUT /api/v1/reservations/{id} in backend/tests/NuclearWeb.Tests.Contract/ReservationsControllerTests.cs
- [x] T029 [P] Contract test DELETE /api/v1/reservations/{id} in backend/tests/NuclearWeb.Tests.Contract/ReservationsControllerTests.cs
- [x] T030 [P] Contract test POST /api/v1/reservations/check-availability in backend/tests/NuclearWeb.Tests.Contract/ReservationsControllerTests.cs

### Rooms API Tests (contracts/rooms.yaml)
- [x] T031 [P] Contract test GET /api/v1/rooms in backend/tests/NuclearWeb.Tests.Contract/RoomsControllerTests.cs
- [x] T032 [P] Contract test POST /api/v1/rooms in backend/tests/NuclearWeb.Tests.Contract/RoomsControllerTests.cs
- [x] T033 [P] Contract test GET /api/v1/rooms/{id} in backend/tests/NuclearWeb.Tests.Contract/RoomsControllerTests.cs
- [x] T034 [P] Contract test PUT /api/v1/rooms/{id} in backend/tests/NuclearWeb.Tests.Contract/RoomsControllerTests.cs
- [x] T035 [P] Contract test DELETE /api/v1/rooms/{id} in backend/tests/NuclearWeb.Tests.Contract/RoomsControllerTests.cs

### Content API Tests (contracts/content.yaml)
- [x] T036 [P] Contract test GET /api/v1/articles in backend/tests/NuclearWeb.Tests.Contract/ArticlesControllerTests.cs
- [x] T037 [P] Contract test POST /api/v1/articles in backend/tests/NuclearWeb.Tests.Contract/ArticlesControllerTests.cs
- [x] T038 [P] Contract test GET /api/v1/articles/{id} in backend/tests/NuclearWeb.Tests.Contract/ArticlesControllerTests.cs
- [x] T039 [P] Contract test PUT /api/v1/articles/{id} in backend/tests/NuclearWeb.Tests.Contract/ArticlesControllerTests.cs
- [x] T040 [P] Contract test DELETE /api/v1/articles/{id} in backend/tests/NuclearWeb.Tests.Contract/ArticlesControllerTests.cs
- [x] T041 [P] Contract test POST /api/v1/articles/{id}/submit in backend/tests/NuclearWeb.Tests.Contract/ArticlesControllerTests.cs
- [x] T042 [P] Contract test POST /api/v1/articles/{id}/approve in backend/tests/NuclearWeb.Tests.Contract/ArticlesControllerTests.cs
- [x] T043 [P] Contract test POST /api/v1/articles/{id}/reject in backend/tests/NuclearWeb.Tests.Contract/ArticlesControllerTests.cs
- [x] T044 [P] Contract test GET /api/v1/articles/published in backend/tests/NuclearWeb.Tests.Contract/ArticlesControllerTests.cs

### Menus API Tests (contracts/menus.yaml)
- [x] T045 [P] Contract test GET /api/v1/menus in backend/tests/NuclearWeb.Tests.Contract/MenusControllerTests.cs
- [x] T046 [P] Contract test POST /api/v1/menus in backend/tests/NuclearWeb.Tests.Contract/MenusControllerTests.cs
- [x] T047 [P] Contract test GET /api/v1/menus/{id} in backend/tests/NuclearWeb.Tests.Contract/MenusControllerTests.cs
- [x] T048 [P] Contract test PUT /api/v1/menus/{id} in backend/tests/NuclearWeb.Tests.Contract/MenusControllerTests.cs
- [x] T049 [P] Contract test DELETE /api/v1/menus/{id} in backend/tests/NuclearWeb.Tests.Contract/MenusControllerTests.cs
- [x] T050 [P] Contract test PUT /api/v1/menus/reorder in backend/tests/NuclearWeb.Tests.Contract/MenusControllerTests.cs

### Users API Tests (contracts/users.yaml)
- [x] T051 [P] Contract test GET /api/v1/users in backend/tests/NuclearWeb.Tests.Contract/UsersControllerTests.cs
- [x] T052 [P] Contract test POST /api/v1/users in backend/tests/NuclearWeb.Tests.Contract/UsersControllerTests.cs
- [x] T053 [P] Contract test GET /api/v1/users/{id} in backend/tests/NuclearWeb.Tests.Contract/UsersControllerTests.cs
- [x] T054 [P] Contract test PUT /api/v1/users/{id} in backend/tests/NuclearWeb.Tests.Contract/UsersControllerTests.cs
- [x] T055 [P] Contract test DELETE /api/v1/users/{id} in backend/tests/NuclearWeb.Tests.Contract/UsersControllerTests.cs
- [x] T056 [P] Contract test POST /api/v1/users/{id}/reset-password in backend/tests/NuclearWeb.Tests.Contract/UsersControllerTests.cs
- [x] T057 [P] Contract test PUT /api/v1/users/me/preferences in backend/tests/NuclearWeb.Tests.Contract/UsersControllerTests.cs

### Files API Tests (contracts/files.yaml)
- [x] T058 [P] Contract test GET /api/v1/files in backend/tests/NuclearWeb.Tests.Contract/FilesControllerTests.cs
- [x] T059 [P] Contract test POST /api/v1/files in backend/tests/NuclearWeb.Tests.Contract/FilesControllerTests.cs
- [x] T060 [P] Contract test GET /api/v1/files/{id} in backend/tests/NuclearWeb.Tests.Contract/FilesControllerTests.cs
- [x] T061 [P] Contract test PUT /api/v1/files/{id} in backend/tests/NuclearWeb.Tests.Contract/FilesControllerTests.cs
- [x] T062 [P] Contract test DELETE /api/v1/files/{id} in backend/tests/NuclearWeb.Tests.Contract/FilesControllerTests.cs
- [x] T063 [P] Contract test GET /api/v1/files/{id}/download in backend/tests/NuclearWeb.Tests.Contract/FilesControllerTests.cs
- [x] T064 [P] Contract test GET /api/v1/files/categories in backend/tests/NuclearWeb.Tests.Contract/FilesControllerTests.cs

## Phase 3.3: Backend Core Models & DbContext (ONLY after tests are failing)
- [x] T065 [P] User entity model in backend/src/NuclearWeb.Core/Entities/User.cs
- [x] T066 [P] MeetingRoom entity model in backend/src/NuclearWeb.Core/Entities/MeetingRoom.cs
- [x] T067 [P] Reservation entity model in backend/src/NuclearWeb.Core/Entities/Reservation.cs
- [x] T068 [P] ContentArticle entity model in backend/src/NuclearWeb.Core/Entities/ContentArticle.cs
- [x] T069 [P] MenuItem entity model in backend/src/NuclearWeb.Core/Entities/MenuItem.cs
- [x] T070 [P] UploadedFile entity model in backend/src/NuclearWeb.Core/Entities/UploadedFile.cs
- [x] T071 [P] RefreshToken entity model in backend/src/NuclearWeb.Core/Entities/RefreshToken.cs
- [x] T072 ApplicationDbContext with all entity configurations in backend/src/NuclearWeb.Infrastructure/Data/ApplicationDbContext.cs
- [x] T073 Entity configuration classes for relationships and indexes in backend/src/NuclearWeb.Infrastructure/Data/Configurations/

## Phase 3.4: Backend Application Layer (Services)
- [x] T074 [P] IAuthService interface in backend/src/NuclearWeb.Core/Interfaces/IAuthService.cs
- [x] T075 [P] IReservationService interface in backend/src/NuclearWeb.Core/Interfaces/IReservationService.cs
- [x] T076 [P] IRoomService interface in backend/src/NuclearWeb.Core/Interfaces/IRoomService.cs
- [x] T077 [P] IArticleService interface in backend/src/NuclearWeb.Core/Interfaces/IArticleService.cs
- [x] T078 [P] IMenuService interface in backend/src/NuclearWeb.Core/Interfaces/IMenuService.cs
- [x] T079 [P] IUserService interface in backend/src/NuclearWeb.Core/Interfaces/IUserService.cs
- [x] T080 [P] IFileService interface in backend/src/NuclearWeb.Core/Interfaces/IFileService.cs
- [ ] T081 AuthService implementation with JWT generation and password hashing in backend/src/NuclearWeb.Application/Services/AuthService.cs
- [ ] T082 ReservationService with conflict detection logic in backend/src/NuclearWeb.Application/Services/ReservationService.cs
- [ ] T083 RoomService with CRUD operations in backend/src/NuclearWeb.Application/Services/RoomService.cs
- [ ] T084 ArticleService with approval workflow state transitions in backend/src/NuclearWeb.Application/Services/ArticleService.cs
- [ ] T085 MenuService with hierarchical menu operations in backend/src/NuclearWeb.Application/Services/MenuService.cs
- [ ] T086 UserService with role-based access logic in backend/src/NuclearWeb.Application/Services/UserService.cs
- [ ] T087 FileService with upload validation (file type, size) in backend/src/NuclearWeb.Application/Services/FileService.cs

## Phase 3.5: Backend API Controllers
- [ ] T088 AuthController with login, refresh, logout, me endpoints in backend/src/NuclearWeb.API/Controllers/AuthController.cs
- [ ] T089 ReservationsController with CRUD and check-availability in backend/src/NuclearWeb.API/Controllers/ReservationsController.cs
- [ ] T090 RoomsController with CRUD operations in backend/src/NuclearWeb.API/Controllers/RoomsController.cs
- [ ] T091 ArticlesController with CRUD, submit, approve, reject in backend/src/NuclearWeb.API/Controllers/ArticlesController.cs
- [ ] T092 MenusController with CRUD and reorder in backend/src/NuclearWeb.API/Controllers/MenusController.cs
- [ ] T093 UsersController with CRUD, reset-password, preferences in backend/src/NuclearWeb.API/Controllers/UsersController.cs
- [ ] T094 FilesController with upload, download, CRUD in backend/src/NuclearWeb.API/Controllers/FilesController.cs

## Phase 3.6: Backend Middleware & Infrastructure
- [ ] T095 JWT authentication middleware in backend/src/NuclearWeb.API/Middleware/JwtAuthenticationMiddleware.cs
- [ ] T096 Role-based authorization policies (Admin, User) in backend/src/NuclearWeb.API/Configuration/AuthorizationPolicies.cs
- [ ] T097 Global exception handling middleware in backend/src/NuclearWeb.API/Middleware/ExceptionHandlingMiddleware.cs
- [ ] T098 Request/response logging middleware in backend/src/NuclearWeb.API/Middleware/RequestLoggingMiddleware.cs
- [ ] T099 CORS configuration for frontend origin in backend/src/NuclearWeb.API/Program.cs
- [ ] T100 File storage infrastructure (local disk storage) in backend/src/NuclearWeb.Infrastructure/FileStorage/LocalFileStorage.cs
- [ ] T101 Database migrations for all entities in backend/src/NuclearWeb.Infrastructure/Migrations/

## Phase 3.7: Frontend Core Setup
- [ ] T102 [P] API client service with axios interceptors in frontend/src/services/api.ts
- [ ] T103 [P] Auth service (login, logout, refresh token) in frontend/src/services/authService.ts
- [ ] T104 [P] Reservations service in frontend/src/services/reservationsService.ts
- [ ] T105 [P] Rooms service in frontend/src/services/roomsService.ts
- [ ] T106 [P] Articles service in frontend/src/services/articlesService.ts
- [ ] T107 [P] Menus service in frontend/src/services/menusService.ts
- [ ] T108 [P] Users service in frontend/src/services/usersService.ts
- [ ] T109 [P] Files service in frontend/src/services/filesService.ts
- [ ] T110 [P] Auth Pinia store in frontend/src/stores/authStore.ts
- [ ] T111 [P] Reservations Pinia store with calendar data caching in frontend/src/stores/reservationsStore.ts
- [ ] T112 [P] Theme Pinia store (light/dark switching) in frontend/src/stores/themeStore.ts
- [ ] T113 [P] Sidebar Pinia store (collapsed state) in frontend/src/stores/sidebarStore.ts
- [ ] T114 Vue Router configuration with auth guards in frontend/src/router/index.ts

## Phase 3.8: Frontend Components - Layout & Navigation
- [ ] T115 AppLayout component with sidebar and header in frontend/src/components/layout/AppLayout.vue
- [ ] T116 Sidebar component with role-based menu items and collapse toggle in frontend/src/components/layout/Sidebar.vue
- [ ] T117 Header component with theme toggle and user menu in frontend/src/components/layout/Header.vue
- [ ] T118 Responsive breakpoint composable for mobile/tablet/desktop in frontend/src/composables/useBreakpoint.ts

## Phase 3.9: Frontend Pages - Authentication
- [ ] T119 Login page with username/password form in frontend/src/pages/Login.vue
- [ ] T120 Logout functionality integrated in Header component

## Phase 3.10: Frontend Pages - Reservations Module
- [ ] T121 Reservations page with FullCalendar integration in frontend/src/pages/reservations/ReservationsPage.vue
- [ ] T122 Calendar toolbar with day/week/month view switcher in frontend/src/components/reservations/CalendarToolbar.vue
- [ ] T123 Create/Edit reservation dialog component in frontend/src/components/reservations/ReservationDialog.vue
- [ ] T124 Conflict detection integration in reservation form in frontend/src/components/reservations/ReservationDialog.vue
- [ ] T125 Reservation details view component in frontend/src/components/reservations/ReservationDetails.vue
- [ ] T126 Room selector component with capacity display in frontend/src/components/reservations/RoomSelector.vue

## Phase 3.11: Frontend Pages - CMS Admin Module
- [ ] T127 Article list page with status filters in frontend/src/pages/cms/ArticleListPage.vue
- [ ] T128 Article editor page with TipTap integration in frontend/src/pages/cms/ArticleEditorPage.vue
- [ ] T129 TipTap toolbar component with multimedia buttons in frontend/src/components/cms/TipTapToolbar.vue
- [ ] T130 Image upload dialog for TipTap in frontend/src/components/cms/ImageUploadDialog.vue
- [ ] T131 YouTube embed dialog for TipTap in frontend/src/components/cms/YouTubeEmbedDialog.vue
- [ ] T132 Article approval workflow UI (submit, approve, reject buttons) in frontend/src/components/cms/ArticleWorkflowActions.vue
- [ ] T133 Available period configuration component in frontend/src/components/cms/AvailablePeriodConfig.vue
- [ ] T134 Menu management page with drag-and-drop reordering in frontend/src/pages/cms/MenuManagementPage.vue
- [ ] T135 Menu item editor dialog (name, link type, parent) in frontend/src/components/cms/MenuItemDialog.vue
- [ ] T136 File upload page with drag-and-drop in frontend/src/pages/cms/FileUploadPage.vue
- [ ] T137 File list component with download links in frontend/src/components/cms/FileList.vue

## Phase 3.12: Frontend Pages - CMS Public Module
- [ ] T138 Public menu component for frontend navigation in frontend/src/components/cms/PublicMenu.vue
- [ ] T139 Article viewer page (public) in frontend/src/pages/cms/ArticleViewerPage.vue
- [ ] T140 Article list page (public, published only) in frontend/src/pages/cms/PublicArticleListPage.vue

## Phase 3.13: Frontend Pages - User Management
- [ ] T141 User list page (admin only) in frontend/src/pages/admin/UserListPage.vue
- [ ] T142 User create/edit dialog in frontend/src/components/admin/UserDialog.vue
- [ ] T143 Password reset dialog in frontend/src/components/admin/PasswordResetDialog.vue
- [ ] T144 User profile page (own profile) in frontend/src/pages/UserProfilePage.vue
- [ ] T145 Preferences editor (theme, sidebar) in frontend/src/components/user/PreferencesEditor.vue

## Phase 3.14: Frontend Pages - Room Management
- [ ] T146 Room list page (admin only) in frontend/src/pages/admin/RoomListPage.vue
- [ ] T147 Room create/edit dialog in frontend/src/components/admin/RoomDialog.vue

## Phase 3.15: Integration Tests (Backend)
- [ ] T148 [P] Integration test: User registration and login flow in backend/tests/NuclearWeb.Tests.Integration/AuthFlowTests.cs
- [ ] T149 [P] Integration test: Create reservation with conflict detection in backend/tests/NuclearWeb.Tests.Integration/ReservationConflictTests.cs
- [ ] T150 [P] Integration test: Article approval workflow (Draft → PendingApproval → Published) in backend/tests/NuclearWeb.Tests.Integration/ArticleWorkflowTests.cs
- [ ] T151 [P] Integration test: Menu hierarchy and reordering in backend/tests/NuclearWeb.Tests.Integration/MenuManagementTests.cs
- [ ] T152 [P] Integration test: File upload with validation in backend/tests/NuclearWeb.Tests.Integration/FileUploadTests.cs
- [ ] T153 [P] Integration test: JWT refresh token flow in backend/tests/NuclearWeb.Tests.Integration/TokenRefreshTests.cs
- [ ] T154 [P] Integration test: Role-based access control (admin vs user) in backend/tests/NuclearWeb.Tests.Integration/AuthorizationTests.cs

## Phase 3.16: E2E Tests (Frontend with Playwright)
- [ ] T155 [P] E2E test: Complete admin workflow from quickstart.md Journey 1 in frontend/tests/e2e/admin-workflow.spec.ts
- [ ] T156 [P] E2E test: Normal user workflow from quickstart.md Journey 2 in frontend/tests/e2e/user-workflow.spec.ts
- [ ] T157 [P] E2E test: Responsive design validation from quickstart.md Journey 3 in frontend/tests/e2e/responsive-design.spec.ts
- [ ] T158 [P] E2E test: Content lifecycle from quickstart.md Journey 4 in frontend/tests/e2e/content-lifecycle.spec.ts

## Phase 3.17: Performance Optimization
- [ ] T159 Performance test: Calendar view switching <200ms in backend/tests/NuclearWeb.Tests.Performance/CalendarPerformanceTests.cs
- [ ] T160 Performance test: Theme switching <100ms in frontend/tests/unit/performance/ThemePerformance.spec.ts
- [ ] T161 Performance test: API response time <500ms (95th percentile) in backend/tests/NuclearWeb.Tests.Performance/ApiResponseTimeTests.cs
- [ ] T162 Implement database query optimization (indexes, eager loading) in backend/src/NuclearWeb.Infrastructure/
- [ ] T163 Implement frontend lazy loading for routes and components in frontend/src/router/index.ts

## Phase 3.18: Polish & Documentation
- [ ] T164 [P] Unit tests for validation helpers in backend/tests/NuclearWeb.Tests.Unit/ValidationTests.cs
- [ ] T165 [P] Unit tests for conflict detection algorithm in backend/tests/NuclearWeb.Tests.Unit/ConflictDetectionTests.cs
- [ ] T166 [P] Unit tests for approval workflow state machine in backend/tests/NuclearWeb.Tests.Unit/WorkflowTests.cs
- [ ] T167 [P] Frontend component unit tests (Vitest) in frontend/tests/unit/components/
- [ ] T168 Code refactoring: Remove duplication in services in backend/src/NuclearWeb.Application/Services/
- [ ] T169 Add API documentation comments (XML docs) in backend/src/NuclearWeb.API/Controllers/
- [ ] T170 Create deployment README.md with Docker instructions in repository root
- [ ] T171 Execute manual testing using quickstart.md scenarios
- [ ] T172 Final security review (CORS, auth, file validation) across backend/src/

## Dependencies
**Critical Path**:
1. **GATE**: Prototypes (T000-T008) MUST complete before any implementation
2. Setup (T009-T020) → All tests (T021-T064) → Models (T065-T073) → Services (T074-T087) → Controllers (T088-T094)
3. Frontend services (T102-T109) require backend contracts defined
4. Integration tests (T148-T154) require both backend and database setup complete
5. E2E tests (T155-T158) require both backend and frontend fully functional

**Parallel Execution Opportunities**:
- Prototype generation (T001-T003) can run in parallel [P]
- All contract tests (T021-T064) can run in parallel [P]
- All entity models (T065-T071) can run in parallel [P]
- All service interfaces (T074-T080) can run in parallel [P]
- All frontend service files (T102-T109) can run in parallel [P]
- All Pinia stores (T110-T113) can run in parallel [P]
- Backend and frontend work can proceed in parallel after T020
- Integration tests (T148-T154) can run in parallel [P]
- E2E tests (T155-T158) can run in parallel [P]

**Blocking Dependencies**:
- **T005-T008 GATE**: User must approve prototype before T009+ can begin
- T012 requires T008 (design tokens extracted)
- T072 (DbContext) requires T065-T071 (all entities)
- T081-T087 (service implementations) require T074-T080 (interfaces) and T072 (DbContext)
- T088-T094 (controllers) require T081-T087 (services)
- T101 (migrations) requires T072 (DbContext)
- T124 (conflict detection UI) requires T082 (ReservationService)
- T132 (workflow UI) requires T084 (ArticleService)
- T148-T154 (integration tests) require T088-T101 (full backend)
- T155-T158 (E2E tests) require T148-T154 (integration tests passing)

## Parallel Execution Example
```bash
# Phase 3.2: Launch all contract tests in parallel (after setup complete)
# Run these 44 tests simultaneously in different test files:
dotnet test backend/tests/NuclearWeb.Tests.Contract/AuthControllerTests.cs &
dotnet test backend/tests/NuclearWeb.Tests.Contract/ReservationsControllerTests.cs &
dotnet test backend/tests/NuclearWeb.Tests.Contract/RoomsControllerTests.cs &
dotnet test backend/tests/NuclearWeb.Tests.Contract/ArticlesControllerTests.cs &
dotnet test backend/tests/NuclearWeb.Tests.Contract/MenusControllerTests.cs &
dotnet test backend/tests/NuclearWeb.Tests.Contract/UsersControllerTests.cs &
dotnet test backend/tests/NuclearWeb.Tests.Contract/FilesControllerTests.cs &

# Phase 3.3: Launch all entity models in parallel
# Create 7 entity files simultaneously (different files)

# Phase 3.7: Launch all frontend services in parallel
# Create 8 service files simultaneously
```

## Validation Checklist
*GATE: Verify before marking Phase 3 complete*

- [x] All 7 contract files have corresponding test tasks (T021-T064 cover all endpoints)
- [x] All 7 entities have model creation tasks (T065-T071)
- [x] All tests come before implementation (Phase 3.2 before 3.3-3.6)
- [x] Parallel tasks are truly independent (different files, no shared state)
- [x] Each task specifies exact file path with project name
- [x] TDD ordering enforced (tests fail first, then implement)
- [x] Quickstart scenarios covered in E2E tests (T155-T158)
- [x] Performance requirements validated (T159-T161)
- [x] Security review included (T172)

## Task Count Summary
- **UI Prototypes (GATE)**: 9 tasks (T000-T008)
- Setup: 12 tasks (T009-T020)
- Contract Tests: 44 tasks (T021-T064)
- Models & DbContext: 9 tasks (T065-T073)
- Service Layer: 14 tasks (T074-T087)
- API Controllers: 7 tasks (T088-T094)
- Backend Infrastructure: 7 tasks (T095-T101)
- Frontend Services & Stores: 13 tasks (T102-T114)
- Frontend Layout: 4 tasks (T115-T118)
- Frontend Auth: 2 tasks (T119-T120)
- Frontend Reservations: 6 tasks (T121-T126)
- Frontend CMS Admin: 11 tasks (T127-T137)
- Frontend CMS Public: 3 tasks (T138-T140)
- Frontend User Management: 5 tasks (T141-T145)
- Frontend Room Management: 2 tasks (T146-T147)
- Backend Integration Tests: 7 tasks (T148-T154)
- Frontend E2E Tests: 4 tasks (T155-T158)
- Performance: 5 tasks (T159-T163)
- Polish: 9 tasks (T164-T172)

**Total: 173 tasks** (including 9 prototype tasks with user approval gate)

## Notes
- **⚠️ CRITICAL GATE**: T000-T008 must complete with user approval before ANY implementation begins
- [P] tasks indicate different files that can be developed in parallel
- Backend and frontend can proceed in parallel after API contracts are defined (T020)
- All tests must fail before implementation (TDD non-negotiable)
- Commit after completing each task
- Run tests continuously during development
- Follow Clean Architecture: Core → Application → Infrastructure → API
- Frontend follows: Services → Stores → Components → Pages
- Docker compose up required for integration and E2E tests
- Performance tests gate deployment readiness
