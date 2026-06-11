import { lowCodeFormInstance } from '@/utils/request'

export const getFormDb = (obj) => {
  return lowCodeFormInstance.get('/api/FormDb/GetFormDbAsync', {
    params: obj
  })
}

// 新建功能
export function create(data) {
  return lowCodeFormInstance.post('/api/FormDb/InsertFormDbAsync', data)
}
// 修改功能
export function update(data) {
  return lowCodeFormInstance.post('/api/FormDb/UpdateFormDbAsync', data)
}

export function deleteFormDb(id) {
  return lowCodeFormInstance.get(`/api/FormDb/DeleteFormDbAsync?id=${id}`)
}

// 获取列表表单配置JSON
export function getConfigData(modelId, type) {
  return lowCodeFormInstance.get(`/api/FormDb/GetFormDbFormData?id=${modelId}&type=${type}`);
}