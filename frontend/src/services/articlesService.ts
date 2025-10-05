import apiClient from './api'

/**
 * 文章服務
 * Articles service
 */

export interface Article {
  id: number
  title: string
  slug: string
  content: string
  summary: string | null
  featuredImage: string | null
  status: string
  availableFrom: string | null
  availableUntil: string | null
  authorId: number
  authorDisplayName?: string
  createdAt: string
  updatedAt: string
  publishedAt: string | null
}

export interface CreateArticleRequest {
  title: string
  slug: string
  content: string
  summary?: string
  featuredImage?: string
  availableFrom?: string
  availableUntil?: string
}

export interface UpdateArticleRequest {
  title?: string
  slug?: string
  content?: string
  summary?: string
  featuredImage?: string
  availableFrom?: string
  availableUntil?: string
}

export interface PaginatedArticles {
  items: Article[]
  totalCount: number
  page: number
  pageSize: number
}

const articlesService = {
  /**
   * 取得文章列表
   * Get articles list with filters
   */
  async getArticles(params: {
    page?: number
    pageSize?: number
    status?: string
    authorId?: number
  }): Promise<PaginatedArticles> {
    const response = await apiClient.get<PaginatedArticles>('/articles', { params })
    return response.data
  },

  /**
   * 取得已發布文章列表
   * Get published articles
   */
  async getPublishedArticles(params: {
    page?: number
    pageSize?: number
  }): Promise<PaginatedArticles> {
    const response = await apiClient.get<PaginatedArticles>('/articles/published', { params })
    return response.data
  },

  /**
   * 取得單一文章
   * Get article by ID
   */
  async getArticle(id: number): Promise<Article> {
    const response = await apiClient.get<Article>(`/articles/${id}`)
    return response.data
  },

  /**
   * 建立文章
   * Create article
   */
  async createArticle(data: CreateArticleRequest): Promise<Article> {
    const response = await apiClient.post<Article>('/articles', data)
    return response.data
  },

  /**
   * 更新文章
   * Update article
   */
  async updateArticle(id: number, data: UpdateArticleRequest): Promise<Article> {
    const response = await apiClient.put<Article>(`/articles/${id}`, data)
    return response.data
  },

  /**
   * 刪除文章
   * Delete article
   */
  async deleteArticle(id: number): Promise<void> {
    await apiClient.delete(`/articles/${id}`)
  },

  /**
   * 提交審核
   * Submit article for approval
   */
  async submitArticle(id: number): Promise<Article> {
    const response = await apiClient.post<Article>(`/articles/${id}/submit`)
    return response.data
  },

  /**
   * 批准文章
   * Approve article
   */
  async approveArticle(id: number): Promise<Article> {
    const response = await apiClient.post<Article>(`/articles/${id}/approve`)
    return response.data
  },

  /**
   * 拒絕文章
   * Reject article
   */
  async rejectArticle(id: number, reason?: string): Promise<Article> {
    const response = await apiClient.post<Article>(`/articles/${id}/reject`, { reason })
    return response.data
  },
}

export default articlesService
