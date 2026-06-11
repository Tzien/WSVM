// 通用 API 响应格式
export interface ApiResponse<T = any> {
  code: number
  data: T
  success: boolean
  message: string
  total: number
}
