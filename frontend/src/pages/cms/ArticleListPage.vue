<template>
  <div class="article-list-page">
    <!-- Header -->
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-bold text-gray-800 dark:text-white">文章管理</h1>
      <button
        @click="handleCreateArticle"
        class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors flex items-center gap-2"
      >
        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
        </svg>
        新增文章
      </button>
    </div>

    <!-- Filters -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4 mb-6">
      <div class="flex flex-col sm:flex-row gap-4">
        <!-- Status Filter -->
        <div class="flex-1">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
            狀態篩選
          </label>
          <select
            v-model="filters.status"
            @change="loadArticles"
            class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500"
          >
            <option value="">全部狀態</option>
            <option value="Draft">草稿</option>
            <option value="PendingApproval">待審核</option>
            <option value="Published">已發布</option>
            <option value="Rejected">已拒絕</option>
          </select>
        </div>

        <!-- Author Filter (Admin only) -->
        <div v-if="isAdmin" class="flex-1">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
            作者篩選
          </label>
          <select
            v-model="filters.authorId"
            @change="loadArticles"
            class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-2 focus:ring-blue-500"
          >
            <option value="">全部作者</option>
            <option value="me">我的文章</option>
          </select>
        </div>

        <!-- Search (future enhancement) -->
        <div class="flex-1">
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">
            搜尋
          </label>
          <input
            v-model="searchQuery"
            type="text"
            placeholder="搜尋文章標題..."
            class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500"
          />
        </div>
      </div>
    </div>

    <!-- Articles Table/List -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden">
      <!-- Loading State -->
      <div v-if="loading" class="flex items-center justify-center p-12">
        <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
        <span class="ml-3 text-gray-600 dark:text-gray-400">載入中...</span>
      </div>

      <!-- Empty State -->
      <div v-else-if="filteredArticles.length === 0" class="text-center p-12">
        <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
        </svg>
        <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">沒有文章</h3>
        <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">開始建立您的第一篇文章吧</p>
      </div>

      <!-- Articles Table -->
      <div v-else class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                標題
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                狀態
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                作者
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                建立時間
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                操作
              </th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-for="article in filteredArticles" :key="article.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4">
                <div class="text-sm font-medium text-gray-900 dark:text-white">
                  {{ article.title }}
                </div>
                <div v-if="article.summary" class="text-sm text-gray-500 dark:text-gray-400 truncate max-w-md">
                  {{ article.summary }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="getStatusBadgeClass(article.status)">
                  {{ getStatusText(article.status) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                {{ article.authorDisplayName || `User #${article.authorId}` }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                {{ formatDate(article.createdAt) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <div class="flex items-center justify-end gap-2">
                  <button
                    @click="handleEditArticle(article.id)"
                    class="text-blue-600 dark:text-blue-400 hover:text-blue-900 dark:hover:text-blue-300"
                    title="編輯"
                  >
                    <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                    </svg>
                  </button>
                  <button
                    @click="handleDeleteArticle(article)"
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

      <!-- Pagination -->
      <div v-if="totalPages > 1" class="bg-white dark:bg-gray-800 px-4 py-3 border-t border-gray-200 dark:border-gray-700 sm:px-6">
        <div class="flex items-center justify-between">
          <div class="text-sm text-gray-700 dark:text-gray-300">
            顯示 {{ (currentPage - 1) * pageSize + 1 }} 到 {{ Math.min(currentPage * pageSize, totalCount) }} 筆，共 {{ totalCount }} 筆
          </div>
          <div class="flex gap-2">
            <button
              @click="prevPage"
              :disabled="currentPage === 1"
              class="px-3 py-1 border border-gray-300 dark:border-gray-600 rounded hover:bg-gray-50 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              上一頁
            </button>
            <button
              @click="nextPage"
              :disabled="currentPage === totalPages"
              class="px-3 py-1 border border-gray-300 dark:border-gray-600 rounded hover:bg-gray-50 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              下一頁
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import articlesService, { type Article } from '@/services/articlesService'

const router = useRouter()
const authStore = useAuthStore()

const articles = ref<Article[]>([])
const loading = ref(false)
const filters = ref({
  status: '',
  authorId: '',
})
const searchQuery = ref('')
const currentPage = ref(1)
const pageSize = ref(20)
const totalCount = ref(0)

const isAdmin = computed(() => authStore.user?.role === 'Admin')
const totalPages = computed(() => Math.ceil(totalCount.value / pageSize.value))

const filteredArticles = computed(() => {
  let result = articles.value

  // Client-side search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(article =>
      article.title.toLowerCase().includes(query) ||
      article.summary?.toLowerCase().includes(query)
    )
  }

  return result
})

async function loadArticles() {
  loading.value = true
  try {
    const params: any = {
      page: currentPage.value,
      pageSize: pageSize.value,
    }

    if (filters.value.status) {
      params.status = filters.value.status
    }

    if (filters.value.authorId === 'me') {
      params.authorId = authStore.user?.id
    }

    const response = await articlesService.getArticles(params)
    articles.value = response.items
    totalCount.value = response.totalCount
  } catch (error) {
    console.error('載入文章列表失敗:', error)
  } finally {
    loading.value = false
  }
}

function handleCreateArticle() {
  router.push('/cms/articles/new')
}

function handleEditArticle(id: number) {
  router.push(`/cms/articles/${id}/edit`)
}

async function handleDeleteArticle(article: Article) {
  if (!confirm(`確定要刪除文章「${article.title}」嗎？`)) {
    return
  }

  try {
    await articlesService.deleteArticle(article.id)
    await loadArticles()
  } catch (error: any) {
    alert(error.message || '刪除失敗')
  }
}

function getStatusText(status: string): string {
  const statusMap: Record<string, string> = {
    Draft: '草稿',
    PendingApproval: '待審核',
    Published: '已發布',
    Rejected: '已拒絕',
  }
  return statusMap[status] || status
}

function getStatusBadgeClass(status: string): string {
  const baseClass = 'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium'
  const statusClasses: Record<string, string> = {
    Draft: 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300',
    PendingApproval: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    Published: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    Rejected: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
  }
  return `${baseClass} ${statusClasses[status] || statusClasses.Draft}`
}

function formatDate(dateStr: string): string {
  const date = new Date(dateStr)
  return date.toLocaleDateString('zh-TW', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
  })
}

function prevPage() {
  if (currentPage.value > 1) {
    currentPage.value--
    loadArticles()
  }
}

function nextPage() {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
    loadArticles()
  }
}

onMounted(() => {
  loadArticles()
})
</script>
