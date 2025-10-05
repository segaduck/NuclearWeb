import { defineStore } from 'pinia'
import { ref } from 'vue'
import reservationsService, { type Reservation } from '@/services/reservationsService'

/**
 * 預約狀態管理（含行事曆資料快取）
 * Reservations Pinia store with calendar data caching
 */
export const useReservationsStore = defineStore('reservations', () => {
  // State
  const reservations = ref<Reservation[]>([])
  const currentReservation = ref<Reservation | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  // 快取配置
  const cacheKey = ref<string>('')
  const cacheTimestamp = ref<number>(0)
  const CACHE_DURATION = 5 * 60 * 1000 // 5 分鐘快取

  // Actions
  async function fetchReservations(params: {
    page?: number
    pageSize?: number
    roomId?: number
    userId?: number
    status?: string
    startDate?: string
    endDate?: string
  }, forceRefresh = false) {
    const newCacheKey = JSON.stringify(params)

    // 檢查快取是否有效
    const now = Date.now()
    if (
      !forceRefresh &&
      cacheKey.value === newCacheKey &&
      now - cacheTimestamp.value < CACHE_DURATION
    ) {
      // 使用快取資料
      return reservations.value
    }

    loading.value = true
    error.value = null

    try {
      const response = await reservationsService.getReservations(params)
      reservations.value = response.items

      // 更新快取
      cacheKey.value = newCacheKey
      cacheTimestamp.value = now

      return response.items
    } catch (err: any) {
      error.value = err.message || '載入預約失敗'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function fetchReservation(id: number) {
    loading.value = true
    error.value = null

    try {
      currentReservation.value = await reservationsService.getReservation(id)
      return currentReservation.value
    } catch (err: any) {
      error.value = err.message || '載入預約詳情失敗'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function createReservation(data: any) {
    loading.value = true
    error.value = null

    try {
      const newReservation = await reservationsService.createReservation(data)

      // 加入列表並清除快取
      reservations.value.unshift(newReservation)
      clearCache()

      return newReservation
    } catch (err: any) {
      error.value = err.message || '建立預約失敗'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function updateReservation(id: number, data: any) {
    loading.value = true
    error.value = null

    try {
      const updated = await reservationsService.updateReservation(id, data)

      // 更新列表中的項目
      const index = reservations.value.findIndex((r) => r.id === id)
      if (index !== -1) {
        reservations.value[index] = updated
      }

      // 更新當前預約
      if (currentReservation.value?.id === id) {
        currentReservation.value = updated
      }

      // 清除快取
      clearCache()

      return updated
    } catch (err: any) {
      error.value = err.message || '更新預約失敗'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function deleteReservation(id: number) {
    loading.value = true
    error.value = null

    try {
      await reservationsService.deleteReservation(id)

      // 從列表移除
      reservations.value = reservations.value.filter((r) => r.id !== id)

      // 清除當前預約
      if (currentReservation.value?.id === id) {
        currentReservation.value = null
      }

      // 清除快取
      clearCache()
    } catch (err: any) {
      error.value = err.message || '刪除預約失敗'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function checkAvailability(data: any) {
    try {
      return await reservationsService.checkAvailability(data)
    } catch (err: any) {
      error.value = err.message || '檢查可用性失敗'
      throw err
    }
  }

  function clearCache() {
    cacheKey.value = ''
    cacheTimestamp.value = 0
  }

  function reset() {
    reservations.value = []
    currentReservation.value = null
    loading.value = false
    error.value = null
    clearCache()
  }

  return {
    // State
    reservations,
    currentReservation,
    loading,
    error,
    // Actions
    fetchReservations,
    fetchReservation,
    createReservation,
    updateReservation,
    deleteReservation,
    checkAvailability,
    clearCache,
    reset,
  }
})
