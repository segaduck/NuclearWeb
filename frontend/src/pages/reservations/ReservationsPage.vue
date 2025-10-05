<template>
  <div class="flex h-[calc(100vh-64px)]">
    <!-- Left Sidebar: Room Status Cards -->
    <aside class="w-80 bg-white dark:bg-gray-800 border-r border-gray-200 dark:border-gray-700 overflow-y-auto">
      <div class="p-4">
        <!-- Collapsible Filters -->
        <div class="mb-4 pb-4 border-b border-gray-200 dark:border-gray-700">
          <div
            class="flex items-center justify-between cursor-pointer hover:bg-gray-50 dark:hover:bg-gray-700 -mx-2 px-2 py-1 rounded"
            @click="toggleFilters"
          >
            <h3 class="text-xs font-bold text-gray-900 dark:text-white">快速篩選</h3>
            <span class="text-xs text-gray-600 dark:text-gray-400">{{ filtersExpanded ? '▼' : '▶' }}</span>
          </div>
          <div v-if="filtersExpanded" class="space-y-1.5 mt-2">
            <label class="flex items-center space-x-2">
              <input
                type="checkbox"
                v-model="filters.onlyAvailable"
                class="w-3.5 h-3.5 text-blue-600 rounded"
              >
              <span class="text-xs text-gray-700 dark:text-gray-300">僅顯示可用</span>
            </label>
            <label class="flex items-center space-x-2">
              <input
                type="checkbox"
                v-model="filters.needsProjector"
                class="w-3.5 h-3.5 text-blue-600 rounded"
              >
              <span class="text-xs text-gray-700 dark:text-gray-300">需要投影機</span>
            </label>
            <label class="flex items-center space-x-2">
              <input
                type="checkbox"
                v-model="filters.needsVideo"
                class="w-3.5 h-3.5 text-blue-600 rounded"
              >
              <span class="text-xs text-gray-700 dark:text-gray-300">需要視訊設備</span>
            </label>
          </div>
        </div>

        <!-- Room Cards -->
        <div class="space-y-4">
          <div
            v-for="room in filteredRooms"
            :key="room.id"
            class="room-card bg-white dark:bg-gray-800 rounded-xl shadow-lg overflow-hidden cursor-pointer transition-transform duration-200 hover:-translate-y-1"
            :class="{ 'border-2 border-blue-600': selectedRoomId === room.id }"
            @click="selectedRoomId = room.id"
          >
            <!-- Room Header -->
            <div
              class="px-4 py-3 flex items-center justify-between"
              :class="getRoomHeaderClass(room)"
            >
              <div>
                <h3 class="text-lg font-black text-white">{{ room.name }}</h3>
                <p class="text-xs opacity-90">{{ room.location }}</p>
              </div>
              <div class="bg-white px-2 py-1 rounded-full">
                <p class="text-xs font-black" :class="getRoomStatusTextClass(room)">
                  {{ getRoomStatusText(room) }}
                </p>
              </div>
            </div>

            <!-- Room Details -->
            <div class="p-4">
              <!-- Capacity -->
              <div class="flex items-center justify-between mb-3">
                <div class="flex items-center space-x-1.5">
                  <span class="text-xl">👥</span>
                  <span class="text-xs text-gray-600 dark:text-gray-400">容納 {{ room.capacity }} 人</span>
                </div>
              </div>

              <!-- Amenities -->
              <div class="flex flex-wrap gap-1.5 mb-3">
                <span
                  v-for="amenity in room.amenities"
                  :key="amenity"
                  class="px-2 py-0.5 bg-blue-50 dark:bg-blue-900 text-blue-700 dark:text-blue-300 text-xs rounded-full"
                >
                  {{ amenity }}
                </span>
              </div>

              <!-- Current or Next Meeting -->
              <div v-if="getRoomCurrentMeeting(room) || getRoomNextMeeting(room)" class="space-y-1.5 mb-3">
                <p class="text-xs font-bold text-gray-900 dark:text-white">
                  {{ getRoomCurrentMeeting(room) ? '目前會議' : '下一場預約' }}
                </p>
                <div
                  class="p-2 rounded-lg"
                  :class="getRoomCurrentMeeting(room) ? 'bg-red-50 dark:bg-red-900/30 border-2 border-red-200 dark:border-red-700' : 'bg-gray-50 dark:bg-gray-700'"
                >
                  <p class="text-xs font-medium text-gray-900 dark:text-white">
                    {{ (getRoomCurrentMeeting(room) || getRoomNextMeeting(room))?.purpose }}
                  </p>
                  <div class="flex items-center space-x-1 mt-0.5">
                    <span class="text-xs text-gray-600 dark:text-gray-400">
                      ⏰ {{ formatTimeRange(getRoomCurrentMeeting(room) || getRoomNextMeeting(room)) }}
                    </span>
                    <span class="text-xs text-gray-400">•</span>
                    <span class="text-xs text-gray-600 dark:text-gray-400">
                      👤 {{ (getRoomCurrentMeeting(room) || getRoomNextMeeting(room))?.createdByName }}
                    </span>
                  </div>
                  <!-- Progress bar for current meeting -->
                  <div v-if="getRoomCurrentMeeting(room)" class="mt-1.5 flex items-center space-x-1.5">
                    <div class="flex-1 bg-red-200 dark:bg-red-800 rounded-full h-1">
                      <div
                        class="bg-red-600 h-1 rounded-full"
                        :style="{ width: getMeetingProgress(getRoomCurrentMeeting(room)) + '%' }"
                      ></div>
                    </div>
                    <span class="text-xs text-gray-600 dark:text-gray-400">
                      剩 {{ getRemainingMinutes(getRoomCurrentMeeting(room)) }}分
                    </span>
                  </div>
                </div>
              </div>

              <!-- Book Button -->
              <button
                v-if="!getRoomCurrentMeeting(room)"
                @click.stop="handleBookRoom(room)"
                class="w-full py-2 bg-green-600 text-white rounded-lg text-sm font-bold hover:bg-green-700 transition-colors"
              >
                立即預約
              </button>
              <button
                v-else
                disabled
                class="w-full py-2 bg-gray-200 dark:bg-gray-700 text-gray-500 dark:text-gray-400 rounded-lg text-sm font-bold cursor-not-allowed"
              >
                使用中
              </button>
            </div>
          </div>
        </div>
      </div>
    </aside>

    <!-- Main Content: Daily/Weekly View -->
    <main class="flex-1 overflow-hidden flex flex-col bg-gray-50 dark:bg-gray-900">
      <!-- Top Bar -->
      <header class="bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700 px-6 py-4">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-2xl font-black text-gray-900 dark:text-white">會議室預約</h1>
            <p class="text-sm text-gray-600 dark:text-gray-400">{{ formatCurrentDate() }}</p>
          </div>
          <div class="flex items-center space-x-3">
            <!-- View Switcher -->
            <div class="flex items-center space-x-1 bg-gray-100 dark:bg-gray-700 p-1 rounded-lg">
              <button
                @click="currentView = 'daily'"
                :class="currentView === 'daily'
                  ? 'px-3 py-1 text-sm bg-blue-600 text-white rounded font-medium'
                  : 'px-3 py-1 text-sm bg-transparent text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600'"
              >
                日視圖
              </button>
              <button
                @click="currentView = 'weekly'"
                :class="currentView === 'weekly'
                  ? 'px-3 py-1 text-sm bg-blue-600 text-white rounded font-medium'
                  : 'px-3 py-1 text-sm bg-transparent text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600'"
              >
                週視圖
              </button>
            </div>
            <!-- Date Navigation -->
            <div class="flex items-center space-x-2">
              <button
                @click="goToPreviousDay"
                class="px-3 py-1 border border-gray-300 dark:border-gray-600 rounded hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                ◀
              </button>
              <button
                @click="goToToday"
                class="px-3 py-1 bg-blue-600 text-white rounded hover:bg-blue-700"
              >
                今天
              </button>
              <button
                @click="goToNextDay"
                class="px-3 py-1 border border-gray-300 dark:border-gray-600 rounded hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                ▶
              </button>
            </div>
          </div>
        </div>
      </header>

      <!-- Daily List View -->
      <div v-if="currentView === 'daily'" class="flex-1 overflow-y-auto p-6">
        <div class="space-y-2">
          <div
            v-for="slot in timeSlots"
            :key="slot.time"
            class="time-slot bg-white dark:bg-gray-800 rounded-lg p-4 min-h-[60px] border-l-4 transition-colors"
            :class="getSlotClass(slot)"
          >
            <div class="flex items-start">
              <div class="w-20 flex-shrink-0">
                <p class="text-sm font-bold text-gray-900 dark:text-white">{{ slot.time }}</p>
              </div>
              <div class="flex-1">
                <div v-if="getSlotReservation(slot)" class="flex items-center justify-between">
                  <div>
                    <p class="text-sm font-bold text-gray-900 dark:text-white">
                      {{ getSlotReservation(slot)?.purpose }}
                    </p>
                    <div class="flex items-center space-x-2 mt-1">
                      <span class="text-xs text-gray-600 dark:text-gray-400">
                        👤 {{ getSlotReservation(slot)?.createdByName }}
                      </span>
                      <span class="text-xs text-gray-400">•</span>
                      <span class="text-xs text-gray-600 dark:text-gray-400">
                        👥 {{ getSlotReservation(slot)?.attendeeCount }} 人
                      </span>
                      <span class="text-xs text-gray-400">•</span>
                      <span class="text-xs text-gray-600 dark:text-gray-400">
                        {{ formatTimeRange(getSlotReservation(slot)) }}
                      </span>
                    </div>
                  </div>
                  <button
                    @click="handleViewReservation(getSlotReservation(slot))"
                    class="px-3 py-1 text-xs bg-blue-600 text-white rounded hover:bg-blue-700"
                  >
                    詳情
                  </button>
                </div>
                <div v-else class="text-sm text-gray-400 dark:text-gray-500">
                  可預約
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Weekly Grid View -->
      <div v-else class="flex-1 overflow-auto p-6">
        <div class="weekly-grid">
          <!-- Header Row -->
          <div class="bg-gray-100 dark:bg-gray-700 p-2 font-bold text-xs text-center">時間</div>
          <div
            v-for="day in weekDays"
            :key="day.date"
            class="bg-gray-100 dark:bg-gray-700 p-2 font-bold text-xs text-center"
          >
            <div>{{ day.dayName }}</div>
            <div class="text-gray-600 dark:text-gray-400">{{ day.dateStr }}</div>
          </div>

          <!-- Time Slots -->
          <template v-for="slot in timeSlots" :key="slot.time">
            <div class="bg-gray-50 dark:bg-gray-800 p-2 text-xs font-medium text-center border border-gray-200 dark:border-gray-700">
              {{ slot.time }}
            </div>
            <div
              v-for="day in weekDays"
              :key="`${day.date}-${slot.time}`"
              class="weekly-slot border border-gray-200 dark:border-gray-700 p-1 text-xs cursor-pointer hover:bg-blue-50 dark:hover:bg-blue-900/30"
              :class="getWeeklySlotClass(day, slot)"
              @click="handleWeeklySlotClick(day, slot)"
            >
              <div v-if="getWeeklySlotReservation(day, slot)" class="text-xs">
                <div class="font-medium truncate">{{ getWeeklySlotReservation(day, slot)?.purpose }}</div>
                <div class="text-gray-600 dark:text-gray-400 truncate">{{ getWeeklySlotReservation(day, slot)?.createdByName }}</div>
              </div>
            </div>
          </template>
        </div>
      </div>
    </main>

    <!-- Reservation Dialog -->
    <ReservationDialog
      v-model:open="isDialogOpen"
      :reservation="selectedReservation"
      :selected-date="selectedDate"
      :selected-room-id="selectedRoomId"
      @saved="handleReservationSaved"
    />

    <!-- Reservation Details -->
    <ReservationDetails
      v-model:open="isDetailsOpen"
      :reservation="selectedReservation"
      @edit="handleEditReservation"
      @delete="handleDeleteReservation"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useReservationsStore } from '@/stores/reservationsStore'
