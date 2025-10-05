import { defineStore } from 'pinia'
import { ref } from 'vue'

/**
 * 側邊欄狀態管理（收合狀態）
 * Sidebar Pinia store (collapsed state)
 */
export const useSidebarStore = defineStore('sidebar', () => {
  // State - 從 localStorage 載入，預設為展開
  const collapsed = ref<boolean>(
    localStorage.getItem('sidebarCollapsed') === 'true'
  )

  // Actions
  function setCollapsed(value: boolean) {
    collapsed.value = value
    localStorage.setItem('sidebarCollapsed', value.toString())
  }

  function toggleCollapsed() {
    setCollapsed(!collapsed.value)
  }

  // 響應式設計：在小螢幕上自動收合
  function initializeResponsive() {
    const handleResize = () => {
      // 768px 以下（mobile/tablet）自動收合
      if (window.innerWidth < 768) {
        setCollapsed(true)
      }
    }

    // 初始檢查
    handleResize()

    // 監聽視窗大小變化
    window.addEventListener('resize', handleResize)

    // 返回清理函數
    return () => {
      window.removeEventListener('resize', handleResize)
    }
  }

  return {
    // State
    collapsed,
    // Actions
    setCollapsed,
    toggleCollapsed,
    initializeResponsive,
  }
})
