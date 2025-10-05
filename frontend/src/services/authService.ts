import apiClient from './api'

/**
 * 認證服務
 * Authentication service (login, logout, refresh token)
 */

export interface LoginRequest {
  username: string
  password: string
}

export interface LoginResponse {
  accessToken: string
  refreshToken: string
  user: {
    id: number
    username: string
    email: string
    displayName: string
    role: string
  }
}

export interface RefreshTokenResponse {
  accessToken: string
  refreshToken: string
}

export interface UserProfile {
  id: number
  username: string
  email: string
  displayName: string
  role: string
  createdAt: string
  lastLoginAt: string | null
}

const authService = {
  /**
   * 登入
   * User login
   */
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await apiClient.post<LoginResponse>('/auth/login', credentials)
    return response.data
  },

  /**
   * 刷新 Token
   * Refresh access token
   */
  async refreshToken(refreshToken: string): Promise<RefreshTokenResponse> {
    const response = await apiClient.post<RefreshTokenResponse>('/auth/refresh', {
      refreshToken,
    })
    return response.data
  },

  /**
   * 登出
   * Logout
   */
  async logout(refreshToken: string): Promise<void> {
    await apiClient.post('/auth/logout', { refreshToken })
  },

  /**
   * 取得當前使用者資料
   * Get current user profile
   */
  async me(): Promise<UserProfile> {
    const response = await apiClient.get<UserProfile>('/auth/me')
    return response.data
  },
}

export default authService
