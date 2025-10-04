
# Implementation Plan: Integrated Multi-Module Web Application Platform

**Branch**: `002-build-an-integrated` | **Date**: 2025-10-03 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/002-build-an-integrated/spec.md`

## Execution Flow (/plan command scope)
```
1. Load feature spec from Input path
   → If not found: ERROR "No feature spec at {path}"
2. Fill Technical Context (scan for NEEDS CLARIFICATION)
   → Detect Project Type from file system structure or context (web=frontend+backend, mobile=app+api)
   → Set Structure Decision based on project type
3. Fill the Constitution Check section based on the content of the constitution document.
4. Evaluate Constitution Check section below
   → If violations exist: Document in Complexity Tracking
   → If no justification possible: ERROR "Simplify approach first"
   → Update Progress Tracking: Initial Constitution Check
5. Execute Phase 0 → research.md
   → If NEEDS CLARIFICATION remain: ERROR "Resolve unknowns"
6. Execute Phase 1 → contracts, data-model.md, quickstart.md, agent-specific template file (e.g., `CLAUDE.md` for Claude Code, `.github/copilot-instructions.md` for GitHub Copilot, `GEMINI.md` for Gemini CLI, `QWEN.md` for Qwen Code or `AGENTS.md` for opencode).
7. Re-evaluate Constitution Check section
   → If new violations: Refactor design, return to Phase 1
   → Update Progress Tracking: Post-Design Constitution Check
