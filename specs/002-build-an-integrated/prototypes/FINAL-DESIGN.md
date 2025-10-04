# Final Design Specification

**Feature**: 002-build-an-integrated
**Date**: 2025-10-03
**Status**: ✅ APPROVED - Ready for Implementation

---

## Selected Design: Prototype C (Professional/Corporate)

### Typography-Focused Approach

Following user feedback, the final design emphasizes **clean typography** over imagery:

- ✅ **No images** on login page
- ✅ **Enhanced typography** using Noto Sans TC
- ✅ **Professional gradient** background on right panel
- ✅ **Feature highlights** instead of visual decoration

---

## Typography System

### Font Family: Noto Sans TC

**Why Noto Sans TC?**
- Optimized for Traditional Chinese characters
- Professional and highly readable
- Multiple weights for clear hierarchy
- Excellent cross-platform support
- Google Fonts availability

### Font Weights & Usage

| Weight | Name | Usage | CSS |
|--------|------|-------|-----|
| **900** | Noto Sans TC Black | Titles, headings, page headers | `font-weight: 900` |
| **700** | Noto Sans TC Bold | Labels, buttons, emphasis | `font-weight: 700` |
| **500** | Noto Sans TC Medium | Body text, descriptions, paragraphs | `font-weight: 500` |
| **400** | Noto Sans TC Regular | Placeholders, secondary text | `font-weight: 400` |

### Implementation Examples

```css
/* Page Title (核能網路平台) */
h1 {
  font-family: 'Noto Sans TC', sans-serif;
  font-weight: 900;  /* Black */
  font-size: 2rem;
  letter-spacing: 0.02em;
}

/* Section Heading (會議室預約與內容管理平台) */
h2 {
  font-family: 'Noto Sans TC', sans-serif;
  font-weight: 900;  /* Black */
  font-size: 2.5rem;
  line-height: 1.3;
}

/* Body Text / Descriptions */
p, .description {
  font-family: 'Noto Sans TC', sans-serif;
  font-weight: 500;  /* Medium */
  font-size: 1.125rem;
  line-height: 1.8;
}

/* Form Labels */
label {
  font-family: 'Noto Sans TC', sans-serif;
  font-weight: 700;  /* Bold */
  font-size: 0.875rem;
  text-transform: uppercase;
}

/* Input Fields */
input {
  font-family: 'Noto Sans TC', sans-serif;
  font-weight: 500;  /* Medium */
  font-size: 15px;
}

/* Buttons */
button {
  font-family: 'Noto Sans TC', sans-serif;
  font-weight: 700;  /* Bold */
  letter-spacing: 0.1em;
  text-transform: uppercase;
}
```

---

## Google Fonts Import

```html
<!-- Add to <head> of all HTML pages -->
<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Noto+Sans+TC:wght@400;500;700;900&display=swap" rel="stylesheet">
```

---

## Login Page Layout

### Left Panel (Login Form)
- **Background**: White (light) / Dark slate (dark)
- **Card**: Bordered, minimal shadow
- **Title**: "核能網路平台" - Noto Sans TC Black (900)
- **Subtitle**: "企業級管理系統" - Noto Sans TC Medium (500)
- **Labels**: Noto Sans TC Bold (700), uppercase
- **Inputs**: Noto Sans TC Medium (500)
- **Button**: Noto Sans TC Bold (700), uppercase

