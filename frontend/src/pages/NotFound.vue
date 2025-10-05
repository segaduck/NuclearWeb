<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100 dark:bg-gray-900 px-4">
    <div class="text-center">
      <h1 class="text-6xl font-bold text-gray-800 dark:text-white mb-4">404</h1>
      <p class="text-xl text-gray-600 dark:text-gray-400 mb-8">找不到頁面</p>
      <router-link
        to="/"
        class="inline-block px-6 py-3 bg-primary-600 hover:bg-primary-700 text-white font-medium rounded-lg transition-colors"
      >
        返回首頁
      </router-link>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { useThemeStore } from '@/stores/themeStore'

const themeStore = useThemeStore()

onMounted(() => {
  themeStore.initialize()
})
</script>
