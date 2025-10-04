// TailwindCSS Configuration for NuclearWeb (Prototype C - Professional/Corporate)
// Generated from specs/002-build-an-integrated/prototypes/prototype-C/tailwind.config.template.js

/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  darkMode: 'class', // Enable class-based dark mode
  theme: {
    extend: {
      colors: {
        // Professional/Corporate Color Palette
        primary: {
          DEFAULT: '#2563EB',
          dark: '#1E40AF',
          light: '#3B82F6',
        },
        secondary: {
          DEFAULT: '#475569',
          dark: '#334155',
          light: '#64748B',
        },
        surface: {
          light: '#FFFFFF',
          dark: '#1E293B',
        },
        background: {
          light: '#F5F7FA',
          dark: '#0F172A',
        },
        border: {
          light: '#D1D5DB',
          dark: '#334155',
        },
        text: {
          primary: {
            light: '#0F172A',
            dark: '#F8FAFC',
          },
          secondary: {
            light: '#64748B',
            dark: '#94A3B8',
          },
        },
        success: {
          DEFAULT: '#059669',
          light: '#10B981',
        },
        warning: {
          DEFAULT: '#D97706',
          light: '#F59E0B',
        },
        error: {
          DEFAULT: '#DC2626',
          light: '#EF4444',
        },
        info: {
          DEFAULT: '#0284C7',
          light: '#0EA5E9',
        },
      },
      fontFamily: {
        sans: [
          'Noto Sans TC',       // Primary font for Traditional Chinese
          'Microsoft JhengHei', // Traditional Chinese fallback
          '微軟正黑體',          // Traditional Chinese system fallback
          'sans-serif',
        ],
        // Specific font weights for different uses
        'display': ['Noto Sans TC', 'sans-serif'],  // For headings (use font-weight: 900)
        'body': ['Noto Sans TC', 'sans-serif'],     // For body text (use font-weight: 500)
      },
      fontWeight: {
        normal: '400',
        medium: '500',     // Noto Sans TC Medium - for body text
        semibold: '700',
        black: '900',      // Noto Sans TC Black - for titles/headings
      },
      fontSize: {
        // Professional sizing
        'xs': '13px',      // 0.8125rem
        'sm': '14px',      // 0.875rem
        'base': '15px',    // 0.9375rem (default)
        'lg': '18px',      // 1.125rem
        'xl': '22px',      // 1.375rem
        '2xl': '28px',     // 1.75rem
        '3xl': '36px',     // 2.25rem
      },
      spacing: {
        // Conservative spacing
        'xs': '4px',
        'sm': '8px',
        'md': '12px',
        'lg': '16px',
        'xl': '24px',
        '2xl': '32px',
      },
      borderRadius: {
        // Minimal rounding
        'none': '0',
        'sm': '2px',
        DEFAULT: '4px',
        'md': '6px',
        'lg': '8px',
        'full': '9999px',
      },
      boxShadow: {
        // Professional shadow system
        'none': 'none',
        'sm': '0 1px 2px rgba(0, 0, 0, 0.05)',
        DEFAULT: '0 1px 3px rgba(0, 0, 0, 0.1)',
        'md': '0 2px 4px rgba(0, 0, 0, 0.1)',
        'lg': '0 4px 8px rgba(0, 0, 0, 0.1)',
        'xl': '0 8px 16px rgba(0, 0, 0, 0.12)',
        // Card shadows (light theme)
        'card': '0 4px 16px rgba(0, 0, 0, 0.08), 0 2px 8px rgba(0, 0, 0, 0.04)',
        'card-hover': '0 8px 24px rgba(0, 0, 0, 0.12), 0 4px 12px rgba(0, 0, 0, 0.06)',
        // Card shadows (dark theme) - use with dark mode
        'card-dark': '0 4px 16px rgba(0, 0, 0, 0.4), 0 2px 8px rgba(0, 0, 0, 0.3)',
        'card-dark-hover': '0 8px 24px rgba(0, 0, 0, 0.5), 0 4px 12px rgba(0, 0, 0, 0.4)',
      },
      transitionDuration: {
        DEFAULT: '150ms',
        'fast': '100ms',
        'normal': '150ms',
        'slow': '200ms',
      },
      transitionTimingFunction: {
        DEFAULT: 'ease',
      },
    },
  },
  plugins: [],
}
