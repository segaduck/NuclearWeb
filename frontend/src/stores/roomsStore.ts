import { defineStore } from 'pinia'
import { ref } from 'vue'
import roomsService, { type MeetingRoom } from '@/services/roomsService'

/**
 * 會議室 Store
 * Rooms store for managing meeting rooms state
 */
export const useRoomsStore = defineStore('rooms', () => {
  // State
  const rooms = ref<MeetingRoom[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  /**
   * 載入所有會議室
   * Load all meeting rooms
   */
  async function fetchRooms(includeInactive = false) {
    loading.value = true
    error.value = null

    try {
      const response = await roomsService.getRooms({
        page: 1,
        pageSize: 100,
        activeOnly: !includeInactive,
      })
      rooms.value = response.items
    } catch (err: any) {
      error.value = err.message || '載入會議室失敗'
      console.error('載入會議室失敗:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * 取得單一會議室
   * Get room by ID
   */
  function getRoomById(id: number): MeetingRoom | undefined {
    return rooms.value.find((r) => r.id === id)
  }

  /**
   * 建立會議室
   * Create room
   */
  async function createRoom(data: {
    name: string
    capacity: number
    location: string
    amenities?: string[]
  }) {
    loading.value = true
    error.value = null

    try {
      const newRoom = await roomsService.createRoom(data)
      rooms.value.push(newRoom)
      return newRoom
    } catch (err: any) {
      error.value = err.message || '建立會議室失敗'
      console.error('建立會議室失敗:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * 更新會議室
   * Update room
   */
  async function updateRoom(
    id: number,
    data: {
      name?: string
      capacity?: number
      location?: string
      amenities?: string[]
      isActive?: boolean
    }
  ) {
    loading.value = true
    error.value = null

    try {
      const updatedRoom = await roomsService.updateRoom(id, data)
      const index = rooms.value.findIndex((r) => r.id === id)
      if (index !== -1) {
        rooms.value[index] = updatedRoom
      }
      return updatedRoom
    } catch (err: any) {
      error.value = err.message || '更新會議室失敗'
      console.error('更新會議室失敗:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * 刪除會議室
   * Delete room
   */
  async function deleteRoom(id: number) {
    loading.value = true
    error.value = null

    try {
      await roomsService.deleteRoom(id)
      rooms.value = rooms.value.filter((r) => r.id !== id)
    } catch (err: any) {
      error.value = err.message || '刪除會議室失敗'
      console.error('刪除會議室失敗:', err)
      throw err
    } finally {
      loading.value = false
    }
  }

  return {
    // State
    rooms,
    loading,
    error,

    // Actions
    fetchRooms,
    getRoomById,
    createRoom,
    updateRoom,
    deleteRoom,
  }
})
