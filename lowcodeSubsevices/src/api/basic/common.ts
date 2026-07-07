import { lowCodeInstance } from '@/utils/request'

enum Api {
  PreviewFile = '/api/FormDb/Uploader/Preview',
  DownloadUrl = '/api/FormDb/DownloadUrl',
  DownloadAll = '/api/FormDb/DownloadAll',
  Merge = '/api/FormDb/merge',
  AMap = '/api/system/Location'
}

// 输入提示
export function getInputTips(data) {
  return lowCodeInstance.get(Api.AMap + '/inputtips', data)
}

export function chunkMerge(data) {
  return lowCodeInstance.post(Api.Merge, data)
}

export function previewFile(data) {
  return lowCodeInstance.get(Api.PreviewFile, { params: data })
}

export function getDownloadUrl(type, fileName) {
  return lowCodeInstance.get(Api.DownloadUrl, { params: { type, fileName } })
}

export function getPackDownloadUrl(type, data) {
  return lowCodeInstance.post(Api.DownloadAll, data, { params: { type } })
}

// 下载导入示例模板
export function getTemplateDownload(url, data) {
  return lowCodeInstance.get(`/api/${url}/TemplateDownload`, data);
}
// 下载导入示例模板
export function getImportPreview(url, data) {
  return lowCodeInstance.get(`/api/${url}/ImportPreview`, data);
}
// 导入数据
export function importData(url, data) {
  return lowCodeInstance.post(`/api/${url}/ImportData`, data );
}
// 导入数据
export function getImportExceptionData(url, data) {
  return lowCodeInstance.post(`/api/${url}/ImportExceptionData`, data );
}