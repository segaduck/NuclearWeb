# Prototype A: Minimalist/Clean

**Design Philosophy**: Simplicity, clarity, and generous whitespace. Focus on content over decoration.

## Design Characteristics

### Visual Style
- **Minimalist aesthetic** with clean lines and subtle borders
- **Generous whitespace** for breathing room
- **Flat design** with minimal shadows (subtle borders instead)
- **Typography-focused** with clear hierarchy
- **Monochromatic base** with accent color for actions

### Color Palette

#### Light Theme
- **Background**: `#FFFFFF` (Pure white)
- **Surface**: `#F8F9FA` (Off-white)
- **Border**: `#E5E7EB` (Light gray)
- **Text Primary**: `#1F2937` (Near black)
- **Text Secondary**: `#6B7280` (Medium gray)
- **Primary Action**: `#3B82F6` (Clean blue)
- **Primary Hover**: `#2563EB` (Darker blue)
- **Success**: `#10B981` (Green)
- **Warning**: `#F59E0B` (Amber)
- **Error**: `#EF4444` (Red)

#### Dark Theme
- **Background**: `#111827` (Very dark gray)
- **Surface**: `#1F2937` (Dark gray)
- **Border**: `#374151` (Medium dark gray)
- **Text Primary**: `#F9FAFB` (Off-white)
- **Text Secondary**: `#9CA3AF` (Light gray)
- **Primary Action**: `#60A5FA` (Lighter blue)
- **Primary Hover**: `#3B82F6` (Blue)
- **Success**: `#34D399` (Light green)
- **Warning**: `#FBBF24` (Light amber)
- **Error**: `#F87171` (Light red)

### Typography
- **Font Family**: `system-ui, -apple-system, "Segoe UI", sans-serif` (System fonts)
- **Headings**:
  - H1: `32px / 2rem`, font-weight: 600, letter-spacing: -0.025em
  - H2: `24px / 1.5rem`, font-weight: 600
  - H3: `20px / 1.25rem`, font-weight: 600
- **Body**: `16px / 1rem`, font-weight: 400, line-height: 1.5
- **Small**: `14px / 0.875rem`, font-weight: 400
- **Labels**: `14px / 0.875rem`, font-weight: 500

### Spacing System
- **xs**: `4px` (0.25rem)
- **sm**: `8px` (0.5rem)
- **md**: `16px` (1rem)
- **lg**: `24px` (1.5rem)
- **xl**: `32px` (2rem)
- **2xl**: `48px` (3rem)

### Component Styling

#### Buttons
- **Border-radius**: `6px` (slightly rounded)
- **Padding**: `10px 20px` (vertical, horizontal)
- **Border**: `1px solid transparent`
- **Transition**: `all 150ms ease`
- **Primary**: Blue background, white text, no border
- **Secondary**: White/dark background, border, primary color text
- **Ghost**: Transparent, text-only, hover background

#### Input Fields
- **Border**: `1px solid #E5E7EB` (light) / `#374151` (dark)
- **Border-radius**: `6px`
- **Padding**: `10px 12px`
- **Focus**: Blue border, subtle shadow
- **Transition**: `border-color 150ms ease`

#### Cards
- **Background**: Surface color
- **Border**: `1px solid border color`
- **Border-radius**: `8px`
- **Padding**: `24px`
- **Shadow**: None (border only)

#### Sidebar
- **Width**: `280px`
- **Background**: Surface color
- **Border-right**: `1px solid border color`
- **Collapsed width**: `64px`
- **Menu items**: Minimal hover effect (background change only)

#### Calendar Events
- **Border-radius**: `4px`
- **Border-left**: `3px solid` (primary color)
- **Background**: Surface color
- **Padding**: `8px`
- **Font-size**: `14px`

## Pages Included

1. **01-login.html** - Login page with theme toggle
2. **02-calendar.html** - Reservations calendar (month view)
3. **03-editor.html** - CMS article editor with TipTap
4. **04-dashboard.html** - Admin dashboard layout

## TailwindCSS Configuration

```javascript
// tailwind.config.js for Prototype A
module.exports = {
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#3B82F6',
          hover: '#2563EB',
        },
        surface: {
          light: '#F8F9FA',
          dark: '#1F2937',
        },
      },
      borderRadius: {
        DEFAULT: '6px',
        lg: '8px',
      },
      fontFamily: {
        sans: ['system-ui', '-apple-system', '"Segoe UI"', 'sans-serif'],
      },
    },
  },
}
```

## Design Principles

1. **Content First**: UI elements don't compete with content
2. **Clarity**: Clear visual hierarchy with typography
3. **Breathing Room**: Generous spacing between elements
4. **Subtle Interactions**: Gentle transitions, no dramatic effects
5. **Consistency**: Uniform spacing and sizing throughout
6. **Accessibility**: High contrast ratios, clear focus states
