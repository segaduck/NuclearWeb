<template>
  <div class="reservations-page">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">會議室預約</h1>
      <button
        @click="handleNewReservation"
        class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
      >
        新增預約
      </button>
    </div>

    <!-- Calendar Toolbar -->
    <CalendarToolbar
      v-model:view="currentView"
      @prev="handlePrev"
      @next="handleNext"
      @today="handleToday"
      :current-date="currentDate"
    />

    <!-- Full Calendar -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-4 mt-4">
      <FullCalendar
        ref="calendarRef"
        :options="calendarOptions"
        class="nuclear-calendar"
      />
    </div>

    <!-- Reservation Dialog (Create/Edit) -->
    <ReservationDialog
      v-model:open="isDialogOpen"
      :reservation="selectedReservation"
      :selected-date="selectedDate"
      @saved="handleReservationSaved"
    />

    <!-- Reservation Details View -->
    <ReservationDetails
      v-model:open="isDetailsOpen"
      :reservation="selectedReservation"
      @edit="handleEditReservation"
      @delete="handleDeleteReservation"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import FullCalendar from '@fullcalendar/vue3'
import type { CalendarOptions, EventClickArg, DateSelectArg } from '@fullcalendar/core'
import dayGridPlugin from '@fullcalendar/daygrid'
import timeGridPlugin from '@fullcalendar/timegrid'
import interactionPlugin from '@fullcalendar/interaction'
import zhTwLocale from '@fullcalendar/core/locales/zh-tw'
import { useReservationsStore } from '@/stores/reservationsStore'
import { useAuthStore } from '@/stores/authStore'
import type { Reservation } from '@/services/reservationsService'
import CalendarToolbar from '@/components/reservations/CalendarToolbar.vue'
import ReservationDialog from '@/components/reservations/ReservationDialog.vue'
import ReservationDetails from '@/components/reservations/ReservationDetails.vue'

const reservationsStore = useReservationsStore()
const authStore = useAuthStore()

const calendarRef = ref()
const currentView = ref<'dayGridMonth' | 'timeGridWeek' | 'timeGridDay'>('dayGridMonth')
const currentDate = ref<Date>(new Date())
const isDialogOpen = ref(false)
const isDetailsOpen = ref(false)
const selectedReservation = ref<Reservation | null>(null)
const selectedDate = ref<Date | null>(null)

// 將預約轉換為 FullCalendar 事件
const calendarEvents = computed(() => {
  return reservationsStore.reservations.map((reservation) => ({
    id: reservation.id.toString(),
    title: `${reservation.meetingRoomName || '會議室'} - ${reservation.purpose || '無主旨'}`,
    start: reservation.startTime,
    end: reservation.endTime,
    backgroundColor: reservation.status === 'Confirmed' ? '#3b82f6' : '#9ca3af',
    borderColor: reservation.status === 'Confirmed' ? '#2563eb' : '#6b7280',
    textColor: '#ffffff',
    extendedProps: {
      reservation,
    },
  }))
})

// FullCalendar 配置
const calendarOptions = computed<CalendarOptions>(() => ({
  plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
  initialView: currentView.value,
  locale: zhTwLocale,
  headerToolbar: false, // 使用自訂工具列
  editable: false,
  selectable: true,
  selectMirror: true,
  dayMaxEvents: true,
  weekends: true,
  events: calendarEvents.value,
  select: handleDateSelect,
  eventClick: handleEventClick,
  datesSet: handleDatesSet,
  slotMinTime: '08:00:00',
  slotMaxTime: '20:00:00',
  allDaySlot: false,
  height: 'auto',
  eventTimeFormat: {
    hour: '2-digit',
    minute: '2-digit',
    hour12: false,
  },
}))

// 處理日期範圍變更
async function handleDatesSet(info: any) {
  currentDate.value = info.view.currentStart
  await loadReservations(info.startStr, info.endStr)
}

// 載入預約資料
async function loadReservations(startDate?: string, endDate?: string) {
  try {
    await reservationsStore.fetchReservations({
      startDate: startDate || new Date().toISOString(),
      endDate: endDate || new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString(),
      pageSize: 1000,
    })
  } catch (error) {
    console.error('載入預約失敗:', error)
  }
}

