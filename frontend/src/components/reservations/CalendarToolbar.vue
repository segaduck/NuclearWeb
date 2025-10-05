<template>
  <div class="calendar-toolbar bg-white dark:bg-gray-800 rounded-lg shadow p-4">
    <div class="flex flex-col sm:flex-row justify-between items-center gap-4">
      <!-- 左側：導航按鈕 -->
      <div class="flex items-center gap-2">
        <button
          @click="$emit('prev')"
          class="px-3 py-2 bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors"
          title="上一個"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
          </svg>
        </button>

        <button
          @click="$emit('today')"
          class="px-4 py-2 bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors font-medium"
        >
          今天
        </button>

        <button
          @click="$emit('next')"
          class="px-3 py-2 bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors"
          title="下一個"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
          </svg>
        </button>
      </div>

      <!-- 中間：當前日期顯示 -->
      <div class="text-lg font-semibold text-gray-800 dark:text-white">
        {{ formattedDate }}
      </div>

      <!-- 右側：視圖切換 -->
      <div class="flex gap-1 bg-gray-100 dark:bg-gray-700 rounded-lg p-1">
        <button
          @click="changeView('dayGridMonth')"
          :class="[
            'px-4 py-2 rounded transition-all',
            view === 'dayGridMonth'
              ? 'bg-white dark:bg-gray-600 text-blue-600 dark:text-blue-400 shadow'
              : 'text-gray-600 dark:text-gray-400 hover:text-gray-800 dark:hover:text-gray-200',
          ]"
        >
          月
        </button>
        <button
          @click="changeView('timeGridWeek')"
          :class="[
            'px-4 py-2 rounded transition-all',
            view === 'timeGridWeek'
              ? 'bg-white dark:bg-gray-600 text-blue-600 dark:text-blue-400 shadow'
              : 'text-gray-600 dark:text-gray-400 hover:text-gray-800 dark:hover:text-gray-200',
          ]"
        >
          週
        </button>
        <button
          @click="changeView('timeGridDay')"
          :class="[
            'px-4 py-2 rounded transition-all',
            view === 'timeGridDay'
              ? 'bg-white dark:bg-gray-600 text-blue-600 dark:text-blue-400 shadow'
              : 'text-gray-600 dark:text-gray-400 hover:text-gray-800 dark:hover:text-gray-200',
          ]"
        >
          日
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  view: 'dayGridMonth' | 'timeGridWeek' | 'timeGridDay'
  currentDate: Date
}

const props = defineProps<Props>()

const emit = defineEmits<{
  'update:view': [value: 'dayGridMonth' | 'timeGridWeek' | 'timeGridDay']
  'prev': []
  'next': []
  'today': []
}>()

// 格式化顯示日期
const formattedDate = computed(() => {
  const date = props.currentDate
  const year = date.getFullYear()
  const month = date.getMonth() + 1
  const day = date.getDate()

  switch (props.view) {
    case 'dayGridMonth':
      return `${year}年 ${month}月`
    case 'timeGridWeek':
      // 顯示週的開始日期
      return `${year}年 ${month}月 ${day}日 - 週檢視`
    case 'timeGridDay':
      return `${year}年 ${month}月 ${day}日`
    default:
      return `${year}年 ${month}月`
  }
})

function changeView(newView: 'dayGridMonth' | 'timeGridWeek' | 'timeGridDay') {
  emit('update:view', newView)
}
</script>
