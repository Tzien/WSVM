
import { businessInstance } from '../utils/request'

export const useInsertConfigApi = (obj) => {
  return businessInstance.post('/api/I18nValue/InsertI18nValueAsync', obj)
}

export const useGetConfigApi = (obj) => {
  return businessInstance.get('/api/I18nValue/GetI18nValueAsync', {
    params:  obj 
  })
}


export const useGetConfigById = (id) => {
  return businessInstance.get('/api/I18nValue/GetI18nValueByIdAsync', {
    params: { id }
  })
}

export const useUpdateConfig = (obj) => {
  return businessInstance.post('/api/I18nValue/UpdateI18nValueAsync', obj)
}

export const useDeleteConfig = (id) => {
  return businessInstance.get('/api/I18nValue/DeleteI18nValueAsync', {
    params: { id }
  })
}