8. Plan Phase 2 → Describe task generation approach (DO NOT create tasks.md)
9. STOP - Ready for /tasks command
```

**IMPORTANT**: The /plan command STOPS at step 9. Phases 2-4 are executed by other commands:
- Phase 2: /tasks command creates tasks.md
- Phase 3-4: Implementation execution (manual or via tools)

## Summary

This feature implements an integrated web platform combining two major modules:

1. **Meeting Room Reservation System**: Calendar-based booking system (day/week/month views) with CRUD operations, conflict prevention, and real-time view switching
2. **Radiation Response Technical Team Visual Database (CMS)**: Content management system with approval workflow, multimedia support, menu management, and file downloads

Both modules share unified authentication, role-based access control (Admin/Normal User), responsive sidebar navigation with collapse functionality, and light/dark theme switching. The system is designed for 10-50 concurrent users in a small organization context.

## Technical Context

**Language/Version**:
- Backend: C# with .NET 9 (ASP.NET Core Web API)
- Frontend: JavaScript/TypeScript with Vue.js 3 (Composition API)

**Primary Dependencies**:
- Backend: ASP.NET Core 9.0, Entity Framework Core 9.0, MySQL Connector
- Frontend: Vue.js 3, Vue Router, Pinia (state management), TailwindCSS, Axios, Vue I18n (internationalization)
- Development: Docker, Docker Compose

**Localization**:
- Primary Language: Traditional Chinese (繁體中文 / zh-Hant)
- All UI labels, messages, error messages, and logs in Traditional Chinese
- Font Support: Microsoft JhengHei (微軟正黑體) for Traditional Chinese characters
- i18n Library: Vue I18n for frontend internationalization

**Storage**: MySQL 8.0+ (relational database for all entities)

**Testing**:
- Backend: xUnit, Moq, FluentAssertions
- Frontend: Vitest, Vue Test Utils, Playwright (E2E)

**Target Platform**:
- Linux server environment (Ubuntu 22.04 LTS or similar)
- .NET 9 application MUST run on Linux (containerized via Docker)
- Browser: Modern browsers (Chrome 100+, Firefox 100+, Safari 15+, Edge 100+)

**Project Type**: Web application (frontend + backend separated)

**Performance Goals**:
- Calendar view switching: <200ms response time
- Theme switching: <100ms (perceived as instant)
- Initial page load: <3 seconds on 3G connection
- API response time: <500ms for 95th percentile
- Support 10-50 concurrent users

**Constraints**:
- File upload limit: 100MB per file
- Supported file types: PDF, DOC, DOCX, XLS, XLSX, PPT, PPTX, PNG, JPG, JPEG, GIF, SVG, MP4, AVI, MOV, WMV, ZIP, RAR, 7Z
- Responsive breakpoints: Desktop (>1024px), Tablet (768-1024px), Mobile (<768px)
- Cross-platform compatibility: Must run on Linux via Docker
- No Blazor (Vue.js for frontend only)
- Language: All UI must be in Traditional Chinese (繁體中文)

**Scale/Scope**:
- User base: 10-50 concurrent users (small organization)
- Meeting rooms: ~10-20 rooms initially
- Content articles: ~100-500 articles expected
- File storage: ~10GB initial allocation
- Database: Single MySQL instance sufficient for scale

## Constitution Check
*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### I. Specification-First Development ✅ PASS
- Feature specification exists at `specs/002-build-an-integrated/spec.md`
- Specification describes WHAT (multi-module platform) and WHY (unified system for reservations + knowledge base)
- Contains testable acceptance criteria (19 scenarios defined)
- Clarifications completed (5 critical questions answered)
- Written for business stakeholders (no technical implementation details in spec)

### II. Test-Driven Development (NON-NEGOTIABLE) ✅ WILL COMPLY
- Phase 1 will generate contract tests for all API endpoints (MUST fail initially)
- Integration tests will be written from acceptance scenarios (19 test cases)
- Implementation will follow Red-Green-Refactor cycle
- Contract tests: One per endpoint (~30-40 endpoints estimated)
- Integration tests: One per user story (~19 tests from acceptance scenarios)

### III. Design Before Implementation ✅ WILL COMPLY
- Phase 0 (Research): Resolving technical decisions (architecture patterns, libraries)
- Phase 1 (Design): Creating data models, API contracts, quickstart guide
- Agent file (CLAUDE.md) will be updated with active technologies
- No coding begins until all design artifacts complete

### IV. Constitution Compliance ✅ PASS
- Initial check: Feature aligns with constitutional principles
- Post-design check: Will verify after Phase 1 completion
- No complexity violations identified
- Simpler alternatives: Single database, standard REST API, proven tech stack

### V. Structured Task Execution ✅ WILL COMPLY
- Tasks will be generated from Phase 1 design artifacts
- Parallel tasks: Model creation [P], contract tests [P], component development [P]
- Sequential dependencies: Tests before implementation, models before services
- TDD ordering enforced
- File paths will be specified explicitly

**Gate Status**: ✅ PASS - Proceed to Phase 0

## Project Structure

### Documentation (this feature)
```
specs/002-build-an-integrated/
├── plan.md              # This file (/plan command output)
├── spec.md              # Feature specification (completed)
├── research.md          # Phase 0 output (to be created)
├── data-model.md        # Phase 1 output (to be created)
├── quickstart.md        # Phase 1 output (to be created)
├── contracts/           # Phase 1 output (to be created)
│   ├── auth.yaml
│   ├── reservations.yaml
│   ├── rooms.yaml
│   ├── content.yaml
│   ├── menus.yaml
│   ├── users.yaml
│   └── files.yaml
└── tasks.md             # Phase 2 output (/tasks command - NOT created by /plan)
```

### Source Code (repository root)
```
backend/
├── src/
│   ├── NuclearWeb.API/           # Web API project
│   │   ├── Controllers/           # API controllers
│   │   ├── Middleware/            # Auth, logging, error handling
│   │   ├── Program.cs             # Application entry point
│   │   └── appsettings.json       # Configuration
│   ├── NuclearWeb.Core/          # Domain models & interfaces
│   │   ├── Entities/              # Domain entities
│   │   ├── Interfaces/            # Repository & service interfaces
│   │   └── DTOs/                  # Data transfer objects
│   ├── NuclearWeb.Infrastructure/ # Data access & external services
│   │   ├── Data/                  # EF Core DbContext
│   │   ├── Repositories/          # Repository implementations
│   │   └── Services/              # External service integrations
│   └── NuclearWeb.Application/   # Business logic
│       ├── Services/              # Application services
│       └── Validators/            # Input validation
├── tests/
│   ├── NuclearWeb.Tests.Contract/ # API contract tests (xUnit)
│   ├── NuclearWeb.Tests.Integration/ # Integration tests
│   └── NuclearWeb.Tests.Unit/     # Unit tests
├── Dockerfile                     # Backend container definition
└── docker-compose.yml             # Full stack orchestration

frontend/
├── src/
│   ├── components/                # Vue components
│   │   ├── common/                # Shared components (sidebar, theme toggle)
│   │   ├── reservations/          # Reservation module components
│   │   └── cms/                   # CMS module components
│   ├── pages/                     # Page-level components
│   │   ├── auth/                  # Login page
│   │   ├── reservations/          # Reservation pages
│   │   ├── cms/                   # CMS pages
│   │   └── admin/                 # Admin pages
│   ├── stores/                    # Pinia stores (state management)
│   ├── router/                    # Vue Router configuration
│   ├── services/                  # API service layer
│   ├── composables/               # Reusable composition functions
│   ├── assets/                    # Images, fonts
│   ├── styles/                    # Global CSS, TailwindCSS config
│   ├── App.vue                    # Root component
│   └── main.ts                    # Application entry point
├── tests/
│   ├── unit/                      # Component unit tests (Vitest)
│   └── e2e/                       # End-to-end tests (Playwright)
├── public/                        # Static assets
├── Dockerfile                     # Frontend container definition
├── package.json
├── vite.config.ts
├── tailwind.config.js
└── tsconfig.json

