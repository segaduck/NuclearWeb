import { defineStore } from 'pinia'
import { ref, watch } from 'vue'

/**
 * 主題狀態管理（明暗模式切換）
 * Theme Pinia store (light/dark switching)
 */
export const useThemeStore = defineStore('theme', () => {
  // State - 從 localStorage 載入，預設為 light
  const theme = ref<'light' | 'dark'>(
    (localStorage.getItem('theme') as 'light' | 'dark') || 'light'
  )

  // Actions
  function setTheme(newTheme: 'light' | 'dark') {
    theme.value = newTheme
    localStorage.setItem('theme', newTheme)
    applyTheme(newTheme)
  }

  function toggleTheme() {
    const newTheme = theme.value === 'light' ? 'dark' : 'light'
    setTheme(newTheme)
  }

  function applyTheme(themeValue: 'light' | 'dark') {
    // 移除舊的 class
    document.body.classList.remove('light', 'dark')

    // 加入新的 class
    document.body.classList.add(themeValue)

    // 更新 meta theme-color
    const metaThemeColor = document.querySelector('meta[name="theme-color"]')
    if (metaThemeColor) {
      metaThemeColor.setAttribute(
        'content',
        themeValue === 'dark' ? '#1e293b' : '#ffffff'
      )
    }
  }

  // 初始化主題
  function initialize() {
    applyTheme(theme.value)

    // 監聽系統主題變更
    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)')
    mediaQuery.addEventListener('change', (e) => {
      // 如果使用者沒有手動設定過主題，則跟隨系統
      if (!localStorage.getItem('theme')) {
        const systemTheme = e.matches ? 'dark' : 'light'
        setTheme(systemTheme)
      }
    })
  }

  // 監聽 theme 變化並套用
  watch(theme, (newTheme) => {
    applyTheme(newTheme)
  })

  return {
    // State
    theme,
    // Actions
    setTheme,
    toggleTheme,
    initialize,
  }
})
