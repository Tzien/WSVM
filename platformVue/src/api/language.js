
import { businessInstance } from '../utils/request'

export const useInsertApi = (obj) => {
  return businessInstance.post('/api/Language/InsertLanguageAsync', obj)
}

export const useGetLanguageApi = (obj) => {
  return businessInstance.get('/api/Language/GetLanguageAsync', {
    params:  obj 
  })
}


export const useGetById = (id) => {
  return businessInstance.get('/api/Language/GetLanguageByIdAsync', {
    params: { id }
  })
}

export const useUpdate = (obj) => {
  return businessInstance.post('/api/Language/UpdateLanguageAsync', obj)
}

export const useDelete = (id) => {
  return businessInstance.get('/api/Language/DeleteLanguageAsync', {
    params: { id }
  })
}

