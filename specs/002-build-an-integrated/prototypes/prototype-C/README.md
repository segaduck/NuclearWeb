# Prototype C: Professional/Corporate

**Design Philosophy**: Traditional, reliable, and information-dense. Professional appearance suitable for enterprise environments.

## Design Characteristics

### Visual Style
- **Traditional layouts** following established UI patterns
- **Conservative color scheme** with blues and grays
- **Data-dense interfaces** maximizing information display
- **Subtle shadows** for minimal depth
- **Professional typography** with clear hierarchy
- **Grid-based layouts** for structured content

### Color Palette

#### Light Theme
- **Background**: `#F5F7FA` (Light blue-gray)
- **Surface**: `#FFFFFF` (Pure white)
- **Primary**: `#2563EB` (Professional blue)
- **Secondary**: `#475569` (Slate gray)
- **Border**: `#D1D5DB` (Medium gray)
- **Text Primary**: `#0F172A` (Very dark blue)
- **Text Secondary**: `#64748B` (Slate)
- **Success**: `#059669` (Dark green)
- **Warning**: `#D97706` (Dark amber)
- **Error**: `#DC2626` (Dark red)
- **Info**: `#0284C7` (Sky blue)

#### Dark Theme
- **Background**: `#0F172A` (Very dark blue)
- **Surface**: `#1E293B` (Dark slate)
- **Primary**: `#3B82F6` (Blue)
- **Secondary**: `#64748B` (Slate)
- **Border**: `#334155` (Dark slate)
- **Text Primary**: `#F8FAFC` (Off-white)
- **Text Secondary**: `#94A3B8` (Light slate)
- **Success**: `#10B981` (Green)
- **Warning**: `#F59E0B` (Amber)
- **Error**: `#EF4444` (Red)
- **Info**: `#0EA5E9` (Sky)

### Typography
- **Font Family**: `'Roboto', 'Segoe UI', Arial, sans-serif` (Professional standard)
- **Headings**:
  - H1: `28px / 1.75rem`, font-weight: 500
  - H2: `22px / 1.375rem`, font-weight: 500
  - H3: `18px / 1.125rem`, font-weight: 500
- **Body**: `15px / 0.9375rem`, font-weight: 400, line-height: 1.5
- **Small**: `13px / 0.8125rem`, font-weight: 400
- **Labels**: `13px / 0.8125rem`, font-weight: 500, text-transform: uppercase

### Spacing System
- **xs**: `4px` (0.25rem)
- **sm**: `8px` (0.5rem)
- **md**: `12px` (0.75rem)
- **lg**: `16px` (1rem)
- **xl**: `24px` (1.5rem)
- **2xl**: `32px` (2rem)

### Component Styling

#### Buttons
- **Border-radius**: `4px` (minimal rounding)
- **Padding**: `8px 16px`
- **Border**: `1px solid`
- **Transition**: `background-color 150ms, border-color 150ms`
- **Primary**: Blue background, white text
- **Secondary**: White/dark background, blue border, blue text
- **Hover**: Darker shade, no lift effect

#### Input Fields
- **Border**: `1px solid #D1D5DB`
- **Border-radius**: `4px`
- **Padding**: `8px 12px`
- **Background**: White/dark
- **Focus**: Blue border, no shadow
- **Transition**: `border-color 150ms`

#### Cards
- **Background**: White/dark surface
- **Border**: `1px solid border color`
- **Border-radius**: `4px`
- **Padding**: `16px`
- **Shadow**: `0 1px 3px rgba(0, 0, 0, 0.1)` (very subtle)

#### Sidebar
- **Width**: `260px`
- **Background**: Darker shade of surface
- **Border-right**: `1px solid border color`
- **Collapsed width**: `60px`
- **Menu items**: Simple highlight on hover (no fancy effects)
- **Active item**: Blue left border, blue text

#### Calendar Events
- **Border-radius**: `2px`
- **Border-left**: `4px solid` (status color)
- **Background**: Surface with opacity
- **Padding**: `6px 8px`
- **Font-size**: `13px`

#### Tables
- **Prominent use** of data tables for lists
- **Striped rows** for readability
- **Sortable columns** with indicators
- **Dense information** display

## Pages Included

1. **01-login.html** - Traditional login form
2. **02-calendar.html** - Data-dense calendar view
3. **03-editor.html** - Professional editor interface
4. **04-dashboard.html** - Table-heavy dashboard

## TailwindCSS Configuration

```javascript
// tailwind.config.js for Prototype C
module.exports = {
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#2563EB',
          dark: '#1E40AF',
        },
        secondary: '#475569',
      },
      borderRadius: {
        DEFAULT: '4px',
      },
      fontFamily: {
        sans: ['Roboto', '"Segoe UI"', 'Arial', 'sans-serif'],
      },
      fontSize: {
        base: '15px',
      },
    },
  },
}
```

## Design Principles

1. **Professional**: Enterprise-grade appearance
2. **Information-Dense**: Maximize content visibility
3. **Familiar**: Traditional patterns users know
4. **Reliable**: Stable, predictable interface
5. **Functional**: Form follows function
6. **Accessible**: High contrast, clear labeling
