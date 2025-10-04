# Prototype B: Modern/Bold

**Design Philosophy**: Vibrant, engaging, and contemporary with strong visual hierarchy and dynamic elements.

## Design Characteristics

### Visual Style
- **Bold and vibrant** color scheme with strong contrasts
- **Card-based layouts** with elevated surfaces
- **Dynamic shadows** for depth and dimension
- **Gradients** for visual interest
- **Modern iconography** and rounded corners
- **Animated interactions** with smooth transitions

### Color Palette

#### Light Theme
- **Background**: `#F3F4F6` (Light gray background)
- **Surface**: `#FFFFFF` (White cards)
- **Primary**: `#8B5CF6` (Vibrant purple)
- **Primary Gradient**: `linear-gradient(135deg, #8B5CF6 0%, #EC4899 100%)`
- **Secondary**: `#06B6D4` (Cyan)
- **Text Primary**: `#111827` (Near black)
- **Text Secondary**: `#6B7280` (Gray)
- **Success**: `#10B981` (Green)
- **Warning**: `#F59E0B` (Amber)
- **Error**: `#EF4444` (Red)
- **Accent**: `#EC4899` (Pink)

#### Dark Theme
- **Background**: `#0F172A` (Very dark blue-gray)
- **Surface**: `#1E293B` (Dark blue-gray)
- **Primary**: `#A78BFA` (Light purple)
- **Primary Gradient**: `linear-gradient(135deg, #A78BFA 0%, #F472B6 100%)`
- **Secondary**: `#22D3EE` (Light cyan)
- **Text Primary**: `#F1F5F9` (Off-white)
- **Text Secondary**: `#94A3B8` (Light gray)
- **Success**: `#34D399` (Light green)
- **Warning**: `#FBBF24` (Light amber)
- **Error**: `#F87171` (Light red)
- **Accent**: `#F472B6` (Light pink)

### Typography
- **Font Family**: `'Inter', system-ui, -apple-system, sans-serif` (Modern web font)
- **Headings**:
  - H1: `36px / 2.25rem`, font-weight: 700, letter-spacing: -0.03em
  - H2: `28px / 1.75rem`, font-weight: 700, letter-spacing: -0.02em
  - H3: `22px / 1.375rem`, font-weight: 600
- **Body**: `16px / 1rem`, font-weight: 400, line-height: 1.6
- **Small**: `14px / 0.875rem`, font-weight: 400
- **Labels**: `14px / 0.875rem`, font-weight: 600

### Spacing System
- **xs**: `4px` (0.25rem)
- **sm**: `8px` (0.5rem)
- **md**: `16px` (1rem)
- **lg**: `24px` (1.5rem)
- **xl**: `32px` (2rem)
- **2xl**: `48px` (3rem)
- **3xl**: `64px` (4rem)

### Component Styling

#### Buttons
- **Border-radius**: `12px` (very rounded)
- **Padding**: `12px 24px`
- **Shadow**: `0 4px 12px rgba(0, 0, 0, 0.15)`
- **Transition**: `all 200ms cubic-bezier(0.4, 0, 0.2, 1)`
- **Primary**: Gradient background, white text, shadow
- **Hover**: Lift effect (translateY(-2px)), increased shadow
- **Active**: Scale down slightly (scale(0.98))

#### Input Fields
- **Border**: `2px solid transparent`
- **Background**: Surface color with subtle shadow
- **Border-radius**: `10px`
- **Padding**: `12px 16px`
- **Shadow**: `0 2px 8px rgba(0, 0, 0, 0.08)`
- **Focus**: Primary color border, increased shadow, glow effect
- **Transition**: `all 200ms ease`

#### Cards
- **Background**: Surface color (white/dark)
- **Border-radius**: `16px`
- **Padding**: `24px`
- **Shadow**: `0 4px 20px rgba(0, 0, 0, 0.1)` (light) / `0 4px 20px rgba(0, 0, 0, 0.3)` (dark)
- **Hover**: Lift effect with increased shadow

#### Sidebar
- **Width**: `280px`
- **Background**: Gradient or solid with shadow
- **Border**: None (shadow instead)
- **Collapsed width**: `72px`
- **Menu items**: Bold hover with gradient background
- **Active item**: Gradient background with shadow

#### Calendar Events
- **Border-radius**: `8px`
- **Background**: Gradient based on event type
- **Padding**: `10px`
- **Shadow**: `0 2px 8px rgba(0, 0, 0, 0.12)`
- **Hover**: Scale up slightly (scale(1.02))

## Pages Included

1. **01-login.html** - Login page with gradient background
2. **02-calendar.html** - Reservations calendar with vibrant cards
3. **03-editor.html** - CMS editor with modern toolbar
4. **04-dashboard.html** - Dashboard with gradient cards

## TailwindCSS Configuration

```javascript
// tailwind.config.js for Prototype B
module.exports = {
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#8B5CF6',
          light: '#A78BFA',
        },
        secondary: '#06B6D4',
        accent: '#EC4899',
      },
      borderRadius: {
        DEFAULT: '10px',
        lg: '12px',
        xl: '16px',
      },
      boxShadow: {
        card: '0 4px 20px rgba(0, 0, 0, 0.1)',
        'card-hover': '0 8px 30px rgba(0, 0, 0, 0.15)',
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
    },
  },
}
```

## Design Principles

1. **Bold & Engaging**: Eye-catching colors and gradients
2. **Depth**: Shadows and elevation create hierarchy
3. **Dynamic**: Smooth animations and hover effects
4. **Modern**: Contemporary design patterns
5. **Vibrant**: Strong color usage for emphasis
6. **Polished**: Professional with attention to detail