import { useRoomsStore } from '@/stores/roomsStore'
import type { Reservation } from '@/services/reservationsService'
import type { MeetingRoom } from '@/services/roomsService'
import ReservationDialog from '@/components/reservations/ReservationDialog.vue'
import ReservationDetails from '@/components/reservations/ReservationDetails.vue'

const reservationsStore = useReservationsStore()
const roomsStore = useRoomsStore()

// State
const currentView = ref<'daily' | 'weekly'>('daily')
const currentDate = ref<Date>(new Date())
const selectedRoomId = ref<number | null>(null)
const filtersExpanded = ref(true)
const filters = ref({
  onlyAvailable: false,
  needsProjector: false,
  needsVideo: false,
})

const isDialogOpen = ref(false)
const isDetailsOpen = ref(false)
const selectedReservation = ref<Reservation | null>(null)
const selectedDate = ref<Date | null>(null)

// Time slots for daily/weekly view (8:00 - 18:00)
const timeSlots = [
  { time: '08:00', hour: 8 },
  { time: '09:00', hour: 9 },
  { time: '10:00', hour: 10 },
  { time: '11:00', hour: 11 },
  { time: '12:00', hour: 12 },
  { time: '13:00', hour: 13 },
  { time: '14:00', hour: 14 },
  { time: '15:00', hour: 15 },
  { time: '16:00', hour: 16 },
  { time: '17:00', hour: 17 },
  { time: '18:00', hour: 18 },
]

