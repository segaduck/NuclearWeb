import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import authService, { type LoginRequest, type UserProfile } from '@/services/authService'

/**
 * 認證狀態管理
 * Auth Pinia store
 */
export const useAuthStore = defineStore('auth', () => {
  // State
  const accessToken = ref<string | null>(localStorage.getItem('accessToken'))
  const refreshTokenValue = ref<string | null>(localStorage.getItem('refreshToken'))
  const user = ref<UserProfile | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!accessToken.value)
  const isAdmin = computed(() => user.value?.role === 'Admin')
  const isUser = computed(() => user.value?.role === 'User')

  // Actions
  async function login(credentials: LoginRequest) {
    try {
      const response = await authService.login(credentials)

      // 儲存 tokens
      accessToken.value = response.accessToken
      refreshTokenValue.value = response.refreshToken
      localStorage.setItem('accessToken', response.accessToken)
      localStorage.setItem('refreshToken', response.refreshToken)

      // 儲存使用者資料
      user.value = response.user

      return response
    } catch (error) {
      // 清除任何殘留的認證資料
      logout()
      throw error
    }
  }

  async function refreshToken() {
    if (!refreshTokenValue.value) {
      throw new Error('No refresh token available')
    }

    try {
      const response = await authService.refreshToken(refreshTokenValue.value)

      // 更新 tokens
      accessToken.value = response.accessToken
      refreshTokenValue.value = response.refreshToken
      localStorage.setItem('accessToken', response.accessToken)
      localStorage.setItem('refreshToken', response.refreshToken)

      return response
    } catch (error) {
      // 刷新失敗，清除登入狀態
      logout()
      throw error
    }
  }

  async function logout() {
    try {
      // 呼叫後端 logout API 撤銷 refresh token
      if (refreshTokenValue.value) {
        await authService.logout(refreshTokenValue.value)
      }
    } catch (error) {
      // 即使後端失敗，也要清除前端狀態
      console.error('Logout API failed:', error)
    } finally {
      // 清除本地狀態
      accessToken.value = null
      refreshTokenValue.value = null
      user.value = null
      localStorage.removeItem('accessToken')
      localStorage.removeItem('refreshToken')
    }
  }

  async function fetchCurrentUser() {
    if (!accessToken.value) {
      return null
    }

    try {
      user.value = await authService.me()
      return user.value
    } catch (error) {
      // Token 可能過期或無效
      logout()
      throw error
    }
  }

  // 初始化時載入使用者資料
  async function initialize() {
    if (accessToken.value) {
      try {
        await fetchCurrentUser()
      } catch (error) {
        // 如果載入失敗，清除狀態
        logout()
      }
    }
  }

  return {
    // State
    accessToken,
    refreshTokenValue,
    user,
    // Getters
    isAuthenticated,
    isAdmin,
    isUser,
    // Actions
    login,
    refreshToken,
    logout,
    fetchCurrentUser,
    initialize,
  }
})
