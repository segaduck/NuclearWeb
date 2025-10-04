<!--
Sync Impact Report:
Version change: Initial version → 1.0.0
Modified principles: N/A (initial constitution)
Added sections: All sections (new constitution)
Removed sections: N/A
Templates requiring updates:
  ✅ plan-template.md - Verified constitution check alignment
  ✅ spec-template.md - Verified requirements alignment
  ✅ tasks-template.md - Verified TDD and task categorization alignment
  ✅ agent-file-template.md - Verified structure alignment
Follow-up TODOs: None
-->

# NuclearWeb Constitution

## Core Principles

### I. Specification-First Development
Every feature begins with a complete, non-technical specification that:
- Describes WHAT users need and WHY (never HOW to implement)
- Contains testable acceptance criteria
- Marks all ambiguities with [NEEDS CLARIFICATION]
- Is written for business stakeholders, not developers

**Rationale**: Clear specifications prevent scope creep, enable better planning, and ensure alignment between business needs and technical implementation.

### II. Test-Driven Development (NON-NEGOTIABLE)
TDD cycle is mandatory for all features:
- Tests MUST be written first and approved by user
- Tests MUST fail before implementation begins
- Implementation follows Red-Green-Refactor strictly
- Contract tests for all API endpoints
- Integration tests for all user stories

**Rationale**: TDD ensures code correctness, prevents regressions, and creates living documentation of system behavior.

### III. Design Before Implementation
Planning artifacts MUST be completed before coding:
- Research phase resolves all technical unknowns
- Data models define entities and relationships
- API contracts specify all interfaces
- Quickstart guides document user workflows
- Agent-specific guidance updated incrementally

**Rationale**: Upfront design prevents costly mid-implementation pivots and ensures architectural consistency.

### IV. Constitution Compliance
All features MUST pass constitution checks at two gates:
- Pre-research: Verify feature aligns with principles
- Post-design: Verify design doesn't introduce violations
- Complexity violations MUST be justified and documented
- Simpler alternatives MUST be evaluated first

**Rationale**: Constitutional governance prevents technical debt accumulation and maintains long-term code quality.

### V. Structured Task Execution
Implementation follows dependency-ordered task lists:
- Tasks generated from design artifacts (never ad-hoc)
- Parallel tasks [P] for independent work
- Sequential tasks for dependencies
- TDD ordering: tests before implementation
- Exact file paths specified for each task

**Rationale**: Structured execution enables parallelization, prevents merge conflicts, and provides clear progress tracking.

## Development Workflow

### Planning Phase
1. Feature specification created via `/specify` command
2. Clarification questions resolved via `/clarify` command
3. Implementation plan generated via `/plan` command
4. Tasks generated via `/tasks` command
5. Implementation executed via `/implement` command or manually

### Quality Gates
- **Specification Gate**: No [NEEDS CLARIFICATION] markers remain
- **Constitution Gate (Pre)**: Feature aligns with principles
- **Constitution Gate (Post)**: Design doesn't violate principles
- **Test Gate**: All tests written and failing before implementation
- **Completion Gate**: All tests passing, quickstart validated

### Documentation Requirements
All features maintain documentation in `specs/[###-feature-name]/`:
- `spec.md`: Non-technical feature specification
- `plan.md`: Technical implementation plan
- `research.md`: Technology decisions and rationale
- `data-model.md`: Entity definitions and relationships
- `quickstart.md`: User workflow validation
- `contracts/`: API interface specifications
- `tasks.md`: Dependency-ordered task list

## Agent Integration

### Agent-Specific Files
Projects maintain agent-specific guidance files:
- `CLAUDE.md`: Claude Code specific guidance
- `.github/copilot-instructions.md`: GitHub Copilot guidance
- `GEMINI.md`: Gemini CLI guidance
- `QWEN.md`: Qwen Code guidance
- `AGENTS.md`: Generic opencode guidance

**Update Policy**: Agent files updated incrementally (O(1) operation) during Phase 1 design via `update-agent-context.ps1` script.

### Content Constraints
- Keep under 150 lines for token efficiency
- Extract only active technologies from plans
- Preserve manual additions between markers
- Update recent changes (last 3 features only)

## Governance

### Amendment Procedure
1. Propose changes with rationale and impact analysis
2. Document affected templates and downstream artifacts
3. Update constitution version following semantic versioning
4. Propagate changes to all dependent templates
5. Commit with version bump and change summary

### Versioning Policy
- **MAJOR**: Backward-incompatible principle changes or removals
- **MINOR**: New principles added or material guidance expansion
- **PATCH**: Clarifications, wording fixes, non-semantic refinements

### Compliance Enforcement
- All slash commands validate against current constitution
- Template updates MUST maintain constitutional alignment
- Violations require documented justification in Complexity Tracking
- Unjustifiable complexity blocks feature approval

### Living Document Status
This constitution evolves with project needs but maintains core values:
- Specification-first thinking
- Test-driven development
- Design before implementation
- Transparent governance

**Version**: 1.0.0 | **Ratified**: 2025-10-03 | **Last Amended**: 2025-10-03