// 處理日期選擇（建立新預約）
function handleDateSelect(selectInfo: DateSelectArg) {
  selectedDate.value = selectInfo.start
  selectedReservation.value = null
  isDialogOpen.value = true

  // 清除選擇
  const calendarApi = selectInfo.view.calendar
  calendarApi.unselect()
}

// 處理事件點擊（查看預約詳情）
function handleEventClick(clickInfo: EventClickArg) {
  const reservation = clickInfo.event.extendedProps.reservation as Reservation
  selectedReservation.value = reservation
  isDetailsOpen.value = true
}

// 處理新增預約按鈕
function handleNewReservation() {
  selectedDate.value = new Date()
  selectedReservation.value = null
  isDialogOpen.value = true
}

// 處理編輯預約
function handleEditReservation(reservation: Reservation) {
  selectedReservation.value = reservation
  isDetailsOpen.value = false
  isDialogOpen.value = true
}

// 處理刪除預約
async function handleDeleteReservation(reservation: Reservation) {
  if (!confirm('確定要刪除此預約嗎？')) {
    return
  }

  try {
    await reservationsStore.deleteReservation(reservation.id)
    isDetailsOpen.value = false
    // 重新載入當前視圖的預約
    const calendarApi = calendarRef.value.getApi()
    const view = calendarApi.view
    await loadReservations(view.activeStart.toISOString(), view.activeEnd.toISOString())
  } catch (error: any) {
    alert(error.message || '刪除預約失敗')
  }
}

// 處理預約儲存成功
async function handleReservationSaved() {
  isDialogOpen.value = false
  // 重新載入當前視圖的預約
  const calendarApi = calendarRef.value.getApi()
  const view = calendarApi.view
  await loadReservations(view.activeStart.toISOString(), view.activeEnd.toISOString())
}

// 工具列處理函數
function handlePrev() {
  const calendarApi = calendarRef.value.getApi()
  calendarApi.prev()
}

function handleNext() {
  const calendarApi = calendarRef.value.getApi()
  calendarApi.next()
}

function handleToday() {
  const calendarApi = calendarRef.value.getApi()
  calendarApi.today()
}

// 監聽視圖變更
watch(currentView, (newView) => {
  const calendarApi = calendarRef.value?.getApi()
  if (calendarApi) {
    const startTime = performance.now()
    calendarApi.changeView(newView)
    const endTime = performance.now()
    console.log(`視圖切換耗時: ${endTime - startTime}ms`)
  }
})

// 初始化
onMounted(async () => {
  await loadReservations()
})
</script>

<style>
/* FullCalendar 樣式自訂 */
.nuclear-calendar {
  --fc-border-color: theme('colors.gray.200');
  --fc-button-bg-color: theme('colors.blue.600');
  --fc-button-border-color: theme('colors.blue.600');
  --fc-button-hover-bg-color: theme('colors.blue.700');
  --fc-button-hover-border-color: theme('colors.blue.700');
  --fc-button-active-bg-color: theme('colors.blue.800');
  --fc-button-active-border-color: theme('colors.blue.800');
  --fc-today-bg-color: theme('colors.blue.50');
}

.dark .nuclear-calendar {
  --fc-border-color: theme('colors.gray.700');
  --fc-page-bg-color: theme('colors.gray.800');
  --fc-neutral-bg-color: theme('colors.gray.700');
  --fc-neutral-text-color: theme('colors.gray.200');
  --fc-today-bg-color: theme('colors.blue.900');
}

.nuclear-calendar .fc-daygrid-day-number,
.nuclear-calendar .fc-col-header-cell-cushion,
.nuclear-calendar .fc-timegrid-slot-label {
  color: theme('colors.gray.700');
}

.dark .nuclear-calendar .fc-daygrid-day-number,
.dark .nuclear-calendar .fc-col-header-cell-cushion,
.dark .nuclear-calendar .fc-timegrid-slot-label {
  color: theme('colors.gray.300');
}

.nuclear-calendar .fc-event {
  cursor: pointer;
  border-radius: 4px;
  padding: 2px 4px;
}

.nuclear-calendar .fc-event:hover {
  opacity: 0.8;
}
</style>
