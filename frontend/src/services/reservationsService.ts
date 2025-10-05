import apiClient from './api'

/**
 * 預約服務
 * Reservations service
 */

export interface Reservation {
  id: number
  meetingRoomId: number
  meetingRoomName?: string
  userId: number
  userDisplayName?: string
  startTime: string
  endTime: string
  purpose: string | null
  attendeeCount: number | null
  status: string
  createdAt: string
  updatedAt: string
}

export interface CreateReservationRequest {
  meetingRoomId: number
  startTime: string
  endTime: string
  purpose?: string
  attendeeCount?: number
}

export interface UpdateReservationRequest {
  startTime?: string
  endTime?: string
  purpose?: string
  attendeeCount?: number
}

export interface CheckAvailabilityRequest {
  meetingRoomId: number
  startTime: string
  endTime: string
  excludeReservationId?: number
}

export interface PaginatedReservations {
  items: Reservation[]
  totalCount: number
  page: number
  pageSize: number
}

const reservationsService = {
  /**
   * 取得預約列表
   * Get reservations list with filters
   */
  async getReservations(params: {
    page?: number
    pageSize?: number
    roomId?: number
    userId?: number
    status?: string
    startDate?: string
    endDate?: string
  }): Promise<PaginatedReservations> {
    const response = await apiClient.get<PaginatedReservations>('/reservations', { params })
    return response.data
  },

  /**
   * 取得單一預約
   * Get reservation by ID
   */
  async getReservation(id: number): Promise<Reservation> {
    const response = await apiClient.get<Reservation>(`/reservations/${id}`)
    return response.data
  },

  /**
   * 建立預約
   * Create reservation
   */
  async createReservation(data: CreateReservationRequest): Promise<Reservation> {
    const response = await apiClient.post<Reservation>('/reservations', data)
    return response.data
  },

  /**
   * 更新預約
   * Update reservation
   */
  async updateReservation(id: number, data: UpdateReservationRequest): Promise<Reservation> {
    const response = await apiClient.put<Reservation>(`/reservations/${id}`, data)
    return response.data
  },

  /**
   * 刪除預約
   * Delete reservation
   */
  async deleteReservation(id: number): Promise<void> {
    await apiClient.delete(`/reservations/${id}`)
  },

  /**
   * 檢查可用性
   * Check availability for time slot
   */
  async checkAvailability(data: CheckAvailabilityRequest): Promise<{ available: boolean; conflicts?: Reservation[] }> {
    const response = await apiClient.post<{ available: boolean; conflicts?: Reservation[] }>('/reservations/check-availability', data)
    return response.data
  },
}

export default reservationsService
