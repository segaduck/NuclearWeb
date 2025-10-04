# Tailwind CSS Usage Examples

## Card Shadow Implementation

### In Vue Components

```vue
<template>
  <!-- Login Card with shadow -->
  <div class="
    w-full max-w-[420px] p-10
    border rounded
    transition-all duration-200

    /* Light mode */
    bg-white border-border-light shadow-card

    /* Dark mode */
    dark:bg-surface-dark dark:border-border-dark dark:shadow-card-dark
  ">
    <!-- Card content -->
  </div>
</template>
```

### Shadow Classes Available

| Class | Light Theme | Dark Theme | Usage |
|-------|-------------|------------|-------|
| `shadow-card` | Subtle elevation | - | Login cards, content cards |
| `dark:shadow-card-dark` | - | Strong elevation | Dark mode cards |
| `shadow-card-hover` | Enhanced elevation | - | Hover states (light) |
| `dark:shadow-card-dark-hover` | - | Enhanced elevation | Hover states (dark) |

### Complete Login Card Example

```vue
<template>
  <div class="login-card">
    <div class="logo">
      <h1 class="
        text-2xl font-black
        mb-3 leading-normal tracking-wide
        text-text-primary-light
        dark:text-text-primary-dark
      ">
        核能安全委員會<br>會內知識平台
      </h1>
      <h2 class="
        text-xl font-black
        mb-2 tracking-wide
        text-primary
      ">
        使用者登入
      </h2>
    </div>

    <form>
      <div class="form-group mb-6">
        <label class="
          block text-xs font-bold
          mb-2 uppercase tracking-wider
          text-text-primary-light dark:text-text-primary-dark
        ">
          使用者名稱
        </label>
        <input
          type="text"
          class="
            w-full px-3.5 py-2.5
            border rounded
            text-base font-medium
            transition-colors duration-150

            bg-white border-border-light text-text-primary-light
            dark:bg-background-dark dark:border-border-dark dark:text-text-primary-dark

            focus:outline-none focus:border-primary
          "
          placeholder="請輸入使用者名稱"
        />
      </div>

      <div class="form-group mb-6">
        <label class="
          block text-xs font-bold
          mb-2 uppercase tracking-wider
          text-text-primary-light dark:text-text-primary-dark
        ">
          密碼
        </label>
        <input
          type="password"
          class="
            w-full px-3.5 py-2.5
            border rounded
            text-base font-medium
            transition-colors duration-150

            bg-white border-border-light text-text-primary-light
            dark:bg-background-dark dark:border-border-dark dark:text-text-primary-dark

            focus:outline-none focus:border-primary
          "
          placeholder="請輸入密碼"
        />
      </div>

      <button
        type="submit"
        class="
          w-full px-5 py-3
          border rounded
          text-base font-bold uppercase tracking-widest
          transition-all duration-150

          bg-primary border-primary text-white
          hover:bg-primary-dark hover:border-primary-dark
        "
      >
        登入
      </button>

      <a
        href="#"
        class="
          block mt-4 text-center
          text-sm font-medium
          transition-colors duration-200

          text-primary
          dark:text-primary-light

          hover:underline
        "
      >
        忘記密碼
      </a>
    </form>
  </div>
</template>

<style scoped>
.login-card {
  @apply w-full max-w-[420px] p-10;
  @apply border rounded;
  @apply transition-all duration-200;

  /* Light mode */
  @apply bg-white border-border-light shadow-card;

  /* Dark mode */
  @apply dark:bg-surface-dark dark:border-border-dark dark:shadow-card-dark;
}

.logo {
  @apply mb-10 text-center;
}

.form-group {
  @apply mb-6;
}
</style>
```

## Typography Classes

### Headings

```vue
<!-- Page Title (Noto Sans TC Black) -->
<h1 class="text-2xl font-black tracking-wide">核能安全委員會</h1>

<!-- Section Title (Noto Sans TC Black) -->
<h2 class="text-xl font-black tracking-wide text-primary">使用者登入</h2>

<!-- Subsection (Noto Sans TC Black) -->
<h3 class="text-lg font-black">功能說明</h3>
```

### Body Text

```vue
<!-- Body Text (Noto Sans TC Medium) -->
<p class="text-base font-medium leading-relaxed">
  透過我們整合的企業級解決方案，簡化您的會議室管理和內容發佈流程。
</p>

<!-- Description (Noto Sans TC Medium) -->
<p class="text-lg font-medium leading-loose opacity-95">
  專為要求可靠性和效率的專業團隊而設計。
</p>
```

### Labels & Buttons

```vue
<!-- Form Label (Noto Sans TC Bold) -->
<label class="text-xs font-bold uppercase tracking-wider">
  使用者名稱
</label>

<!-- Button (Noto Sans TC Bold) -->
<button class="text-base font-bold uppercase tracking-widest">
  登入
</button>

<!-- Link (Noto Sans TC Medium) -->
<a class="text-sm font-medium">忘記密碼</a>
```

## Color Classes

### Text Colors

```vue
<!-- Primary text -->
<p class="text-text-primary-light dark:text-text-primary-dark">內容</p>

<!-- Secondary text -->
<p class="text-text-secondary-light dark:text-text-secondary-dark">說明</p>

<!-- Primary color -->
<span class="text-primary">重要</span>
```

### Background Colors

```vue
<!-- Surface -->
<div class="bg-white dark:bg-surface-dark">Card</div>

<!-- Background -->
<div class="bg-background-light dark:bg-background-dark">Page</div>

<!-- Primary -->
<button class="bg-primary hover:bg-primary-dark">按鈕</button>
```

### Border Colors

```vue
<!-- Default border -->
<div class="border border-border-light dark:border-border-dark">Card</div>

<!-- Primary border -->
<input class="border border-primary" />
```

## Complete CSS Class Reference

```css
/* Font Families */
.font-sans       /* Noto Sans TC */
.font-display    /* Noto Sans TC (for headings) */
.font-body       /* Noto Sans TC (for body) */

/* Font Weights */
.font-normal     /* 400 */
.font-medium     /* 500 - Noto Sans TC Medium */
.font-semibold   /* 700 */
.font-black      /* 900 - Noto Sans TC Black */

/* Shadows */
.shadow-card              /* Light card shadow */
.shadow-card-hover        /* Light card hover shadow */
.dark:shadow-card-dark    /* Dark card shadow */
.dark:shadow-card-dark-hover  /* Dark card hover shadow */

/* Border Radius */
.rounded         /* 4px */
.rounded-md      /* 6px */
.rounded-lg      /* 8px */
```
