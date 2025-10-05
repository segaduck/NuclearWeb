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
            <DialogPanel class="w-full max-w-lg transform overflow-hidden rounded-lg bg-white dark:bg-gray-800 shadow-xl transition-all">
              <div v-if="reservation">
                <!-- Header -->
                <div class="border-b border-gray-200 dark:border-gray-700 px-6 py-4">
                  <DialogTitle class="text-lg font-semibold text-gray-900 dark:text-white">
                    預約詳情
                  </DialogTitle>
                </div>

                <!-- Content -->
                <div class="px-6 py-4 space-y-4">
                  <!-- Status Badge -->
                  <div>
                    <span
                      :class="[
                        'inline-flex items-center px-3 py-1 rounded-full text-sm font-medium',
                        reservation.status === 'Confirmed'
                          ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
                          : 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300',
                      ]"
                    >
                      {{ reservation.status === 'Confirmed' ? '已確認' : '已取消' }}
                    </span>
                  </div>

                  <!-- Meeting Room -->
                  <div>
                    <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-1">會議室</h3>
                    <p class="text-base text-gray-900 dark:text-white">
                      {{ reservation.meetingRoomName || `會議室 #${reservation.meetingRoomId}` }}
                    </p>
                  </div>

                  <!-- Time -->
                  <div class="grid grid-cols-2 gap-4">
                    <div>
                      <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-1">開始時間</h3>
                      <p class="text-base text-gray-900 dark:text-white">
                        {{ formatDateTime(reservation.startTime) }}
                      </p>
                    </div>
                    <div>
                      <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-1">結束時間</h3>
                      <p class="text-base text-gray-900 dark:text-white">
                        {{ formatDateTime(reservation.endTime) }}
                      </p>
                    </div>
                  </div>

                  <!-- Duration -->
                  <div>
                    <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-1">會議時長</h3>
                    <p class="text-base text-gray-900 dark:text-white">
                      {{ calculateDuration(reservation.startTime, reservation.endTime) }}
                    </p>
                  </div>

                  <!-- Purpose -->
                  <div v-if="reservation.purpose">
                    <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-1">會議主旨</h3>
                    <p class="text-base text-gray-900 dark:text-white">
                      {{ reservation.purpose }}
                    </p>
                  </div>

                  <!-- Attendee Count -->
                  <div v-if="reservation.attendeeCount">
                    <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-1">預計人數</h3>
                    <p class="text-base text-gray-900 dark:text-white">
                      {{ reservation.attendeeCount }} 人
                    </p>
                  </div>

                  <!-- Creator -->
                  <div v-if="reservation.userDisplayName">
                    <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-1">預約人</h3>
                    <p class="text-base text-gray-900 dark:text-white">
                      {{ reservation.userDisplayName }}
                    </p>
                  </div>

                  <!-- Created At -->
                  <div>
                    <h3 class="text-sm font-medium text-gray-500 dark:text-gray-400 mb-1">建立時間</h3>
                    <p class="text-sm text-gray-600 dark:text-gray-400">
                      {{ formatDateTime(reservation.createdAt) }}
                    </p>
                  </div>
                </div>

                <!-- Footer -->
                <div class="border-t border-gray-200 dark:border-gray-700 px-6 py-4 flex justify-end gap-3">
                  <button
                    type="button"
                    @click="handleClose"
                    class="px-4 py-2 border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
                  >
                    關閉
                  </button>

                  <button
                    v-if="canEdit"
                    type="button"
                    @click="handleEdit"
                    class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                  >
                    編輯
                  </button>

                  <button
                    v-if="canDelete"
                    type="button"
                    @click="handleDelete"
                    class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
                  >
                    刪除
                  </button>
                </div>
              </div>
            </DialogPanel>
          </TransitionChild>
        </div>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { Dialog, DialogPanel, DialogTitle, TransitionRoot, TransitionChild } from '@headlessui/vue'
import { useAuthStore } from '@/stores/authStore'
import type { Reservation } from '@/services/reservationsService'

interface Props {
  open: boolean
  reservation?: Reservation | null
}

const props = defineProps<Props>()

const emit = defineEmits<{
  'update:open': [value: boolean]
  'edit': [reservation: Reservation]
  'delete': [reservation: Reservation]
}>()

const authStore = useAuthStore()

// 權限檢查
const canEdit = computed(() => {
  if (!props.reservation) return false
  // 管理員或預約建立者可以編輯
  return (
    authStore.user?.role === 'Admin' ||
    authStore.user?.id === props.reservation.userId
  )
})

const canDelete = computed(() => {
  if (!props.reservation) return false
  // 管理員或預約建立者可以刪除
  return (
    authStore.user?.role === 'Admin' ||
    authStore.user?.id === props.reservation.userId
  )
})

function formatDateTime(dateTimeStr: string): string {
  const date = new Date(dateTimeStr)
  return date.toLocaleString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    hour12: false,
  })
}

function calculateDuration(startStr: string, endStr: string): string {
  const start = new Date(startStr)
  const end = new Date(endStr)
  const diffMs = end.getTime() - start.getTime()
  const diffMins = Math.floor(diffMs / (1000 * 60))

  const hours = Math.floor(diffMins / 60)
  const mins = diffMins % 60

  if (hours > 0 && mins > 0) {
    return `${hours} 小時 ${mins} 分鐘`
  } else if (hours > 0) {
    return `${hours} 小時`
  } else {
    return `${mins} 分鐘`
  }
}

function handleClose() {
  emit('update:open', false)
}

function handleEdit() {
  if (props.reservation) {
    emit('edit', props.reservation)
  }
}

function handleDelete() {
  if (props.reservation) {
    emit('delete', props.reservation)
  }
}
</script>
