import apiClient from './api'

/**
 * 選單服務
 * Menus service
 */

export interface MenuItem {
  id: number
  name: string
  linkType: string
  linkTarget: string | null
  parentId: number | null
  displayOrder: number
  isActive: boolean
  createdAt: string
  updatedAt: string
  children?: MenuItem[]
}

export interface CreateMenuRequest {
  name: string
  linkType: string
  linkTarget?: string
  parentId?: number
  displayOrder?: number
}

export interface UpdateMenuRequest {
  name?: string
  linkType?: string
  linkTarget?: string
  parentId?: number
  displayOrder?: number
  isActive?: boolean
}

export interface ReorderMenuRequest {
  menuItems: Array<{
    id: number
    displayOrder: number
  }>
}

const menusService = {
  /**
   * 取得選單列表
   * Get menu items list
   */
  async getMenus(params?: { activeOnly?: boolean }): Promise<MenuItem[]> {
    const response = await apiClient.get<MenuItem[]>('/menus', { params })
    return response.data
  },

  /**
   * 取得單一選單
   * Get menu item by ID
   */
  async getMenu(id: number): Promise<MenuItem> {
    const response = await apiClient.get<MenuItem>(`/menus/${id}`)
    return response.data
  },

  /**
   * 建立選單
   * Create menu item
   */
  async createMenu(data: CreateMenuRequest): Promise<MenuItem> {
    const response = await apiClient.post<MenuItem>('/menus', data)
    return response.data
  },

  /**
   * 更新選單
   * Update menu item
   */
  async updateMenu(id: number, data: UpdateMenuRequest): Promise<MenuItem> {
    const response = await apiClient.put<MenuItem>(`/menus/${id}`, data)
    return response.data
  },

  /**
   * 刪除選單
   * Delete menu item
   */
  async deleteMenu(id: number): Promise<void> {
    await apiClient.delete(`/menus/${id}`)
  },

  /**
   * 重新排序選單
   * Reorder menu items
   */
  async reorderMenus(data: ReorderMenuRequest): Promise<void> {
    await apiClient.put('/menus/reorder', data)
  },
}

export default menusService
