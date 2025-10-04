# UI Design Decision

**Selected Prototype**: C (Professional/Corporate)

**Date**: 2025-10-03

**Decision Maker**: User

## Rationale

Prototype C was selected because:

1. **Target Audience**: Our customers prefer traditional, professional interfaces
2. **Enterprise Context**: The platform is for professional/technical teams who value reliability and information density
3. **Functional Priority**: Users prioritize functionality and data visibility over visual flair
4. **Conservative Aesthetic**: Professional blues and grays align with nuclear/radiation industry standards

## Key Factors

1. **Professional Appearance**: Enterprise-grade look suitable for radiation response teams
2. **Information Density**: Maximizes screen real estate for data-heavy workflows
3. **Familiar Patterns**: Traditional layouts users already understand
4. **Reliability Perception**: Conservative design conveys stability and trustworthiness
5. **Industry Standards**: Aligns with expectations in nuclear/technical fields

## Trade-offs Accepted

**What we're NOT getting from Prototype A**:
- Generous whitespace and breathing room
- Timeless minimalist aesthetic
- Faster loading (though Prototype C is still efficient)

**What we're NOT getting from Prototype B**:
- Eye-catching modern aesthetics
- Vibrant gradients and bold colors
- Delightful animations and interactions
- Contemporary "wow" factor

## Implementation Notes

### Language Requirements
- **Primary Language**: Traditional Chinese (繁體中文)
- All UI labels, messages, and logs must be in Traditional Chinese
- Implementation requires i18n infrastructure (vue-i18n)

### Visual Enhancements
- **Typography Enhancement**: Using Noto Sans TC font family
  - **Noto Sans TC Black (weight: 900)**: For titles and headings
  - **Noto Sans TC Medium (weight: 500)**: For body text and descriptions
- **Right Panel**: Blue gradient background with feature highlights
- **No images**: Clean, typography-focused design

### Backup Plans
- Prototypes A and B are retained as backup options
- Located in `prototype-A/` and `prototype-B/` directories
- Can be referenced if design direction needs adjustment

### Design System

**Color Palette** (from Prototype C):
```javascript
// Light Theme
background: '#F5F7FA'
surface: '#FFFFFF'
primary: '#2563EB'
secondary: '#475569'
border: '#D1D5DB'
textPrimary: '#0F172A'
textSecondary: '#64748B'

// Dark Theme
background: '#0F172A'
surface: '#1E293B'
primary: '#3B82F6'
border: '#334155'
textPrimary: '#F8FAFC'
textSecondary: '#94A3B8'
```

**Typography**:
```javascript
fontFamily: "'Noto Sans TC', 'Microsoft JhengHei', '微軟正黑體', sans-serif"
// Note: Noto Sans TC optimized for Traditional Chinese
fontSize: '15px' (base)
fontWeight:
  - 400 (normal)
  - 500 (medium - Noto Sans TC Medium for body text)
  - 700 (semibold)
  - 900 (black - Noto Sans TC Black for titles/headings)
```

**Spacing**: 4px base unit (xs: 4, sm: 8, md: 12, lg: 16, xl: 24, 2xl: 32)

**Border Radius**: 4px (minimal rounding)

**Shadows**: Very subtle (`0 1px 3px rgba(0, 0, 0, 0.1)`)

### Component Guidelines

**Buttons**:
- Border-radius: 4px
- Padding: 8px 16px
- Border: 1px solid
- Primary: #2563EB background, white text
- Text: Uppercase with letter-spacing

**Input Fields**:
- Border: 1px solid #D1D5DB
- Border-radius: 4px
- Padding: 8px 12px
- Focus: Blue border, no shadow

**Cards**:
- Background: White/dark surface
- Border: 1px solid
- Border-radius: 4px
- Padding: 16px
- Shadow: 0 1px 3px rgba(0, 0, 0, 0.1)

**Sidebar**:
- Width: 260px
- Collapsed: 60px
- Border-right: 1px solid
- Active item: Blue left border

## Next Steps

1. ✅ Document decision (this file)
2. 🔄 Create nuclear power image options (3 variants)
3. 🔄 Extract design tokens to TailwindCSS config
4. 🔄 Add Traditional Chinese i18n infrastructure
5. ⏳ Update plan.md and tasks.md with language requirements
6. ⏳ Begin Phase 3.1 implementation

## Validation Criteria

Design decision is successful if:
- Users find the interface professional and trustworthy
- Information density supports efficient workflows
- Traditional Chinese text displays correctly with proper fonts
- Nuclear power imagery reinforces brand identity
- Interface feels familiar to enterprise users

---

**Status**: Decision documented, awaiting image selection and token extraction