### Right Panel (Branding)
- **Background**: Blue gradient (135deg, #2563EB → #1E40AF)
- **Main Title**: "會議室預約與內容管理平台"
  - Font: Noto Sans TC Black (900)
  - Size: 2.5rem
  - Color: White
  - Line height: 1.3
- **Description**: Noto Sans TC Medium (500)
  - Size: 1.125rem
  - Color: White (95% opacity)
  - Line height: 1.8
- **Features List**: 4 feature items with icons
  - Icons: Emoji-based (📅 📝 📁 👥)
  - Text: Noto Sans TC Medium (500)

---

## Color Palette

### Light Theme
```
Background: #F5F7FA
Surface: #FFFFFF
Primary: #2563EB
Secondary: #475569
Border: #D1D5DB
Text Primary: #0F172A
Text Secondary: #64748B
```

### Dark Theme
```
Background: #0F172A
Surface: #1E293B
Primary: #3B82F6
Secondary: #64748B
Border: #334155
Text Primary: #F8FAFC
Text Secondary: #94A3B8
```

---

## TailwindCSS Configuration

**File**: `frontend/tailwind.config.js`

```javascript
module.exports = {
  content: ["./index.html", "./src/**/*.{vue,js,ts,jsx,tsx}"],
  darkMode: 'class',
  theme: {
    extend: {
      fontFamily: {
        sans: [
          'Noto Sans TC',
          'Microsoft JhengHei',
          '微軟正黑體',
          'sans-serif',
        ],
      },
      fontWeight: {
        normal: '400',
        medium: '500',   // For body text
        semibold: '700', // For labels, buttons
        black: '900',    // For headings, titles
      },
      colors: {
        primary: {
          DEFAULT: '#2563EB',
          dark: '#1E40AF',
          light: '#3B82F6',
        },
        // ... rest of colors
      },
    },
  },
}
```

---

## Vue I18n Setup

### Install Dependencies

```bash
npm install vue-i18n
```

### Create Translation Files

**`frontend/src/locales/zh-Hant.json`**:
```json
{
  "common": {
    "login": "登入",
    "logout": "登出",
    "username": "使用者名稱",
    "password": "密碼",
    "submit": "提交",
    "cancel": "取消",
    "save": "儲存",
    "delete": "刪除",
    "edit": "編輯",
    "search": "搜尋"
  },
  "auth": {
    "title": "核能網路平台",
    "subtitle": "企業級管理系統",
    "placeholderUsername": "請輸入使用者名稱",
    "placeholderPassword": "請輸入密碼",
    "loginButton": "登入",
    "brandTitle": "會議室預約與內容管理平台",
    "brandDescription": "透過我們整合的企業級解決方案，簡化您的會議室管理和內容發佈流程。",
    "brandTagline": "專為要求可靠性和效率的專業團隊而設計。"
  },
  "features": {
    "reservations": "智慧會議室預約系統",
    "cms": "專業內容管理平台",
    "files": "安全檔案管理系統",
    "users": "進階使用者權限控制"
  },
  "theme": {
    "light": "淺色模式",
    "dark": "深色模式"
  }
}
```

### Configure i18n

**`frontend/src/i18n.ts`**:
```typescript
import { createI18n } from 'vue-i18n'
import zhHant from './locales/zh-Hant.json'

const i18n = createI18n({
  legacy: false,
  locale: 'zh-Hant',
  fallbackLocale: 'zh-Hant',
  messages: {
    'zh-Hant': zhHant
  }
})

export default i18n
```

### Usage in Components

```vue
<template>
  <h1>{{ t('auth.title') }}</h1>
  <p>{{ t('auth.brandDescription') }}</p>
</template>

<script setup>
import { useI18n } from 'vue-i18n'
const { t } = useI18n()
</script>
```

---

## File Structure

```
frontend/
├── public/
├── src/
│   ├── locales/
│   │   └── zh-Hant.json          # Traditional Chinese translations
│   ├── i18n.ts                    # i18n configuration
│   ├── main.ts                    # App entry (import i18n)
│   └── ...
├── index.html                     # Include Noto Sans TC font
└── tailwind.config.js             # Font configuration
```

---

## Implementation Checklist

### Phase 1: Setup
- [ ] Install Vue I18n: `npm install vue-i18n`
- [ ] Create `src/locales/zh-Hant.json`
- [ ] Create `src/i18n.ts`
- [ ] Import i18n in `main.ts`
- [ ] Add Noto Sans TC to `index.html`
- [ ] Update `tailwind.config.js` with font config

### Phase 2: Apply Typography
- [ ] Use `font-black` (900) for all h1, h2 titles
- [ ] Use `font-medium` (500) for all body text
- [ ] Use `font-bold` (700) for labels and buttons
- [ ] Test font rendering in Chinese

### Phase 3: Translation
- [ ] Replace all hardcoded text with `t()` calls
- [ ] Verify all UI strings are in zh-Hant.json
- [ ] Test language rendering

### Phase 4: Testing
- [ ] Visual review on different browsers
- [ ] Check font weight rendering
- [ ] Verify Traditional Chinese characters display correctly
- [ ] Test light/dark theme toggle

---

## Final Preview

**File**: `prototype-C/01-login-final.html`

**Features**:
- ✅ Noto Sans TC Black (900) for titles
- ✅ Noto Sans TC Medium (500) for descriptions
- ✅ No images, typography-focused
- ✅ Professional gradient background
- ✅ Feature highlights with icons
- ✅ Traditional Chinese throughout
- ✅ Light/Dark theme toggle

---

**Status**: ✅ Design finalized and approved
**Next Step**: Begin implementation Phase 3.1 (T009+)
