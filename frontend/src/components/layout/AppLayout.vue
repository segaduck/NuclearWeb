<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Sidebar -->
    <Sidebar />

    <!-- Main Content Area -->
    <div
      class="transition-all duration-300"
      :class="sidebarStore.collapsed ? 'lg:pl-16' : 'lg:pl-64'"
    >
      <!-- Header -->
      <Header />

      <!-- Page Content -->
      <main class="p-4 sm:p-6 lg:p-8">
        <router-view />
      </main>
    </div>

    <!-- Mobile Overlay -->
    <div
      v-if="!sidebarStore.collapsed && isMobile"
      class="fixed inset-0 bg-black bg-opacity-50 z-30 lg:hidden"
      @click="sidebarStore.setCollapsed(true)"
    ></div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useSidebarStore } from '@/stores/sidebarStore'
import { useThemeStore } from '@/stores/themeStore'
import { useBreakpoint } from '@/composables/useBreakpoint'
import Sidebar from './Sidebar.vue'
import Header from './Header.vue'

const sidebarStore = useSidebarStore()
const themeStore = useThemeStore()
const { isMobile } = useBreakpoint()

onMounted(() => {
  // 初始化主題
  themeStore.initialize()

  // 初始化側邊欄響應式行為
  sidebarStore.initializeResponsive()
})
</script>
