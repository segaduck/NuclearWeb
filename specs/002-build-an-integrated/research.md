# Research: Technical Decisions

**Feature**: Integrated Multi-Module Web Application Platform
**Date**: 2025-10-03
**Status**: Complete

## Overview

This document contains technical research findings and decisions for the integrated web platform. All decisions prioritize simplicity, proven technologies, and alignment with the constitutional requirement for Linux deployment via Docker.

---

## 1. .NET 9 on Linux Deployment

### Decision
Use **mcr.microsoft.com/dotnet/aspnet:9.0** as the Docker base image for the ASP.NET Core runtime.

### Rationale
- Official Microsoft image optimized for ASP.NET Core applications
- Includes only runtime dependencies (smaller image size vs SDK image)
- Well-tested on Linux environments
- Regular security updates from Microsoft
- Alpine variant available for even smaller footprint (`aspnet:9.0-alpine`)

### Linux-Specific Configuration
- Use **Kestrel** web server (default, works well on Linux)
- Configure for reverse proxy (Nginx) if needed in production
- Set `ASPNETCORE_URLS=http://+:5000` environment variable
- Use `/app` as working directory (Linux convention)
- Run as non-root user for security

### Performance Optimizations
- Enable **Server GC** mode (default for ASP.NET Core)
- Use `--read-only` Docker flag where possible
- Implement health checks (`/health` endpoint)
- Configure connection pooling for database
- Use async/await throughout for I/O operations

### Alternatives Considered
- **.NET SDK image**: Rejected (too large, ~700MB vs ~200MB for runtime)
- **Self-contained deployment**: Rejected (increases deployment size, harder to patch)
- **Windows Containers**: Not applicable (requirement is Linux)

---

## 2. Calendar Component Selection

### Decision
Use **FullCalendar** (v6.x) with Vue 3 adapter (`@fullcalendar/vue3`).

### Rationale
- Most mature and feature-complete calendar library
- Native support for day/week/month views
- Excellent performance (handles 1000+ events)
- Built-in event dragging, resizing, and date selection
- Extensive customization options
- Active development and good documentation
- MIT license (permissive)

### Implementation Approach
- Day view: `timeGridDay` plugin
- Week view: `timeGridWeek` plugin
- Month view: `dayGridMonth` plugin
- Real-time switching: Client-side view change (no server round-trip)
- Events loaded via API: Fetch events for visible date range only

### Performance Strategy (<200ms requirement)
- Client-side view switching (instant - no network call)
- Lazy load events: Fetch only visible date range
- Use `eventSources` with dynamic date range filtering
- Implement client-side caching (Pinia store)
- Debounce filter changes (300ms) to reduce API calls

### Alternatives Considered
- **Vue-Cal**: Rejected (less feature-complete, smaller community)
- **Custom calendar**: Rejected (reinventing the wheel, high maintenance cost)
- **QCalendar**: Rejected (designed for Quasar, heavier integration)

---

## 3. Rich Text Editor for CMS

### Decision
Use **TipTap** (v2.x) as the WYSIWYG editor.

### Rationale
- Headless, framework-agnostic (works perfectly with Vue 3)
- Built on ProseMirror (robust, extensible)
- Modular plugin system
- Excellent Vue 3 integration
- Real-time collaborative editing support (future feature)
- Active development by maintainers
- TypeScript-first

### Multimedia Embedding
- **YouTube**: Use `youtube` extension (paste URL → embed)
- **Images**: Custom upload button → API upload → insert image node
- **Videos**: Similar to images, upload → insert video element
- **Slides**: Upload as PDF → render with `pdf.js` or embed as iframe