// Computed
const filteredRooms = computed(() => {
  let rooms = roomsStore.rooms.filter(r => r.isActive)

  if (filters.value.onlyAvailable) {
    rooms = rooms.filter(r => !getRoomCurrentMeeting(r))
  }

  if (filters.value.needsProjector) {
    rooms = rooms.filter(r => r.amenities?.includes('投影機'))
  }

  if (filters.value.needsVideo) {
    rooms = rooms.filter(r => r.amenities?.includes('視訊會議設備') || r.amenities?.includes('視訊'))
  }

  return rooms
})

const weekDays = computed(() => {
  const days = []
  const startOfWeek = new Date(currentDate.value)
  startOfWeek.setDate(startOfWeek.getDate() - startOfWeek.getDay()) // Sunday

  for (let i = 0; i < 7; i++) {
    const day = new Date(startOfWeek)
    day.setDate(day.getDate() + i)
    days.push({
      date: day.toISOString().split('T')[0],
      dateStr: `${day.getMonth() + 1}/${day.getDate()}`,
      dayName: ['日', '一', '二', '三', '四', '五', '六'][day.getDay()],
      fullDate: day,
    })
  }

  return days
})

const todayReservations = computed(() => {
  if (!selectedRoomId.value) return []

  const dateStr = currentDate.value.toISOString().split('T')[0]
  return reservationsStore.reservations.filter(r =>
    r.meetingRoomId === selectedRoomId.value &&
    r.startTime.startsWith(dateStr) &&
    r.status === 'Confirmed'
  )
})

