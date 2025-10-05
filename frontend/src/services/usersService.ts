import apiClient from './api'

/**
 * 使用者服務
 * Users service
 */

export interface User {
  id: number
  username: string
  email: string
  displayName: string
  role: string
  isActive: boolean
  preferences: string | null
  createdAt: string
  updatedAt: string
  lastLoginAt: string | null
}

export interface CreateUserRequest {
  username: string
  email: string
  password: string
  displayName: string
  role: string
}

export interface UpdateUserRequest {
  email?: string
  displayName?: string
  role?: string
  isActive?: boolean
}

export interface ResetPasswordRequest {
  newPassword: string
}

export interface UpdatePreferencesRequest {
  preferences: Record<string, any>
}

export interface PaginatedUsers {
  items: User[]
  totalCount: number
  page: number
  pageSize: number
}

const usersService = {
  /**
   * 取得使用者列表
   * Get users list
   */
  async getUsers(params: {
    page?: number
    pageSize?: number
    role?: string
    activeOnly?: boolean
  }): Promise<PaginatedUsers> {
    const response = await apiClient.get<PaginatedUsers>('/users', { params })
    return response.data
  },

  /**
   * 取得單一使用者
   * Get user by ID
   */
  async getUser(id: number): Promise<User> {
    const response = await apiClient.get<User>(`/users/${id}`)
    return response.data
  },

  /**
   * 建立使用者
   * Create user
   */
  async createUser(data: CreateUserRequest): Promise<User> {
    const response = await apiClient.post<User>('/users', data)
    return response.data
  },

  /**
   * 更新使用者
   * Update user
   */
  async updateUser(id: number, data: UpdateUserRequest): Promise<User> {
    const response = await apiClient.put<User>(`/users/${id}`, data)
    return response.data
  },

  /**
   * 刪除使用者
   * Delete user
   */
  async deleteUser(id: number): Promise<void> {
    await apiClient.delete(`/users/${id}`)
  },

  /**
   * 重設密碼
   * Reset user password
   */
  async resetPassword(id: number, data: ResetPasswordRequest): Promise<void> {
    await apiClient.post(`/users/${id}/reset-password`, data)
  },

  /**
   * 更新偏好設定
   * Update user preferences
   */
  async updatePreferences(data: UpdatePreferencesRequest): Promise<User> {
    const response = await apiClient.put<User>('/users/me/preferences', data)
    return response.data
  },
}

export default usersService
