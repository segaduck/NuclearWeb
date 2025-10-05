import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'

/**
 * Vue Router 配置（含認證守衛）
 * Vue Router configuration with auth guards
 */

// DEBUG: Check if this file is being loaded
console.log('🔍 ROUTER FILE LOADED - WITH TEST ROUTE:', new Date().toISOString())

// FORCE RECOMPILE - TEST ROUTE ADDED
const routes: RouteRecordRaw[] = [
  {
    path: '/test',
    name: 'Test',
    component: () => import('@/pages/TestPage.vue'),
    meta: { requiresAuth: false, title: 'Test' },
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/pages/Login.vue'),
    meta: { requiresAuth: false, title: '登入' },
  },
  {
    path: '/',
    component: () => import('@/components/layout/AppLayout.vue'),
    meta: { requiresAuth: true },
    children: [
      {
        path: '',
        redirect: '/reservations',
      },
      // 預約管理
      {
        path: 'reservations',
        name: 'Reservations',
        component: () => import('@/pages/reservations/ReservationsPage.vue'),
        meta: { requiresAuth: true, title: '會議室預約' },
      },
      // CMS 管理（僅限 Admin）
      {
        path: 'cms',
        meta: { requiresAuth: true, requiresAdmin: true },
        children: [
          {
            path: 'articles',
            name: 'ArticleList',
            component: () => import('@/pages/cms/ArticleListPage.vue'),
            meta: { requiresAuth: true, requiresAdmin: true, title: '文章列表' },
          },
          {
            path: 'articles/new',
            name: 'ArticleCreate',
            component: () => import('@/pages/cms/ArticleEditorPage.vue'),
            meta: { requiresAuth: true, requiresAdmin: true, title: '新增文章' },
          },
          {
            path: 'articles/:id/edit',
            name: 'ArticleEdit',
            component: () => import('@/pages/cms/ArticleEditorPage.vue'),
            meta: { requiresAuth: true, requiresAdmin: true, title: '編輯文章' },
            props: true,
          },
          {
            path: 'menus',
            name: 'MenuManagement',
            component: () => import('@/pages/cms/MenuManagementPage.vue'),
            meta: { requiresAuth: true, requiresAdmin: true, title: '選單管理' },
          },
          {
            path: 'files',
            name: 'FileUpload',
            component: () => import('@/pages/cms/FileUploadPage.vue'),
            meta: { requiresAuth: true, requiresAdmin: true, title: '檔案管理' },
          },
        ],
      },
      // CMS 公開頁面
      {
        path: 'articles',
        name: 'PublicArticleList',
        component: () => import('@/pages/cms/PublicArticleListPage.vue'),
        meta: { requiresAuth: true, title: '文章列表' },
      },
      {
        path: 'articles/:id',
        name: 'ArticleViewer',
        component: () => import('@/pages/cms/ArticleViewerPage.vue'),
        meta: { requiresAuth: true, title: '文章閱讀' },
        props: true,
      },
      // 使用者管理（僅限 Admin）
      {
        path: 'admin',
        meta: { requiresAuth: true, requiresAdmin: true },
        children: [
          {
            path: 'users',
            name: 'UserList',
            component: () => import('@/pages/admin/UserListPage.vue'),
            meta: { requiresAuth: true, requiresAdmin: true, title: '使用者管理' },
          },
          {
            path: 'rooms',
            name: 'RoomList',
            component: () => import('@/pages/admin/RoomListPage.vue'),
            meta: { requiresAuth: true, requiresAdmin: true, title: '會議室管理' },
          },
        ],
      },
      // 個人檔案
      {
        path: 'profile',
        name: 'UserProfile',
        component: () => import('@/pages/UserProfilePage.vue'),
        meta: { requiresAuth: true, title: '個人檔案' },
      },
    ],
  },
  // 404 頁面
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: () => import('@/pages/NotFound.vue'),
    meta: { requiresAuth: false, title: '找不到頁面' },
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
})

/**
 * 全域前置守衛 - 認證檢查
 * Global before guard - authentication check
 */
router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore()

  // 設定頁面標題
  document.title = to.meta.title
    ? `${to.meta.title} - NuclearWeb`
    : 'NuclearWeb'

  // 如果路由需要認證
  if (to.meta.requiresAuth) {
    // 檢查是否已登入
    if (!authStore.isAuthenticated) {
      // 未登入，重定向到登入頁
      next({
        name: 'Login',
        query: { redirect: to.fullPath },
      })
      return
    }

    // 如果需要載入使用者資料
    if (!authStore.user) {
      try {
        await authStore.fetchCurrentUser()
      } catch (error) {
        // 載入使用者資料失敗，清除登入狀態並重定向
        authStore.logout()
        next({
          name: 'Login',
          query: { redirect: to.fullPath },
        })
        return
      }
    }

    // 檢查是否需要 Admin 權限
    if (to.meta.requiresAdmin && !authStore.isAdmin) {
      // 沒有 Admin 權限，重定向到首頁
      next({ name: 'Reservations' })
      return
    }
  }

  // 如果已登入且前往登入頁，重定向到首頁
  if (to.name === 'Login' && authStore.isAuthenticated) {
    next({ name: 'Reservations' })
    return
  }

  // 允許導航
  next()
})

// 擴展 RouteMeta 類型
declare module 'vue-router' {
  interface RouteMeta {
    requiresAuth?: boolean
    requiresAdmin?: boolean
    title?: string
  }
}

export default router
