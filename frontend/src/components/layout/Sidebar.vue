<template>
  <aside
    class="fixed top-0 left-0 h-full bg-white dark:bg-gray-800 border-r border-gray-200 dark:border-gray-700 transition-all duration-300 z-40"
    :class="sidebarStore.collapsed ? 'w-16' : 'w-64'"
  >
    <!-- Logo Area -->
    <div class="h-16 flex items-center justify-center border-b border-gray-200 dark:border-gray-700">
      <h1
        v-if="!sidebarStore.collapsed"
        class="text-xl font-bold text-gray-800 dark:text-white"
      >
        NuclearWeb
      </h1>
      <span
        v-else
        class="text-xl font-bold text-gray-800 dark:text-white"
      >
        N
      </span>
    </div>

    <!-- Navigation Menu -->
    <nav class="mt-4 px-2">
      <template v-for="item in menuItems" :key="item.name">
        <router-link
          v-if="!item.adminOnly || authStore.isAdmin"
          :to="item.path"
          class="flex items-center px-4 py-3 mb-1 rounded-lg transition-colors"
          :class="[
            isActive(item.path)
              ? 'bg-primary-100 dark:bg-primary-900 text-primary-600 dark:text-primary-400'
              : 'text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700'
          ]"
          :title="sidebarStore.collapsed ? item.label : ''"
        >
          <component :is="item.icon" class="w-5 h-5 flex-shrink-0" />
          <span
            v-if="!sidebarStore.collapsed"
            class="ml-3 text-sm font-medium"
          >
            {{ item.label }}
          </span>
        </router-link>
      </template>
    </nav>

    <!-- Collapse Toggle Button -->
    <button
      @click="sidebarStore.toggleCollapsed()"
      class="absolute bottom-4 left-1/2 -translate-x-1/2 p-2 rounded-lg bg-gray-100 dark:bg-gray-700 hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors"
      :title="sidebarStore.collapsed ? '展開側邊欄' : '收合側邊欄'"
    >
      <svg
        class="w-5 h-5 text-gray-600 dark:text-gray-300 transition-transform"
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
import { computed } from 'vue'
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

interface MenuItem {
  name: string
  label: string
  path: string
  icon: any
  adminOnly?: boolean
}

const menuItems = computed<MenuItem[]>(() => [
  {
    name: 'reservations',
    label: '會議室預約',
    path: '/reservations',
    icon: CalendarIcon,
  },
  {
    name: 'articles',
    label: '文章列表',
    path: '/articles',
    icon: DocumentTextIcon,
  },
  {
    name: 'cms-articles',
    label: 'CMS 文章',
    path: '/cms/articles',
    icon: DocumentTextIcon,
    adminOnly: true,
  },
  {
    name: 'cms-menus',
    label: '選單管理',
    path: '/cms/menus',
    icon: HomeIcon,
    adminOnly: true,
  },
  {
    name: 'cms-files',
    label: '檔案管理',
    path: '/cms/files',
    icon: FolderIcon,
    adminOnly: true,
  },
  {
    name: 'admin-users',
    label: '使用者管理',
    path: '/admin/users',
    icon: UsersIcon,
    adminOnly: true,
  },
  {
    name: 'admin-rooms',
    label: '會議室管理',
    path: '/admin/rooms',
    icon: Cog6ToothIcon,
    adminOnly: true,
  },
])

const isActive = (path: string) => {
  return route.path.startsWith(path)
}
</script>