// Methods
function toggleFilters() {
  filtersExpanded.value = !filtersExpanded.value
}

function getRoomHeaderClass(room: MeetingRoom) {
  const current = getRoomCurrentMeeting(room)
  if (current) {
    return 'bg-red-500'
  }
  return 'bg-green-500'
}

function getRoomStatusTextClass(room: MeetingRoom) {
  const current = getRoomCurrentMeeting(room)
  if (current) {
    return 'text-red-600'
  }
  return 'text-green-600'
}

function getRoomStatusText(room: MeetingRoom) {
  const current = getRoomCurrentMeeting(room)
  if (current) {
    return '使用中'
  }
  return '可用'
}

function getRoomCurrentMeeting(room: MeetingRoom) {
  const now = new Date()
  return reservationsStore.reservations.find(r =>
    r.meetingRoomId === room.id &&
    r.status === 'Confirmed' &&
    new Date(r.startTime) <= now &&
    new Date(r.endTime) > now
  )
}

function getRoomNextMeeting(room: MeetingRoom) {
  const now = new Date()
  const upcoming = reservationsStore.reservations
    .filter(r =>
      r.meetingRoomId === room.id &&
      r.status === 'Confirmed' &&
      new Date(r.startTime) > now
    )
    .sort((a, b) => new Date(a.startTime).getTime() - new Date(b.startTime).getTime())

  return upcoming[0] || null
}

function getMeetingProgress(meeting: Reservation | null) {
  if (!meeting) return 0

  const start = new Date(meeting.startTime).getTime()
  const end = new Date(meeting.endTime).getTime()
  const now = Date.now()

  const progress = ((now - start) / (end - start)) * 100
  return Math.min(Math.max(progress, 0), 100)
}

function getRemainingMinutes(meeting: Reservation | null) {
  if (!meeting) return 0

  const end = new Date(meeting.endTime).getTime()
  const now = Date.now()
  const remaining = Math.max(0, Math.floor((end - now) / 1000 / 60))

  return remaining
}

function formatTimeRange(meeting: Reservation | null | undefined) {
  if (!meeting) return ''

  const start = new Date(meeting.startTime)
  const end = new Date(meeting.endTime)

  return `${start.getHours().toString().padStart(2, '0')}:${start.getMinutes().toString().padStart(2, '0')}-${end.getHours().toString().padStart(2, '0')}:${end.getMinutes().toString().padStart(2, '0')}`
}

function formatCurrentDate() {
  const days = ['日', '一', '二', '三', '四', '五', '六']
  const year = currentDate.value.getFullYear()
  const month = currentDate.value.getMonth() + 1
  const date = currentDate.value.getDate()
  const day = days[currentDate.value.getDay()]

  return `${year}年${month}月${date}日 週${day}`
}

