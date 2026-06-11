import { lowCodeFormInstance } from '../utils/request'

export const useInsertApi = (obj) => {
  return lowCodeFormInstance.post('/api/CommonFields/InsertCommonFieldsAsync', obj)
}

export const useGetApi = (obj) => {
  return lowCodeFormInstance.get('/api/CommonFields/GetCommonFieldsAsync', {
    params: obj
  })
}

export const useGetById = (id) => {
  return lowCodeFormInstance.get('/api/CommonFields/GetCommonFieldsByIdAsync', {
    params: { id }
  })
}

export const useUpdate = (obj) => {
  return lowCodeFormInstance.post('/api/CommonFields/UpdateCommonFieldsAsync', obj)
}

export const useDelete = (id) => {
  return lowCodeFormInstance.get('/api/CommonFields/DeleteCommonFieldsAsync', {
    params: { id }
  })
}

export const useGetFieldsXiaLa = () => {
  return lowCodeFormInstance.get('/api/CommonFields/GetFieldsXiaLaAsync')
}
