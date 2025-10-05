import { ref, onMounted, onUnmounted } from 'vue'

/**
 * 響應式斷點組合式函數
 * Responsive breakpoint composable for mobile/tablet/desktop
 */

export type Breakpoint = 'mobile' | 'tablet' | 'desktop'

export function useBreakpoint() {
  const breakpoint = ref<Breakpoint>('desktop')
  const width = ref(window.innerWidth)

  // TailwindCSS 預設斷點
  const breakpoints = {
    mobile: 0,
    tablet: 768,
    desktop: 1024,
  }

  const isMobile = ref(false)
  const isTablet = ref(false)
  const isDesktop = ref(true)

  const updateBreakpoint = () => {
    width.value = window.innerWidth

    if (width.value < breakpoints.tablet) {
      breakpoint.value = 'mobile'
      isMobile.value = true
      isTablet.value = false
      isDesktop.value = false
    } else if (width.value < breakpoints.desktop) {
      breakpoint.value = 'tablet'
      isMobile.value = false
      isTablet.value = true
      isDesktop.value = false
    } else {
      breakpoint.value = 'desktop'
      isMobile.value = false
      isTablet.value = false
      isDesktop.value = true
    }
  }

  onMounted(() => {
    updateBreakpoint()
    window.addEventListener('resize', updateBreakpoint)
  })

  onUnmounted(() => {
    window.removeEventListener('resize', updateBreakpoint)
  })

  return {
    breakpoint,
    width,
    isMobile,
    isTablet,
    isDesktop,
  }
}