function getSlotReservation(slot: { time: string; hour: number }) {
  if (!selectedRoomId.value) return null

  return todayReservations.value.find(r => {
    const start = new Date(r.startTime)
    return start.getHours() === slot.hour
  })
}

function getSlotClass(slot: { time: string; hour: number }) {
  const reservation = getSlotReservation(slot)
  if (reservation) {
    const now = new Date()
    const start = new Date(reservation.startTime)
    const end = new Date(reservation.endTime)

    if (now >= start && now < end) {
      return 'border-red-500 bg-red-50 dark:bg-red-900/20'
    }
    return 'border-gray-300 dark:border-gray-600 bg-gray-50 dark:bg-gray-700'
  }
  return 'border-green-500 bg-green-50 dark:bg-green-900/20'
}

function getWeeklySlotReservation(day: any, slot: { time: string; hour: number }) {
  if (!selectedRoomId.value) return null

  return reservationsStore.reservations.find(r => {
    const start = new Date(r.startTime)
    return (
      r.meetingRoomId === selectedRoomId.value &&
      r.status === 'Confirmed' &&
      start.toISOString().split('T')[0] === day.date &&
      start.getHours() === slot.hour
    )
  })
}

function getWeeklySlotClass(day: any, slot: { time: string; hour: number }) {
  const reservation = getWeeklySlotReservation(day, slot)
  if (reservation) {
    return 'bg-red-100 dark:bg-red-900/30'
  }
  return 'bg-white dark:bg-gray-800'
}

function handleWeeklySlotClick(day: any, slot: { time: string; hour: number }) {
  const reservation = getWeeklySlotReservation(day, slot)
  if (reservation) {
    selectedReservation.value = reservation
    isDetailsOpen.value = true
  } else {
    const date = new Date(day.fullDate)
    date.setHours(slot.hour, 0, 0, 0)
    selectedDate.value = date
    selectedReservation.value = null
    isDialogOpen.value = true
  }
}

function handleBookRoom(room: MeetingRoom) {
  selectedRoomId.value = room.id
  selectedDate.value = new Date()
  selectedReservation.value = null
  isDialogOpen.value = true
}

function handleViewReservation(reservation: Reservation | null) {
  if (!reservation) return
  selectedReservation.value = reservation
  isDetailsOpen.value = true
}

function handleEditReservation(reservation: Reservation) {
  selectedReservation.value = reservation
  isDetailsOpen.value = false
  isDialogOpen.value = true
}

async function handleDeleteReservation(reservation: Reservation) {
  if (!confirm('確定要刪除此預約嗎？')) {
    return
  }

  try {
    await reservationsStore.deleteReservation(reservation.id)
    isDetailsOpen.value = false
    await loadData()
  } catch (error: any) {
    alert(error.message || '刪除預約失敗')
  }
}

async function handleReservationSaved() {
  isDialogOpen.value = false
  await loadData()
}

function goToToday() {
  currentDate.value = new Date()
  loadData()
}

function goToPreviousDay() {
  const newDate = new Date(currentDate.value)
  newDate.setDate(newDate.getDate() - 1)
  currentDate.value = newDate
  loadData()
}

function goToNextDay() {
  const newDate = new Date(currentDate.value)
  newDate.setDate(newDate.getDate() + 1)
  currentDate.value = newDate
  loadData()
}

async function loadData() {
  try {
    await Promise.all([
      roomsStore.fetchRooms(),
      reservationsStore.fetchReservations({
        startDate: new Date(currentDate.value.getTime() - 7 * 24 * 60 * 60 * 1000).toISOString(),
        endDate: new Date(currentDate.value.getTime() + 14 * 24 * 60 * 60 * 1000).toISOString(),
        pageSize: 1000,
      }),
    ])

    // Auto-select first room if none selected
    if (!selectedRoomId.value && filteredRooms.value.length > 0) {
      selectedRoomId.value = filteredRooms.value[0].id
    }
  } catch (error) {
    console.error('載入資料失敗:', error)
  }
}

// Initialize
onMounted(() => {
  loadData()
})
</script>

<style scoped>
.room-card {
  transition: transform 0.2s;
}

.room-card:hover {
  transform: translateY(-2px);
}

.time-slot {
  min-height: 60px;
}

.weekly-grid {
  display: grid;
  grid-template-columns: 80px repeat(7, 1fr);
  gap: 0;
}

.weekly-slot {
  min-height: 60px;
}
</style>
