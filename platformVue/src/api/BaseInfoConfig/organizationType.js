import { businessInstance } from '@/utils/request'

export const useGetAllSysInfoTypeAsyncAsync = () => {
  return businessInstance.get('/api/SysOraganizeType/GetAllSysInfoTypeAsync')
}

export const useGetSysOraganizeTypeAsync = (obj) => {
  return businessInstance.get(
    '/api/SysOraganizeType/GetSysOraganizeTypeAsync',
    {
      params: obj
    }
  )
}

export const AddSysInfoTypeAsync = (obj) => {
  return businessInstance.post('/api/SysOraganizeType/AddSysInfoTypeAsync', obj)
}

export const useUpdateSysInfoTypeAsync = (obj) => {
  return businessInstance.post(
    '/api/SysOraganizeType/UpdateSysInfoTypeAsync',
    obj
  )
}

export const useSoftSysOraganizeTypeAsync = (id) => {
  return businessInstance.get(
    '/api/SysOraganizeType/SoftSysOraganizeTypeAsync',
    {
      params: { id }
    }
  )
}
