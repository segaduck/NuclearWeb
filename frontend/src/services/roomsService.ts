import apiClient from './api'

/**
 * 會議室服務
 * Rooms service
 */

export interface MeetingRoom {
  id: number
  name: string
  capacity: number
  location: string
  amenities: string | null
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface CreateRoomRequest {
  name: string
  capacity: number
  location: string
  amenities?: string[]
}

export interface UpdateRoomRequest {
  name?: string
  capacity?: number
  location?: string
  amenities?: string[]
  isActive?: boolean
}

export interface PaginatedRooms {
  items: MeetingRoom[]
  totalCount: number
  page: number
  pageSize: number
}

const roomsService = {
  /**
   * 取得會議室列表
   * Get meeting rooms list
   */
  async getRooms(params: {
    page?: number
    pageSize?: number
    activeOnly?: boolean
  }): Promise<PaginatedRooms> {
    const response = await apiClient.get<PaginatedRooms>('/rooms', { params })
    return response.data
  },

  /**
   * 取得所有會議室（不分頁）
   * Get all meeting rooms (no pagination)
   */
  async getAllRooms(includeInactive = false): Promise<MeetingRoom[]> {
    const response = await apiClient.get<MeetingRoom[]>('/rooms/all', {
      params: { includeInactive },
    })
    return response.data
  },

  /**
   * 取得單一會議室
   * Get room by ID
   */
  async getRoom(id: number): Promise<MeetingRoom> {
    const response = await apiClient.get<MeetingRoom>(`/rooms/${id}`)
    return response.data
  },

  /**
   * 建立會議室
   * Create room
   */
  async createRoom(data: CreateRoomRequest): Promise<MeetingRoom> {
    const response = await apiClient.post<MeetingRoom>('/rooms', data)
    return response.data
  },

  /**
   * 更新會議室
   * Update room
   */
  async updateRoom(id: number, data: UpdateRoomRequest): Promise<MeetingRoom> {
    const response = await apiClient.put<MeetingRoom>(`/rooms/${id}`, data)
    return response.data
  },

  /**
   * 刪除會議室
   * Delete room
   */
  async deleteRoom(id: number): Promise<void> {
    await apiClient.delete(`/rooms/${id}`)
  },
}

export default roomsService
