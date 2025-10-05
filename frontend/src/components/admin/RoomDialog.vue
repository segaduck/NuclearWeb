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
                  {{ isEditMode ? '編輯會議室' : '新增會議室' }}
                </DialogTitle>
              </div>

              <!-- Form -->
              <form @submit.prevent="handleSubmit" class="px-6 py-4 space-y-4">
                <!-- Room Name -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    會議室名稱 <span class="text-red-500">*</span>
                  </label>
                  <input
                    v-model="formData.name"
                    type="text"
                    required
                    maxlength="100"
                    placeholder="例：大會議室A"
                    class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    :class="{ 'border-red-500': errors.name }"
                  />
                  <p v-if="errors.name" class="mt-1 text-sm text-red-500">{{ errors.name }}</p>
                </div>

                <!-- Capacity and Location -->
                <div class="grid grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                      容納人數 <span class="text-red-500">*</span>
                    </label>
                    <input
                      v-model.number="formData.capacity"
                      type="number"
                      required
                      min="1"
                      max="1000"
                      placeholder="例：20"
                      class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      :class="{ 'border-red-500': errors.capacity }"
                    />
                    <p v-if="errors.capacity" class="mt-1 text-sm text-red-500">{{ errors.capacity }}</p>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                      位置
                    </label>
                    <input
                      v-model="formData.location"
                      type="text"
                      maxlength="255"
                      placeholder="例：3樓東側"
                      class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    />
                  </div>
                </div>

                <!-- Amenities -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    設施設備
                  </label>
                  <div class="grid grid-cols-2 gap-2">
                    <label
                      v-for="amenity in availableAmenities"
                      :key="amenity"
                      class="flex items-center p-3 border border-gray-300 dark:border-gray-600 rounded-lg cursor-pointer hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
                    >
                      <input
                        type="checkbox"
                        :value="amenity"
                        v-model="formData.amenities"
                        class="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                      />
                      <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">{{ amenity }}</span>
                    </label>
                  </div>
                </div>

                <!-- Custom Amenity -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
                    自訂設施
                  </label>
                  <div class="flex gap-2">
                    <input
                      v-model="customAmenity"
                      type="text"
                      placeholder="輸入其他設施..."
                      class="flex-1 px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                      @keyup.enter="addCustomAmenity"
                    />
                    <button
                      type="button"
                      @click="addCustomAmenity"
                      class="px-4 py-2 bg-gray-600 text-white rounded-lg hover:bg-gray-700 transition-colors"
                    >
                      新增
                    </button>
                  </div>
                  <div v-if="formData.amenities.length > 0" class="mt-2 flex flex-wrap gap-1">
                    <span
                      v-for="(amenity, index) in formData.amenities"
                      :key="index"
                      class="inline-flex items-center px-2 py-1 rounded text-sm bg-blue-100 dark:bg-blue-900 text-blue-800 dark:text-blue-200"
                    >
                      {{ amenity }}
                      <button
                        type="button"
                        @click="removeAmenity(index)"
                        class="ml-1 hover:text-blue-600 dark:hover:text-blue-400"
                      >
                        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                        </svg>
                      </button>
                    </span>
                  </div>
                </div>

                <!-- Active Status -->
                <div v-if="isEditMode">
                  <label class="flex items-center cursor-pointer">
                    <input
                      type="checkbox"
                      v-model="formData.isActive"
                      class="w-4 h-4 text-blue-600 border-gray-300 rounded focus:ring-blue-500"
                    />
                    <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">啟用此會議室</span>
                  </label>
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
                  :disabled="submitting"
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
import roomsService, { type MeetingRoom } from '@/services/roomsService'

interface Props {
  open: boolean
  room?: MeetingRoom | null
}

const props = defineProps<Props>()

const emit = defineEmits<{
  'update:open': [value: boolean]
  'saved': []
}>()

const availableAmenities = [
  '投影機',
  '白板',
  '視訊會議',
  '電話',
  '網路',
  'WiFi',
  '音響設備',
  '講台',
]

const formData = ref({
  name: '',
  capacity: 10,
  location: '',
  amenities: [] as string[],
  isActive: true,
})

const customAmenity = ref('')
const errors = ref<Record<string, string>>({})
const submitting = ref(false)

const isEditMode = computed(() => !!props.room)

// 初始化表單
watch(() => props.open, (isOpen) => {
  if (isOpen) {
    resetForm()
    if (props.room) {
      // 編輯模式：填入現有資料
      formData.value = {
        name: props.room.name,
        capacity: props.room.capacity,
        location: props.room.location || '',
        amenities: parseAmenities(props.room.amenities),
        isActive: props.room.isActive,
      }
    }
  }
})

function parseAmenities(amenitiesJson: string | null): string[] {
  if (!amenitiesJson) return []
  try {
    const parsed = JSON.parse(amenitiesJson)
    return Array.isArray(parsed) ? parsed : []
  } catch {
    return []
  }
}

function resetForm() {
  formData.value = {
    name: '',
    capacity: 10,
    location: '',
    amenities: [],
    isActive: true,
  }
  customAmenity.value = ''
  errors.value = {}
}

function addCustomAmenity() {
  const amenity = customAmenity.value.trim()
  if (amenity && !formData.value.amenities.includes(amenity)) {
    formData.value.amenities.push(amenity)
    customAmenity.value = ''
  }
}

function removeAmenity(index: number) {
  formData.value.amenities.splice(index, 1)
}

function validate(): boolean {
  errors.value = {}

  if (!formData.value.name.trim()) {
    errors.value.name = '請輸入會議室名稱'
  }

  if (!formData.value.capacity || formData.value.capacity < 1) {
    errors.value.capacity = '容納人數必須大於 0'
  }

  if (formData.value.capacity > 1000) {
    errors.value.capacity = '容納人數不可超過 1000'
  }

  return Object.keys(errors.value).length === 0
}

async function handleSubmit() {
  if (!validate()) {
    return
  }

  submitting.value = true

  try {
    const payload = {
      name: formData.value.name.trim(),
      capacity: formData.value.capacity,
      location: formData.value.location.trim() || '',
      amenities: formData.value.amenities,
      isActive: formData.value.isActive,
    }

    if (isEditMode.value && props.room) {
      await roomsService.updateRoom(props.room.id, payload)
    } else {
      await roomsService.createRoom(payload)
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
