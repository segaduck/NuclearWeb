<template>
  <aside
    class="fixed top-0 left-0 h-full bg-white dark:bg-gray-800 border-r border-gray-200 dark:border-gray-600 transition-all duration-200 z-40"
    :class="sidebarStore.collapsed ? 'w-[60px]' : 'w-64'"
  >
    <!-- Logo/Header Area -->
    <div
      class="h-16 flex items-center justify-center px-4 bg-primary-600"
      :class="sidebarStore.collapsed ? 'justify-center px-2' : ''"
    >
      <h1 v-if="!sidebarStore.collapsed" class="text-white text-lg font-semibold">
        AI服務平台
      </h1>
      <svg v-else class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
      </svg>
    </div>

    <!-- Navigation Menu -->
    <nav class="mt-8">
      <div class="px-4 space-y-2">
        <!-- Top Level Items (no section header) -->
        <router-link
          to="/reservations"
          class="nav-item"
          :class="{ 'nav-item-active': isActive('/reservations') }"
        >
          <CalendarIcon class="w-5 h-5 flex-shrink-0" />
          <span v-if="!sidebarStore.collapsed">會議室預約</span>
        </router-link>

        <router-link
          to="/articles"
          class="nav-item"
          :class="{ 'nav-item-active': isActive('/articles') }"
        >
          <DocumentTextIcon class="w-5 h-5 flex-shrink-0" />
          <span v-if="!sidebarStore.collapsed">文章列表</span>
        </router-link>

        <!-- CMS Section -->
        <div v-if="!sidebarStore.collapsed" class="pt-4">
          <h3 class="px-3 text-xs font-semibold theme-text-muted uppercase tracking-wider">內容管理</h3>
          <div class="mt-2 space-y-1">
            <router-link
              v-if="authStore.isAdmin"
              to="/cms/articles"
              class="nav-item"
              :class="{ 'nav-item-active': isActive('/cms/articles') }"
            >
              <DocumentTextIcon class="w-5 h-5 flex-shrink-0" />
              <span>CMS 文章</span>
            </router-link>

            <router-link
              v-if="authStore.isAdmin"
              to="/cms/menus"
              class="nav-item"
              :class="{ 'nav-item-active': isActive('/cms/menus') }"
            >
              <HomeIcon class="w-5 h-5 flex-shrink-0" />
              <span>選單管理</span>
            </router-link>

            <router-link
              v-if="authStore.isAdmin"
              to="/cms/files"
              class="nav-item"
              :class="{ 'nav-item-active': isActive('/cms/files') }"
            >
              <FolderIcon class="w-5 h-5 flex-shrink-0" />
              <span>檔案管理</span>
            </router-link>
          </div>
        </div>

        <!-- System Management Section -->
        <div v-if="!sidebarStore.collapsed" class="pt-4">
          <h3 class="px-3 text-xs font-semibold theme-text-muted uppercase tracking-wider">系統管理</h3>
          <div class="mt-2 space-y-1">
            <router-link
              v-if="authStore.isAdmin"
              to="/admin/users"
              class="nav-item"
              :class="{ 'nav-item-active': isActive('/admin/users') }"
            >
              <UsersIcon class="w-5 h-5 flex-shrink-0" />
              <span>使用者管理</span>
            </router-link>

            <router-link
              v-if="authStore.isAdmin"
              to="/admin/rooms"
              class="nav-item"
              :class="{ 'nav-item-active': isActive('/admin/rooms') }"
            >
              <Cog6ToothIcon class="w-5 h-5 flex-shrink-0" />
              <span>會議室管理</span>
            </router-link>
          </div>
        </div>
      </div>
    </nav>

    <!-- Collapse Toggle Button -->
    <button
      @click="sidebarStore.toggleCollapsed()"
      class="absolute bottom-4 left-1/2 -translate-x-1/2 p-1.5 rounded hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
      :title="sidebarStore.collapsed ? '展開側邊欄' : '收合側邊欄'"
    >
      <svg
        class="w-4 h-4 text-gray-500 dark:text-gray-400 transition-transform"
        :class="sidebarStore.collapsed ? 'rotate-180' : ''"
        fill="none"
        stroke="currentColor"
        viewBox="0 0 24 24"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M15 19l-7-7 7-7"
        />
      </svg>
    </button>
  </aside>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router'
import { useSidebarStore } from '@/stores/sidebarStore'
import { useAuthStore } from '@/stores/authStore'
import {
  CalendarIcon,
  DocumentTextIcon,
  FolderIcon,
  UsersIcon,
  HomeIcon,
  Cog6ToothIcon,
} from '@heroicons/vue/24/outline'

const route = useRoute()
const sidebarStore = useSidebarStore()
const authStore = useAuthStore()

const isActive = (path: string) => {
  return route.path.startsWith(path)
}
</script>
