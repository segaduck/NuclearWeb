import axios, { type AxiosInstance, type InternalAxiosRequestConfig, type AxiosResponse } from 'axios'
import { useAuthStore } from '@/stores/authStore'

/**
 * API 客戶端服務
 * API client service with axios interceptors
 */

const apiClient: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api/v1',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
})

/**
 * 請求攔截器 - 自動添加 JWT Token
 * Request interceptor - automatically add JWT token
 */
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const authStore = useAuthStore()

    if (authStore.accessToken) {
      config.headers.Authorization = `Bearer ${authStore.accessToken}`
    }

    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

/**
 * 響應攔截器 - 處理 401 錯誤和自動刷新 Token
 * Response interceptor - handle 401 errors and auto-refresh token
 */
apiClient.interceptors.response.use(
  (response: AxiosResponse) => {
    return response
  },
  async (error) => {
    const originalRequest = error.config

    // 401 錯誤且未重試過
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true

      const authStore = useAuthStore()

      try {
        // 嘗試刷新 Token
        await authStore.refreshToken()

        // 重新發送原始請求
        if (authStore.accessToken) {
          originalRequest.headers.Authorization = `Bearer ${authStore.accessToken}`
        }

        return apiClient(originalRequest)
      } catch (refreshError) {
        // 刷新失敗，清除登入狀態並跳轉到登入頁
        authStore.logout()
        window.location.href = '/login'
        return Promise.reject(refreshError)
      }
    }

    return Promise.reject(error)
  }
)

export default apiClient
