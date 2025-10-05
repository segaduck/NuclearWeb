<template>
  <div class="room-list-page">
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">會議室管理</h1>
      <button
        @click="handleCreateRoom"
        class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors flex items-center gap-2"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
        </svg>
        新增會議室
      </button>
    </div>

    <!-- Rooms Grid -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden">
      <!-- Loading State -->
      <div v-if="loading" class="flex items-center justify-center p-12">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        <span class="ml-3 text-gray-600 dark:text-gray-400">載入中...</span>
      </div>

      <!-- Empty State -->
      <div v-else-if="rooms.length === 0" class="text-center p-12">
        <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
        </svg>
        <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">沒有會議室</h3>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">開始建立您的第一間會議室吧</p>
      </div>

      <!-- Rooms Table -->
      <div v-else class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                會議室名稱
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                容納人數
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                位置
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                設施
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                狀態
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                操作
              </th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-for="room in rooms" :key="room.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-medium text-gray-900 dark:text-white">
                  {{ room.name }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center text-sm text-gray-900 dark:text-white">
                  <svg class="w-4 h-4 mr-1 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                  </svg>
                  {{ room.capacity }} 人
                </div>
              </td>
              <td class="px-6 py-4 text-sm text-gray-500 dark:text-gray-400">
                {{ room.location || '-' }}
              </td>
              <td class="px-6 py-4">
                <div class="flex flex-wrap gap-1">
                  <span
                    v-for="(amenity, index) in parseAmenities(room.amenities)"
                    :key="index"
                    class="inline-flex items-center px-2 py-0.5 rounded text-xs bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200"
                  >
                    {{ amenity }}
                  </span>
                  <span v-if="parseAmenities(room.amenities).length === 0" class="text-sm text-gray-400">
                    無
                  </span>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span
                  :class="[
                    'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                    room.isActive
                      ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
                      : 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300',
                  ]"
                >
                  {{ room.isActive ? '啟用' : '停用' }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <div class="flex items-center justify-end gap-2">
                  <button
                    @click="handleEditRoom(room)"
                    class="text-blue-600 dark:text-blue-400 hover:text-blue-900 dark:hover:text-blue-300"
                    title="編輯"
                  >
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                    </svg>
                  </button>
                  <button
                    @click="handleToggleActive(room)"
                    :class="[
                      'hover:opacity-75',
                      room.isActive ? 'text-gray-600 dark:text-gray-400' : 'text-green-600 dark:text-green-400'
                    ]"
                    :title="room.isActive ? '停用' : '啟用'"
                  >
                    <svg v-if="room.isActive" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18.364 18.364A9 9 0 005.636 5.636m12.728 12.728A9 9 0 015.636 5.636m12.728 12.728L5.636 5.636" />
                    </svg>
                    <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                  </button>
                  <button
                    @click="handleDeleteRoom(room)"
                    class="text-red-600 dark:text-red-400 hover:text-red-900 dark:hover:text-red-300"
                    title="刪除"
                  >
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Room Dialog -->
    <RoomDialog
      v-model:open="isDialogOpen"
      :room="selectedRoom"
      @saved="handleRoomSaved"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import roomsService, { type MeetingRoom } from '@/services/roomsService'
import RoomDialog from '@/components/admin/RoomDialog.vue'

const rooms = ref<MeetingRoom[]>([])
const loading = ref(false)
const isDialogOpen = ref(false)
const selectedRoom = ref<MeetingRoom | null>(null)

async function loadRooms() {
  loading.value = true
  try {
    const response = await roomsService.getRooms({ pageSize: 100, activeOnly: false })
    rooms.value = response.items
  } catch (error) {
    console.error('載入會議室列表失敗:', error)
  } finally {
    loading.value = false
  }
}

function parseAmenities(amenitiesJson: string | null): string[] {
  if (!amenitiesJson) return []
  try {
    const parsed = JSON.parse(amenitiesJson)
    return Array.isArray(parsed) ? parsed : []
  } catch {
    return []
  }
}

function handleCreateRoom() {
  selectedRoom.value = null
  isDialogOpen.value = true
}

function handleEditRoom(room: MeetingRoom) {
  selectedRoom.value = room
  isDialogOpen.value = true
}

async function handleToggleActive(room: MeetingRoom) {
  try {
    await roomsService.updateRoom(room.id, { isActive: !room.isActive })
    await loadRooms()
  } catch (error: any) {
    alert(error.message || '更新狀態失敗')
  }
}

async function handleDeleteRoom(room: MeetingRoom) {
  if (!confirm(`確定要刪除會議室「${room.name}」嗎？`)) {
    return
  }

  try {
    await roomsService.deleteRoom(room.id)
    await loadRooms()
  } catch (error: any) {
    alert(error.message || '刪除失敗')
  }
}

async function handleRoomSaved() {
  isDialogOpen.value = false
  await loadRooms()
}

onMounted(() => {
  loadRooms()
})
</script>
