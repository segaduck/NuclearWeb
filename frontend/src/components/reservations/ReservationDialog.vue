<template>
  <TransitionRoot :show="open" as="template">
    <Dialog as="div" class="relative z-50" @close="handleClose">
      <TransitionChild
        as="template"
        enter="ease-out duration-300"
        enter-from="opacity-0"
        enter-to="opacity-100"
        leave="ease-in duration-200"
        leave-from="opacity-100"
        leave-to="opacity-0"
      >
        <div class="fixed inset-0 bg-black bg-opacity-30 transition-opacity" />
      </TransitionChild>

      <div class="fixed inset-0 z-10 overflow-y-auto">
        <div class="flex min-h-full items-center justify-center p-4">
          <TransitionChild
            as="template"
            enter="ease-out duration-300"
            enter-from="opacity-0 scale-95"
            enter-to="opacity-100 scale-100"
            leave="ease-in duration-200"
            leave-from="opacity-100 scale-100"
            leave-to="opacity-0 scale-95"
          >
            <DialogPanel class="w-full max-w-2xl transform overflow-hidden rounded-lg bg-white dark:bg-gray-800 shadow-xl transition-all">
              <!-- Header -->
              <div class="border-b border-gray-200 dark:border-gray-700 px-6 py-4">
                <DialogTitle class="text-lg font-semibold text-gray-900 dark:text-white">
                  {{ isEditMode ? '編輯預約' : '新增預約' }}
                </DialogTitle>
              </div>

              <!-- Form -->
              <form @submit.prevent="handleSubmit" class="px-6 py-4 space-y-4">
                <!-- Room Selector -->
                <RoomSelector v-model="formData.meetingRoomId" :error="errors.meetingRoomId" />

                <!-- Date and Time -->
                <div class="grid grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                      開始日期時間 <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.startTime"
                      type="datetime-local"
                      required
                      class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      :class="{ 'border-red-500': errors.startTime }"
                    />
                    <p v-if="errors.startTime" class="mt-1 text-sm text-red-500">{{ errors.startTime }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                      結束日期時間 <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model="formData.endTime"
                      type="datetime-local"
                      required
                      class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      :class="{ 'border-red-500': errors.endTime }"
                    />
                    <p v-if="errors.endTime" class="mt-1 text-sm text-red-500">{{ errors.endTime }}</p>
                  </div>
                </div>

                <!-- Purpose -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    會議主旨
                  </label>
                  <input
                    v-model="formData.purpose"
                    type="text"
                    maxlength="500"
                    placeholder="請輸入會議主旨"
                    class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  />
                </div>

                <!-- Attendee Count -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    預計人數
                  </label>
                  <input
                    v-model.number="formData.attendeeCount"
                    type="number"
                    min="1"
                    placeholder="請輸入預計參加人數"
                    class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    :class="{ 'border-red-500': errors.attendeeCount }"
                  />
                  <p v-if="errors.attendeeCount" class="mt-1 text-sm text-red-500">{{ errors.attendeeCount }}</p>
                </div>

                <!-- Conflict Warning -->
                <div v-if="conflictWarning" class="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-4">
                  <div class="flex items-start">
                    <svg class="w-5 h-5 text-red-600 dark:text-red-400 mt-0.5 mr-2" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd" />
                    </svg>
                    <div class="flex-1">
                      <h4 class="font-medium text-red-800 dark:text-red-300">時間衝突</h4>
                      <p class="text-sm text-red-700 dark:text-red-400 mt-1">{{ conflictWarning }}</p>
                    </div>
                  </div>
                </div>

                <!-- Loading State -->
                <div v-if="checking" class="flex items-center justify-center py-2">
                  <div class="animate-spin rounded-full h-6 w-6 border-b-2 border-blue-600"></div>
                  <span class="ml-2 text-gray-600 dark:text-gray-400">檢查可用性...</span>
                </div>
              </form>

              <!-- Footer -->
              <div class="border-t border-gray-200 dark:border-gray-700 px-6 py-4 flex justify-end gap-3">
                <button
                  type="button"
                  @click="handleClose"
                  class="px-4 py-2 border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
                >
                  取消
                </button>
                <button
                  type="submit"
                  @click="handleSubmit"
                  :disabled="submitting || checking || !!conflictWarning"
                  class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  {{ submitting ? '儲存中...' : '儲存' }}
                </button>
              </div>
            </DialogPanel>
          </TransitionChild>
        </div>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import { Dialog, DialogPanel, DialogTitle, TransitionRoot, TransitionChild } from '@headlessui/vue'
import { useReservationsStore } from '@/stores/reservationsStore'
import type { Reservation } from '@/services/reservationsService'
import RoomSelector from './RoomSelector.vue'

interface Props {
  open: boolean
  reservation?: Reservation | null
  selectedDate?: Date | null
  selectedRoomId?: number | null
}

const props = defineProps<Props>()

const emit = defineEmits<{
  'update:open': [value: boolean]
  'saved': []
}>()

const reservationsStore = useReservationsStore()

// Form state
const formData = ref({
  meetingRoomId: 0,
  startTime: '',
  endTime: '',
  purpose: '',
  attendeeCount: null as number | null,
})

const errors = ref<Record<string, string>>({})
const submitting = ref(false)
const checking = ref(false)
const conflictWarning = ref('')

const isEditMode = computed(() => !!props.reservation)

// 初始化表單
watch(() => props.open, (isOpen) => {
  if (isOpen) {
    resetForm()
    if (props.reservation) {
      // 編輯模式：填入現有資料
      formData.value = {
        meetingRoomId: props.reservation.meetingRoomId,
        startTime: formatDateTimeLocal(new Date(props.reservation.startTime)),
        endTime: formatDateTimeLocal(new Date(props.reservation.endTime)),
        purpose: props.reservation.purpose || '',
        attendeeCount: props.reservation.attendeeCount,
      }
    } else {
      // 新增模式
      if (props.selectedRoomId) {
        formData.value.meetingRoomId = props.selectedRoomId
      }
      if (props.selectedDate) {
        const start = new Date(props.selectedDate)
        const end = new Date(start.getTime() + 60 * 60 * 1000) // 預設1小時
        formData.value.startTime = formatDateTimeLocal(start)
        formData.value.endTime = formatDateTimeLocal(end)
      }
    }
  }
})

// 監聽時間變更，檢查衝突
watch(
  () => [formData.value.meetingRoomId, formData.value.startTime, formData.value.endTime],
  async ([roomId, start, end]) => {
    if (roomId && start && end) {
      await checkConflict()
    }
  },
  { deep: true }
)

function formatDateTimeLocal(date: Date): string {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  const hours = String(date.getHours()).padStart(2, '0')
  const minutes = String(date.getMinutes()).padStart(2, '0')
  return `${year}-${month}-${day}T${hours}:${minutes}`
}

function resetForm() {
  formData.value = {
    meetingRoomId: 0,
    startTime: '',
    endTime: '',
    purpose: '',
    attendeeCount: null,
  }
  errors.value = {}
  conflictWarning.value = ''
}

async function checkConflict() {
  if (!formData.value.meetingRoomId || !formData.value.startTime || !formData.value.endTime) {
    return
  }

  checking.value = true
  conflictWarning.value = ''

  try {
    const result = await reservationsStore.checkAvailability({
      meetingRoomId: formData.value.meetingRoomId,
      startTime: new Date(formData.value.startTime).toISOString(),
      endTime: new Date(formData.value.endTime).toISOString(),
      excludeReservationId: props.reservation?.id,
    })

    if (!result.available && result.conflicts && result.conflicts.length > 0) {
      const conflict = result.conflicts[0]
      const startTime = new Date(conflict.startTime).toLocaleString('zh-TW')
      const endTime = new Date(conflict.endTime).toLocaleString('zh-TW')
      conflictWarning.value = `此時段已有預約：${startTime} - ${endTime}`
    }
  } catch (error: any) {
    console.error('檢查可用性失敗:', error)
  } finally {
    checking.value = false
  }
}

function validate(): boolean {
  errors.value = {}

  if (!formData.value.meetingRoomId) {
    errors.value.meetingRoomId = '請選擇會議室'
  }

  if (!formData.value.startTime) {
    errors.value.startTime = '請選擇開始時間'
  }

  if (!formData.value.endTime) {
    errors.value.endTime = '請選擇結束時間'
  }

  if (formData.value.startTime && formData.value.endTime) {
    const start = new Date(formData.value.startTime)
    const end = new Date(formData.value.endTime)
    if (end <= start) {
      errors.value.endTime = '結束時間必須晚於開始時間'
    }
  }

  return Object.keys(errors.value).length === 0
}

async function handleSubmit() {
  if (!validate()) {
    return
  }

  if (conflictWarning.value) {
    return
  }

  submitting.value = true

  try {
    const payload = {
      meetingRoomId: formData.value.meetingRoomId,
      startTime: new Date(formData.value.startTime).toISOString(),
      endTime: new Date(formData.value.endTime).toISOString(),
      purpose: formData.value.purpose || null,
      attendeeCount: formData.value.attendeeCount || null,
    }

    if (isEditMode.value && props.reservation) {
      await reservationsStore.updateReservation(props.reservation.id, payload)
    } else {
      await reservationsStore.createReservation(payload)
    }

    emit('saved')
    handleClose()
  } catch (error: any) {
    alert(error.message || '儲存失敗')
  } finally {
    submitting.value = false
  }
}

function handleClose() {
  emit('update:open', false)
}
</script>
