import { lowCodeFormInstance } from '../../utils/request'

export const getDBTables = (obj) => {
  return lowCodeFormInstance.get('/api/DataModel/GetDBTables', {
    params: obj
  })
}

export const getDBXia = () => {
  return lowCodeFormInstance.get('/api/DataModel/GetDBXia')
}

export const useAddUserAsync = (obj) => {
  return lowCodeFormInstance.post('/api/DataModel/AddTable', obj)
}

export const useUpdateTable = (obj) => {
  return lowCodeFormInstance.post('/api/DataModel/UpdateTable', obj)
}

export const usedelAsync = (linkId, table) => {
  return lowCodeFormInstance.get('/api/DataModel/DelTable', {
    params: { linkId, table }
  })
}

export const useGetTableAsync = (linkId, tableName) => {
  return lowCodeFormInstance.get('/api/DataModel/GetTableAsync', {
    params: { linkId, tableName }
  })
}
