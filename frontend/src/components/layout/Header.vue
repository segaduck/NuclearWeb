<template>
  <header class="h-16 bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700 flex items-center justify-between px-4 sm:px-6">
    <!-- Left: Page Title or Breadcrumb -->
    <div class="flex items-center">
      <h2 class="text-lg font-semibold text-gray-800 dark:text-white">
        {{ pageTitle }}
      </h2>
    </div>

    <!-- Right: Theme Toggle & User Menu -->
    <div class="flex items-center space-x-4">
      <!-- Theme Toggle Button -->
      <button
        @click="themeStore.toggleTheme()"
        class="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
        :title="themeStore.theme === 'light' ? '切換到深色模式' : '切換到淺色模式'"
      >
        <SunIcon
          v-if="themeStore.theme === 'dark'"
          class="w-5 h-5 text-gray-600 dark:text-gray-300"
        />
        <MoonIcon
          v-else
          class="w-5 h-5 text-gray-600 dark:text-gray-300"
        />
      </button>

      <!-- User Menu Dropdown -->
      <div class="relative">
        <button
          @click="showUserMenu = !showUserMenu"
          class="flex items-center space-x-2 p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
        >
          <div class="w-8 h-8 rounded-full bg-primary-500 text-white flex items-center justify-center text-sm font-medium">
            {{ userInitials }}
          </div>
          <span class="hidden sm:block text-sm font-medium text-gray-700 dark:text-gray-300">
            {{ authStore.user?.displayName }}
          </span>
          <ChevronDownIcon class="w-4 h-4 text-gray-500 dark:text-gray-400" />
        </button>

        <!-- Dropdown Menu -->
        <div
          v-if="showUserMenu"
          v-click-outside="closeUserMenu"
          class="absolute right-0 mt-2 w-48 bg-white dark:bg-gray-800 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700 py-1 z-50"
        >
          <router-link
            to="/profile"
            class="block px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700"
            @click="showUserMenu = false"
          >
            個人檔案
          </router-link>
          <div class="border-t border-gray-200 dark:border-gray-700 my-1"></div>
          <button
            @click="handleLogout"
            class="w-full text-left px-4 py-2 text-sm text-red-600 dark:text-red-400 hover:bg-gray-100 dark:hover:bg-gray-700"
          >
            登出
          </button>
        </div>
      </div>
    </div>
  </header>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useThemeStore } from '@/stores/themeStore'
import { useAuthStore } from '@/stores/authStore'
import {
  SunIcon,
  MoonIcon,
  ChevronDownIcon,
} from '@heroicons/vue/24/outline'

const router = useRouter()
const route = useRoute()
const themeStore = useThemeStore()
const authStore = useAuthStore()

const showUserMenu = ref(false)

const pageTitle = computed(() => {
  return route.meta.title || 'NuclearWeb'
})

const userInitials = computed(() => {
  const displayName = authStore.user?.displayName || 'User'
  return displayName
    .split(' ')
    .map(n => n[0])
    .join('')
    .toUpperCase()
    .slice(0, 2)
})

const closeUserMenu = () => {
  showUserMenu.value = false
}

const handleLogout = async () => {
  try {
    await authStore.logout()
    router.push('/login')
  } catch (error) {
    console.error('Logout failed:', error)
    // 即使失敗也強制跳轉到登入頁
    router.push('/login')
  }
}

// Click outside directive
const vClickOutside = {
  mounted(el: any, binding: any) {
    el.clickOutsideEvent = (event: Event) => {
      if (!(el === event.target || el.contains(event.target))) {
        binding.value()
      }
    }
    document.addEventListener('click', el.clickOutsideEvent)
  },
  unmounted(el: any) {
    document.removeEventListener('click', el.clickOutsideEvent)
  },
}
</script>
