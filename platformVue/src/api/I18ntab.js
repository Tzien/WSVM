
import { businessInstance } from '../utils/request'

export const useInsertApi = (obj) => {
  return businessInstance.post('/api/I18nTag/InsertI18nTagAsync', obj)
}

export const useGetTabApi = (id,name) => {
  return businessInstance.get('/api/I18nTag/GetI18nTagAsync', {
    params: { id,name }
  })
}


export const useGetById = (id) => {
  return businessInstance.get('/api/I18nTag/GetI18nTagByIdAsync', {
    params: { id }
  })
}

export const useUpdate = (obj) => {
  return businessInstance.post('/api/I18nTag/UpdateI18nTagAsync', obj)
}

export const useDelete = (id) => {
  return businessInstance.get('/api/I18nTag/DeleteI18nTagAsync', {
    params: { id }
  })
}