### Frontend-Backend Sync
- Store content as **JSON** (TipTap's native format) in database
- Convert to HTML for frontend display using TipTap's `generateHTML()`
- WYSIWYG guarantee: Same JSON → Same HTML (deterministic)
- Version control: Store JSON snapshots for each save

### Alternatives Considered
- **Quill**: Rejected (less extensible, delta format less flexible)
- **CKEditor 5**: Rejected (heavier, more complex licensing for advanced features)
- **ProseMirror directly**: Rejected (too low-level, steeper learning curve)

---

## 4. Authentication & Authorization

### Decision
Use **JWT (JSON Web Tokens)** with refresh token pattern.

### Rationale
- Stateless authentication (scales well, no server-side session storage)
- Works seamlessly with SPA architecture
- Easy to extend to AD SSO/OAuth later (same token pattern)
- Industry standard for REST APIs
- Supports role-based claims in token

### Token Strategy
- **Access Token**: Short-lived (15 minutes), contains user ID + roles
- **Refresh Token**: Long-lived (7 days), stored in HTTP-only cookie
- **Storage**: Access token in memory (Pinia store), refresh token in HTTP-only cookie (secure, httpOnly, sameSite)
- **Rotation**: Refresh tokens rotated on each refresh (security best practice)

### Role-Based Access Control (RBAC)
- Use **Claims-based authorization** in ASP.NET Core
- Roles stored as claims in JWT (`roles: ["Admin"]` or `["User"]`)
- Backend: `[Authorize(Roles = "Admin")]` attribute on controllers
- Frontend: Route guards check token claims before navigation

### Future AD SSO/OAuth Preparation
- Implement OAuth 2.0 flow structure (authorization code flow)
- Keep authentication logic in separate service layer
- Support multiple auth providers via strategy pattern
- JWT token format remains the same (issuer changes)

### Alternatives Considered
- **Session-based auth**: Rejected (stateful, doesn't scale horizontally without sticky sessions)
- **API Keys**: Rejected (not suitable for user-facing application)
- **OAuth only**: Deferred (username/password first, OAuth as future enhancement)

---

## 5. File Upload & Storage

### Decision
Use **filesystem storage** with uploads saved to `/app/uploads/` directory (Docker volume-mounted).

### Rationale
- Simple implementation (no external dependencies)
- Cost-effective for 10GB initial allocation
- Easy to backup (standard file backup tools)
- Fast access (local disk I/O)
- Can migrate to object storage (S3/Azure Blob) later if needed

### Large File Upload Strategy (100MB max)
- **Chunked uploads**: No (complexity not justified for 100MB limit)
- **Direct upload**: Yes (simpler, fewer moving parts)
- **Progress tracking**: Client-side using Axios `onUploadProgress`
- **Timeout**: Set to 5 minutes for 100MB uploads
- **Multipart form-data**: Standard approach

### File Type Validation
- **Client-side**: Check file extension (user experience, fast feedback)
- **Server-side**: Validate MIME type using `MimeTypes` library (security)
- **Double-check**: Read file headers (magic numbers) for critical types
- **Whitelist approach**: Only allow specified extensions

### Security Considerations
- Store files outside web root (prevent direct execution)
- Generate random filenames (UUID) to prevent path traversal
- Validate file size before processing
- Scan for malware: **Optional** (antivirus integration deferred to Phase 2)
- Serve files through controller action (access control enforced)

### Alternatives Considered
- **Object Storage (S3/Azure Blob)**: Deferred (over-engineering for initial scale, add later if needed)
- **Database BLOBs**: Rejected (poor performance, bloats database)
- **CDN**: Deferred (not needed for 10-50 users)

---

## 6. State Management

### Decision
Use **Pinia** (Vue's official state management) with module-based store structure.

### Rationale
- Official Vue.js state management (replaces Vuex)
- Simpler API than Vuex (no mutations, just actions)
- TypeScript-first design
- Excellent Vue 3 Composition API integration
- Dev tools support

### Store Structure
```
stores/
├── auth.ts          # User authentication, roles, token management
├── theme.ts         # Light/dark theme, sidebar collapse state
├── reservations.ts  # Reservation module state
├── rooms.ts         # Meeting rooms data
├── cms.ts           # CMS content, articles, menus
└── files.ts         # File browsing, uploads
```

### Persistent State Strategy
- Use **pinia-plugin-persistedstate**
- Persist to `localStorage`:
  - `theme.current` (light/dark)
  - `theme.sidebarCollapsed` (boolean)
  - `auth.user` (cached user info)
  - `auth.refreshToken` (HTTP-only cookie, not localStorage)

### Module Isolation
- Each store is independent (loosely coupled)
- Cross-store communication via composition (import other stores)
- Shared types in `types/` directory
- API calls in separate `services/` layer (stores call services)

### Alternatives Considered
- **Vuex**: Rejected (deprecated in favor of Pinia)
- **Plain reactive()**: Rejected (no dev tools, manual persistence)
- **Composition API only**: Rejected (global state management needed)

---

## 7. API Design Patterns

### Decision
Follow **RESTful API** conventions with resource-based URLs.

### REST Resource Naming
- Use plural nouns: `/api/reservations`, `/api/rooms`, `/api/articles`
- Nested resources: `/api/rooms/{id}/reservations` (reservations for a room)
- Actions as sub-resources: `/api/articles/{id}/publish`, `/api/articles/{id}/approve`
- Filtering via query params: `/api/reservations?date=2025-10-03&roomId=5`

### Error Response Standardization
```json
{
  "error": {
    "code": "RESERVATION_CONFLICT",
    "message": "Room already booked for this time period",
    "details": {
      "conflictingReservationId": 123,
      "roomId": 5,
      "timeSlot": "2025-10-03T10:00:00Z - 2025-10-03T12:00:00Z"
    }
  }
}
```
- Use HTTP status codes correctly (400, 401, 403, 404, 409, 500)
- Include machine-readable error codes
- Provide helpful error messages
- Optional details object for debugging

### Pagination Strategy
```
GET /api/articles?page=1&pageSize=20
Response:
{
  "data": [...],
  "pagination": {
    "currentPage": 1,
    "pageSize": 20,
    "totalItems": 150,
    "totalPages": 8
  }
}
```
- Use query parameters for page/pageSize
- Return pagination metadata in response
- Default pageSize: 20
- Max pageSize: 100

### API Versioning
- **URL versioning**: `/api/v1/reservations`
- Simple, explicit, easy to route
- Version 1 for initial release
- Deprecation policy: Support previous version for 6 months

### Alternatives Considered
- **GraphQL**: Rejected (over-engineering for this use case, REST is sufficient)
- **Header versioning**: Rejected (less discoverable than URL versioning)
- **No versioning**: Rejected (risky for future changes)

---

## 8. Testing Strategy

### Decision
Use **xUnit** for backend tests, **Vitest** for frontend unit tests, **Playwright** for E2E tests.

### Backend Testing
- **Contract Tests**: xUnit + FluentAssertions + ASP.NET Test Server
  - Test HTTP requests/responses against OpenAPI schema
  - Validate status codes, headers, response shape
  - No database dependency (mocked repository layer)
- **Integration Tests**: xUnit + Test Containers (MySQL)
  - Real database interactions
  - Seed test data before each test
  - Cleanup after each test
- **Unit Tests**: xUnit + Moq
  - Test business logic in isolation
  - Mock dependencies

### Frontend Testing
- **Component Tests**: Vitest + Vue Test Utils
  - Test components in isolation
  - Mock API responses
  - Verify UI behavior
- **E2E Tests**: Playwright
  - Test full user workflows
  - Run against real backend (test environment)
  - Critical paths: login, create reservation, publish article

### Test Data Management
- **Backend**: Use factory pattern for test data generation
- **Frontend**: Mock data files (`mocks/`) imported in tests
- **E2E**: Seed script (`seed-test-data.sql`) run before tests
- **Isolation**: Each test gets fresh database (Test Containers)

### CI/CD Pipeline
```
1. Lint & Format Check
2. Backend Unit Tests (parallel)
3. Frontend Unit Tests (parallel)
4. Backend Integration Tests
5. Contract Tests
6. E2E Tests (critical paths only)
7. Build Docker images
8. Deploy to staging
```

### Alternatives Considered
- **NUnit**: Rejected (xUnit is more modern, better async support)
- **Jest** (frontend): Rejected (Vitest is faster, better Vite integration)
- **Cypress**: Rejected (Playwright has better multi-browser support)

---

## Summary of Key Decisions

| Area | Technology | Justification |
|------|-----------|---------------|
| Backend Runtime | .NET 9 on Linux (Docker) | Official support, proven performance |
| Frontend Framework | Vue 3 + TypeScript | Modern, reactive, excellent DX |
| Calendar | FullCalendar v6 | Most complete, proven, performant |
| Rich Text Editor | TipTap v2 | Headless, extensible, Vue-friendly |
| Authentication | JWT + Refresh Tokens | Stateless, scalable, standard |
| File Storage | Filesystem (Docker volume) | Simple, sufficient for scale |
| State Management | Pinia | Official, simple, TypeScript-first |
| API Design | RESTful | Standard, well-understood |
| Backend Testing | xUnit | Industry standard for .NET |
| Frontend Testing | Vitest + Playwright | Fast, modern, comprehensive |
| Database | MySQL 8.0 | Required, proven, EF Core support |
| CSS Framework | TailwindCSS | Utility-first, highly customizable |

---

## Risks & Mitigations

### Risk: .NET 9 on Linux Performance
- **Mitigation**: Use official Docker images, enable server GC, implement caching
- **Monitoring**: Add health checks, performance logging

### Risk: Large File Uploads (100MB)
- **Mitigation**: Set appropriate timeouts, implement progress tracking
- **Monitoring**: Log upload times, alert on failures

### Risk: Calendar Performance with Many Events
- **Mitigation**: Load only visible date range, implement client-side caching
- **Monitoring**: Track render times, optimize if >200ms

### Risk: WYSIWYG Content Inconsistencies
- **Mitigation**: Use TipTap's JSON format (deterministic), validate on save
- **Monitoring**: Content preview before publish

---

## Open Questions (Deferred to Implementation)

The following decisions can be made during implementation without blocking design:

1. Specific password hashing algorithm (bcrypt vs Argon2)
2. Exact EF Core migration strategy (automatic vs manual)
3. Logging framework configuration (Serilog sinks)
4. Specific Tailwind CSS theme customization
5. Exact health check implementation details
6. Specific monitoring/observability tools

These are implementation details that don't affect the contract design.

---

**Status**: ✅ All critical technical decisions resolved. Ready for Phase 1 (Design & Contracts).
