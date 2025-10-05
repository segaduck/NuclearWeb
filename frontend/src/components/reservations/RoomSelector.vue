<template>
  <div>
    <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
      會議室 <span class="text-red-500">*</span>
    </label>

    <select
      :value="modelValue"
      @change="handleChange"
      required
      class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
      :class="{ 'border-red-500': error }"
    >
      <option value="0" disabled>請選擇會議室</option>
      <option
        v-for="room in rooms"
        :key="room.id"
        :value="room.id"
        class="py-2"
      >
        {{ room.name }} (容納 {{ room.capacity }} 人) {{ room.location ? `- ${room.location}` : '' }}
      </option>
    </select>

    <!-- Error Message -->
    <p v-if="error" class="mt-1 text-sm text-red-500">{{ error }}</p>

    <!-- Selected Room Details -->
    <div v-if="selectedRoom" class="mt-3 p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
      <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-2">會議室資訊</h4>
      <div class="space-y-1 text-sm text-gray-600 dark:text-gray-400">
        <div class="flex items-center">
          <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
          </svg>
          <span>容納人數：{{ selectedRoom.capacity }} 人</span>
        </div>

        <div v-if="selectedRoom.location" class="flex items-center">
          <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
          </svg>
          <span>位置：{{ selectedRoom.location }}</span>
        </div>

        <div v-if="amenitiesList.length > 0" class="flex items-start">
          <svg class="w-4 h-4 mr-2 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
          </svg>
          <div class="flex-1">
            <span>設施：</span>
            <div class="flex flex-wrap gap-1 mt-1">
              <span
                v-for="(amenity, index) in amenitiesList"
                :key="index"
                class="inline-flex items-center px-2 py-0.5 rounded text-xs bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200"
              >
                {{ amenity }}
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="loading" class="mt-3 flex items-center justify-center">
      <div class="animate-spin rounded-full h-5 w-5 border-b-2 border-blue-600"></div>
      <span class="ml-2 text-sm text-gray-600 dark:text-gray-400">載入會議室...</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import roomsService, { type MeetingRoom } from '@/services/roomsService'

interface Props {
  modelValue: number
  error?: string
}

const props = defineProps<Props>()

const emit = defineEmits<{
  'update:modelValue': [value: number]
}>()

const rooms = ref<MeetingRoom[]>([])
const loading = ref(false)

const selectedRoom = computed(() => {
  return rooms.value.find((r) => r.id === props.modelValue) || null
})

const amenitiesList = computed(() => {
  if (!selectedRoom.value?.amenities) return []
  try {
    const parsed = JSON.parse(selectedRoom.value.amenities)
    return Array.isArray(parsed) ? parsed : []
  } catch {
    return []
  }
})

function handleChange(event: Event) {
  const target = event.target as HTMLSelectElement
  emit('update:modelValue', Number(target.value))
}

async function loadRooms() {
  loading.value = true
  try {
    const response = await roomsService.getRooms({ activeOnly: true, pageSize: 100 })
    rooms.value = response.items
  } catch (error) {
    console.error('載入會議室列表失敗:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadRooms()
})
</script>
