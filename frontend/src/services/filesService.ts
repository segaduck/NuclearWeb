import apiClient from './api'

/**
 * 檔案服務
 * Files service
 */

export interface UploadedFile {
  id: number
  fileName: string
  originalFileName: string
  mimeType: string
  fileSize: number
  filePath: string
  category: string | null
  description: string | null
  uploadedById: number
  uploaderDisplayName?: string
  createdAt: string
  updatedAt: string
}

export interface UpdateFileRequest {
  category?: string
  description?: string
}

export interface PaginatedFiles {
  items: UploadedFile[]
  totalCount: number
  page: number
  pageSize: number
}

const filesService = {
  /**
   * 取得檔案列表
   * Get files list
   */
  async getFiles(params: {
    page?: number
    pageSize?: number
    category?: string
    uploadedById?: number
  }): Promise<PaginatedFiles> {
    const response = await apiClient.get<PaginatedFiles>('/files', { params })
    return response.data
  },

  /**
   * 取得單一檔案
   * Get file by ID
   */
  async getFile(id: number): Promise<UploadedFile> {
    const response = await apiClient.get<UploadedFile>(`/files/${id}`)
    return response.data
  },

  /**
   * 上傳檔案
   * Upload file
   */
  async uploadFile(file: File, category?: string, description?: string): Promise<UploadedFile> {
    const formData = new FormData()
    formData.append('file', file)
    if (category) formData.append('category', category)
    if (description) formData.append('description', description)

    const response = await apiClient.post<UploadedFile>('/files', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
    return response.data
  },

  /**
   * 更新檔案資訊
   * Update file metadata
   */
  async updateFile(id: number, data: UpdateFileRequest): Promise<UploadedFile> {
    const response = await apiClient.put<UploadedFile>(`/files/${id}`, data)
    return response.data
  },

  /**
   * 刪除檔案
   * Delete file
   */
  async deleteFile(id: number): Promise<void> {
    await apiClient.delete(`/files/${id}`)
  },

  /**
   * 下載檔案
   * Download file
   */
  async downloadFile(id: number): Promise<Blob> {
    const response = await apiClient.get(`/files/${id}/download`, {
      responseType: 'blob',
    })
    return response.data
  },

  /**
   * 取得檔案分類列表
   * Get file categories
   */
  async getCategories(): Promise<string[]> {
    const response = await apiClient.get<string[]>('/files/categories')
    return response.data
  },
}

export default filesService