docker-compose.yml                 # Orchestrates backend, frontend, MySQL
.env.example                       # Environment variables template
README.md                          # Project setup instructions
CLAUDE.md                          # Claude Code agent guidance (to be updated)
```

**Structure Decision**:

This is a **web application** architecture with clear separation of concerns:

- **backend/**: .NET 9 Web API following Clean Architecture (API → Application → Core → Infrastructure layers)
- **frontend/**: Vue.js 3 SPA with component-based architecture
- **Docker**: Both frontend and backend containerized for Linux deployment
- **MySQL**: Separate container in docker-compose stack

The structure supports:
- Independent development and testing of frontend/backend
- Clean Architecture principles (domain-centric, dependency inversion)
- TDD with separate test projects per layer
- Docker Compose orchestration for full-stack local development and Linux deployment

## Phase 0: Outline & Research

### Research Tasks

Based on Technical Context analysis, the following areas require research:

1. **.NET 9 on Linux Deployment**
   - Decision needed: Optimal Docker base image for .NET 9
   - Decision needed: Linux-specific configuration requirements
   - Decision needed: Performance optimization for containerized .NET on Linux

2. **Calendar Component Selection**
   - Decision needed: Vue.js calendar library (FullCalendar, Vue-Cal, or custom)
   - Decision needed: Day/week/month view implementation approach
   - Decision needed: Real-time updates strategy (<200ms switching requirement)

3. **Rich Text Editor for CMS**
   - Decision needed: WYSIWYG editor (TipTap, Quill, CKEditor)
   - Decision needed: Multimedia embedding approach (YouTube, videos, slides)
   - Decision needed: Frontend-backend content sync strategy

4. **Authentication & Authorization**
   - Decision needed: JWT vs Session-based auth
   - Decision needed: Token refresh strategy
   - Decision needed: Role-based access control implementation pattern
   - Decision needed: Future AD SSO/OAuth integration preparation

5. **File Upload & Storage**
   - Decision needed: File storage strategy (filesystem vs object storage)
   - Decision needed: File upload chunking for large files (up to 100MB)
   - Decision needed: File type validation approach
   - Decision needed: Virus scanning integration (optional for security)

6. **State Management**
   - Decision needed: Pinia store structure for multi-module app
   - Decision needed: Persistent state strategy (theme, sidebar collapse, user preferences)
   - Decision needed: Module isolation pattern

7. **API Design Patterns**
   - Decision needed: REST resource naming conventions
   - Decision needed: Error response standardization
   - Decision needed: Pagination strategy for lists
   - Decision needed: API versioning approach

8. **Testing Strategy**
   - Decision needed: Contract test framework (xUnit + REST assertions)
   - Decision needed: Frontend E2E test approach (Playwright setup)
   - Decision needed: Test data management
   - Decision needed: CI/CD pipeline structure

**Output**: research.md (to be created in Phase 0 execution)

## Phase 1: Design & Contracts
*Prerequisites: research.md complete*

This phase will generate:

1. **data-model.md**: Entity definitions with fields, relationships, validation rules
   - User, Role, MeetingRoom, Reservation entities
   - ContentArticle, MenuItem, UploadedFile entities
   - State transitions (draft → pending → published → expired)

2. **contracts/*.yaml**: OpenAPI 3.0 specifications for all endpoints
   - auth.yaml: Login, logout, refresh token, session management
   - reservations.yaml: CRUD operations, conflict checking, filtering
   - rooms.yaml: Room management, availability queries
   - content.yaml: Article CRUD, approval workflow, available period management
   - menus.yaml: Menu structure management
   - users.yaml: User management, role assignment
   - files.yaml: Upload, download, browsing, organization

3. **Contract Tests**: Generated from contracts (xUnit + FluentAssertions)
   - One test class per endpoint group
   - Tests MUST fail initially (no implementation)
   - Schema validation, status code assertions

4. **quickstart.md**: Step-by-step validation scenarios
   - Setup: Docker Compose up, seed data
   - User journey tests for each acceptance scenario
   - Validation: All tests pass, UI flows work end-to-end

5. **CLAUDE.md**: Updated agent guidance
   - Active technologies: .NET 9, Vue 3, TailwindCSS, MySQL, Docker
   - Project structure reference
   - Recent changes: Multi-module platform added

**Output**: All Phase 1 artifacts in specs/002-build-an-integrated/

## Phase 1.5: UI Prototype Design (User Approval Gate)
*Prerequisites: Phase 1 complete (contracts, data-model)*
*GATE: User must approve UI prototype before implementation begins*

This phase generates 3 distinct UI prototypes for user selection:

**Objective**: Create visual mockups of key pages to establish design direction before any code implementation

**Prototype Deliverables** (3 variants):
1. **Prototype A: Minimalist/Clean**
   - Simple, clean lines with generous whitespace
   - Subtle colors, minimal visual hierarchy
   - Focus on content, reduced chrome

2. **Prototype B: Modern/Bold**
   - Vibrant colors, strong contrast
   - Card-based layouts, elevated surfaces
   - Dynamic shadows and gradients

3. **Prototype C: Professional/Corporate**
   - Traditional layout patterns
   - Conservative color palette (blues, grays)
   - Data-dense, information-focused

**Key Pages to Prototype** (each in 3 variants):
1. Login page with theme toggle
2. Reservations calendar page (month view with sidebar)
3. CMS article editor page with TipTap toolbar
4. Admin dashboard/layout with navigation

**Design Elements to Define**:
- Color scheme (light + dark theme)
- Typography scale (headings, body, labels)
- Component styling (buttons, inputs, cards, modals)
- Sidebar navigation appearance
- Calendar event styling
- Form layouts and validation states
- Loading states and empty states

**Approval Process**:
1. Generate 3 complete prototype sets
2. Present to user with screenshots/mockups
3. User selects preferred prototype (A, B, or C)
4. Selected prototype becomes design system foundation
5. Extract design tokens (colors, spacing, typography) for TailwindCSS config

**Output**:
- `specs/002-build-an-integrated/prototypes/` directory
- `prototype-A/` - Images and description
- `prototype-B/` - Images and description
- `prototype-C/` - Images and description
- `design-decision.md` - User's selected prototype and rationale

**GATE**: ✋ STOP - Implementation cannot proceed until user approves a prototype

## Phase 2: Task Planning Approach
*This section describes what the /tasks command will do - DO NOT execute during /plan*

**Task Generation Strategy**:
- Load `.specify/templates/tasks-template.md` as base
- Generate from Phase 1 artifacts:
  - 7 contract files → 7 contract test tasks [P]
  - 7 entity models → 7 model creation tasks [P]
  - 19 acceptance scenarios → 19 integration test tasks
  - ~30-40 API endpoints → implementation tasks
  - Frontend components (sidebar, calendar, CMS editor, etc.) → UI tasks

**Ordering Strategy**:
- **Phase 3.1 Setup**: Docker, database, project structure, linting
- **Phase 3.2 Tests First (TDD)**: All contract tests [P], integration test stubs [P]
- **Phase 3.3 Backend Core**: Models [P] → Repositories [P] → Services → Controllers
- **Phase 3.4 Frontend Core**: Stores [P] → Services [P] → Components [P] → Pages
- **Phase 3.5 Integration**: Connect frontend to backend, test workflows
- **Phase 3.6 Polish**: Error handling, loading states, accessibility, documentation

**Estimated Output**: 60-80 tasks in tasks.md

**Parallel Execution Opportunities**:
- Different entity models [P]
- Different API contract tests [P]
- Different Vue components in different files [P]
- Frontend and backend work can proceed in parallel after contracts defined

**IMPORTANT**: This phase is executed by the /tasks command, NOT by /plan

## Phase 3+: Future Implementation
*These phases are beyond the scope of the /plan command*

**Phase 3**: Task execution (/tasks command creates tasks.md)
**Phase 4**: Implementation (execute tasks.md following constitutional principles)
**Phase 5**: Validation (run tests, execute quickstart.md, performance validation)

## Complexity Tracking
*Fill ONLY if Constitution Check has violations that must be justified*

No constitutional violations identified. The design follows:
- Standard web application architecture (frontend + backend)
- Proven technology stack (.NET 9, Vue.js 3, MySQL)
- RESTful API design
- Clean Architecture separation
- Docker containerization for Linux deployment

No complexity deviations require justification.

## Progress Tracking
*This checklist is updated during execution flow*

**Phase Status**:
- [x] Phase 0: Research complete (/plan command)
- [x] Phase 1: Design complete (/plan command)
- [x] Phase 1.5: UI Prototypes generated and approved (manual)
- [x] Phase 2: Task planning complete (/plan command - describe approach only)
- [x] Phase 3: Tasks generated (/tasks command)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

**Gate Status**:
- [x] Initial Constitution Check: PASS
- [x] Post-Design Constitution Check: PASS
- [x] All NEEDS CLARIFICATION resolved
- [x] Complexity deviations documented: N/A (no violations)
- [x] UI Prototype approved by user: APPROVED (Prototype C - Typography focused)

---
*Based on Constitution v1.0.0 - See `.specify/memory/constitution.md`*
